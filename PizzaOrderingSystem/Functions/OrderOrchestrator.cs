using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class OrderOrchestrator
    {
        [FunctionName("OrderOrchestrator")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            log.LogInformation("Starting pizza order orchestration.");

            // Step 1: Submit the order
            var orderId = await context.CallActivityAsync<string>("SubmitOrder", "Pizza Order");

            // Step 2: Wait for order confirmation
            log.LogInformation($"Waiting for confirmation of order {orderId}.");
            var confirmationResult = await context.WaitForExternalEvent<bool>("OrderConfirmation");

            if (!confirmationResult)
            {
                log.LogInformation($"Order {orderId} was rejected.");
                return $"Order {orderId} was rejected by the user.";
            }

            log.LogInformation($"Order {orderId} was confirmed. Proceeding with payment.");

            // Step 3: Process payment
            var paymentResult = await context.CallActivityAsync<bool>("ProcessPayment", orderId);
            if (!paymentResult)
            {
                log.LogError($"Payment failed for order {orderId}.");
                return $"Order {orderId} failed due to payment issues.";
            }

            // Step 4: Prepare the pizza in the kitchen
            var kitchenResult = await context.CallActivityAsync<string>("PreparePizza", orderId);
            log.LogInformation($"Kitchen result: {kitchenResult}");

            // Step 5: Update delivery status
            var deliveryStatus = await context.CallActivityAsync<string>("UpdateDeliveryStatus", orderId);
            log.LogInformation($"Delivery status: {deliveryStatus}");

            // Step 6: Complete the order
            var completionResult = await context.CallActivityAsync<string>("CompleteOrder", orderId);
            log.LogInformation($"Order completion: {completionResult}");

            return $"Order {orderId} completed successfully!";
        }
    }
}
