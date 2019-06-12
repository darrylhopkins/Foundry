using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    public class User
    {
        /* what falls under user_rule_set */
        [DeserializeAs(Name = "first_name")]
        public string FirstName { get; set; }

        [DeserializeAs(Name = "last_name")]
        public string LastName { get; set; }

        [DeserializeAs(Name = "email")]
        public string Email { get; set; }

        [DeserializeAs(Name = "sso_id")]
        public string SingleSignOnId { get; set; }

        [DeserializeAs(Name = "employee_id")]
        public string EmployeeId { get; set; }

        [DeserializeAs(Name = "student_id")]
        public string StudentId { get; set; }

        [DeserializeAs(Name = "location_id")]
        public string LocationId { get; set; }

        /* second registration array */
        public List<UserType> Types { get; set; }

        [DeserializeAs(Name = "position")]
        public string Position { get; set; }

        [DeserializeAs(Name = "first_day_of_work")]
        public DateTime FirstDay { get; set; }

        [DeserializeAs(Name = "last_day_of_work")]
        public DateTime LastDay { get; set; }

        /* to indicate if there is another ruleset (admin) */
        public bool IsAdmin { get; set; }

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
                "\"email\": \"" + this.Email + "\",\n" +
                "\"sso_id\": \"" + this.SingleSignOnId + "\",\n" +
                "\"employee_id\": \"" + this.EmployeeId + "\",\n" +
                "\"location_id\": \"" + this.LocationId + "\",\n" +
                "},\n";

            for (var i = 0; i < Types.Count; i++)
            {
                Json += "{\n" +
                    "\"rule_set\": \"" + UserType.GetDescription(Types.ElementAt(i).Type) + "\",\n" +
                    "\"role\": \"" + UserType.GetDescription(Types.ElementAt(i).Role) + "\",\n" +
                    "\"position\": \"" + this.Position + "\",\n" +
                    "\"first_day_of_work\": \"" + this.FirstDay +"\",\n" +
                    "\"last_day_of_work\": \"" + this.LastDay + "\",\n" +
                    "}";

                if (i != Types.Count-1)
                {
                    Json += ",";
                }
                Json += "\n";
            }

            Json += "]\n}\n}\n}";

            return Json;
        }
    }
}
