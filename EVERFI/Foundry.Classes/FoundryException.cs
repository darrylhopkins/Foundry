using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;

namespace EVERFI.Foundry.Classes
{
    

    internal class ErrorList
    {
        [JsonProperty("errors")]
        internal List<ErrorContent> ListOfErrors { get; set; }
    }

    internal class ErrorContent
    {
        [JsonProperty("title")]
        public String Message1 { get; set; }

        [JsonProperty("message")]
        public String Message2 { get; set; }
    }

   


    public class FoundryException : Exception
    {
        public int ErrorCode { get; set; }

        public string Response { get; set; }
        //internal ErrorList ErrorMessageList { get; set; }
       
        public override string Message { get; }

        public FoundryException(string message)
        {   

        }
        

        public String ConfigureErrorMessage(int ErrorCode, String Response)
        {
            String message = " ";
            ErrorList ErrorMessageList;
            if (ErrorCode == 422)
            {
                List<ErrorList> error = JsonConvert.DeserializeObject<List<ErrorList>>(Response);
                ErrorMessageList = error.ElementAt(0);
                
            }
            else{
                ErrorMessageList = JsonConvert.DeserializeObject<ErrorList>(Response);
            }
            foreach (ErrorContent content in ErrorMessageList.ListOfErrors)
            {
                if (content.Message1 != null)
                {
                    message += content.Message1;
                }

                message += " ";

                if (content.Message2 != null)
                {
                    message += content.Message2;
                }

            }
            return message;
        }

   
    public FoundryException(int ErrorCode, string Response)
    {
        this.ErrorCode = ErrorCode;
        this.Response = Response;
        this.Message = ConfigureErrorMessage(ErrorCode, Response);
       

    }

}
}
