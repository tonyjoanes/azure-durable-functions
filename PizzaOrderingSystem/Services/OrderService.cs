using System;
using System.Threading.Tasks;
using AzureDurableFunctions.PizzaOrderingSystem.Models;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly Random _random = new Random();

        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderResult> SubmitOrderAsync(PizzaOrder order)
        {
            try
            {
                _logger.LogInformation("Submitting order {OrderId}", order.OrderId);
                await Task.Delay(1000);
                return new OrderSubmitted
                {
                    OrderId = order.OrderId,
                    Message = "Order submitted successfully",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting order {OrderId}", order.OrderId);
                return new OrderError
                {
                    OrderId = order.OrderId,
                    ErrorMessage = "Failed to submit order",
                    Exception = ex,
                };
            }
        }

        public async Task<OrderResult> ProcessPaymentAsync(string orderId)
        {
            try
            {
                _logger.LogInformation("Processing payment for order {OrderId}", orderId);
                await Task.Delay(1500);

                // Simulate random payment success/failure
                if (_random.Next(100) < 90) // 90% success rate
                {
                    return new PaymentProcessed { OrderId = orderId, Amount = 19.99m };
                }
                return new PaymentError
                {
                    OrderId = orderId,
                    ErrorMessage = "Payment declined",
                    AttemptedAmount = 19.99m,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for order {OrderId}", orderId);
                return new PaymentError
                {
                    OrderId = orderId,
                    ErrorMessage = "Payment processing failed",
                    AttemptedAmount = 19.99m,
                };
            }
        }

        public async Task<OrderResult> PreparePizzaAsync(string orderId)
        {
            try
            {
                _logger.LogInformation("Preparing pizza for order {OrderId}", orderId);
                await Task.Delay(2000);

                // Simulate random preparation success/failure
                if (_random.Next(100) < 95) // 95% success rate
                {
                    return new PizzaPrepared
                    {
                        OrderId = orderId,
                        PreparationDetails = "Pizza prepared with extra cheese and pepperoni",
                    };
                }
                return new PreparationError
                {
                    OrderId = orderId,
                    ErrorMessage = "Failed to prepare pizza",
                    FailedStep = "Oven malfunction",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing pizza for order {OrderId}", orderId);
                return new PreparationError
                {
                    OrderId = orderId,
                    ErrorMessage = "Pizza preparation failed",
                    FailedStep = "Unknown",
                };
            }
        }

        public async Task<OrderResult> UpdateDeliveryStatusAsync(string orderId)
        {
            try
            {
                _logger.LogInformation("Updating delivery status for order {OrderId}", orderId);
                await Task.Delay(1000);

                // Simulate random delivery success/failure
                if (_random.Next(100) < 85) // 85% success rate
                {
                    return new DeliveryUpdated { OrderId = orderId, Status = "Out for delivery" };
                }
                return new DeliveryError
                {
                    OrderId = orderId,
                    ErrorMessage = "Delivery delayed",
                    CurrentLocation = "Traffic congestion",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating delivery status for order {OrderId}", orderId);
                return new DeliveryError
                {
                    OrderId = orderId,
                    ErrorMessage = "Failed to update delivery status",
                    CurrentLocation = "Unknown",
                };
            }
        }

        public async Task<OrderResult> CompleteOrderAsync(string orderId)
        {
            try
            {
                _logger.LogInformation("Completing order {OrderId}", orderId);
                await Task.Delay(500);
                return new OrderCompleted
                {
                    OrderId = orderId,
                    CompletionMessage = "Order delivered successfully",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing order {OrderId}", orderId);
                return new OrderError
                {
                    OrderId = orderId,
                    ErrorMessage = "Failed to complete order",
                };
            }
        }

        public async Task<PizzaOrder> GetOrderAsync(string orderId)
        {
            _logger.LogInformation("Retrieving order {OrderId}", orderId);
            await Task.Delay(500);
            return new PizzaOrder { OrderId = orderId, Status = OrderStatus.Confirmed };
        }
    }
}
