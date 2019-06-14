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
    public enum Type
    {
        [Description("he_learner")]
        HELearner,
        [Description("he_admin")]
        HEAdmin,
        [Description("fac_staff_learner")]
        FacStaffLearner,
        [Description("fac_staff_admin")]
        FacStaffAdmin,
        [Description("cc_learner")]
        CCLearner,
        [Description("cc_admin")]
        CCAdmin,
        [Description("next_learner")]
        AdultFinancialLearner,
        [Description("at_work_manager")]
        AdultFinancialManager,
        [Description("event_volunteer")]
        EventVolunteer,
        [Description("event_manager")]
        EventManager
    };

    public enum Role
    {
        [Description("undergrad")]
        Undergraduate,
        [Description("graduate")]
        Graduate,
        [Description("non_traditional")]
        NonTraditional,
        [Description("greek")]
        Greek,
        [Description("primary")]
        Primary,
        [Description("supervisor")]
        Supervisor,
        [Description("non_supervisor")]
        NonSupervisor,
        [Description("default")]
        Default
    };
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
        public Type UserType { get; set; }

        public Role UserRole { get; set; }

        public bool IsAdmin { get; set; }

        [DeserializeAs(Name = "position")]
        public string Position { get; set; }

        [DeserializeAs(Name = "first_day_of_work")]
        public DateTime FirstDay { get; set; }

        [DeserializeAs(Name = "last_day_of_work")]
        public DateTime LastDay { get; set; }

        public string GetDescription(Enum value)
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
                "\n},\n";

            Json += "{\n" +
                "\"rule_set\": \"" + this.GetDescription(UserType) + "\",\n" +
                "\"role\": \"" + this.GetDescription(UserRole) + "\"";
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

            Json += "\n}";

            if (this.IsAdmin)
            {
                Json += ",\n" +
                    "{\n" +
                    "\"rule_set\": \"" + this.GetAdmin(UserType) + "\",\n" +
                    "\"role\": \"primary\"\n" +
                    "}";
            }

            Json += "\n";

            Json += "]\n}\n}\n}";

            return Json;
        }

        private string GetAdmin(Type type)
        {
            switch (type)
            {
                case Type.HELearner:
                    return "he_admin";
                case Type.FacStaffLearner:
                    return "fac_staff_admin";
                case Type.CCLearner:
                    return "cc_admin";
                case Type.AdultFinancialLearner:
                    return "at_work_manager";
                case Type.EventVolunteer:
                    return "event_manager";
                default:
                    return "";
            }
        }
    }
}
