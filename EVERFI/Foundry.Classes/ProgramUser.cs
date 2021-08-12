using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EVERFI.Foundry.Classes
{
    //"data" header for data array in response
    internal class ProgramUserDataHeaderList
    {
        [JsonProperty("data")]
        internal List<ProgramUser> ProgressDataHeaderList { get; set; }

    }
    internal class UserProgramData
    {
        [JsonProperty("id")]
        internal String Id { get; set; }

        [JsonProperty("type")]
        internal String Type { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        internal String Email { get; set; }

        [JsonProperty("last_name")]
        internal String LastName { get; set; }

        [JsonProperty("first_name")]
        internal String FirstName { get; set; }

        [JsonProperty("foundry_user_id", NullValueHandling = NullValueHandling.Ignore)]
        internal String FoundryUserId { get; set; }
    }

    internal class ProgramData
    {
        [JsonProperty("id")]
        internal String Id { get; set; }

        [JsonProperty("name")]
        internal String Name { get; set; }
    }
    internal class ResponseData
    {
       
        [JsonProperty("id")]
        internal String Id { get; set; }

        [JsonProperty("answer_text")]
        internal String AnswerText { get; set; }

        [JsonProperty("question_id")]
        internal int QuestionId { get; set; }

        [JsonProperty("question_text")]
        internal String QuestionText { get; set; }

    }
    internal class UserContent
    {
        [JsonProperty("id")]
        internal int Id { get; set; }

        [JsonProperty("state")]
        internal String State { get; set; }

        [JsonProperty("opened_at", NullValueHandling = NullValueHandling.Ignore)]
        internal DateTime OpenedAt { get; set; }

        [JsonProperty("started_at", NullValueHandling = NullValueHandling.Ignore)]
        internal DateTime StartedAt { get; set; }

        [JsonProperty("completed_at", NullValueHandling = NullValueHandling.Ignore)]
        internal DateTime CompletedAt { get; set; }

        [JsonProperty("content_name")]
        internal String ContentName { get; set; }
    }
    internal class IncentiveActivity
    {

        [JsonProperty("id")]
        internal int Id { get; set; }

        [JsonProperty("status")]
        internal String Status { get; set; }

        [JsonProperty("completion", NullValueHandling = NullValueHandling.Ignore)]
        internal String Completion { get; set; }

        [JsonProperty("incentive_id")]
        internal int IncentiveId { get; set; }

        [JsonProperty("start_datetime")]
        internal DateTime StartDateTime { get; set; }

        [JsonProperty("incentive_title")]
        internal String IncentiveTitle { get; set; }

        [JsonProperty("incentive_playlist_id")]
        internal String IncentivePlaylistId { get; set; }

        [JsonProperty("incentive_playlist_namee")]
        internal String IncentivePlaylistName { get; set; }

    }
    public class ProgramUser
    {
        //id, user, deleted, program, responses, updated at, user content, incentive activities
        [JsonProperty("id")]
        internal String Id { get; set; }

        [JsonProperty("user")]
        internal UserProgramData User { get; set; }

        [JsonProperty("deleted")]
        internal Boolean Deleted { get; set; }

        [JsonProperty("program")]
        internal ProgramData Program { get; set; }

        [JsonProperty("reponses")]
        internal List<ResponseData>  Responses { get; set; }

        [JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
        internal DateTime UpdatedAt { get; set; }

        [JsonProperty("user_content")]
        internal List<UserContent> UserContents { get; set; }

        [JsonProperty("incentive_activites")]
        internal List<IncentiveActivity> IncentiveActivities { get; set; }

        public ProgramUser()
        {
        }
    }
}
