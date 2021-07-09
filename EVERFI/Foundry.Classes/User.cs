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
    internal class UserDataJsonList
    {

        [JsonProperty("data")]
        internal List<UserData> Data { get; set; }

    }

    internal class UserData
    {
        [JsonProperty("id")]
        internal string UserId { get; set; }

        [JsonProperty("relationships")]
        internal MultRelationships multipleRelationships { get; set; }

        [JsonProperty("attributes")]
        internal User UserAttributes { get; set; }


    }

    internal class UserDataJson
    {
        [JsonProperty("data")]
        internal UserData Data { get; set; }
    }

    internal class UserDataIncludedList
    {

        [JsonProperty("included")]
        internal List<UserDataIncluded> IncludedList { get; set; }
    }
    internal class UserDataIncludedJson
    {
        [JsonProperty("included")]
        internal UserDataIncluded DataIncluded { get; set; }
    }

    internal class UserDataIncluded
    {
        [JsonProperty("id")]
        internal string LabelId { get; set; }

        [JsonProperty("attributes")]
        internal LabelsAttributes LabelsAttributes { get; set; }

    }

    internal class LabelsAttributes
    {
        [JsonProperty("category_id")]
        internal string CategoryID { get; set; }

        [JsonProperty("category_name")]
        internal string CategoryLabelName { get; set; }

        [JsonProperty("name")]
        internal string LabelName { get; set; }
    }

    internal class MultRelationships
    {
        [JsonProperty("category_labels")]
        internal IncludedCategoryDataList categoryLabels { get; set; }
    }


    internal class IncludedCategoryDataList
    {
        [JsonProperty("data")]
        internal List<RelationshipData> RelationshipsData { get; set; }
    }



    internal class Relationships
    {
        [JsonProperty("category")]
        internal IncludedCategoryData CategoryData { get; set; }
    }

    internal class IncludedCategoryData
    {
        [JsonProperty("data")]
        internal RelationshipData RelationshipData { get; set; }
    }

    internal class RelationshipData
    {
        [JsonProperty("id")]
        internal string LabelId { get; set; }
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
        [JsonProperty("active")]
        public Boolean active { get; set; }

        [JsonProperty("first_name", Required = Required.Always)]
        public string FirstName { get; set; }

        [JsonProperty("last_name", Required = Required.Always)]
        public string LastName { get; set; }

        [JsonProperty("email", Required = Required.Always)]
        public string Email { get; set; }

        [JsonProperty("created_at", Required = Required.Always)]
        public DateTime created { get; internal set; }

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string username { get; set; }

        [JsonProperty("sso_id")]
        public string SingleSignOnId { get; set; }

        [JsonProperty("employee_id")]
        public string EmployeeId { get; set; }

        [JsonProperty("student_id")]
        public string StudentId { get; set; }

        [JsonProperty("sign_in_count")]
        public int SignInCount { get; internal set; }

        [JsonProperty("suppress_invites", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean DoesSupreessInvites { get; set; }

        [JsonProperty("suppress_reminders", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean DoesSupreessReminders { get; set; }

        [JsonProperty("category_labels")]
        public List<string> categoryLabels { get; internal set; }

        public Location Location { get; set; }

        [JsonProperty("location_id")]
        public string LocationId { get; internal set; }

        [JsonProperty("parent_email", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentEmail { get; set; }
       
        public List<UserType> UserTypes { get; set; }
        public List<Label> Labels { get; set; }


        public string Position { get; set; }

        public DateTime FirstDay { get; set; }

        public DateTime LastDay { get; set; }


        [JsonProperty("under_13", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsUnderThirteen { get; set; }

        [JsonProperty("last_sign_in_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime lastSignIn { get; set; }

        public string UserId { get; internal set; }


        public User()
        {
            this.UserTypes = new List<UserType>();
            this.Labels = new List<Label>();
            this.categoryLabels = new List<string>();
            //this.TypeUser = new List<UserRuleSetData>();


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

        }
        internal void createCategoryLabels()
        {
            for (var i = 0; i < this.Labels.Count; i++)
            {
                this.categoryLabels.Add(this.Labels.ElementAt(i).Id);
            }
        }
    }
}
