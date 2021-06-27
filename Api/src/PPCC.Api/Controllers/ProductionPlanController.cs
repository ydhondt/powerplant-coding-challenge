using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PPCC.Service;
using PPCC.Service.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PPCC.Api.Controllers
{
    [Route("")]
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {
        private readonly IProductionPlanCalculator _productionPlanCalculator;
        private readonly ILogger<ProductionPlanController> _logger;
        private readonly IBroadcastService _broadcastService;

        public ProductionPlanController(IProductionPlanCalculator productionPlanCalculator, ILogger<ProductionPlanController> logger, IBroadcastService broadcastService)
        {
            _productionPlanCalculator = productionPlanCalculator;
            _logger = logger;
            _broadcastService = broadcastService;
        }

        [HttpPost("productionplan", Name = "Production Plan Calculator")]
        public async Task<IActionResult> CalculateProductionPlan([FromBody] Payload payload)
        {
            _logger.LogInformation($"Received request to calculate production plan for load {payload.Load} at {DateTime.Now.ToShortTimeString() }.");

            var result = _productionPlanCalculator.Calculate(payload.Load, payload.Powerplants, payload.Fuels);

            await _broadcastService.BroadcastMessageAsync(string.Concat(JsonSerializer.Serialize(payload), Environment.NewLine, Environment.NewLine, JsonSerializer.Serialize(result), Environment.NewLine, Environment.NewLine));

            return Ok(result);
        }
    }
}
