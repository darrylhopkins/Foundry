﻿using Newtonsoft.Json;
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

        const int returnPerPage = 100;
        int currPage = 1;

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

        public User AddUser(User MyUser) // Return given user if invalid add
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

            UserDataJson userData = JsonConvert.DeserializeObject<UserDataJson>(response.Content);
            Console.WriteLine("User successfully added.");

            User user = userData.Data.UserAttributes;
            user.ConfigureUserData(userData.Data);

            return user;
        }

        public string UpdateUser(User MyUser) // Return exception if invalid update
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

            IRestResponse response = _client.Execute<UserDataJson>(request);
            UserDataJson userData = JsonConvert.DeserializeObject<UserDataJson>(response.Content);

            User retrievedUser = userData.Data.UserAttributes;
            retrievedUser.ConfigureUserData(userData.Data);

            Console.WriteLine("User Retrieved: " + retrievedUser.FirstName + " " + retrievedUser.LastName + "...");

            return retrievedUser;
        }

        /*public List<User> GetUsers()
        {
            Console.WriteLine("Getting all users...");

            RestRequest request = new RestRequest("/{version}/admin/users/", Method.GET);
            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            UserDataJsonList userData = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);
            List<User> users = new List<User>();

            foreach (UserData data in userData.Data)
            {
                User newUser = data.UserAttributes;
                newUser.ConfigureUserData(data);
                users.Add(newUser);
            }

            return users;
        }*/

        public List<User> GetUsers(int page)
        {
            Console.WriteLine("Getting " + returnPerPage + "users on page " + page.ToString() + "...");

            RestRequest request = new RestRequest("/{version}/admin/users/?page[page]={page_num}&page[per_page]={num_per}", Method.GET);
            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("page_num", page, ParameterType.UrlSegment);
            request.AddParameter("num_per", returnPerPage, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            UserDataJsonList userData = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);
            List<User> users = new List<User>();

            foreach (UserData data in userData.Data)
            {
                User newUser = data.UserAttributes;
                newUser.ConfigureUserData(data);
                users.Add(newUser);
            }

            return users;
        }

        public (List<User>, bool) GetUsers()
        {
            bool returnValue = true;

            RestRequest request = new RestRequest("/{version}/admin/users", Method.GET);
            // "/{version}/admin/users/?page[page]={page_num}&page[per_page]={num_per}",

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("page[page]", currPage, ParameterType.QueryString);
            request.AddParameter("page[per_page]", returnPerPage, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            UserDataJsonList userData = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);
            List<User> users = new List<User>();

            foreach (UserData data in userData.Data)
            {
                User newUser = data.UserAttributes;
                newUser.ConfigureUserData(data);
                users.Add(newUser);
            }

            MetaJson metaData = JsonConvert.DeserializeObject<MetaJson>(response.Content);

            Console.WriteLine("Returning " + users.Count + " users. Page " + currPage + " of " + Math.Ceiling((double)metaData.Meta.Count / returnPerPage));

            if (currPage*returnPerPage >= metaData.Meta.Count)
            {
                returnValue = false;
                currPage = 1;
            }
            else
            {
                currPage += 1;
            }

            return (users, returnValue);
        }

        public int GetUserCount()
        {
            RestRequest request = new RestRequest("/{version}/admin/users/?page[page]={page_num}&page[per_page]={num_per}", Method.GET);
            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("page_num", 1, ParameterType.UrlSegment);
            request.AddParameter("num_per", returnPerPage, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            MetaJson metaData = JsonConvert.DeserializeObject<MetaJson>(response.Content);

            return metaData.Meta.Count;
        }

        /*public List<User> GetUsers() // TODO: Test this!!
        {
            Console.WriteLine("Getting all users...");

            List<User> users = new List<User>();
            bool getNext = true;
            int curr_page = 1;
            int total_returned = 0;

            while (getNext)
            {
                RestRequest request = new RestRequest("/{version}/admin/users/?page[page]={page_num}&page[per_page]={num_per}", Method.GET);
                request.Parameters.Clear();
                request.AddParameter("version", _ver, ParameterType.UrlSegment);
                request.AddParameter("page_num", curr_page, ParameterType.UrlSegment);
                request.AddParameter("num_per", return_per_page, ParameterType.UrlSegment);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

                IRestResponse response = _client.Execute(request);
                UserDataJsonList userData = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);

                foreach (UserData data in userData.Data)
                {
                    User newUser = data.UserAttributes;
                    newUser.ConfigureUserData(data);
                    users.Add(newUser);
                }

                total_returned += return_per_page;

                MetaJson metaData = JsonConvert.DeserializeObject<MetaJson>(response.Content);
                if (total_returned >= metaData.Meta.Count)
                {
                    getNext = false;
                }
                else
                {
                    curr_page += 1;
                }
            }

            Console.WriteLine("All users returned. Total Pages: " + curr_page.ToString());

            return users;
        }*/

        public Location AddLocation(Location MyLocation)
        {
            Console.WriteLine("Adding location " + MyLocation.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/locations", Method.POST);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute<Location>(request);

            //verify if adding was okay with status code

            LocationDataJson locationData = JsonConvert.DeserializeObject<LocationDataJson>(response.Content);
            Console.WriteLine("Location successfully added.");

            Location location = locationData.LocationData.LocationAttributes;
            location.Id = locationData.LocationData.Id;

            return location;
        }

        public string UpdateLocation(Location MyLocation)
        {
            Console.WriteLine("Updating location " + MyLocation.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/locations", Method.PATCH);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);

            // do things with location if necessary

            return response.Content;
        }

        public List<Location> GetLocations()
        {
            Console.WriteLine("Getting all locations...");

            RestRequest request = new RestRequest("/{version}/admin/locations", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            LocationDataJsonList locationData = JsonConvert.DeserializeObject<LocationDataJsonList>(response.Content);
            List<Location> locations = new List<Location>();

            foreach (LocationData data in locationData.LocationsData)
            {
                Location newLocation = data.LocationAttributes;
                newLocation.AddIdFromData(data);
                locations.Add(newLocation);
            }

            return locations;
        }

        public Location GetLocationById(string LocationId)
        {
            Console.WriteLine("Getting location " + LocationId + "...");

            RestRequest request = new RestRequest("/{version}/admin/locations/{location_id}", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute<LocationDataJson>(request);
            LocationDataJson locationData = JsonConvert.DeserializeObject<LocationDataJson>(response.Content);
            Location location = locationData.LocationData.LocationAttributes;
            location.AddIdFromData(locationData.LocationData);

            return location;
        }
    }
}