using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cz.Api.Order.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = new
            {
                Message = $"Name:{nameof(cz.Api.Order)}，Time：{DateTime.Now}",
                IP = Request.HttpContext.Connection.LocalIpAddress.ToString(),
            };
            return Ok(result);
        }
    }
}
