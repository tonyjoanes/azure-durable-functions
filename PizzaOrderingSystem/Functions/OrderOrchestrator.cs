using System;
using System.Threading.Tasks;
using AzureDurableFunctions.PizzaOrderingSystem.Models;
using AzureDurableFunctions.PizzaOrderingSystem.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public class OrderOrchestrator
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderOrchestrator> _logger;

        public OrderOrchestrator(IOrderService orderService, ILogger<OrderOrchestrator> logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("OrderOrchestrator")]
        public async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context
        )
        {
            var correlationId = context.InstanceId;
            _logger.LogInformation(
                "Starting pizza order orchestration. CorrelationId: {CorrelationId}",
                correlationId
            );

            try
            {
                // Get the order data from input
                var order = context.GetInput<PizzaOrder>();
                _logger.LogInformation("Order data received for order {OrderId}", order.OrderId);

                // Step 1: Submit the order
                var submitResult = await context.CallActivityWithRetryAsync<OrderResult>(
                    "SubmitOrder",
                    new RetryOptions(TimeSpan.FromSeconds(5), 3),
                    order
                );

                if (!submitResult.IsSuccess())
                {
                    _logger.LogError(
                        "Order submission failed: {Message}",
                        submitResult.GetMessage()
                    );
                    return submitResult.GetMessage();
                }

                // Step 2: Wait for order confirmation
                _logger.LogInformation(
                    "Waiting for confirmation of order {OrderId}",
                    order.OrderId
                );
                var confirmationResult = await context.WaitForExternalEvent<bool>(
                    "OrderConfirmation"
                );

                if (!confirmationResult)
                {
                    _logger.LogInformation("Order {OrderId} was rejected", order.OrderId);
                    return $"Order {order.OrderId} was rejected by the user.";
                }

                _logger.LogInformation(
                    "Order {OrderId} was confirmed. Proceeding with payment",
                    order.OrderId
                );

                // Step 3: Process payment
                var paymentResult = await context.CallActivityWithRetryAsync<OrderResult>(
                    "ProcessPayment",
                    new RetryOptions(TimeSpan.FromSeconds(5), 3),
                    order.OrderId
                );

                if (!paymentResult.IsSuccess())
                {
                    _logger.LogError("Payment failed: {Message}", paymentResult.GetMessage());
                    return paymentResult.GetMessage();
                }

                // Step 4: Prepare the pizza in the kitchen
                var kitchenResult = await context.CallActivityWithRetryAsync<OrderResult>(
                    "PreparePizza",
                    new RetryOptions(TimeSpan.FromSeconds(5), 3),
                    order.OrderId
                );

                if (!kitchenResult.IsSuccess())
                {
                    _logger.LogError(
                        "Pizza preparation failed: {Message}",
                        kitchenResult.GetMessage()
                    );
                    return kitchenResult.GetMessage();
                }

                // Step 5: Update delivery status
                var deliveryResult = await context.CallActivityWithRetryAsync<OrderResult>(
                    "UpdateDeliveryStatus",
                    new RetryOptions(TimeSpan.FromSeconds(5), 3),
                    order.OrderId
                );

                if (!deliveryResult.IsSuccess())
                {
                    _logger.LogError(
                        "Delivery update failed: {Message}",
                        deliveryResult.GetMessage()
                    );
                    return deliveryResult.GetMessage();
                }

                // Step 6: Complete the order
                var completionResult = await context.CallActivityWithRetryAsync<OrderResult>(
                    "CompleteOrder",
                    new RetryOptions(TimeSpan.FromSeconds(5), 3),
                    order.OrderId
                );

                if (!completionResult.IsSuccess())
                {
                    _logger.LogError(
                        "Order completion failed: {Message}",
                        completionResult.GetMessage()
                    );
                    return completionResult.GetMessage();
                }

                return completionResult.GetMessage();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing order. CorrelationId: {CorrelationId}",
                    correlationId
                );
                throw;
            }
        }
    }
}
