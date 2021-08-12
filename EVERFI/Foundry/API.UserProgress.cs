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
        public (List<UserProgress>, bool) GetUserProgress(int scrollSize)
        {

            bool keepGoing = true;
            List<UserProgress> userProgressList = new List<UserProgress>();
            RestRequest request = new RestRequest();
            request.Method = Method.GET;
            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.Resource = "{version}/progress/user_assignments";
            request.AddParameter("since", since, ParameterType.QueryString);
            request.AddParameter("scroll_id", scrollId, ParameterType.QueryString);
            request.AddParameter("scroll_size", scrollSize, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response);

            NextUserData next = JsonConvert.DeserializeObject<NextUserData>(response.Content);

            if (next.Next.ScrollId != null)
            {
                DateTime sinceDate = next.Next.Since;
                since = sinceDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
                scrollId = next.Next.ScrollId;
            }
            else
            {
                keepGoing = false;
            }

            UserProgressDataHeaderList progressData = JsonConvert.DeserializeObject<UserProgressDataHeaderList>(response.Content);
            foreach (UserProgress data in progressData.ProgressDataHeaderList)
            {
                userProgressList.Add(data);

            }
            return (userProgressList, keepGoing);

        }
	}
}
