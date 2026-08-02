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
    /// Orchestrates parallel execution across specialized workers (Text, Image, Audio)
    /// and forwards aggregated worker results to SupervisorAgent.
    /// </summary>
    public class OrchestratorAgent : A2AAgentBase
    {
        private readonly TextWorkerAgent _textWorker;
        private readonly ImageWorkerAgent _imageWorker;
        private readonly AudioWorkerAgent _audioWorker;
        private readonly SupervisorAgent _supervisor;

        public OrchestratorAgent(
            TextWorkerAgent textWorker,
            ImageWorkerAgent imageWorker,
            AudioWorkerAgent audioWorker,
            SupervisorAgent supervisor,
            ILogger<OrchestratorAgent> logger)
            : base("orchestrator-01", "OrchestratorAgent", logger)
        {
            _textWorker = textWorker;
            _imageWorker = imageWorker;
            _audioWorker = audioWorker;
            _supervisor = supervisor;
        }

        public async Task<A2AMessageEnvelope<WorkerTaskPayload>> ExecuteOrchestrationAsync(
            A2AMessageEnvelope<WorkerTaskPayload> routeMessage,
            CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("[OrchestratorAgent] Decomposing workflow for CorrelationId: {Id}", routeMessage.CorrelationId);

            var workers = new List<Task<A2AMessageEnvelope<WorkerTaskPayload>>>();

            // 1. Text Worker (Always required for reasoning & NLP)
            var textMsg = CreateWorkerMessage(routeMessage.CorrelationId, "text-worker", "PROCESS_NLP_TASK", routeMessage.Payload!);
            workers.Add(_textWorker.ProcessTextAsync(textMsg, cancellationToken));

            // 2. Image Worker (If image present in request plan)
            var imgMsg = CreateWorkerMessage(routeMessage.CorrelationId, "image-worker", "PROCESS_VISION_TASK", routeMessage.Payload!);
            workers.Add(_imageWorker.ProcessImageAsync(imgMsg, cancellationToken));

            // 3. Audio Worker (If audio present in request plan)
            var audMsg = CreateWorkerMessage(routeMessage.CorrelationId, "audio-worker", "PROCESS_AUDIO_TASK", routeMessage.Payload!);
            workers.Add(_audioWorker.ProcessAudioAsync(audMsg, cancellationToken));

            // Execute parallel A2A worker calls
            var results = await Task.WhenAll(workers);

            Logger.LogInformation("[OrchestratorAgent] {Count} workers finished. Dispatching to Supervisor.", results.Length);

            var supervisorMessage = new A2AMessageEnvelope<WorkerTaskPayload>
            {
                CorrelationId = routeMessage.CorrelationId,
                Sender = AgentRole,
                Recipient = _supervisor.AgentRole,
                Intent = "AGGREGATE_AND_SYNTHESIZE",
                Priority = MessagePriority.High,
                Payload = new WorkerTaskPayload
                {
                    TaskDescription = routeMessage.Payload!.TaskDescription,
                    Modality = "supervisor-aggregation",
                    InputData = results,
                    ConfidenceScore = 0.95
                }
            };

            return await _supervisor.SynthesizeResponseAsync(supervisorMessage, cancellationToken);
        }

        private A2AMessageEnvelope<WorkerTaskPayload> CreateWorkerMessage(
            string correlationId, string targetRole, string intent, WorkerTaskPayload sourcePayload)
        {
            return new A2AMessageEnvelope<WorkerTaskPayload>
            {
                CorrelationId = correlationId,
                Sender = AgentRole,
                Recipient = targetRole,
                Intent = intent,
                Priority = MessagePriority.Normal,
                Payload = new WorkerTaskPayload
                {
                    TaskDescription = sourcePayload.TaskDescription,
                    Modality = targetRole.Replace("-worker", ""),
                    InputData = sourcePayload.InputData
                }
            };
        }

        protected override Task OnMessageReceivedAsync<T>(A2AMessageEnvelope<T> message, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
