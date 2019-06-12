using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    public class API
    {
        const string BaseUrl = "https://api.fifoundry-staging.net/v1";

        readonly IRestClient _client;

        string _accountSid;

        public API(string accountSid, string secretKey)
        {
            _client = new RestClient(BaseUrl);
            _client.Authenticator = new HttpBasicAuthenticator(accountSid, secretKey);
            _accountSid = accountSid;
        }

        private T Execute<T>(RestRequest request) where T : new() // private or public
        {
            request.AddParameter("AccountSid", _accountSid, ParameterType.UrlSegment); // used on every request
            var response = _client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var apiException = new ApplicationException(message, response.ErrorException);
                throw apiException;
            }
            return response.Data;
        }

        public void AddUser(User MyUser)
        {
            RestRequest request = new RestRequest("/admin/registration_sets", Method.POST); // is this right?
            request.AddParameter("application/json", MyUser.GetJson(), ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            var response = _client.Execute(request);

        }
    }
}