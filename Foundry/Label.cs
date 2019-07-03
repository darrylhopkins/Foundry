using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    internal class LabelData
    {

    }

    internal class LabelAttributes
    {
        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("users_count")]
        internal int UserCount { get; set; }
    }

    public class Label
    {
        [JsonProperty("id")]
        public string Id { get; internal set; }

        public string Name { get; set; }

        public int UserCount { get; internal set; }

        internal LabelAttributes Attributes { get; set; }

        internal void ConfigureLabel()
        {
            this.Name = Attributes.Name;
            this.UserCount = Attributes.UserCount;
        }
    }
}
