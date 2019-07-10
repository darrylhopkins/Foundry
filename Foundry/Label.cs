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
        [JsonProperty("data")]
        internal Label Data { get; set; }
    }

    internal class LabelAttributes
    {
        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("users_count")]
        internal int UserCount { get; set; }
    }

    internal class LabelRelationships
    {
        [JsonProperty("category")]
        internal LabelCategory LabelCategory { get; set; }
    }

    internal class LabelCategory
    {
        [JsonProperty("data")]
        internal CategoryId Category { get; set; }
    }

    internal class CategoryId
    {
        [JsonProperty("id")]
        internal string Id { get; set; }
    }

    public class Label
    {
        [JsonProperty("id")]
        public string Id { get; internal set; }

        public string Name { get; set; }

        public int UserCount { get; internal set; }

        public string CategoryId { get; internal set; }

        [JsonProperty("attributes")]
        internal LabelAttributes Attributes { get; set; }

        [JsonProperty("relationships")]
        internal LabelRelationships Relationships { get; set; }

        internal void ConfigureLabel()
        {
            this.Name = Attributes.Name;
            this.UserCount = Attributes.UserCount;

            this.CategoryId = Relationships.LabelCategory.Category.Id;
        }
    }
}
