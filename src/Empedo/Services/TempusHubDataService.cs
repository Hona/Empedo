using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Empedo.Logging;
using Empedo.Models.TempusHub;
using Empedo.Models.TempusHub.Activity;
using Newtonsoft.Json;

namespace Empedo.Services
{
    public static class TempusHubDataService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://tempushub.xyz")
        };

        private static async Task<T> GetResponseAsync<T>(string request)
        {
            try
            {
                var response = await _httpClient.GetAsync("/api" + request);

                if (response.IsSuccessStatusCode)
                {
                    object stringValue = await response.Content.ReadAsStringAsync();

                    Logger.LogInfo("TempusHub: " + request);
                    // If T is a string, don't deserialise
                    return typeof(T) == typeof(string)
                        ? (T) stringValue
                        : JsonConvert.DeserializeObject<T>((string) stringValue);
                }

                Logger.LogError("Couldn't get Tempus API request: " + request);
                throw new Exception("Couldn't get Tempus API request: " + request);
            }
            catch (Exception e)
            {
                throw new Exception("Failed on: " + request, e);
            }
        }

        public static async Task<List<TopPlayerOnline>> GetTopPlayersOnlineAsync()
            => await GetResponseAsync<List<TopPlayerOnline>>("/topplayersonline");

        public static async Task<List<RecentRecord>> GetRecentMapWRsAsync()
            => await GetResponseAsync<List<RecentRecord>>("/activity/MapWR");

        public static async Task<List<RecentRecord>> GetRecentCourseWRsAsync()
            => await GetResponseAsync<List<RecentRecord>>("/activity/CourseWR");

        public static async Task<List<RecentRecord>> GetRecentBonusWRsAsync()
            => await GetResponseAsync<List<RecentRecord>>("/activity/BonusWR");

        public static async Task<List<RecentRecord>> GetRecentMapTTsAsync()
            => await GetResponseAsync<List<RecentRecord>>("/activity/MapTT");
    }
}