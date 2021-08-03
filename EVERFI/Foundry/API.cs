using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using EVERFI.Foundry.Classes;

namespace EVERFI.Foundry
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
        CreatedAt,
        [Description("sso_id")]
        SingleSignOnId,
        [Description("employee_id")]
        EmployeeId

    }
    internal enum RequestType
    {
        GetRequest,
        PatchRequest,
        PostRequest,
        DeleteRequest

    }

    public partial class API
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

        public API(string accountSid, string secretKey, string BaseUrl)
        {
            _client = new RestClient(BaseUrl);
            _client.Authenticator = new HttpBasicAuthenticator(accountSid, secretKey);
            this.BaseUrl = BaseUrl;

            // set "source" header with various information
            Assembly thisAssem = typeof(Foundry.API).Assembly;
            Type myType = typeof(Foundry.API);
            AssemblyName thisAssemName = thisAssem.GetName();
            Version ver = thisAssemName.Version;

            var header_source = "client_id:" + accountSid + ";" +
                "Assembly:" + thisAssemName.Name + ";" +
                "Namespace:" + myType.Namespace + ";" +
                "Source:Foundry API SDK;" +
                "Version:" + ver;

            _client.AddDefaultHeader("Source", header_source);

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
                throw new FoundryException((int)response.StatusCode, response.Content);
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
            else if (request  == RequestType.DeleteRequest)
            {
                if (numericCode != 204)
                {
                    throw new FoundryException(numericCode, response.Content);
                }

            }

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

        internal static string UserJson(User user) //Change to internal when done
        {
            string Json = "{\n" +
                "\"data\": {\n" +
                "\"type\": \"registration_sets\",\n";

            // add "id" for updates
            if(!string.IsNullOrEmpty(user.UserId))
            {
                Json += "\"id\": \"" + user.UserId + "\",\n";
            }

            Json+=
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
                Json += ",\n\"location_id\": \"" + user.Location.Id + "\"";
            }
            else if (user.LocationId != null)
            {
                Json += ",\n\"location_id\": \"" + user.LocationId + "\"" ;
            }
           
            Json += ",\n\"category_labels\":" + "[\n";
           for (var i = 0; i < user.Labels.Count; i++)
           {
               Json += "\"" + user.Labels.ElementAt(i).Id + "\"";
               if ((i + 1) != user.Labels.Count)
               {
                   Json += ",";
               }
           }
           Json += "\n]";

           Json += "\n}";
            
            for (var i = 0; i < user.UserTypes.Count; i++)
            {
                Json += ",\n{\n" +
                "\"rule_set\": \"" + UserType.GetDescription(user.UserTypes.ElementAt(i).Type) + "\",\n" +
                "\"role\": \"" + UserType.GetDescription(user.UserTypes.ElementAt(i).Role) + "\"";
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
                "\t\t\"address_longitude\": \"" + location.Longitude + "\",\n" +
                "\t\t\"address_name\": \"" + location.AddressName + "\",\n" +
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
                if (i != users.Count - 1)
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
