using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using EVERFI.Foundry.Objects;

namespace EVERFI.Foundry
{
    public partial class API
    {
        public Label AddLabel(Category MyCategory, Label MyLabel)
        {
            Console.WriteLine("Adding label " + MyLabel.Name + " to category " + MyCategory.Name);

            RestRequest request = new RestRequest("/{version}/admin/category_labels", Method.POST);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.LabelJson(MyCategory, MyLabel), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 201)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            LabelData labelData = JsonConvert.DeserializeObject<LabelData>(response.Content);
            Console.WriteLine("Label successfully added.");
            Label label = labelData.Data;

            label.ConfigureLabel();

            return label;
        }

        public Label UpdateLabel(Label MyLabel)
        {
            Console.WriteLine("Updating label...");

            RestRequest request = new RestRequest("/{version}/admin/category_labels/{id}", Method.PATCH);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", MyLabel.Id, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.LabelJson(null, MyLabel), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            LabelData labelData = JsonConvert.DeserializeObject<LabelData>(response.Content);
            Console.WriteLine("Label successfully updated.");
            Label label = labelData.Data;

            label.ConfigureLabel();

            return label;
        }

        public string DeleteLabel(Label MyLabel)
        {
            Console.WriteLine("Deleting label " + MyLabel.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/category_labels/{id}", Method.DELETE);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", MyLabel.Id, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 204)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            Console.WriteLine("Label successfully deleted.");

            return response.Content;
        }

        public Label GetLabelById(string LabelId)
        {
            Console.WriteLine("Getting label " + LabelId + "...");

            RestRequest request = new RestRequest("/{version}/admin/category_labels/{id}", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", LabelId, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            LabelData labelData = JsonConvert.DeserializeObject<LabelData>(response.Content);
            Label label = labelData.Data;

            label.ConfigureLabel();

            return label;
        }
    }
}
