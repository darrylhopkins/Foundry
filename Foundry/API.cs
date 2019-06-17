using Newtonsoft.Json;
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
        const string BaseUrl = "https://api.fifoundry-staging.net/";
        const string _ver = "v1";

        readonly IRestClient _client;
        AccessToken _token;

        string _accountSid;

        public API(string accountSid, string secretKey)
        {
            _client = new RestClient(BaseUrl+"/oauth/token");
            _client.Authenticator = new HttpBasicAuthenticator(accountSid, secretKey);

            RestRequest request = new RestRequest(Method.POST);

            request.Parameters.Clear();
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("username", accountSid);
            request.AddParameter("password", secretKey);

            //make the API request and get the response
            IRestResponse response = _client.Execute<AccessToken>(request);

            Console.WriteLine(response.Content);
            Console.WriteLine("Waiting here...");
            Console.ReadLine();

            //return an AccessToken
            _token = JsonConvert.DeserializeObject<AccessToken>(response.Content);

            _accountSid = accountSid;
        }

        public T Execute<T>(RestRequest request) where T : new() // might make this private
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
            RestRequest request = new RestRequest("{ver}/admin/registration_sets", Method.POST);
            request.AddParameter("ver", _ver, ParameterType.UrlSegment);
            request.AddJsonBody(MyUser);
            _client.Execute(request);
            // return something to track status

        }

        public User GetUserById(string UserId)
        {
            RestRequest request = new RestRequest("{ver}/admin/users/{id}", Method.GET);
            request.AddParameter("ver", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", UserId, ParameterType.UrlSegment);

            var queryResult = _client.Execute(request);
            Console.WriteLine(queryResult.Content);

            User newUser = new User();
            //newUser.FromJson();
            return newUser;
        }

        /*public List<User> GetUsers()
        {

        }*/
    }
}