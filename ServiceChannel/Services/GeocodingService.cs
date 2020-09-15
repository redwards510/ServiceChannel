using GuigleApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServiceChannel.Services
{
    public interface IGeocodingService
    {
        Task<GeocodedAddress> GeocodeAddress(string address);
    }
    /// <summary>
    /// Service for calling Google's geocoding provided address fragments and returning a county and state, or just a state
    /// </summary>
    public class GeocodingService : IGeocodingService
    {
        public async Task<GeocodedAddress> GeocodeAddress(string address)
        {
            var httpClient = new HttpClient();
            var googleGeocodingApi = new GoogleGeocodingApi("AIzaSyBqBbxUfjM7wksH6KvDYb5wbUk6lV7Ox2Y");
            var loc = await googleGeocodingApi.SearchAddress(httpClient, address);
            var result = new GeocodedAddress
            {
                County = loc.Results.FirstOrDefault()?.CityShortName?.Replace("County", ""),
                State = loc.Results.FirstOrDefault()?.StateLongName
            };
            return result;
        }
    }

    public class GeocodedAddress
    {
        public string County { get; set; }
        public string State { get; set; }
    }
}
