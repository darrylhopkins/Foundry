using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EVERFI.Foundry.Classes
{
    internal class MetaJson
    {
        [JsonProperty("meta")]
        internal MetaData Meta { get; set; }
    }

    internal class MetaData
    {
        [JsonProperty("total_count")]
        internal Int32 Count { get; set; }
    }

    internal class UserDataJson
    {
        [JsonProperty("data")]
        internal UserData Data { get; set; }
        [JsonProperty("included")]
        internal UserIncludedData Included { get; set; }

    }

    internal class UserDataJsonList
    {
        [JsonProperty("data")]
        internal List<UserData> Data { get; set; }
    }

    internal class UserData
    {
        [JsonProperty("id")]
        internal string UserId { get; set; }

        [JsonProperty("attributes")]
        internal User UserAttributes { get; set; }
    }

    internal class ExternalAttributes
    {
        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        internal string Position { get; set; }

        [JsonProperty("first_day_of_work", NullValueHandling = NullValueHandling.Ignore)]
        internal DateTime FirstDay { get; set; }

        [JsonProperty("last_day_of_work", NullValueHandling = NullValueHandling.Ignore)]
        internal DateTime LastDay { get; set; }
    }

    public class User
    {
        [JsonProperty("external_attributes")]
        private ExternalAttributes ExternalAttributes { get; set; }

        [JsonProperty("user_types")]
        private Dictionary<string, string> TypesDictionary { get; set; }

        /* user_rule_set */
        [JsonProperty("first_name", Required = Required.Always)]
        public string FirstName { get; set; }

        [JsonProperty("last_name", Required = Required.Always)]
        public string LastName { get; set; }

        [JsonProperty("email", Required = Required.Always)]
        public string Email { get; set; }

        [JsonProperty("sso_id")]
        public string SingleSignOnId { get; set; }

        [JsonProperty("employee_id")]
        public string EmployeeId { get; set; }

        [JsonProperty("student_id")]
        public string StudentId { get; set; }

        public Location Location { get; set; }

        [JsonProperty("location_id")]
        public string LocationId { get; internal set; }

        /* second registration array */
        public List<UserType> UserTypes { get; set; }

        public List<Label> Labels { get; set; }

        public string Position { get; set; }

        public DateTime FirstDay { get; set; }

        public DateTime LastDay { get; set; }

        public string UserId { get; internal set; }

        public User()
        {
            this.UserTypes = new List<UserType>();
        }

        internal void ConfigureUserData(UserData data)
        {
            this.Position = this.ExternalAttributes.Position;
            this.FirstDay = this.ExternalAttributes.FirstDay;
            this.LastDay = this.ExternalAttributes.LastDay;

            this.UserId = data.UserId;
            
            foreach (var type in this.TypesDictionary.Keys)
            {
                this.UserTypes.Add(new UserType(UserType.StringToType(type), UserType.StringToRole(this.TypesDictionary[type])));
            }

            var l = new Label();
            l.Id = "33";
            l.Name = "xxxzzz";
            l.CategoryId = "44";
            l.UserCount = 32;
            this.Labels.Add(l);

        }
    }
}
