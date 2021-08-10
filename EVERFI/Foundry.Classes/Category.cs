using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVERFI.Foundry.Classes
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
        internal CategoryLabelsData CategoryLabels { get; set; }
    }

    internal class CategoryLabelsData
    {
        [JsonProperty("data")]
        internal List<CategoryLabelsIncluded> Label { get; set; }
    }
   
    internal class CategoryLabels
    {
        [JsonProperty("included")]
        internal List<LabelInformation> AllLabels { get; set; }
    }
   
    public class CategoryLabelsIncluded{

        [JsonProperty("id")]
        internal String CategoryLabelId { get; set; }

        [JsonProperty("type")]
        internal String Type { get; set; }
  
   
    }

    internal class LabelInformation
    {
        [JsonProperty("id")]
        internal string CategoryLabelId { get; set; }

        [JsonProperty("attributes")]
        internal CategoryLabelAttributes Attributes { get; set; }

        [JsonProperty("relationships")]
        internal CategoryLabelRelationships CategoryLabelRelationships { get; set; }
    }
   

    internal class CategoryLabelRelationships
    {
        [JsonProperty("category")]
        internal CategoryLabelData Category { get; set; }
    }
    internal class CategoryLabelData
    {
        [JsonProperty("data")]
        internal CategoryLabelsIncluded CategoryLabel { get; set; }
    }
     
    internal class CategoryLabelAttributes
    {
        [JsonProperty("category_id")]
        internal string CategoryId { get; set; }

        [JsonProperty("category_name")]
        internal String CategoryName { get; set; }

        [JsonProperty("name")]
        internal String CategoryLabelName { get; set; }

        [JsonProperty("users_count")]
        internal int UserCount { get; set; }
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

        public List<CategoryLabelsIncluded> Labels { get; internal set; }

        public List<Label> AllLabels { get; internal set; }

        internal void ConfigureCategory()
        {
            this.Name = Attributes.Name;
            this.UserCount = Attributes.UserCount;

            // This only holds the ids of the Labels
            this.Labels = Relationships.CategoryLabels.Label;

        }
        
    }
}
