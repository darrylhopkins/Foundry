using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    internal partial class UserData
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    internal partial class Data
    {
        [JsonProperty("id")]
        public string UserId { get; set; }

        [JsonProperty("attributes")]
        public User UserAttributes { get; set; }
    }

    internal partial class ExternalAttributes
    {
        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("first_day_of_work")]
        public DateTime FirstDay { get; set; }

        [JsonProperty("last_day_of_work")]
        public DateTime LastDay { get; set; }
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

        [JsonProperty("location_id")]
        public string LocationId { get; set; }

        /* second registration array */
        public List<UserType> UserTypes { get; set; }

        public string Position { get; set; }

        public DateTime FirstDay { get; set; }

        public DateTime LastDay { get; set; }

        public User()
        {
            this.UserTypes = new List<UserType>();
        }

        public string GetJson()
        {
            string Json = "{\n" +
                "\"data\": {\n" +
                "\"type\": \"registration_sets\",\n" +
                "\"attributes\": {\n" +
                "\"registrations\": [\n" +
                "{\n" +
                "\"rule_set\": \"user_rule_set\",\n" +
                "\"first_name\": \"" + this.FirstName + "\",\n" +
                "\"last_name\": \"" + this.LastName + "\",\n" +
                "\"email\": \"" + this.Email + "\"";

            if (this.SingleSignOnId != null)
            {
                Json += "\"sso_id\": \"" + this.SingleSignOnId + "\"";
            }
            if (this.EmployeeId != null)
            {
                Json += ",\n\"employee_id\": \"" + this.EmployeeId + "\"";
            }
            if (this.StudentId != null)
            {
                Json += ",\n\"student_id\": \"" + this.StudentId + "\"";
            }
            Json += ",\n\"location_id\": \"" + this.LocationId + "\"" +
                "\n}";

            for (var i = 0; i < UserTypes.Count; i++)
            {
                Json += ",\n{\n" +
                "\"rule_set\": \"" + Foundry.UserType.GetDescription(this.UserTypes.ElementAt(i).Type) + "\",\n" +
                "\"role\": \"" + Foundry.UserType.GetDescription(this.UserTypes.ElementAt(i).Role) + "\"";
                if (i == 0)
                {
                    if (this.Position != null)
                    {
                        Json += ",\n\"position\": \"" + this.Position + "\"";
                    }
                    if (!this.FirstDay.Equals(DateTime.MinValue))
                    {
                        Json += ",\n\"first_day_of_work\": \"" + this.FirstDay + "\"";
                    }
                    if (!this.LastDay.Equals(DateTime.MinValue))
                    {
                        Json += ",\n\"last_day_of_work\": \"" + this.LastDay + "\"";
                    }
                }

                Json += "\n}";
            }

            Json += "\n";

            Json += "]\n}\n}\n}";

            return Json;
        }

        internal void ConfigureUserData()
        {
            this.Position = this.ExternalAttributes.Position;
            this.FirstDay = this.ExternalAttributes.FirstDay;
            this.LastDay = this.ExternalAttributes.LastDay;
            
            foreach (var type in this.TypesDictionary.Keys)
            {
                this.UserTypes.Add(new UserType(UserType.StringToType(type), UserType.StringToRole(this.TypesDictionary[type])));
            }
        }
    }
}
