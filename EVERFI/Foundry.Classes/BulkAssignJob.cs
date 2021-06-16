using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVERFI.Foundry.Classes
{
    internal class JobJson
    {
        [JsonProperty("data")]
        internal BulkAssignJob BulkAssignJob { get; set; }
    }

    internal class JobAttributes
    {
        [JsonProperty("status")]
        internal string Status { get; set; }

        // Add Processing Errors in future update

    }

    public class BulkAssignJob
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string Status { get; internal set; }

        [JsonProperty("attributes")]
        internal JobAttributes Attributes { get; set; }

        internal void ConfigureJob()
        {
            this.Status = Attributes.Status;
        }
    }
}
