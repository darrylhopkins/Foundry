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
    }
}
