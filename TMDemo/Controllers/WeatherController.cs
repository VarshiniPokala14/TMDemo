//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Threading.Tasks;
//using TrekMasters.Data;
//using TrekMasters.Models;

//namespace TrekMasters.Controllers
//{
//    public class WeatherController : Controller
//    {
//        private readonly AppDbContext _context;
//        private readonly string ApiKey = "db1ccd3d0dee5ecff8ed6c883f23cc19";  
//        private readonly string BaseUrl = "https://api.weatherapi.com/v1/forecast.json";

//        public WeatherController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // Fetch weather for the next 7 days starting from tomorrow
//        public async Task<IActionResult> FetchWeatherByTrek(string trekName)
//        {
//            // Find the trek based on its name
//            var trek = _context.Treks.FirstOrDefault(t => t.Name == trekName);
//            if (trek == null)
//                return NotFound("Trek not found");

//            // Use Region or Name to fetch weather
//            var locationQuery = trek.Region; // We use Region to fetch weather, change to trek.Name if required.

//            var weatherData = await FetchWeatherAsync(locationQuery);
//            if (weatherData != null)
//            {
//                ViewBag.WeatherData = weatherData;
//            }

//            return RedirectToAction("TrekDetails", "Trek", new { id = trek.TrekId });
//        }

//        // Fetch weather from API for the next 7 days
//        private async Task<List<dynamic>> FetchWeatherAsync(string locationQuery)
//        {
//            var url = $"{BaseUrl}?key={ApiKey}&q={Uri.EscapeDataString(locationQuery)}&days=8"; // Fetch 8 days (starting from today)
//            using (HttpClient client = new HttpClient())
//            {
//                try
//                {
//                    var response = await client.GetStringAsync(url);
//                    var weatherApiResponse = JsonConvert.DeserializeObject<dynamic>(response);

//                    // Extract weather forecast for the next 7 days (starting from tomorrow)
//                    var forecast = weatherApiResponse.forecast.forecastday;
//                    var weatherData = new List<dynamic>();

//                    foreach (var day in forecast.Skip(1)) // Skip today and fetch data starting from tomorrow
//                    {
//                        weatherData.Add(new
//                        {
//                            Date = day.date,
//                            TemperatureHigh = day.day.maxtemp_c,
//                            TemperatureLow = day.day.mintemp_c,
//                            Description = day.day.condition.text,
//                            WindSpeed = day.day.maxwind_kph,
//                            Humidity = day.day.avghumidity,
//                            Precipitation = day.day.totalprecip_mm
//                        });
//                    }

//                    return weatherData;
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Error fetching weather data: {ex.Message}");
//                    return null;
//                }
//            }
//        }
//    }
//}
