using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EVERFI.Foundry.Classes;
using RestSharp.Authenticators;

namespace EVERFI.Foundry
{
    public partial class API
    {
        public RestRequest ConfigureRequest(int val)
        {

            RestRequest request = new RestRequest();
            request.Parameters.Clear();
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            switch (val)
            {
                case 1: //by email
                    Console.WriteLine("Getting user by email...");
                    request.Resource = "{version}/admin/users/";
                    request.Method = Method.GET;
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("include", "category_labels", ParameterType.QueryString);
                    break;
                case 2: //by ID
                    Console.WriteLine("Getting user by ID...");
                    request.Resource = "{version}/admin/users/{id}";
                    request.Method = Method.GET;
                    request.AddParameter("include", "category_labels", ParameterType.QueryString);
                    request.AddHeader("Content-Type", "application/json");

                    break;
                case 3: //add user
                    Console.WriteLine("Adding...");
                    request.Resource = "{version}/admin/registration_sets";
                    request.Method = Method.POST;
                    break;
                case 4: //update user
                    Console.WriteLine("Updating user...");
                    request.Resource = "{version}/admin/registration_sets/{id}";
                    request.Method = Method.PATCH;
                    break;
                case 5: //all users
                    Console.WriteLine("Getting all users...");
                    request.Resource = "/{version}/admin/users/";
                    request.Method = Method.GET;
                    request.AddParameter("version", _ver, ParameterType.UrlSegment);
                    request.AddParameter("page[page]", currPage, ParameterType.QueryString);
                    request.AddParameter("include", "category_labels", ParameterType.QueryString);
                    request.AddParameter("page[per_page]", returnPerPage, ParameterType.QueryString);
                    request.AddHeader("Content-Type", "application/json");
                    break;
                case 6: //user count
                    Console.WriteLine("Getting user count...");
                    request.Resource = "/{version}/admin/users/?page[page]={page_num}&page[per_page]={num_per}";
                    request.Method = Method.GET;
                    request.Parameters.Clear();
                    request.AddParameter("version", _ver, ParameterType.UrlSegment);
                    request.AddParameter("page_num", 1, ParameterType.UrlSegment);
                    request.AddParameter("num_per", returnPerPage, ParameterType.UrlSegment);
                    request.AddHeader("Content-Type", "application/json");
                    break;
                case 7: //by search
                    Console.WriteLine("Getting user by search...");
                    request.Resource = "{version}/admin/users/";
                    request.Method = Method.GET;
                    request.Parameters.Clear();
                    request.AddParameter("include", "category_labels", ParameterType.QueryString);
                    request.AddHeader("Content-Type", "application/json");
                    break;
                case 8: //boolean get users
                    Console.WriteLine("Getting users...");
                    request.Resource = "{version}/admin/users/";
                    request.Method = Method.GET;
                    request.Parameters.Clear();
                    request.AddParameter("page[page]", currPage, ParameterType.QueryString);
                    request.AddParameter("page[per_page]", returnPerPage, ParameterType.QueryString);
                    request.AddHeader("Content-Type", "application/json");
                    break;
            }
            Console.WriteLine(request.Body);
            return request;

        }
        public void responseModifer(IRestResponse response, int type)
        {

            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;
            if (type == 1)
            {
                if (numericCode != 200)
                {
                    throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
                }
            }
            else if (type == 2)
            {
                if (numericCode != 201)
                {
                    throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
                }
            }

        }
        public User helperFunction(IRestResponse response, bool noList)
        {
            UserDataJsonList userDataList;
            UserDataJson userData;
            UserDataIncludedList userDataIncluded = JsonConvert.DeserializeObject<UserDataIncludedList>(response.Content);
            User user = new User();
            if (noList)
            {
                userData = JsonConvert.DeserializeObject<UserDataJson>(response.Content);
                user = userData.Data.UserAttributes;

                user.ConfigureUserData(userData.Data);
                if (user.Location != null)
                {
                    user.Location = GetLocationById(user.LocationId);
                }


            }
            else
            {
                userDataList = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);
                foreach (UserData data in userDataList.Data)
                {
                    user = data.UserAttributes;
                    user.ConfigureUserData(data);

                    if (user.Location != null)
                    {

                        user.Location = GetLocationById(user.LocationId);
                    }
                }
            }

            foreach (var labelAttribute in userDataIncluded.IncludedList)
            {
                Label newLabel = new Label();
                newLabel.Name = labelAttribute.LabelsAttributes.LabelName;
                newLabel.CategoryName = labelAttribute.LabelsAttributes.CategoryLabelName;
                newLabel.Id = labelAttribute.LabelId;
                newLabel.CategoryId = labelAttribute.LabelsAttributes.CategoryID;
                user.Labels.Add(newLabel);
               
            }
            user.createCategoryLabels();
            return user;
        }
        public List<User> helperFunction2(IRestResponse response)
        {
            UserDataJsonList userData = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);
            UserDataIncludedList userDataIncluded = JsonConvert.DeserializeObject<UserDataIncludedList>(response.Content);
            List<Label> allLabels = new List<Label>();
            foreach (var labelAttribute in userDataIncluded.IncludedList)
            {
                Label newLabel = new Label();
                newLabel.Name = labelAttribute.LabelsAttributes.LabelName;
                newLabel.CategoryName = labelAttribute.LabelsAttributes.CategoryLabelName;
                newLabel.Id = labelAttribute.LabelId;
                newLabel.CategoryId = labelAttribute.LabelsAttributes.CategoryID;
                allLabels.Add(newLabel);

            }

            List<User> users = new List<User>();

            foreach (UserData data in userData.Data)
            {
                User newUser = data.UserAttributes;
                newUser.ConfigureUserData(data);
                foreach (Label lab in allLabels)
                {
                    List<RelationshipData> relationship = data.multipleRelationships.categoryLabels.RelationshipsData;
                    if (relationship != null && relationship.Any())
                    {
                        if (relationship[0].LabelId == lab.Id)
                        {
                            newUser.Labels.Add(lab);
                        }
                    }
                }

                if (newUser.Location != null)
                {

                    newUser.Location = GetLocationById(newUser.LocationId);
                }
                users.Add(newUser);
            }
            return users;
        }
        public User AddUser(User MyUser)
        {

            IRestRequest request = ConfigureRequest(3);
            request.AddParameter("application/json", API.UserJson(MyUser), ParameterType.RequestBody);

            IRestResponse response = _client.Execute<User>(request);

            responseModifer(response, 2);

            UserDataJson userData = JsonConvert.DeserializeObject<UserDataJson>(response.Content);
            Console.WriteLine("User successfully added.");

            User user = userData.Data.UserAttributes;

            if (user.Location != null)
            {
                user.Location = GetLocationById(user.LocationId);
            }

            user.ConfigureUserData(userData.Data);
            user.createCategoryLabels();
            user.addUserType();

            return user;


        }

        public User UpdateUser(User MyUser)
        {
            RestRequest request = ConfigureRequest(4);

            request.AddParameter("id", MyUser.UserId, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.UserJson(MyUser), ParameterType.RequestBody);
            _client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_token.access_token, _token.token_type);

            IRestResponse response = _client.Execute(request);
            responseModifer(response, 1);

            UserDataJson userData = JsonConvert.DeserializeObject<UserDataJson>(response.Content);

            Console.WriteLine("User successfully updated.");

            User user = userData.Data.UserAttributes;

            if (user.Location != null)
            {
                user.Location = GetLocationById(user.LocationId);
            }

            user.ConfigureUserData(userData.Data);
            user.createCategoryLabels();

            return user;
        }

        public User GetUserById(string UserId)
        {
            RestRequest request = ConfigureRequest(2);

            request.AddParameter("id", UserId, ParameterType.UrlSegment);

            IRestResponse response = _client.Execute(request);
            responseModifer(response, 1);

            User user = helperFunction(response, true);
            return user;
        }

        public User GetUserByEmail(string UserEmail)
        {
            Console.WriteLine("Getting User(s) with email: " + UserEmail + "...");

            RestRequest request = ConfigureRequest(1);
            request.AddParameter("filter[email]", UserEmail, ParameterType.QueryString);

            IRestResponse response = _client.Execute(request);
            responseModifer(response, 1);

            User user = helperFunction(response, false);

            return user;
        }

        // TODO: Implement paging in this
        public List<User> GetUsersBySearch(Dictionary<SearchTerms, string> searchTerms)
        {
            RestRequest request = ConfigureRequest(7);
            foreach (SearchTerms term in searchTerms.Keys)
            {
                request.AddParameter("filter[" + GetDescription(term) + "]", searchTerms[term], ParameterType.QueryString);
            }

            IRestResponse response = _client.Execute(request);
            responseModifer(response, 1);

            List<User> users = helperFunction2(response);

            return users;
        }

        public List<User> GetUsers(int page)
        {
            Console.WriteLine("Getting " + returnPerPage + "users on page " + page.ToString() + "...");

            RestRequest request = ConfigureRequest(5);

            IRestResponse response = _client.Execute(request);
            responseModifer(response, 1);

            List<User> users = helperFunction2(response);

            return users;

        }

        public (List<User>, bool) GetUsers()
        {
            bool returnValue = true;

            RestRequest request = ConfigureRequest(8);
            IRestResponse response = _client.Execute(request);
            responseModifer(response, 1);

            List<User> users = helperFunction2(response);

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
            RestRequest request = ConfigureRequest(6);
            IRestResponse response = _client.Execute(request);
            responseModifer(response, 1);

            MetaJson metaData = JsonConvert.DeserializeObject<MetaJson>(response.Content);

            return metaData.Meta.Count;
        }

    }

}





