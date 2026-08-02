using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using A2A.Core;
using A2A.Protocol;
using Microsoft.Extensions.Logging;

namespace A2A.Agents
{
    /// <summary>
    /// Evaluates incoming user requests, detects required modalities (Text, Image, Audio),
    /// and dispatches an A2A execution plan to the OrchestratorAgent.
    /// </summary>
    public class RouterAgent : A2AAgentBase
    {
        private readonly OrchestratorAgent _orchestrator;

        public RouterAgent(OrchestratorAgent orchestrator, ILogger<RouterAgent> logger)
            : base("router-01", "RouterAgent", logger)
        {
            _orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
        }

        public async Task<A2AMessageEnvelope<WorkerTaskPayload>> RouteUserQueryAsync(
            string userQuery,
            bool hasImage,
            bool hasAudio,
            string correlationId,
            CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("[RouterAgent] Analyzing query: '{Query}' (Image: {Img}, Audio: {Aud})",
                userQuery, hasImage, hasAudio);

            var planPayload = new WorkerTaskPayload
            {
                TaskDescription = userQuery,
                Modality = "multimodal-plan",
                InputData = new
                {
                    Query = userQuery,
                    RequireTextAnalysis = true,
                    RequireImageAnalysis = hasImage,
                    RequireAudioAnalysis = hasAudio
                },
                Reasoning = new List<string>
                {
                    "Detected natural language query -> Route text worker.",
                    hasImage ? "Detected image attachment -> Enable vision OCR/defect inspection." : "No image detected -> Skip vision.",
                    hasAudio ? "Detected audio waveform/transcript -> Enable speech tone/intent analysis." : "No audio detected -> Skip speech."
                }
            };

            var routeMessage = new A2AMessageEnvelope<WorkerTaskPayload>
            {
                CorrelationId = correlationId,
                Sender = AgentRole,
                Recipient = _orchestrator.AgentRole,
                Intent = "ORCHESTRATE_MULTIMODAL_FLOW",
                Priority = MessagePriority.High,
                Payload = planPayload
            };

            return await _orchestrator.ExecuteOrchestrationAsync(routeMessage, cancellationToken);
        }

        protected override Task OnMessageReceivedAsync<T>(A2AMessageEnvelope<T> message, CancellationToken cancellationToken)
        {
            // Router inbox handler
            return Task.CompletedTask;
        }
    }
}
