using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace azure_durable_functions.PizzaOrderingSystem.Functions
{
    public class ConfirmationRequest
    {
        public bool Confirm { get; set; }
    }

    public static class OrderConfirmationFunction
    {
        [FunctionName("ConfirmOrder")]
        public static async Task<IActionResult> ConfirmOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log
        )
        {
            string instanceId = req.Query["instanceId"];
            if (string.IsNullOrEmpty(instanceId))
            {
                return new BadRequestObjectResult("Please pass an instanceId on the query string");
            }

            log.LogInformation($"Confirming order for instance ID: {instanceId}");

            try
            {
                // Check if this is a simple confirmation (no body) or has JSON body
                bool confirmOrder = true; // Default to confirm

                if (req.ContentLength > 0)
                {
                    // Parse the request body as JSON if present
                    string requestBody = await new System.IO.StreamReader(
                        req.Body
                    ).ReadToEndAsync();
                    if (!string.IsNullOrEmpty(requestBody))
                    {
                        var confirmationRequest = JsonSerializer.Deserialize<ConfirmationRequest>(
                            requestBody
                        );
                        confirmOrder = confirmationRequest?.Confirm ?? true;
                    }
                }

                // Raise the event to the orchestration
                await client.RaiseEventAsync(instanceId, "OrderConfirmation", confirmOrder);

                var message = confirmOrder ? "confirmed" : "rejected";
                log.LogInformation($"Order {message} for instance {instanceId}");

                return new OkObjectResult(
                    new
                    {
                        message = $"Order has been {message}",
                        instanceId = instanceId,
                        confirmed = confirmOrder,
                    }
                );
            }
            catch (JsonException ex)
            {
                log.LogError(ex, "Error parsing request body");
                return new BadRequestObjectResult(
                    "Invalid JSON in request body. Expected format: {\"confirm\": true|false}"
                );
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing confirmation request");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
