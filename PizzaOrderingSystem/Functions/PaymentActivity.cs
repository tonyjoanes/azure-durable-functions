using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class PaymentActivity
    {
        [FunctionName("ProcessPayment")]
        public static async Task<bool> ProcessPayment(
            [ActivityTrigger] string orderId,
            ILogger log)
        {
            log.LogInformation($"Processing payment for order {orderId}.");

            // Simulate payment processing
            await Task.Delay(2000); // Simulate a delay

            // For demo purposes, assume payment is successful
            return true;
        }
    }
} 