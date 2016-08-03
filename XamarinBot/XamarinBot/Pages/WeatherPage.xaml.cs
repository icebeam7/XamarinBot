using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using XamarinBot.Models;
using XamarinBot.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace XamarinBot.Pages
{
	public partial class WeatherPage : ContentPage
	{
        public ObservableCollection<ChatMessage> Chat { get; set; }

        public WeatherPage ()
		{
            InitializeComponent ();
            Chat = new ObservableCollection<ChatMessage>();
            lsvChat.ItemsSource = Chat;
        }

        async void btnSend_Clicked(object sender, EventArgs e)
        {
            try
            {
                string message = txtMessage.Text;
                Chat.Add(new ChatMessage() { Message = message, IsIncoming = true });

                string response = await EnviaMensaje(message);
                Chat.Add(new ChatMessage() { Message = response, IsIncoming = false });
            }
            catch(Exception ex)
            {
                Chat.Add(new ChatMessage() { Message = "Sorry, I did not understand you. Try again, please :-)" });
            }
            finally
            {
                txtMessage.Text = "";
            }
        }

        async Task<string> EnviaMensaje(string query)
        {
            OpenWeatherService openWeatherService = new OpenWeatherService();
            string city, time, condition;

            LuisService luisService = new LuisService();
            LuisObject luis = await luisService.QueryAsync(query);

            if (luis == null)
                return "Error: LUIS SERVICE NOT FOUND";

            if (luis.intents.Count() == 0)
                return "Error: LUIS SERVICE INTENTS NOT FOUND";

            switch (luis.intents[0]?.intent)
            {
                case "Weather":
                    if (luis.entities.Count() == 0)
                        return "Error: LUIS SERVICE ENTITIES NOT FOUND";

                    city = luis.entities.Where(ent => ent.type == "City").FirstOrDefault()?.entity;
                    time = luis.entities.Where(ent => ent.type == "Time").FirstOrDefault()?.entity;

                    if (city == null)
                        return "Error: Please specify the city.";

                    if (time == null)
                        time = DateTime.Now.ToShortDateString(); //Default time is now..

                    DateTime requestedDt = DateTime.Now;
                    switch (time)
                    {
                        case "yesterday": requestedDt.AddDays(-1); break;
                        case "tomorrow": requestedDt.AddDays(1); break;
                        case "next week": requestedDt.AddDays(7); break;
                        case "last week": requestedDt.AddDays(-7); break;
                    }

                    string replyBase;

                    if ((requestedDt - DateTime.Now).Days > 0)
                    {
                        var forecast = await openWeatherService.GetForecastData(city, requestedDt);

                        List lastDayWeather = forecast.list.Last();

                        string description = lastDayWeather.weather.FirstOrDefault()?.description;
                        string lowAt = Math.Round(lastDayWeather.temp.min) + "°";
                        string highAt = Math.Round(lastDayWeather.temp.max) + "°";
                        string cityName = forecast.city.name + ", " + forecast.city.country;

                        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        dtDateTime = dtDateTime.AddSeconds(lastDayWeather.dt).ToLocalTime();
                        DateTime date = dtDateTime;

                        replyBase = string.Format(OpenWeatherService.ForecastMessage,
                            date.ToString("dddd, MMMM, yyyy"), description, cityName, lowAt, highAt);
                    }
                    else
                    {
                        var weather = await openWeatherService.GetWeatherData(city);

                        string description = weather.weather.FirstOrDefault()?.description;
                        string lowAt = weather.main.temp_min + "";
                        string highAt = weather.main.temp_min + "";
                        string cityName = "";
                        cityName = weather.name + ", " + weather.sys.country;

                        replyBase = string.Format(OpenWeatherService.WeatherMessage,
                            description, cityName, lowAt, highAt);
                    }

                    return replyBase;

                case "Condition":
                    city = luis.entities.Where(ent => ent.type == "City").FirstOrDefault()?.entity;
                    condition = luis.entities.Where(ent => ent.type == "Condition").FirstOrDefault()?.entity;

                    if (city == null)
                        return "Error: Please specify the city.";

                    var weatherForecast = await openWeatherService.GetWeatherData(city);
                    string descriptionF = weatherForecast.weather.FirstOrDefault()?.description;
                    string status = weatherForecast.weather.FirstOrDefault()?.main;

                    string cityNameF = weatherForecast.name + ", " + weatherForecast.sys.country;
                    descriptionF = descriptionF.Replace("nice", "clear|sun|bright|fine|partially cloudy").Replace("good", "clear|sun|bright|fine").Replace("bad", "rain|snow|cloud").Replace("cold", "snow|hail|sleet|blizzard").Replace("day", "").Replace("night", "").Replace("morning", "").Replace("afternoon", "");
                    string message =
                        (condition.ToLower().StartsWith(status.ToLower()) || descriptionF.Contains(condition))
                        ? string.Format(OpenWeatherService.YesMessage, descriptionF, city)
                        : string.Format(OpenWeatherService.NoMessage, descriptionF, city);

                    return message;

                default:
                    return "Sorry, I did not understand you. Try again, please :-)";
            }

            return "---";
        }
	}
}
