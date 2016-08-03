using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using XamarinBot.Models;

namespace XamarinBot.Services
{
    public class LuisService
    {
        private string LuisAppID = "";
        private string LuisSubscriptionKey = "";

        public async Task<LuisObject> QueryAsync(string query)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync(new Uri($"https://api.projectoxford.ai/luis/v1/application?id={LuisAppID}&subscription-key={LuisSubscriptionKey}&q={query}"));

                    LuisObject luis = JsonConvert.DeserializeObject<LuisObject>(response);
                    if (luis != null) return luis;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
