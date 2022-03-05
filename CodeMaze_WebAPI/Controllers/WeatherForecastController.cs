using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CodeMaze_WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IRepositoryWrapper _repoWrapper;
        public WeatherForecastController(IRepositoryWrapper repositoryWrapper)
        {
            this._repoWrapper = repositoryWrapper;       
        }

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public  IEnumerable<string> Get()
        {
            var domesticAccounts = _repoWrapper.AccountRepository.FindByCondition(x => x.AccountType.Equals("Domestic"));
            var owners = _repoWrapper.OwnerRepository.FindAll();
            return new string[] {"Value 1", "Value2" };

            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = Random.Shared.Next(-20, 55),
            //    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //})
            //.ToArray();
        }
    }
}