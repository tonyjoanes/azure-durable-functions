using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace azure_durable_functions.PizzaOrderingSystem.Functions
{
    public static class ListPendingOrders
    {
        [FunctionName("ListPendingOrders")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/pending")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            log.LogInformation("Getting list of pending orders");

            try
            {
                var pendingOrders = new List<object>();

                // Get all orchestration instances
                var condition = new OrchestrationStatusQueryCondition
                {
                    RuntimeStatus = new[]
                    {
                        OrchestrationRuntimeStatus.Running  // These are the pending orders
                    },
                    PageSize = 100
                };

                var queryResult = await client.ListInstancesAsync(condition, default);

                foreach (var instance in queryResult.DurableOrchestrationState)
                {
                    // Only include OrderOrchestrator instances
                    if (instance.Name == "OrderOrchestrator")
                    {
                        // Default order details
                        string size = "Unknown";
                        string[] toppings = new string[] { };
                        string address = "Unknown";
                        string phone = "Unknown";

                        // Try to extract order details from input
                        if (instance.Input != null)
                        {
                            try
                            {
                                var inputData = instance.Input.ToObject<dynamic>();
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
                                log.LogWarning($"Failed to parse input for instance {instance.InstanceId}: {ex.Message}");
                            }
                        }

                        var pendingOrder = new
                        {
                            id = instance.InstanceId,
                            status = "pending",
                            size = size,
                            toppings = toppings,
                            address = address,
                            phone = phone,
                            timestamp = instance.CreatedTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            createdTime = instance.CreatedTime,
                            lastUpdatedTime = instance.LastUpdatedTime
                        };

                        pendingOrders.Add(pendingOrder);
                    }
                }

                log.LogInformation($"Found {pendingOrders.Count} pending orders");

                return new OkObjectResult(new
                {
                    orders = pendingOrders.OrderByDescending(o => ((dynamic)o).createdTime),
                    count = pendingOrders.Count,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                log.LogError($"Error getting pending orders: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }
    }
} 