using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    internal class LocationDataJson
    {
        [JsonProperty("data")]
        public LocationData LocationData { get; set; }
    }

    internal class LocationDataJsonList
    {
        [JsonProperty("data")]
        public List<LocationData> LocationsData { get; set; }
    }

    public class LocationData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string Type = "locations";

        [JsonProperty("attributes")]
        public Location LocationAttributes { get; set; }
    }

    public class Location
    {
        [JsonProperty("id")]
        public string Id { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("organization_id")]
        public string OrgId { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("contact_name")]
        public string ContactName { get; set; }

        [JsonProperty("contact_email")]
        public string ContactEmail { get; set; }

        [JsonProperty("contact_phone")]
        public string ContactPhone { get; set; }

        [JsonProperty("address_formatted")]
        public string FormattedAddress { get; }

        [JsonProperty("address_street_number")]
        public string StreetNumber { get; set; }

        [JsonProperty("address_route")]
        public string Route { get; set; }

        [JsonProperty("address_neighborhood")]
        public string Neighborhood { get; set; }

        [JsonProperty("address_locality")]
        public string City { get; set; }

        [JsonProperty("address_administrative_area_level_2")]
        public string County { get; set; }

        [JsonProperty("address_administrative_area_level_1")]
        public string State { get; set; }

        [JsonProperty("address_postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("address_country")]
        public string Country { get; set; }

        [JsonProperty("address_latitude")]
        public float Latitude { get; set; }

        [JsonProperty("address_longitude")]
        public float Longitude { get; set; }

        [JsonProperty("address_name")]
        public string AddressName { get; set; }

        [JsonProperty("address_room")]
        public string AddressRoom { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public void AddIdFromData(LocationData data)
        {
            this.Id = data.Id;
        }

        public string ToJson() //Change to internal when done
        {
            string Json = "{\n" +
                "\"data\": {\n";
            if (this.Id != null)
            {
                Json += "\t\"id\": \"" + this.Id + "\",\n";
            }

            Json += "\t\"type\": \"locations\",\n" +
                "\t\"attributes\": {\n" +
                "\t\t\"name\": \"" + this.Name + "\",\n" +
                "\t\t\"external_id\": \"" + this.ExternalId + "\",\n" +
                "\t\t\"contact_email\": \"" + this.ContactEmail + "\",\n" +
                "\t\t\"contact_name\": \"" + this.ContactName + "\",\n" +
                "\t\t\"contact_phone\": \"" + this.ContactPhone + "\",\n" +
                "\t\t\"address_street_number\": \"" + this.StreetNumber + "\",\n" +
                "\t\t\"address_route\": \"" + this.Route + "\",\n" +
                "\t\t\"address_neighborhood\": \"" + this.Neighborhood + "\",\n" +
                "\t\t\"address_locality\": \"" + this.City + "\",\n" +
                "\t\t\"address_administrative_area_level_1\": \"" + this.State + "\",\n" +
                "\t\t\"address_administrative_area_level_2\": \"" + this.County + "\",\n" +
                "\t\t\"address_postal_code\": \"" + this.PostalCode + "\",\n" +
                "\t\t\"address_country\": \"" + this.Country + "\",\n" +
                "\t\t\"address_latitude\": \"" + this.Latitude + "\",\n" +
                "\t\t\"address_longitude\": \"" + this.Longitude + "\"\n" +
                "\t\t\"address_name\": \"" + this.AddressName + "\"\n" +
                "\t\t\"address_room\": \"" + this.AddressRoom + "\"\n" +
                "\t}\n}\n}\n";

            return Json;
        }
    }
}
