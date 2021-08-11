using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Web;
using System.DirectoryServices;
using System.Json;
using System.Net.Http;

namespace EVERFI.Foundry.Classes
{


    internal class ErrorList
    {
        [JsonProperty("errors")]
        internal List<ErrorContent> ListOfErrors { get; set; }

        [JsonProperty("error")]
        internal string errorMessage { get;   }
    }

   //2 headers for message, need to deserialize seperately
    internal class ErrorContent
    {
        [JsonProperty("field_name")]
        public String FieldName { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public String Message1 { get; set; }

       
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public String Message2 { get; set; }
       
    }
    //error message object - only has 1 message to avoid null messages during deserialization
    public class ErrorMessage
    {
        public String FieldName;
        public String Message;

        public ErrorMessage(String field, String mess)
        {
            this.FieldName = field;
            this.Message = mess;
        }
    }
   

    public class FoundryException : Exception
    {
        public int ErrorCode { get;}

        public string Response { get; }

        public override string Message { get; }

        public List<ErrorMessage> ErrorMessages { get; set; }

        //Creates error message using response 
        internal String ConfigureErrorMessage(String Response)
        {
            String message = "";
            ErrorList ErrorMessageList;
            ErrorContent e = new ErrorContent();
            this.ErrorMessages = new List<ErrorMessage>();


            if (Response.Substring(0, 1).Equals("["))
            {

                List<ErrorList> error = JsonConvert.DeserializeObject<List<ErrorList>>(Response);
                ErrorMessageList = error.ElementAt(0);
               
            }
            else
            {
             
                ErrorMessageList = JsonConvert.DeserializeObject<ErrorList>(Response);
            }
            if (ErrorMessageList.ListOfErrors != null)
            {
                //assigns message - excudes null values that appear during deseraizliation 
                foreach (ErrorContent content in ErrorMessageList.ListOfErrors)
                {
                    //creates new error message for error message list - will only include non null values from ErrorContent
                    ErrorMessage em = new ErrorMessage("", "");
                    em.FieldName = content.FieldName;
                    if (content.Message1 != null)
                    {
                        em.Message = content.Message1;
                        message += content.Message1 + "  ";
                    }

                    if (content.Message2 != null)
                    {
                        em.Message = content.Message2;
                        message += content.Message2 + "  ";

                    }
                    this.ErrorMessages.Add(em);
                }
            }
            else
            {
                message = ErrorMessageList.errorMessage;
               
            }
            return message;
        }
      
       
        public FoundryException(int error, string field, string message)
        {
            this.ErrorMessages = new List<ErrorMessage>();
            this.Message = message;
            this.ErrorCode = error;
            ErrorMessages.Add(new ErrorMessage(field, message));
        }

        public FoundryException(int ErrorCode, string Response)
        {
            this.ErrorCode = ErrorCode;
            this.Response = Response;

            if (Response.Length < 1)
            {
                var description = ((System.Net.HttpStatusCode)ErrorCode).ToString();
                this.Message = description;
            }
            
            else if(!(Response.Substring(0,1).Equals("[")  || Response.Substring(0, 1).Equals("{")))
            {
                this.Message = Response;
            }
            

            else
            {
                this.Message = ConfigureErrorMessage(Response);
            }
           
        }

    }
}