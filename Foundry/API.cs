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
        readonly AccessToken _token;

        string _accountSid;

        public API(string accountSid, string secretKey)
        {
            _client = new RestClient(BaseUrl);
            _client.Authenticator = new HttpBasicAuthenticator(accountSid, secretKey);

            RestRequest request = new RestRequest("/oauth/token", Method.POST);

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

        public string AddUser(User MyUser)
        {
            RestRequest request = new RestRequest("{version}/admin/registration_sets", Method.POST);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", MyUser.GetJson(), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);

            return response.Content;
        }

        public User GetUserById(string UserId)
        {
            RestRequest request = new RestRequest("{ver}/admin/users/{id}", Method.GET);
            request.AddParameter("ver", _ver, ParameterType.UrlSegment);
            RestRequest request = new RestRequest("{version}/admin/users/{id}", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", UserId, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute<User>(request);
            User RetrievedUser = JsonConvert.DeserializeObject<User>(response.Content);


            User newUser = new User();
            //newUser.FromJson();
            return newUser;
        }

        /*public List<User> GetUsers()
        {

        }*/

        public List<User> GetUsers()
        {
            RestRequest request = new RestRequest("/{version}/admin/users/", Method.GET);
            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            List<UserData> data = JsonConvert.DeserializeObject<List<UserData>>(response.Content);
            List<User> users = new List<User>();

            foreach (UserData userData in data)
            {
                User newUser = userData.Data.UserAttributes;
                newUser.ConfigureUserData();
                users.Add(newUser);
            }

            return users;
        }
    }
}