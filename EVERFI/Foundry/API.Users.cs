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
        internal RestRequest ConfigureRequest()
        {
            RestRequest request = new RestRequest();
            request.Parameters.Clear();
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            return request;

        }
        internal enum RequestType
        {
            GetRequest,
            PatchRequest,
            PostRequest

        }
        internal void checkResponseSuccess(IRestResponse response, RequestType request)
        {

            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;
            if (request == RequestType.GetRequest || request == RequestType.PatchRequest)
            {
                if (numericCode != 200)
                {
                    throw new FoundryException(numericCode, response.Content);
                }
            }
            else if (request == RequestType.PostRequest)
            {
                if (numericCode != 201)
                {
                    throw new FoundryException(numericCode, response.Content);
                }
            }

        }
        public List<Label> CreateLabelList(IRestResponse response)
        {
            UserDataIncludedList userDataIncluded = JsonConvert.DeserializeObject<UserDataIncludedList>(response.Content);
            List<UserDataIncluded> labels = userDataIncluded.IncludedList;
            List<Label> userLabels = new List<Label>();
            if (labels != null && labels.Any())
            {
                foreach (var labelAttribute in labels)
                {
                    Label newLabel = new Label();
                    newLabel.Name = labelAttribute.LabelsAttributes.LabelName;
                    newLabel.CategoryName = labelAttribute.LabelsAttributes.CategoryLabelName;
                    newLabel.Id = labelAttribute.LabelId;
                    newLabel.CategoryId = labelAttribute.LabelsAttributes.CategoryID;
                    userLabels.Add(newLabel);

                }
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
                user.createCategoryLabels();
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
                    newUser.createCategoryLabels();
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

            IRestResponse response = _client.Execute<List<User>>(request);

            checkResponseSuccess(response, RequestType.PostRequest);

            UserDataJson userData = JsonConvert.DeserializeObject<UserDataJson>(response.Content);
            Console.WriteLine("User successfully added.");

            User user = userData.Data.UserAttributes;
            

            if (user.Location != null)
            {
                user.Location = GetLocationById(user.LocationId);
            }
            
            user.ConfigureUserData(userData.Data);
            user.createCategoryLabels();
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
            checkResponseSuccess(response, RequestType.PatchRequest);

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
            Console.WriteLine("Getting user by ID...");
            RestRequest request = ConfigureRequest();
            request.Resource = "{version}/admin/users/{id}";
            request.Method = Method.GET;
            request.AddParameter("include", "category_labels", ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("id", UserId, ParameterType.UrlSegment);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, RequestType.GetRequest);

            List<User> users = getUsersInformation(response, true);
            User user = users[0];
            

            return user;
        }

        public User GetUserByEmail(string UserEmail)
        {
            Console.WriteLine("Getting User(s) with email: " + UserEmail + "...");

            RestRequest request = ConfigureRequest();
            request.Resource = "{version}/admin/users";
            request.Method = Method.GET;
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("include", "category_labels", ParameterType.QueryString);
            request.AddParameter("filter[email]", UserEmail, ParameterType.QueryString);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, RequestType.GetRequest);

            List<User> users = getUsersInformation(response, false);
            if (!users.Any())
            {
                throw new FoundryException(422, "email", "This email does not exist");
            }

            User user = users[0];

            return user;
            
        }

        // TODO: Implement paging in this
        public List<User> GetUsersBySearch(Dictionary<SearchTerms, string> searchTerms)
        {
            Console.WriteLine("Getting user by search...");

            RestRequest request = new RestRequest("{version}/admin/users/", Method.GET);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            foreach (SearchTerms term in searchTerms.Keys)
            {
                request.AddParameter("filter[" + GetDescription(term) + "]", searchTerms[term], ParameterType.QueryString);
            }
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            request.AddParameter("include", "category_labels", ParameterType.QueryString);
           
            IRestResponse response = _client.Execute(request);
            Console.WriteLine(response);
            checkResponseSuccess(response, RequestType.GetRequest);

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
            checkResponseSuccess(response, RequestType.GetRequest);

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
            checkResponseSuccess(response, RequestType.GetRequest);

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
            checkResponseSuccess(response, RequestType.GetRequest);

            MetaJson metaData = JsonConvert.DeserializeObject<MetaJson>(response.Content);

            return metaData.Meta.Count;
        }

        public UserProgress GetUserProgress(DateTime sinceDate, int scrollSize)
        {
            List<UserProgress> userProgressesList = new List<UserProgress>();
            RestRequest request = new RestRequest();
            request.Method = Method.GET;
            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.Resource = "{version}/progress/user_assignments";
            request.AddParameter("since", sinceDate, ParameterType.QueryString);
            request.AddParameter("scroll_size", scrollSize, ParameterType.QueryString);
            request.AddHeader("Content-Type", "application/json");

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, RequestType.GetRequest);

            UserProgressDataHeaderList progressData = JsonConvert.DeserializeObject<UserProgressDataHeaderList>(response.Content);
            foreach(UserProgressDataHeader data in progressData.ProgressDataHeaderList)
            {
                foreach(UserProgress list in data.ProgressList)
                {
                    userProgressesList.Add(list);
                }
            }

            UserProgress userProgress = userProgressesList[0];

            return userProgress;

        }

    }

}