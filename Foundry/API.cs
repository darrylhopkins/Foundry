using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        // TODO: Add try catch to make sure id and key are valid
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

            //return an AccessToken
            _token = JsonConvert.DeserializeObject<AccessToken>(response.Content);
            if (_token.token_type == null)
            {
                Console.WriteLine(response.Content);
                Console.ReadLine();
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Authorization Succeed. You are now accessing EVERFI Foundry.");
            }

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

        public User AddUser(User MyUser)
        {
            if (MyUser.FirstName == null || MyUser.LastName == null || MyUser.Email == null || MyUser.UserTypes.Count < 1)
            {
                Console.WriteLine("Illegal User: Missing First Name, Last Name, Email, or UserType");
                Console.ReadLine();
                Environment.Exit(1);
            }

            Console.WriteLine("Adding User: " + MyUser.FirstName + " " + MyUser.LastName + "...");

            RestRequest request = new RestRequest("{version}/admin/registration_sets", Method.POST);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", MyUser.GetJson(), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute<User>(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode == 422)
            {
                Console.WriteLine(response.Content);
                Console.ReadLine();
                Environment.Exit(422);
            }

            UserData userData = JsonConvert.DeserializeObject<UserData>(response.Content);
            Console.WriteLine("User successfully added.");

            User user = userData.Data.UserAttributes;
            user.UserId = userData.Data.UserId;
            user.ConfigureUserData();

            return user;
        }

        public string UpdateUser(User MyUser) // TODO: Add verification of update
        {
            if (MyUser.FirstName == null || MyUser.LastName == null || MyUser.Email == null || MyUser.UserTypes.Count < 1)
            {
                Console.WriteLine("Illegal User: Missing First Name, Last Name, Email, or UserType");
                Console.ReadLine();
                Environment.Exit(1);
            }

            RestRequest request = new RestRequest("{version}/admin/registration_sets/{id}", Method.PATCH);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", MyUser.UserId, ParameterType.UrlSegment);
            request.AddParameter("application/json", MyUser.GetJson(), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);

            return response.Content;
        }

        public User GetUserById(string UserId)
        {
            Console.WriteLine("Getting User " + UserId + "...");

            RestRequest request = new RestRequest("{version}/admin/users/{id}", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", UserId, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute<UserData>(request);
            UserData userData = JsonConvert.DeserializeObject<UserData>(response.Content);

            User retrievedUser = userData.Data.UserAttributes;
            retrievedUser.UserId = userData.Data.UserId;
            retrievedUser.ConfigureUserData();

            Console.WriteLine("User Retrieved: " + retrievedUser.FirstName + " " + retrievedUser.LastName + "...");

            return retrievedUser;
        }

        public List<User> GetUsers()
        {
            Console.WriteLine("Getting all users...");

            RestRequest request = new RestRequest("/{version}/admin/users/", Method.GET);
            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            UserDataList userData = JsonConvert.DeserializeObject<UserDataList>(response.Content);
            List<User> users = new List<User>();

            foreach (Data data in userData.Data)
            {
                User newUser = data.UserAttributes;
                newUser.ConfigureUserData();
                users.Add(newUser);
            }

            return users;
        }
    }
}