using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using EVERFI.Foundry.Classes;

namespace EVERFI.Foundry
{
    public partial class API
    {
        public BulkAssignJob BulkAssignLabels(List<User> usersList, Label label)
        {
            if (usersList.Count > bulkActionCap)
            {
                throw new FoundryException(422, "limit", "The limit for the bulk add function is 500 users!");
            }

            Console.WriteLine("Assigning " + label.Name + " to users provided.");

            RestRequest request = new RestRequest("/{version}/admin/bulk_actions/category", Method.POST);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.BulkUserLabelJson(usersList, label), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 201)
            {
                throw new FoundryException(numericCode, response.Content);
            }

            JobJson jobJson = JsonConvert.DeserializeObject<JobJson>(response.Content);
            BulkAssignJob job = jobJson.BulkAssignJob;

            Console.WriteLine("Labels added to " + usersList.Count + " users.");

            return job;
        }

        public BulkAssignJob GetJobById(string JobId)
        {
            RestRequest request = new RestRequest("/{version}/admin/bulk_actions/category/{id}", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", JobId, ParameterType.UrlSegment);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(numericCode, response.Content);
            }

            JobJson jobJson = JsonConvert.DeserializeObject<JobJson>(response.Content);
            BulkAssignJob job = jobJson.BulkAssignJob;

            return job;
        }
    }
}
