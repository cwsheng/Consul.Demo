using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Consul.Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CallServiceController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<CallServiceController> _logger;

        public CallServiceController(ILogger<CallServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("service_result")]
        public async Task<IActionResult> GetService([FromServices] ICallService service)
        {
            return Ok(new
            {
                Goods = await service.GetGoodsService(),
                Order = await service.GetOrderService()
            });
        }
    }
}
