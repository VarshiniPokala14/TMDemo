namespace TrekMasters.Service
{
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public class WeatherService
    {
        private readonly string _apiKey = "3ee56fc14011ffd3d065ac140cc7411f";  // Replace with your API key
        private readonly string _baseUrl = "https://api.openweathermap.org/data/2.5/forecast";

        public async Task<List<WeatherForecast>> GetWeatherForecastAsync(string location)
        {
            var weatherData = new List<WeatherForecast>();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"{_baseUrl}?q={location}&units=metric&cnt=35&appid={_apiKey}";
                    var response = await client.GetStringAsync(url);
                    dynamic weatherApiResponse = JsonConvert.DeserializeObject(response);
                    var forecast = weatherApiResponse.list;

                    foreach (var day in forecast)
                    {
                        weatherData.Add(new WeatherForecast
                        {
                            Date = day.dt_txt,
                            Temperature = day.main.temp,
                            Description = day.weather[0].description,
                            WindSpeed = day.wind.speed,
                            Humidity = day.main.humidity,
                            Icon = day.weather[0].icon
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching weather data: {ex.Message}");
                }
            }
            return weatherData;
        }
        public async Task<WeatherForecast> GetWeatherForDateAsync(string location, DateTime targetDate)
        {
            try
            {
                // Fetch the 5-day forecast data
                var forecasts = await GetWeatherForecastAsync(location);

                // Match the forecast closest to the target date (ignoring time if needed)
                var matchingForecast = forecasts.FirstOrDefault(f =>
                    DateTime.TryParse(f.Date, out DateTime forecastDate) &&
                    forecastDate.Date == targetDate.Date);

                if (matchingForecast != null)
                {
                    return matchingForecast;
                }
                else
                {
                    // If no data exists for the given date, handle accordingly (e.g., return null or a default object)
                    Console.WriteLine($"No forecast found for {targetDate.ToShortDateString()} in {location}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather data for {targetDate.ToShortDateString()}: {ex.Message}");
                return null;
            }
        }

    }


    
}
