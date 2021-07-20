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
        internal List<UserProgressDataHeader> ProgressDataHeaderList { get; set; }

        
    }
    internal class UserProgressDataHeader
    {
        [JsonProperty("id")]
        internal int ID { get; set; }

        [JsonProperty("progress")]
        internal List<UserProgress> ProgressList { get; set; }

        [JsonProperty("assignment")]
        internal AssignmentData Assignment { get; set; }

        [JsonProperty("user")]
        internal User UserInformation { get; set; }

        [JsonProperty("assigned_at")]
        internal DateTime AssignedAt { get; set; }

    }
    /*
    internal class UserProgressDataList
    {
        [JsonProperty("progress")]
        internal List<UserProgressData> ProgressList { get; set; }
    }
    */
    /*
    internal class UserProgressData
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
        internal int PercentCompleted { get; set; }

    }
    */

    internal class AssignmentData
    {
        [JsonProperty("id")]
        internal int AssignmentID { get; set; }

        [JsonProperty("name")]
        internal string AssignmentName { get; set; }

        [JsonProperty("training_period_id")]
        internal string TrainingPeriodID { get; set; }

        [JsonProperty("training_period_name")]
        internal string TrainingPeriodName { get; set; }



    }

    public class UserProgress
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
        internal int PercentCompleted { get; set; }

        public UserProgress()
        {
            
        }
    }
}
