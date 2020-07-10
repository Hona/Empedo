using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Empedo.Logging;
using Empedo.Models.TempusHub;
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
                        ? (T)stringValue
                        : JsonConvert.DeserializeObject<T>((string)stringValue);
                }
                else
                {
                    Logger.LogError("Couldn't get Tempus API request: " + request);
                    throw new Exception("Couldn't get Tempus API request: " + request);
                }
            }
            catch
            {
                throw new Exception("Failed on: " + request);
            }
        }

        public static async Task<List<TopPlayerOnline>> GetTopPlayersOnlineAsync()
            => await GetResponseAsync<List<TopPlayerOnline>>("/topplayersonline");
    }
}
