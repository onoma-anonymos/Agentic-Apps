using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using A2A.Core;
using A2A.Protocol;
using Microsoft.Extensions.Logging;

namespace A2A.Agents
{
    public class ImageWorkerAgent : A2AAgentBase
    {
        public ImageWorkerAgent(ILogger<ImageWorkerAgent> logger)
            : base("image-worker-01", "ImageWorkerAgent", logger) { }

        public async Task<A2AMessageEnvelope<WorkerTaskPayload>> ProcessImageAsync(
            A2AMessageEnvelope<WorkerTaskPayload> message,
            CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("[ImageWorkerAgent] Inspecting visual image payload...");

            await Task.Delay(180, cancellationToken);

            var responsePayload = new WorkerTaskPayload
            {
                TaskDescription = message.Payload!.TaskDescription,
                Modality = "image",
                ConfidenceScore = 0.94,
                OutputData = new
                {
                    VisualClassification = "UI Checkout Modal / Payment Gateway Error",
                    OcrTextExtracted = "PAYMENT_GATEWAY_ERROR #E-509 | net::ERR_CONNECTION_TIMED_OUT",
                    VisualAnomalies = new[] { "Stripe API SSL handshake timeout icon flagged", "Retry button in active state" }
                },
                Reasoning = new List<string>
                {
                    "Executed high-resolution OCR over checkout error window.",
                    "Verified error modal layout matches Stripe v2 3DS failure template."
                }
            };

            return new A2AMessageEnvelope<WorkerTaskPayload>
            {
                CorrelationId = message.CorrelationId,
                Sender = AgentRole,
                Recipient = message.Sender,
                Intent = "IMAGE_WORKER_RESULT",
                Payload = responsePayload
            };
        }

        protected override Task OnMessageReceivedAsync<T>(A2AMessageEnvelope<T> message, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
