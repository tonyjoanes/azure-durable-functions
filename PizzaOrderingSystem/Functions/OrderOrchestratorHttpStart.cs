using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class OrderOrchestratorHttpStart
    {
        [FunctionName("OrderOrchestrator_HttpStart")]
        public static async Task<IActionResult> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("Starting pizza order orchestration via HTTP trigger.");

            // Read the request body to get order data
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic orderData = null;
            
            if (!string.IsNullOrEmpty(requestBody))
            {
                try
                {
                    orderData = JsonConvert.DeserializeObject(requestBody);
                    log.LogInformation($"Order data received: {requestBody}");
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to parse order data: {ex.Message}");
                    return new BadRequestObjectResult("Invalid order data format");
                }
            }

            string instanceId = await starter.StartNewAsync("OrderOrchestrator", orderData);
            log.LogInformation($"Started orchestration with ID = '{instanceId}' and order data.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
} 