#nullable enable
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace AzureDurableFunctions.PizzaOrderingSystem.Models
{
    [JsonConverter(typeof(OrderResultConverter))]
    public abstract record OrderResult
    {
        public string OrderId { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public abstract string ResultType { get; }
    }

    // Success cases
    public sealed record OrderSubmitted : OrderResult
    {
        public string Message { get; init; } = string.Empty;
        public override string ResultType => nameof(OrderSubmitted);
    }

    public sealed record PaymentProcessed : OrderResult
    {
        public decimal Amount { get; init; }
        public override string ResultType => nameof(PaymentProcessed);
    }

    public sealed record PizzaPrepared : OrderResult
    {
        public string PreparationDetails { get; init; } = string.Empty;
        public override string ResultType => nameof(PizzaPrepared);
    }

    public sealed record DeliveryUpdated : OrderResult
    {
        public string Status { get; init; } = string.Empty;
        public override string ResultType => nameof(DeliveryUpdated);
    }

    public sealed record OrderCompleted : OrderResult
    {
        public string CompletionMessage { get; init; } = string.Empty;
        public override string ResultType => nameof(OrderCompleted);
    }

    // Error cases
    public sealed record OrderError : OrderResult
    {
        public string ErrorMessage { get; init; } = string.Empty;

        [JsonIgnore]
        public Exception? Exception { get; init; }
        public override string ResultType => nameof(OrderError);
    }

    public sealed record PaymentError : OrderResult
    {
        public string ErrorMessage { get; init; } = string.Empty;
        public decimal AttemptedAmount { get; init; }
        public override string ResultType => nameof(PaymentError);
    }

    public sealed record PreparationError : OrderResult
    {
        public string ErrorMessage { get; init; } = string.Empty;
        public string FailedStep { get; init; } = string.Empty;
        public override string ResultType => nameof(PreparationError);
    }

    public sealed record DeliveryError : OrderResult
    {
        public string ErrorMessage { get; init; } = string.Empty;
        public string CurrentLocation { get; init; } = string.Empty;
        public override string ResultType => nameof(DeliveryError);
    }

    public class OrderResultConverter : JsonConverter<OrderResult>
    {
        public override OrderResult ReadJson(
            JsonReader reader,
            Type objectType,
            OrderResult existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            var jo = JObject.Load(reader);
            var resultType = jo["ResultType"]?.Value<string>();
            var orderId = jo["OrderId"]?.Value<string>() ?? string.Empty;
            var timestamp = jo["Timestamp"]?.Value<DateTime>() ?? DateTime.UtcNow;

            return resultType switch
            {
                nameof(OrderSubmitted) => new OrderSubmitted
                {
                    OrderId = orderId,
                    Timestamp = timestamp,
                    Message = jo["Message"]?.Value<string>() ?? string.Empty,
                },
                nameof(PaymentProcessed) => new PaymentProcessed
                {
                    OrderId = orderId,
                    Timestamp = timestamp,
                    Amount = jo["Amount"]?.Value<decimal>() ?? 0m,
                },
                nameof(PizzaPrepared) => new PizzaPrepared
                {
                    OrderId = orderId,
                    Timestamp = timestamp,
                    PreparationDetails = jo["PreparationDetails"]?.Value<string>() ?? string.Empty,
                },
                nameof(DeliveryUpdated) => new DeliveryUpdated
                {
                    OrderId = orderId,
                    Timestamp = timestamp,
                    Status = jo["Status"]?.Value<string>() ?? string.Empty,
                },
                nameof(OrderCompleted) => new OrderCompleted
                {
                    OrderId = orderId,
                    Timestamp = timestamp,
                    CompletionMessage = jo["CompletionMessage"]?.Value<string>() ?? string.Empty,
                },
                nameof(OrderError) => new OrderError
                {
                    OrderId = orderId,
                    Timestamp = timestamp,
                    ErrorMessage = jo["ErrorMessage"]?.Value<string>() ?? string.Empty,
                },
                nameof(PaymentError) => new PaymentError
                {
                    OrderId = orderId,
                    Timestamp = timestamp,
                    ErrorMessage = jo["ErrorMessage"]?.Value<string>() ?? string.Empty,
                    AttemptedAmount = jo["AttemptedAmount"]?.Value<decimal>() ?? 0m,
                },
                nameof(PreparationError) => new PreparationError
                {
                    OrderId = orderId,
                    Timestamp = timestamp,
                    ErrorMessage = jo["ErrorMessage"]?.Value<string>() ?? string.Empty,
                    FailedStep = jo["FailedStep"]?.Value<string>() ?? string.Empty,
                },
                nameof(DeliveryError) => new DeliveryError
                {
                    OrderId = orderId,
                    Timestamp = timestamp,
                    ErrorMessage = jo["ErrorMessage"]?.Value<string>() ?? string.Empty,
                    CurrentLocation = jo["CurrentLocation"]?.Value<string>() ?? string.Empty,
                },
                _ => throw new JsonSerializationException($"Unknown result type: {resultType}"),
            };
        }

        public override void WriteJson(
            JsonWriter writer,
            OrderResult value,
            JsonSerializer serializer
        )
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            };
            var jsonSerializer = JsonSerializer.Create(settings);

            var jo = new JObject();
            jo["ResultType"] = value.ResultType;
            jo["OrderId"] = value.OrderId;
            jo["Timestamp"] = value.Timestamp;

            switch (value)
            {
                case OrderSubmitted s:
                    jo["Message"] = s.Message;
                    break;
                case PaymentProcessed p:
                    jo["Amount"] = p.Amount;
                    break;
                case PizzaPrepared p:
                    jo["PreparationDetails"] = p.PreparationDetails;
                    break;
                case DeliveryUpdated d:
                    jo["Status"] = d.Status;
                    break;
                case OrderCompleted c:
                    jo["CompletionMessage"] = c.CompletionMessage;
                    break;
                case OrderError e:
                    jo["ErrorMessage"] = e.ErrorMessage;
                    break;
                case PaymentError e:
                    jo["ErrorMessage"] = e.ErrorMessage;
                    jo["AttemptedAmount"] = e.AttemptedAmount;
                    break;
                case PreparationError e:
                    jo["ErrorMessage"] = e.ErrorMessage;
                    jo["FailedStep"] = e.FailedStep;
                    break;
                case DeliveryError e:
                    jo["ErrorMessage"] = e.ErrorMessage;
                    jo["CurrentLocation"] = e.CurrentLocation;
                    break;
            }

            jo.WriteTo(writer);
        }
    }

    // Extension methods for pattern matching
    public static class OrderResultExtensions
    {
        public static bool IsSuccess(this OrderResult result) =>
            result switch
            {
                OrderSubmitted => true,
                PaymentProcessed => true,
                PizzaPrepared => true,
                DeliveryUpdated => true,
                OrderCompleted => true,
                _ => false,
            };

        public static string GetMessage(this OrderResult result) =>
            result switch
            {
                OrderSubmitted s => s.Message,
                PaymentProcessed p => $"Payment of {p.Amount:C} processed successfully",
                PizzaPrepared p => p.PreparationDetails,
                DeliveryUpdated d => d.Status,
                OrderCompleted c => c.CompletionMessage,
                OrderError e => $"Error: {e.ErrorMessage}",
                PaymentError e => $"Payment Error: {e.ErrorMessage}",
                PreparationError e => $"Preparation Error: {e.ErrorMessage} at step {e.FailedStep}",
                DeliveryError e => $"Delivery Error: {e.ErrorMessage} at {e.CurrentLocation}",
                _ => "Unknown result type",
            };
    }
}
