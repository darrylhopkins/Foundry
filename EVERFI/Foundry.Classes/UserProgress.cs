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

    //"next" header in response
    internal class NextUserData
    {
        [JsonProperty("next")]
        internal NextData Next { get; set; }
    }

    //everything under "next" in response
    internal class NextData
    {
        [JsonProperty("since")]
        internal DateTime Since { get; set; }

        [JsonProperty("scroll_id", NullValueHandling = NullValueHandling.Ignore)]
        internal string ScrollId { get; set; }

        [JsonProperty("scroll_size")]
        internal int ScrollSize { get; set; }
        
       [JsonProperty("href")]
        internal string HRef { get; set; }
    }

    //"data" header for data array in response
    internal class UserProgressDataHeaderList
    {
        [JsonProperty("data")]
        internal List<UserProgress> ProgressDataHeaderList { get; set; }

    }

    internal class UserProgressDataHeader
    {

        [JsonProperty("id")]
        internal string ProgressID { get; set; }

        [JsonProperty("name")]
        internal string CourseName { get; set; }

        [JsonProperty("due_on")]
        internal DateTime DueOn { get; set; }

        [JsonProperty("content_id")]
        internal string ContentID { get; set; }

        [JsonProperty("started_at", NullValueHandling = NullValueHandling.Ignore)]
        internal DateTime StartedAt { get; set; }

        [JsonProperty("completed_at", NullValueHandling = NullValueHandling.Ignore)]
        internal DateTime CompletedAt { get; set; }

        [JsonProperty("content_status", NullValueHandling = NullValueHandling.Ignore)]
        internal string ContentStatus { get; set; }

        [JsonProperty("last_progress_at", NullValueHandling = NullValueHandling.Ignore)]
        internal string LastProgressAt { get; set; }

        [JsonProperty("percent_completed")]
        internal float PercentCompleted { get; set; }




    }
    internal class UserDataShortened
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("email")]
        public string Email;

        [JsonProperty("active")]
        public Boolean Active { get; set; }

        [JsonProperty("sso_id")]
        public string SSOId;

        [JsonProperty("first_name", Required = Required.Always)]
        public string FirstName { get; set; }

        [JsonProperty("last_name", Required = Required.Always)]
        public string LastName { get; set; }

        [JsonProperty("employee_id")]
        public string EmployeeId { get; set; }

        [JsonProperty("student_id")]
        public string StudentId { get; set; }

    }
  

   

    public class AssignmentData
    {
        [JsonProperty("id")]
        public int AssignmentID { get; set; }

        [JsonProperty("name")]
        public string AssignmentName { get; set; }

        [JsonProperty("training_period_id")]
        public string TrainingPeriodID { get; set; }

        [JsonProperty("training_period_name")]
        public string TrainingPeriodName { get; set; }



    }

    public class UserProgress
    {
        [JsonProperty("progress")]
        internal List<UserProgressDataHeader> ProgressList { get; set; }

        [JsonProperty("assignment")]
        public AssignmentData Assignment { get; set; }

        [JsonProperty("user")]
        internal UserDataShortened UserInformation { get; set; }

        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("assigned_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime AssignedAt { get; set; }


       

        public UserProgress()
        {
            
        }
    }
}
