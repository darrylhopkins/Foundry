using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    internal class CategoryData
    {
        [JsonProperty("data")]
        internal Category Data { get; set; }
    }

    internal class CategoryListData
    {
        [JsonProperty("data")]
        internal List<Category> Data { get; set; }
    }

    internal class CategoryAttributes
    {
        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("users_count")]
        internal int UserCount { get; set; }
    }

    internal class CategoryRelationships
    {
        [JsonProperty("category_labels")]
        internal CategoryLabels CategoryLabels { get; set; }
    }

    internal class CategoryLabels
    {
        [JsonProperty("data")]
        internal List<Label> Labels { get; set; }
    }

    public class Category
    {
        [JsonProperty("id")]
        public string Id { get; internal set; }

        public string Name { get; set; }

        public int UserCount { get; internal set; }

        [JsonProperty("attributes")]
        internal CategoryAttributes Attributes { get; set; }

        [JsonProperty("relationships")]
        internal CategoryRelationships Relationships { get; set; }

        public List<Label> Labels { get; internal set; }

        internal void ConfigureCategory()
        {
            this.Name = Attributes.Name;
            this.UserCount = Attributes.UserCount;

            // This only holds the ids of the Labels
            this.Labels = Relationships.CategoryLabels.Labels;
        }
    }
}
