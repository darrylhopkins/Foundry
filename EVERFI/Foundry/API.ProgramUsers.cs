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

        public RestRequest ConfigureProgramUserRequest(DateTime sinceDate, int scrollSize) {
            RestRequest request = new RestRequest();
            request.Parameters.Clear();
            request.Method = Method.GET;
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.Resource = "{version}/progress/program_users";
            request.AddParameter("scroll_size", scrollSize, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");

            return request;

        }

        public (List<ProgramUser>, String, DateTime) deserializeProgramUsers(DateTime sinceDate, IRestResponse response)
        {
            List<ProgramUser> programUserList = new List<ProgramUser>();
            String scrollId = null;
            NextUserData next = JsonConvert.DeserializeObject<NextUserData>(response.Content);
           
            sinceDate = next.Next.Since;
            ProgramUserDataHeaderList programUserData = JsonConvert.DeserializeObject<ProgramUserDataHeaderList>(response.Content);
            if (next.Next.ScrollId != null)
            {
                scrollId = next.Next.ScrollId;
            }
            Console.WriteLine(scrollId);
            foreach (ProgramUser program in programUserData.ProgressDataHeaderList)
            {
                programUserList.Add(program);
            }
            return (programUserList, scrollId, sinceDate);
        }

        public (List<ProgramUser>, String, DateTime) GetProgramUsers(DateTime sinceDate, int scrollSize)
        {
            
            RestRequest request = ConfigureProgramUserRequest(sinceDate, scrollSize);
            since = sinceDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            request.AddParameter("since", since, ParameterType.QueryString);
            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response);
            return deserializeProgramUsers(sinceDate, response);
     
        }

        public (List<ProgramUser>, String, DateTime) GetProgramUsers(DateTime sinceDate, int scrollSize, String scrollId) {
            List<ProgramUser> programUserList = new List<ProgramUser>();
             RestRequest request = ConfigureProgramUserRequest(sinceDate, scrollSize);
            since = sinceDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            request.AddParameter("since", since, ParameterType.QueryString);
            request.AddParameter("scroll_id", scrollId, ParameterType.QueryString);
            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response);
            return deserializeProgramUsers(sinceDate, response);

        }
    }
}
