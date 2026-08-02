using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using A2A.Core;
using A2A.Protocol;
using Microsoft.Extensions.Logging;

namespace A2A.Agents
{
    public class TextWorkerAgent : A2AAgentBase
    {
        public TextWorkerAgent(ILogger<TextWorkerAgent> logger)
            : base("text-worker-01", "TextWorkerAgent", logger) { }

        public async Task<A2AMessageEnvelope<WorkerTaskPayload>> ProcessTextAsync(
            A2AMessageEnvelope<WorkerTaskPayload> message,
            CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("[TextWorkerAgent] Processing NLP task: '{Task}'", message.Payload?.TaskDescription);

            // Simulate high-throughput LLM reasoning delay / inference
            await Task.Delay(140, cancellationToken);

            var responsePayload = new WorkerTaskPayload
            {
                TaskDescription = message.Payload!.TaskDescription,
                Modality = "text",
                ConfidenceScore = 0.96,
                OutputData = new
                {
                    Summary = "Identified critical user intent and extracted key technical entities.",
                    Sentiment = "Urgent / Action-Required",
                    KeyEntities = new[] { "Stripe 3D Secure", "E-509 Handshake Timeout", "Apex Logistics" }
                },
                Reasoning = new List<string>
                {
                    "Parsed syntax structure and sentiment intensity.",
                    "Matched error code E-509 against known gateway timeout documentation."
                }
            };

            return new A2AMessageEnvelope<WorkerTaskPayload>
            {
                CorrelationId = message.CorrelationId,
                Sender = AgentRole,
                Recipient = message.Sender,
                Intent = "TEXT_WORKER_RESULT",
                Payload = responsePayload
            };
        }

        protected override Task OnMessageReceivedAsync<T>(A2AMessageEnvelope<T> message, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
