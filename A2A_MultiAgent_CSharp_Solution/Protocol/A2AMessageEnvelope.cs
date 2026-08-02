using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace A2A.Protocol
{
    /// <summary>
    /// Represents an Agent-to-Agent (A2A) structured message envelope used across
    /// Microsoft.SemanticKernel.Agents and custom A2A worker pipelines.
    /// </summary>
    /// <typeparam name="TPayload">Type of the message payload.</typeparam>
    public class A2AMessageEnvelope<TPayload>
    {
        [JsonPropertyName("messageId")]
        public string MessageId { get; set; } = Guid.NewGuid().ToString("N");

        [JsonPropertyName("correlationId")]
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        [JsonPropertyName("sender")]
        public string Sender { get; set; } = string.Empty;

        [JsonPropertyName("recipient")]
        public string Recipient { get; set; } = string.Empty;

        [JsonPropertyName("intent")]
        public string Intent { get; set; } = string.Empty;

        [JsonPropertyName("priority")]
        public MessagePriority Priority { get; set; } = MessagePriority.Normal;

        [JsonPropertyName("payload")]
        public TPayload? Payload { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public enum MessagePriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
    }

    public class WorkerTaskPayload
    {
        [JsonPropertyName("taskDescription")]
        public string TaskDescription { get; set; } = string.Empty;

        [JsonPropertyName("modality")]
        public string Modality { get; set; } = "text";

        [JsonPropertyName("inputData")]
        public object? InputData { get; set; }

        [JsonPropertyName("outputData")]
        public object? OutputData { get; set; }

        [JsonPropertyName("confidenceScore")]
        public double ConfidenceScore { get; set; }

        [JsonPropertyName("reasoning")]
        public List<string> Reasoning { get; set; } = new();
    }
}
