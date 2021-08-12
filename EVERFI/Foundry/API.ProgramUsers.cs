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
        public (List<ProgramUser>, bool) GetProgramUsers(int scrollSize)
        {
            bool keepGoing = true;
            List<ProgramUser> programUserList = new List<ProgramUser>();
            RestRequest request = new RestRequest();
            request.Parameters.Clear();
            request.Method = Method.GET;
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.Resource = "{version}/progress/program_users";
            request.AddParameter("since", since, ParameterType.QueryString);
            request.AddParameter("scroll_size", scrollSize, ParameterType.QueryString);
            request.AddParameter("scroll_id", scrollId, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");

            IRestResponse response = _client.Execute(request);


            checkResponseSuccess(response);
            NextUserData next = JsonConvert.DeserializeObject<NextUserData>(response.Content);
            ProgramUserDataHeaderList programUserData = JsonConvert.DeserializeObject<ProgramUserDataHeaderList>(response.Content);
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
            foreach (ProgramUser program in programUserData.ProgressDataHeaderList)
            {
                programUserList.Add(program);
            }


            return (programUserList, keepGoing);
        }

    }
}
