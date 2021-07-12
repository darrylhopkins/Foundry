using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using EVERFI.Foundry.Classes;
using RestSharp.Authenticators;
using System.Linq;

namespace EVERFI.Foundry
{
    public partial class API
    {
        public RestRequest ConfigureRequest()
        {

            RestRequest request = new RestRequest();
            request.Parameters.Clear();
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            return request;

        }
        public void checkResponseSuccess(IRestResponse response, int type)
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
        public List<Label> CreateLabelList(IRestResponse response)
        {
            UserDataIncludedList userDataIncluded = JsonConvert.DeserializeObject<UserDataIncludedList>(response.Content);
            List<UserDataIncluded> labels = userDataIncluded.IncludedList;
            List<Label> userLabels = new List<Label>();
            foreach (var labelAttribute in labels)
            {
                Label newLabel = new Label();
                newLabel.Name = labelAttribute.LabelsAttributes.LabelName;
                newLabel.CategoryName = labelAttribute.LabelsAttributes.CategoryLabelName;
                newLabel.Id = labelAttribute.LabelId;
                newLabel.CategoryId = labelAttribute.LabelsAttributes.CategoryID;
                userLabels.Add(newLabel);

            }
            return userLabels;

        }
        /* deserializes and returns the user(s) with the correct data
         * if-else statement to decide whether to configure a user using the API response that has an array format
         * or just 1 user
         */
        public List<User> getUsersInformation(IRestResponse response, bool oneUser)
        {
            UserDataJson userData;
            UserDataJsonList userDataList;
            List<Label> userLabels = CreateLabelList(response);
            List<User> users = new List<User>();

            if (oneUser)
            {
                User user = new User();
                userData = JsonConvert.DeserializeObject<UserDataJson>(response.Content);
                user = userData.Data.UserAttributes;

                user.ConfigureUserData(userData.Data);
                if (user.Location != null)
                {
                    user.Location = GetLocationById(user.LocationId);
                }
                user.Labels = userLabels;
                users.Add(user);
            }
            else
            {
                userDataList = JsonConvert.DeserializeObject<UserDataJsonList>(response.Content);
                foreach (UserData data in userDataList.Data)
                {
                    User newUser = data.UserAttributes;
                    newUser.ConfigureUserData(data);

                    foreach (Label lab in userLabels)
                    {
                        List<RelationshipData> relationship = data.multipleRelationships.categoryLabels.RelationshipsData;
                        if (relationship != null && relationship.Any())
                        {
                            foreach (RelationshipData labelInfo in relationship)
                            {
                                if (labelInfo.LabelId == lab.Id)
                                {
                                    newUser.Labels.Add(lab);

                                }
                            }
                        }
                    }

                    if (newUser.Location != null)
                    {
                        newUser.Location = GetLocationById(newUser.LocationId);
                    }
                    users.Add(newUser);
                }
            }
            return users;
        }
           
        public User AddUser(User MyUser)
        {
            Console.WriteLine("Adding...");
            IRestRequest request = ConfigureRequest();
            request.Resource = "{version}/admin/registration_sets";
            request.Method = Method.POST;
            request.AddParameter("application/json", API.UserJson(MyUser), ParameterType.RequestBody);

            IRestResponse response = _client.Execute<User>(request);

            checkResponseSuccess(response, 2);

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
            Console.WriteLine("Updating user...");
            RestRequest request = ConfigureRequest();
            request.Resource = "{version}/admin/registration_sets/{id}";
            request.Method = Method.PATCH;
            request.AddParameter("id", MyUser.UserId, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.UserJson(MyUser), ParameterType.RequestBody);
            _client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_token.access_token, _token.token_type);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, 1);

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

        public List<User> GetUserById(string UserId)
        {
            Console.WriteLine("Getting user by ID...");
            RestRequest request = ConfigureRequest();
            request.Resource = "{version}/admin/users/{id}";
            request.Method = Method.GET;
            request.AddParameter("include", "category_labels", ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("id", UserId, ParameterType.UrlSegment);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, 1);

            List<User> user = getUsersInformation(response, true);

            return user;
        }

        public List<User> GetUserByEmail(string UserEmail)
        {
            Console.WriteLine("Getting User(s) with email: " + UserEmail + "...");

            RestRequest request = ConfigureRequest();
            request.Resource = "{version}/admin/users";
            request.Method = Method.GET;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("include", "category_labels", ParameterType.QueryString);
            request.AddParameter("filter[email]", UserEmail, ParameterType.QueryString);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, 1);

            List<User> user = getUsersInformation(response, false);


            return user;
        }

        // TODO: Implement paging in this
        public List<User> GetUsersBySearch(Dictionary<SearchTerms, string> searchTerms)
        {
            Console.WriteLine("Getting user by search...");
            RestRequest request = ConfigureRequest();
            request.Resource = "{version}/admin/users/";
            request.Method = Method.GET;
            request.Parameters.Clear();
            request.AddParameter("include", "category_labels", ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");
            foreach (SearchTerms term in searchTerms.Keys)
            {
                request.AddParameter("filter[" + GetDescription(term) + "]", searchTerms[term], ParameterType.QueryString);
            }

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, 1);

            List<User> users = getUsersInformation(response, false);


            return users;
        }

        public List<User> GetUsers(int page)
        {
            Console.WriteLine("Getting " + returnPerPage + "users on page " + page.ToString() + "...");

            RestRequest request = ConfigureRequest();
            request.Resource = "/{version}/admin/users/";
            request.Method = Method.GET;
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("page[page]", currPage, ParameterType.QueryString);
            request.AddParameter("include", "category_labels", ParameterType.QueryString);
            request.AddParameter("page[per_page]", returnPerPage, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, 1);

            List<User> users = getUsersInformation(response, false);

            return users;

        }

        public (List<User>, bool) GetUsers()
        {
            bool returnValue = true;
            Console.WriteLine("Getting users...");
            RestRequest request = ConfigureRequest();
            request.Resource = "{version}/admin/users/";
            request.Method = Method.GET;
            request.Parameters.Clear();
            request.AddParameter("page[page]", currPage, ParameterType.QueryString);
            request.AddParameter("page[per_page]", returnPerPage, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, 1);

            List<User> users = getUsersInformation(response, false);

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
            Console.WriteLine("Getting user count...");
            RestRequest request = ConfigureRequest();
            request.Resource = "/{version}/admin/users/?page[page]={page_num}&page[per_page]={num_per}";
            request.Method = Method.GET;
            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("page_num", 1, ParameterType.UrlSegment);
            request.AddParameter("num_per", returnPerPage, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, 1);

            MetaJson metaData = JsonConvert.DeserializeObject<MetaJson>(response.Content);

            return metaData.Meta.Count;
        }

    }

}