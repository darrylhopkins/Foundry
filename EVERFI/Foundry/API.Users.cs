using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using EVERFI.Foundry.Classes;
using RestSharp.Authenticators;

namespace EVERFI.Foundry
{
    public partial class API
    {
        public User AddUser(User MyUser)
        {

            RestRequest request = new RestRequest("{version}/admin/registration_sets", Method.POST);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.UserJson(MyUser), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute<User>(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 201)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            UserDataJson userData = JsonConvert.DeserializeObject<UserDataJson>(response.Content);
            Console.WriteLine("User successfully added.");

            User user = userData.Data.UserAttributes;

            if (user.Location != null)
            {
                user.Location = GetLocationById(user.LocationId);
            }

            user.ConfigureUserData(userData.Data);

            return user;
        }

        public User UpdateUser(User MyUser)
        {

            RestRequest request = new RestRequest("{version}/admin/registration_sets/{id}", Method.PATCH);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", MyUser.UserId, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.UserJson(MyUser), ParameterType.RequestBody);
            _client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_token.access_token, _token.token_type);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            UserDataJson userData = JsonConvert.DeserializeObject<UserDataJson>(response.Content);
            Console.WriteLine("User successfully updated.");

            User user = userData.Data.UserAttributes;

            if (user.Location != null)
            {
                user.Location = GetLocationById(user.LocationId);
            }

            user.ConfigureUserData(userData.Data);

            return user;
        }

        public User GetUserById(string UserId)
        {
            Console.WriteLine("Getting User " + UserId + "...");

            RestRequest request = new RestRequest("{version}/admin/users/{id}", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", UserId, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            UserDataJson userData = JsonConvert.DeserializeObject<UserDataJson>(response.Content);

            User retrievedUser = userData.Data.UserAttributes;
            retrievedUser.ConfigureUserData(userData.Data);
            if (retrievedUser.Location != null)
            {
                retrievedUser.Location = GetLocationById(retrievedUser.LocationId);
            }

            Console.WriteLine("User Retrieved: " + retrievedUser.FirstName + " " + retrievedUser.LastName + "...");

            return retrievedUser;
        }

        public List<User> GetUserByEmail(string UserEmail)
        {
            Console.WriteLine("Getting User(s) with email: " + UserEmail + "...");

            RestRequest request = new RestRequest("{version}/admin/users/", Method.GET);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("filter[email]", UserEmail, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            UserDataJsonList userData = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);
            List<User> users = new List<User>();

            foreach (UserData data in userData.Data)
            {
                User newUser = data.UserAttributes;
                newUser.ConfigureUserData(data);
                if (newUser.Location != null)
                {
                    newUser.Location = GetLocationById(newUser.LocationId);
                }
                users.Add(newUser);
                Console.WriteLine("User Retrieved: " + newUser.FirstName + " " + newUser.LastName + "...");
            }

            return users;
        }

        // TODO: Implement paging in this
        public List<User> GetUsersBySearch(Dictionary<SearchTerms, string> searchTerms)
        {
            RestRequest request = new RestRequest("{version}/admin/users/", Method.GET);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            foreach (SearchTerms term in searchTerms.Keys)
            {
                request.AddParameter("filter[" + GetDescription(term) + "]", searchTerms[term], ParameterType.QueryString);
            }
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            UserDataJsonList userData = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);
            List<User> users = new List<User>();

            foreach (UserData data in userData.Data)
            {
                User newUser = data.UserAttributes;
                newUser.ConfigureUserData(data);
                if (newUser.Location != null)
                {
                    newUser.Location = GetLocationById(newUser.LocationId);
                }
                users.Add(newUser);
            }

            return users;
        }

        public List<User> GetUsers(int page)
        {
            Console.WriteLine("Getting " + returnPerPage + "users on page " + page.ToString() + "...");

            RestRequest request = new RestRequest("/{version}/admin/users", Method.GET);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("page[page]", currPage, ParameterType.QueryString);
            request.AddParameter("page[per_page]", returnPerPage, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            UserDataJsonList userData = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);
            List<User> users = new List<User>();

            foreach (UserData data in userData.Data)
            {
                User newUser = data.UserAttributes;
                newUser.ConfigureUserData(data);
                if (newUser.Location != null)
                {
                    newUser.Location = GetLocationById(newUser.LocationId);
                }
                users.Add(newUser);
            }

            return users;
        }

        public (List<User>, bool) GetUsers()
        {
            bool returnValue = true;

            RestRequest request = new RestRequest("/{version}/admin/users", Method.GET);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("page[page]", currPage, ParameterType.QueryString);
            request.AddParameter("page[per_page]", returnPerPage, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            UserDataJsonList userData = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);
            List<User> users = new List<User>();

            foreach (UserData data in userData.Data)
            {
                User newUser = data.UserAttributes;
                newUser.ConfigureUserData(data);
                if (newUser.Location != null)
                {
                    newUser.Location = GetLocationById(newUser.LocationId);
                }
                users.Add(newUser);
            }

            MetaJson metaData = JsonConvert.DeserializeObject<MetaJson>(response.Content);

            Console.WriteLine("Returning " + users.Count + " users. Page " + currPage + " of " + Math.Ceiling((double)metaData.Meta.Count / returnPerPage));

            if (currPage * returnPerPage >= metaData.Meta.Count)
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

        public void ResetGetUsers()
        {
            currPage = 1;
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
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            MetaJson metaData = JsonConvert.DeserializeObject<MetaJson>(response.Content);

            return metaData.Meta.Count;
        }
    }
}
