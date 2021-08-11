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
            checkResponseSuccess(response);
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
            checkResponseSuccess(response);

            LocationDataJson locationData = JsonConvert.DeserializeObject<LocationDataJson>(response.Content);
            Location location = locationData.LocationData.LocationAttributes;
            location.AddIdFromData(locationData.LocationData);

            FoundryLocations.Remove(MyLocation);
            FoundryLocations.Add(location);

            return location;
        }

        public List<Location> GetLocations()
        {
            if (FoundryLocations != null && FoundryLocations.Count > 0)
            {
                return FoundryLocations;
            }

            RestRequest request = new RestRequest("/{version}/admin/locations", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("SDK_Method", "GetLocations");
            _client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(_token.access_token, _token.token_type);


            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response);

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

        /// <summary>
        /// Returns the Location with the specified LocationId. This property is a backend auto-generated number for a Location
        /// and is used to look up a specific Location. If not found, then expect a FoundryException.
        /// </summary>
        /// <param name="LocationId"></param>
        /// <returns></returns>
        public Location GetLocationById(string LocationId)
        {
            Location location = GetLocationFromCache(LocationId);

            if (location != null && !string.IsNullOrEmpty(location.Id))
                return location;

            RestRequest request = new RestRequest("/{version}/admin/locations/{location_id}", Method.GET);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("location_id", LocationId, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response);

            LocationDataJson locationData = JsonConvert.DeserializeObject<LocationDataJson>(response.Content);
            location = locationData.LocationData.LocationAttributes;
            location.AddIdFromData(locationData.LocationData);

            return location;
        }

        /// <summary>
        /// Deletes the provided Location. If the Location cannot be deleted, then a FoundryException will be returned.
        /// A Location that is referenced by other objects, like Users, cannot be delted.
        /// </summary>
        /// <param name="LocationToDelete">The Location to delete</param>
        /// <returns></returns>
        public string DeleteLocation(Location LocationToDelete)
        {
            Console.WriteLine("Deleting location " + LocationToDelete.Name + "...");

            RestRequest request = new RestRequest("/{version}/admin/locations/{id}", Method.DELETE);
            request.AddParameter("version", _ver, ParameterType.UrlSegment);
            request.AddParameter("id", LocationToDelete.Id, ParameterType.UrlSegment);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("Authorization", _token.token_type + " " + _token.access_token, ParameterType.HttpHeader);

            IRestResponse response = _client.Execute(request);
            checkResponseSuccess(response);

            var message = "Location " + LocationToDelete.Name + " successfully deleted.";

            FoundryLocations.Remove(LocationToDelete);

            Console.WriteLine(message);

            return (String.IsNullOrEmpty(response.Content) ? message : response.Content);
        }

        /// <summary>
        /// Return the Location with the specified External Location ID, which is an organization-defined code
        /// for each Location. If more than one Location has this External Location ID,
        /// then you will get a FoundryException with a 422.
        /// If there is no Location with this Id, then you will get a FoundryException with a 404.
        /// </summary>
        /// <param name="ExternalLocationId">The External Location ID</param>
        /// <returns>Location</returns>
        public Location GetLocationByExternalId(string ExternalLocationId)
        {
            var locs = FoundryLocations.FindAll(x => x.ExternalId == ExternalLocationId);

            if (locs.Count < 1)
            {
                throw new FoundryException(404, "Not Found");
            }
            else if (locs.Count > 1)
            {
                throw new FoundryException(422, "Multiple Locations have an External Location Id of '" + ExternalLocationId + "'. " +
                    "You must run GetLocations and find the specific Location you need, or else " +
                    "resolve the source issue of having duplicate External Location Id values.");
            }
            return locs[0];
        }

        private Location GetLocationFromCache(string LocationId)
        {
            return FoundryLocations.Find(x => x.Id == LocationId);
        }
    }
}
