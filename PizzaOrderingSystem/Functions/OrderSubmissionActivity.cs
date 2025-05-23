using System;
using System.Threading.Tasks;
using AzureDurableFunctions.PizzaOrderingSystem.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class OrderSubmissionActivity
    {
        [FunctionName("SubmitOrder")]
        public static async Task<OrderResult> SubmitOrder(
            [ActivityTrigger] PizzaOrder order,
            ILogger log
        )
        {
            log.LogInformation("Submitting order {OrderId}", order.OrderId);

            try
            {
                // Simulate some processing time
                await Task.Delay(1000);
                return new OrderSubmitted { OrderId = order.OrderId, Message = "Order submitted successfully" };
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error submitting order {OrderId}", order.OrderId);
                return new OrderError { OrderId = order.OrderId, ErrorMessage = "Failed to submit order", Exception = ex };
            }
        }
    }
}
