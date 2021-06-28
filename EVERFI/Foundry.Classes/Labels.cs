using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EVERFI.Foundry.Classes
{
    //collection of label - label ID, label name, category ID
    internal class labelIDs {

        [JsonProperty("lable_id")]
        internal string Id { get; set;}
    }


    internal class labelNames 
    {
        [JsonProperty("label_name")]
        internal string Name { get; set; }

    }
    internal class categoryIDs
    {
        [JsonProperty("category_id")]
        internal string CategoryId { get; set; }
    }
    public class Labels
    {
        internal labelIDs labelID;
        internal labelNames labelName;
        internal categoryIDs categoryID;


    }
}