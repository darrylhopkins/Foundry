using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    public enum SearchTerms
    {
        [Description("email")]
        Email,
        [Description("first_name")]
        FirstName,
        [Description("last_name")]
        LastName,
        [Description("full_name")]
        FullName,
        [Description("groups")]
        Groups,
        [Description("legacy_import_id")]
        LegacyImportId,
        [Description("location_id")]
        LocationId,
        [Description("organization_id")]
        OrganizationId,
        [Description("roster_ids")]
        RosterIds,
        [Description("rule_set_names")]
        RuleSetNames,
        [Description("rule_set_roles")]
        RuleSetRoles,
        [Description("is_learner")]
        IsLearner,
        [Description("is_manager")]
        IsManager,
        [Description("active")]
        Active,
        [Description("student_id")]
        StudentId,
        [Description("business_lines")]
        BusinessLines,
        [Description("custom_grouping_values")]
        CustomGroupingValues,
        [Description("created_at")]
        CreatedAt
    }

    public class API
    {
        private string BaseUrl;
        const string _ver = "v1";

        const int bulkActionCap = 500;
        const int returnPerPage = 100;
        int currPage;

        readonly IRestClient _client;
        readonly AccessToken _token;

        internal List<Location> FoundryLocations;

        string _accountSid;

        // TODO: Add try catch to make sure id and key are valid
        public API(string accountSid, string secretKey, string BaseUrl)
        {
            _client = new RestClient(BaseUrl);
            _client.Authenticator = new HttpBasicAuthenticator(accountSid, secretKey);
            this.BaseUrl = BaseUrl;

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
            currPage = 1;
            FoundryLocations = GetLocations();
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

        // Users:
        public User AddUser(User MyUser) // Return given user if invalid add
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

        public User UpdateUser(User MyUser) // Return exception if invalid update
        {

            RestRequest request = new RestRequest("{version}/admin/registration_sets/{id}", Method.PATCH);

            request.Parameters.Clear();
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", MyUser.UserId, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.UserJson(MyUser), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

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

        // Locations:
        public Location AddLocation(Location MyLocation)
        {
            Console.WriteLine("Adding location " + MyLocation.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/locations", Method.POST);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.LocationJson(MyLocation), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 201)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            //verify if adding was okay with status code

            LocationDataJson locationData = JsonConvert.DeserializeObject<LocationDataJson>(response.Content);
            Console.WriteLine("Location successfully added.");

            Location location = locationData.LocationData.LocationAttributes;
            location.Id = locationData.LocationData.Id;

            FoundryLocations.Add(location);
            return location;
        }

        public Location UpdateLocation(Location MyLocation)
        {
            Console.WriteLine("Updating location " + MyLocation.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/locations/{location_id}", Method.PATCH);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("location_id", MyLocation.Id, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.LocationJson(MyLocation), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            LocationDataJson locationData = JsonConvert.DeserializeObject<LocationDataJson>(response.Content);
            Location location = locationData.LocationData.LocationAttributes;
            location.AddIdFromData(locationData.LocationData);

            FoundryLocations.Remove(MyLocation);
            FoundryLocations.Add(location);

            return location;
        }

        public List<Location> GetLocations()
        {
            RestRequest request = new RestRequest("/{version}/admin/locations");
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

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
            RestRequest request = new RestRequest("/{version}/admin/locations/{location_id}", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("location_id", LocationId, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            LocationDataJson locationData = JsonConvert.DeserializeObject<LocationDataJson>(response.Content);
            Location location = locationData.LocationData.LocationAttributes;
            location.AddIdFromData(locationData.LocationData);

            return location;
        }

        // Categories:
        public Category AddCategory(Category MyCategory)
        {
            Console.WriteLine("Adding category " + MyCategory.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/categories", Method.POST); //TODO
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.CategoryJson(MyCategory), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 201)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            CategoryData categoryData = JsonConvert.DeserializeObject<CategoryData>(response.Content);
            Console.WriteLine("Category successfully added.");

            return categoryData.Data;
        }

        public Category GetCategoryById(string CategoryId, bool WithLabels) // Should we always return with List<Label>?
        {
            Console.WriteLine("Getting category " + CategoryId + "...");

            RestRequest request = new RestRequest("/{version}/admin/categories/{id}", Method.GET); //TODO
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", CategoryId, ParameterType.UrlSegment);
            if (WithLabels)
            {
                request.AddParameter("include", "category_labels", ParameterType.QueryString);
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

            CategoryData categoryData = JsonConvert.DeserializeObject<CategoryData>(response.Content);

            Category category = categoryData.Data;

            category.ConfigureCategory();
            if (WithLabels)
            {
                for (int i = 0; i < category.Labels.Count; i++)
                {
                    category.Labels[i] = GetLabelById(category.Labels[i].Id);
                }
            }
            /*else
            {
                category.Labels.Clear();
            }*/

            return category;
        }

        public List<Category> GetCategories(bool WithLabels)
        {
            Console.WriteLine("Getting categories...");

            RestRequest request = new RestRequest("/{version}/admin/categories", Method.GET); //TODO
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            if (WithLabels)
            {
                request.AddParameter("include", "category_labels", ParameterType.QueryString);
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

            CategoryListData categoryData = JsonConvert.DeserializeObject<CategoryListData>(response.Content);
            List<Category> categories = new List<Category>();

            foreach (Category category in categoryData.Data)
            {
                category.ConfigureCategory();
                if (WithLabels)
                {
                    for (int i = 0; i < category.Labels.Count; i++)
                    {
                        category.Labels[i] = GetLabelById(category.Labels[i].Id);
                    }
                }
                /*else
                {
                    category.Labels.Clear();
                }*/
                categories.Add(category);
            }
            return categories;
        }

        public Category UpdateCategory(Category MyCategory)
        {
            Console.WriteLine("Updating category...");

            RestRequest request = new RestRequest("/{version}/admin/categories/{id}", Method.PATCH);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", MyCategory.Id, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.CategoryJson(MyCategory), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            CategoryData categoryData = JsonConvert.DeserializeObject<CategoryData>(response.Content);
            Console.WriteLine("Category successfully updated.");
            Category category = categoryData.Data;

            category.ConfigureCategory();

            return category;
        }

        public string DeleteCategory(Category MyCategory)
        {
            Console.WriteLine("Deleting category " + MyCategory.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/categories/{id}", Method.DELETE);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", MyCategory.Id, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 204)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            Console.WriteLine("Category successfully deleted.");

            return response.Content;
        }

        // Labels:
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

        // Category Label Users:
        // Should there be one for individual users
        public BulkAssignJob BulkAssignLabels(List<User> usersList, Label label)
        {
            if (usersList.Count > bulkActionCap)
            {
                throw new FoundryException("The limit for the bulk add function is 500 users!");
            }

            Console.WriteLine("Assigning " + label.Name + " to users provided.");

            RestRequest request = new RestRequest("/{version}/admin/bulk_actions/category", Method.POST);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.BulkUserLabelJson(usersList, label), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 201)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            JobJson jobJson = JsonConvert.DeserializeObject<JobJson>(response.Content);
            BulkAssignJob job = jobJson.BulkAssignJob;

            Console.WriteLine("Labels added to " + usersList.Count + " users.");

            return job;
        }

        public BulkAssignJob GetJobById(string JobId)
        {
            RestRequest request = new RestRequest("/{version}/admin/bulk_actions/category/{id}", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", JobId, ParameterType.UrlSegment);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            HttpStatusCode statusCode = response.StatusCode;
            int numericCode = (int)statusCode;

            if (numericCode != 200)
            {
                throw new FoundryException(response.ErrorMessage, numericCode, response.Content);
            }

            JobJson jobJson = JsonConvert.DeserializeObject<JobJson>(response.Content);
            BulkAssignJob job = jobJson.BulkAssignJob;

            return job;
        }

        // Internal Static Methods:
        internal static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description
                ?? value.ToString();
        }

        public static string UserJson(User user) //Change to internal when done
        {
            string Json = "{\n" +
                "\"data\": {\n" +
                "\"type\": \"registration_sets\",\n" +
                "\"attributes\": {\n" +
                "\"registrations\": [\n" +
                "{\n" +
                "\"rule_set\": \"user_rule_set\",\n" +
                "\"first_name\": \"" + user.FirstName + "\",\n" +
                "\"last_name\": \"" + user.LastName + "\",\n" +
                "\"email\": \"" + user.Email + "\"";

            if (user.SingleSignOnId != null)
            {
                Json += ",\n\"sso_id\": \"" + user.SingleSignOnId + "\"";
            }
            if (user.EmployeeId != null)
            {
                Json += ",\n\"employee_id\": \"" + user.EmployeeId + "\"";
            }
            if (user.StudentId != null)
            {
                Json += ",\n\"student_id\": \"" + user.StudentId + "\"";
            }

            if (user.Location != null)
            {
                Json += ",\n\"location_id\": \"" + user.Location.Id + "\"" +
                "\n}";
            }
            else if (user.LocationId != null)
            {
                Json += ",\n\"location_id\": \"" + user.LocationId + "\"" +
                "\n}";
            }
            else
            {
                Json += "\n}";
            }

            for (var i = 0; i < user.UserTypes.Count; i++)
            {
                Json += ",\n{\n" +
                "\"rule_set\": \"" + Foundry.UserType.GetDescription(user.UserTypes.ElementAt(i).Type) + "\",\n" +
                "\"role\": \"" + Foundry.UserType.GetDescription(user.UserTypes.ElementAt(i).Role) + "\"";
                if (i == 0)
                {
                    if (user.Position != null)
                    {
                        Json += ",\n\"position\": \"" + user.Position + "\"";
                    }
                    if (!user.FirstDay.Equals(DateTime.MinValue))
                    {
                        Json += ",\n\"first_day_of_work\": \"" + user.FirstDay + "\"";
                    }
                    if (!user.LastDay.Equals(DateTime.MinValue))
                    {
                        Json += ",\n\"last_day_of_work\": \"" + user.LastDay + "\"";
                    }
                }

                Json += "\n}";
            }

            Json += "\n";

            Json += "]\n}\n}\n}";

            return Json;
        }

        internal static string LocationJson(Location location) //Change to internal when done
        {
            string Json = "{\n" +
                "\"data\": {\n";
            if (location.Id != null)
            {
                Json += "\t\"id\": \"" + location.Id + "\",\n";
            }

            Json += "\t\"type\": \"locations\",\n" +
                "\t\"attributes\": {\n" +
                "\t\t\"name\": \"" + location.Name + "\",\n" +
                "\t\t\"external_id\": \"" + location.ExternalId + "\",\n" +
                "\t\t\"contact_email\": \"" + location.ContactEmail + "\",\n" +
                "\t\t\"contact_name\": \"" + location.ContactName + "\",\n" +
                "\t\t\"contact_phone\": \"" + location.ContactPhone + "\",\n" +
                "\t\t\"address_street_number\": \"" + location.StreetNumber + "\",\n" +
                "\t\t\"address_route\": \"" + location.Route + "\",\n" +
                "\t\t\"address_neighborhood\": \"" + location.Neighborhood + "\",\n" +
                "\t\t\"address_locality\": \"" + location.City + "\",\n" +
                "\t\t\"address_administrative_area_level_1\": \"" + location.State + "\",\n" +
                "\t\t\"address_administrative_area_level_2\": \"" + location.County + "\",\n" +
                "\t\t\"address_postal_code\": \"" + location.PostalCode + "\",\n" +
                "\t\t\"address_country\": \"" + location.Country + "\",\n" +
                "\t\t\"address_latitude\": \"" + location.Latitude + "\",\n" +
                "\t\t\"address_longitude\": \"" + location.Longitude + "\"\n" +
                "\t\t\"address_name\": \"" + location.AddressName + "\"\n" +
                "\t\t\"address_room\": \"" + location.AddressRoom + "\"\n" +
                "\t}\n}\n}\n";

            return Json;
        }

        internal static string CategoryJson(Category category)
        {
            string Json = "{\n" +
                "\"data\": {\n";
            if (category.Id != null)
            {
                Json += "\t\"id\": \"" + category.Id + "\",\n";
            }

            Json += "\t\"type\": \"categories\",\n" +
                "\t\"attributes\": {\n" +
                "\t\t\"name\": \"" + category.Name + "\"\n" +
                "\t}\n" +
                "}\n}";

            return Json;
        }

        internal static string LabelJson(Category category, Label label)
        {
            string Json = "{\n" +
                "\"data\": {\n";
            if (label.Id != null)
            {
                Json += "\t\"id\": \"" + label.Id + "\",\n";
            }

            Json += "\t\"type\": \"category_labels\",\n" +
                "\t\"attributes\": {\n" +
                "\t\t\"name\": \"" + label.Name + "\"";
            if (category != null)
            {
                Json += ",\n\t\t\"category_id\": \"" + category.Id + "\"\n";
            }
                Json += "\t}\n" +
                "}\n}";

            return Json;
        }

        internal static string BulkUserLabelJson(List<User> users, Label label)
        {
            string Json = "{\n" +
                "\"data\": {\n" +
                "\"type\": \"bulk_action_categories\",\n" +
                "\"attributes\": {\n" +
                "\"user_ids\": [\n";

            for (int i = 0; i < users.Count; i++)
            {
                Json += "\"" + users.ElementAt(i).UserId + "\"";
                if (i != users.Count-1)
                {
                    Json += ",";
                }
                Json += "\n";
            }

            Json += "],\n" +
                "\"category_label\": \"" + label.Name + "\",\n" +
                "\"category_id\": \"" + label.CategoryId + "\"\n" +
                "}\n" +
                "}\n" +
                "}";

            return Json;
        }
    }
}