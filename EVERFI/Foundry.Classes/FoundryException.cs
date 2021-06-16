using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVERFI.Foundry.Classes
{
    public class FoundryException : Exception
    {
        public int ErrorCode { get; set; }

        public string Response { get; set; }

        public FoundryException(string message) : base(message)
        {

        }

        public FoundryException(string message, int ErrorCode, string Response) : base(message)
        {
            this.ErrorCode = ErrorCode;
            this.Response = Response;
        }
    }
}
