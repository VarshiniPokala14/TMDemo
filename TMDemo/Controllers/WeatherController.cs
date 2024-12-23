//using Newtonsoft.Json;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;

//namespace TrekMasters.Controllers
//{
//    public class WeatherController : Controller
//    {
//        private readonly string _apiKey = "3ee56fc14011ffd3d065ac140cc7411f"; // Replace with your OpenWeather API key
//        private readonly string _baseUrl = "https://api.openweathermap.org/data/2.5/forecast"; // OpenWeather API endpoint
//        private readonly WeatherService _weatherService;
//        public WeatherController(WeatherService weatherService)
//        {
//            _weatherService = weatherService;
//        }
//        [HttpGet]
//        public async Task<IActionResult> GetWeatherForDate(string date, string region)
//        {
//            if (string.IsNullOrWhiteSpace(date) || string.IsNullOrWhiteSpace(region))
//                return BadRequest("Invalid date or region");

//            // Fetch weather data for the specific date
//            var weatherData = await _weatherService.FetchWeatherForDateAsync(date, region);

//            if (weatherData == null)
//                return NotFound("Weather data not available");

//            return Json(weatherData);
//        }


//    }
//}
