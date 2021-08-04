using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using EVERFI.Foundry.Classes;
using RestSharp.Authenticators;

namespace EVERFI.Foundry
{
    public partial class API
    {
        public Location AddLocation(Location MyLocation)
        {
            Console.WriteLine("Adding location " + MyLocation.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/locations", Method.POST);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.LocationJson(MyLocation), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, RequestType.PostRequest);
            //verify if adding was okay with status code

            LocationDataJson locationData = JsonConvert.DeserializeObject<LocationDataJson>(response.Content);
            Console.WriteLine("Location successfully added.");

            Location location = locationData.LocationData.LocationAttributes;
            location.Id = locationData.LocationData.Id;

            FoundryLocations.Add(location);
            return location;
        }

        public Location UpdateLocation(Location MyLocation)
        {
            Console.WriteLine("Updating location " + MyLocation.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/locations/{location_id}", Method.PATCH);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("location_id", MyLocation.Id, ParameterType.UrlSegment);
            request.AddParameter("application/json", API.LocationJson(MyLocation), ParameterType.RequestBody);
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, RequestType.PatchRequest);

            LocationDataJson locationData = JsonConvert.DeserializeObject<LocationDataJson>(response.Content);
            Location location = locationData.LocationData.LocationAttributes;
            location.AddIdFromData(locationData.LocationData);

            FoundryLocations.Remove(MyLocation);
            FoundryLocations.Add(location);

            return location;
        }

        public List<Location> GetLocations()
        {
            RestRequest request = new RestRequest("/{version}/admin/locations", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("SDK_Method", "GetLocations");
            _client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_token.access_token, _token.token_type);


            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, RequestType.GetRequest);

            LocationDataJsonList locationData = JsonConvert.DeserializeObject<LocationDataJsonList>(response.Content);
            List<Location> locations = new List<Location>();

            foreach (LocationData data in locationData.LocationsData)
            {
                Location newLocation = data.LocationAttributes;
                newLocation.AddIdFromData(data);
                locations.Add(newLocation);
            }

            return locations;
        }

        public Location GetLocationById(string LocationId)
        {
            
            var location = new Location();
            try
            {
                location = GetLocationFromCache(LocationId);
            }
            catch (Exception ex)
            { }
            /*
            if (!string.IsNullOrEmpty(location.Id))
                return location;
            */
            RestRequest request = new RestRequest("/{version}/admin/locations/{location_id}", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("location_id", LocationId, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response, RequestType.GetRequest);

            LocationDataJson locationData = JsonConvert.DeserializeObject<LocationDataJson>(response.Content);
            location = locationData.LocationData.LocationAttributes;
            location.AddIdFromData(locationData.LocationData);

            return location;
        }

        private Location GetLocationFromCache(string LocationId)
        {
            return FoundryLocations.Find(x => x.Id == LocationId);
        }
    }
}
