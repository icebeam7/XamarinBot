using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using XamarinBot.Models;

namespace XamarinBot.Services
{
    public class OpenWeatherService
    {
        public static string ForecastMessage = "Hello. For {0}, it'll be {1} in {2}.. with low temp at {3} and high at {4}..";
        public static string WeatherMessage = "Hello. It's {0} in {1}.. with low temp at {2} and high at {3}..";
        public static string YesMessage = "Hi. actually yes, it's {0} in {1}.";
        public static string NoMessage = "Hi.. actually no, it's {0} in {1}.";
        public static string HelloMessage = "Hello! Ask me anything about the weather";

        private string OpenWeatherAppID = "";

        public async Task<WeatherObject> GetWeatherData(string query)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/weather?q={query}&appid={OpenWeatherAppID}&units=metric&lang=en"));

                    WeatherObject weather = JsonConvert.DeserializeObject<WeatherObject>(response);
                    return weather;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<WeatherForecastObject> GetForecastData(string query, DateTime dt)
        {
            try
            {
                int days = (dt - DateTime.Now).Days;

                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/forecast/daily?q={query}&appid={OpenWeatherAppID}&units=metric&lang=en&cnt={days}"));
                    WeatherForecastObject forecast = JsonConvert.DeserializeObject<WeatherForecastObject>(response);
                    return forecast;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}