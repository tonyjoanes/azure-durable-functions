using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public class ConfirmationRequest
    {
        public bool Confirm { get; set; }
    }

    public static class OrderConfirmationFunction
    {
        [FunctionName("ConfirmOrder")]
        public static async Task<IActionResult> ConfirmOrder(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log
        )
        {
            string instanceId = req.Query["instanceId"];
            if (string.IsNullOrEmpty(instanceId))
            {
                return new BadRequestObjectResult("Please pass an instanceId on the query string");
            }

            try
            {
                // Parse the request body as JSON
                string requestBody = await new System.IO.StreamReader(req.Body).ReadToEndAsync();
                var confirmationRequest = JsonSerializer.Deserialize<ConfirmationRequest>(
                    requestBody
                );

                if (confirmationRequest == null)
                {
                    return new BadRequestObjectResult(
                        "Invalid request body. Expected JSON with 'confirm' property."
                    );
                }

                // Raise the event to the orchestration
                await client.RaiseEventAsync(
                    instanceId,
                    "OrderConfirmation",
                    confirmationRequest.Confirm
                );

                return new OkObjectResult(
                    $"Order confirmation status ({confirmationRequest.Confirm}) has been sent to instance {instanceId}"
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
