using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace azure_durable_functions.PizzaOrderingSystem.Functions
{
    public static class OrderStatus
    {
        [FunctionName("OrderStatus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "OrderStatus/{orderId}")]
                HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            string orderId,
            ILogger log
        )
        {
            log.LogInformation($"Getting status for order ID: {orderId}");

            try
            {
                // Get the orchestration status using the order ID as instance ID
                var status = await client.GetStatusAsync(orderId);

                if (status == null)
                {
                    log.LogWarning($"Order not found: {orderId}");
                    return new NotFoundObjectResult(
                        new { error = "Order not found", orderId = orderId }
                    );
                }

                // Default order details
                string size = "Unknown";
                string[] toppings = new string[] { };
                string address = "Unknown";
                string phone = "Unknown";

                // Try to extract order details from input (when order was created)
                if (status.Input != null)
                {
                    try
                    {
                        var inputData = status.Input.ToObject<dynamic>();
                        size = inputData?.size?.ToString() ?? size;
                        address = inputData?.address?.ToString() ?? address;
                        phone = inputData?.phone?.ToString() ?? phone;

                        if (inputData?.toppings != null)
                        {
                            var toppingsArray = inputData.toppings as JArray;
                            if (toppingsArray != null)
                            {
                                toppings = toppingsArray.ToObject<string[]>() ?? toppings;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogWarning($"Failed to parse input data: {ex.Message}");
                    }
                }

                // Try to extract from custom status (if orchestration sets it)
                if (status.CustomStatus != null)
                {
                    try
                    {
                        var customStatus = status.CustomStatus.ToObject<dynamic>();
                        size = customStatus?.size?.ToString() ?? size;
                        address = customStatus?.address?.ToString() ?? address;
                        phone = customStatus?.phone?.ToString() ?? phone;

                        if (customStatus?.toppings != null)
                        {
                            var toppingsArray = customStatus.toppings as JArray;
                            if (toppingsArray != null)
                            {
                                toppings = toppingsArray.ToObject<string[]>() ?? toppings;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogWarning($"Failed to parse custom status: {ex.Message}");
                    }
                }

                // Return the exact format expected by the React component
                var orderResponse = new
                {
                    id = orderId,
                    status = GetOrderStatus(status.RuntimeStatus.ToString()),
                    size = size,
                    toppings = toppings,
                    address = address,
                    phone = phone,
                    timestamp = status.CreatedTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                };

                return new OkObjectResult(orderResponse);
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting order status: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        private static string GetOrderStatus(string runtimeStatus)
        {
            return runtimeStatus switch
            {
                "Running" => "processing",
                "Completed" => "completed",
                "Failed" => "failed",
                "Canceled" => "canceled",
                "Terminated" => "terminated",
                "Pending" => "pending",
                _ => "unknown",
            };
        }
    }
}
