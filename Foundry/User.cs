using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        public List<UserType> UserTypes { get; set; }

        [DeserializeAs(Name = "position")]
        public string Position { get; set; }

        [DeserializeAs(Name = "first_day_of_work")]
        public DateTime FirstDay { get; set; }

        [DeserializeAs(Name = "last_day_of_work")]
        public DateTime LastDay { get; set; }

        // In default constructor, initialize UserTypes list

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
    }
}
