using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class HealthCheck
    {
        [FunctionName("HealthCheck")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Health check endpoint called.");

            return new OkObjectResult(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                message = "Pizza Ordering System is running",
                endpoint = "health"
            });
        }
    }
} 