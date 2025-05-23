using System;
using System.Threading.Tasks;
using AzureDurableFunctions.PizzaOrderingSystem.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class OrderCompletionActivity
    {
        [FunctionName("CompleteOrder")]
        public static async Task<OrderResult> CompleteOrder(
            [ActivityTrigger] string orderId,
            ILogger log
        )
        {
            log.LogInformation("Completing order {OrderId}", orderId);

            try
            {
                // Simulate order completion
                await Task.Delay(500);
                return new OrderCompleted
                {
                    OrderId = orderId,
                    CompletionMessage = "Order completed successfully",
                };
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error completing order {OrderId}", orderId);
                return new OrderError
                {
                    OrderId = orderId,
                    ErrorMessage = "Failed to complete order",
                    Exception = ex,
                };
            }
        }
    }
}
