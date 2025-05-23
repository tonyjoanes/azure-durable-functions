using System;
using System.Threading.Tasks;
using AzureDurableFunctions.PizzaOrderingSystem.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class PaymentActivity
    {
        [FunctionName("ProcessPayment")]
        public static async Task<OrderResult> ProcessPayment(
            [ActivityTrigger] string orderId,
            ILogger log
        )
        {
            log.LogInformation("Processing payment for order {OrderId}", orderId);

            try
            {
                // Simulate payment processing
                await Task.Delay(1500);
                return new PaymentProcessed { OrderId = orderId, Amount = 19.99m };
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing payment for order {OrderId}", orderId);
                return new PaymentError { OrderId = orderId, ErrorMessage = "Payment processing failed", AttemptedAmount = 19.99m };
            }
        }
    }
}
