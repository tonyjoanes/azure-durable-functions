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

            return resultType switch
            {
                nameof(OrderSubmitted) => jo.ToObject<OrderSubmitted>(serializer),
                nameof(PaymentProcessed) => jo.ToObject<PaymentProcessed>(serializer),
                nameof(PizzaPrepared) => jo.ToObject<PizzaPrepared>(serializer),
                nameof(DeliveryUpdated) => jo.ToObject<DeliveryUpdated>(serializer),
                nameof(OrderCompleted) => jo.ToObject<OrderCompleted>(serializer),
                nameof(OrderError) => jo.ToObject<OrderError>(serializer),
                nameof(PaymentError) => jo.ToObject<PaymentError>(serializer),
                nameof(PreparationError) => jo.ToObject<PreparationError>(serializer),
                nameof(DeliveryError) => jo.ToObject<DeliveryError>(serializer),
                _ => throw new JsonSerializationException($"Unknown result type: {resultType}"),
            };
        }

        public override void WriteJson(
            JsonWriter writer,
            OrderResult value,
            JsonSerializer serializer
        )
        {
            serializer.Serialize(writer, value);
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
