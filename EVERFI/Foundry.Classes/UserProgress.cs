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
    internal class NextUserData
    {
        [JsonProperty("next")]
        internal NextData Next { get; set; }
    }
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


    public class UserProgress
    {
        public UserProgress()
        {
        }
    }
}
