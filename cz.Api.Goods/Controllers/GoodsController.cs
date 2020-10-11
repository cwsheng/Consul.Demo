using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace cz.Api.Goods.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class GoodsController : ControllerBase
    {
        private readonly ILogger<GoodsController> _logger;

        public GoodsController(ILogger<GoodsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = new
            {
                Message = $"Name:{nameof(cz.Api.Goods)}，Time：{DateTime.Now}",
                LocalIp = Request.HttpContext.Connection.LocalIpAddress.ToString(),
            };
            return Ok(result);
        }
    }
}
