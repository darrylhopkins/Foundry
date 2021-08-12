using System;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using EVERFI.Foundry.Classes;
using RestSharp.Authenticators;
using System.Linq;
using System.Globalization;

namespace EVERFI.Foundry
{
    public partial class API
    {
        internal RestRequest CreateUserProgressRequest(int scrollSize)
        {
            RestRequest request = new RestRequest();
            request.Method = Method.GET;
            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.Resource = ("{version}/progress/user_assignments");
            request.AddParameter("scroll_size", scrollSize, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");

            return request;
        }
        internal IRestResponse setUserProgress(RestRequest request, DateTime sinceDate)
        {
            since = sinceDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            request.AddParameter("since", since, ParameterType.QueryString);
            IRestResponse response = _client.Execute(request);

            return response;
        }

        internal (List<UserProgress>, String, DateTime) deserializeUserProgress(DateTime sinceDate, IRestResponse response)
        {
            List<UserProgress> userProgressList = new List<UserProgress>();
            NextUserData next = JsonConvert.DeserializeObject<NextUserData>(response.Content);

            sinceDate = next.Next.Since;

            if (next.Next.ScrollId != null)
            {
                scrollId = next.Next.ScrollId;
            }
            else
            {
                scrollId = null;
            }

            UserProgressDataHeaderList progressData = JsonConvert.DeserializeObject<UserProgressDataHeaderList>(response.Content);
            foreach (UserProgress data in progressData.ProgressDataHeaderList)
            {
                userProgressList.Add(data);

            }
            return (userProgressList, scrollId, sinceDate);
        }

        public (List<UserProgress>, String, DateTime) GetUserProgress(DateTime sinceDate, int scrollSize)
        {
            //List<UserProgress> userProgressList = new List<UserProgress>();
            RestRequest request = CreateUserProgressRequest(scrollSize);
            IRestResponse response = setUserProgress(request, sinceDate);
            checkResponseSuccess(response);
            return deserializeUserProgress(sinceDate, response);
        }
        
        public (List<UserProgress>, String, DateTime) GetUserProgress(DateTime sinceDate, String scrollId, int scrollSize)
        {
           
            List<UserProgress> userProgressList = new List<UserProgress>();
            RestRequest request = CreateUserProgressRequest(scrollSize);
            request.AddParameter("scroll_id", scrollId, ParameterType.QueryString);
            IRestResponse response = setUserProgress(request, sinceDate);
            checkResponseSuccess(response);
            return deserializeUserProgress(sinceDate, response);
        }
    
    }
}
