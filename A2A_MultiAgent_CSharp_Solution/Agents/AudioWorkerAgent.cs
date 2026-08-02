using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using A2A.Core;
using A2A.Protocol;
using Microsoft.Extensions.Logging;

namespace A2A.Agents
{
    public class AudioWorkerAgent : A2AAgentBase
    {
        public AudioWorkerAgent(ILogger<AudioWorkerAgent> logger)
            : base("audio-worker-01", "AudioWorkerAgent", logger) { }

        public async Task<A2AMessageEnvelope<WorkerTaskPayload>> ProcessAudioAsync(
            A2AMessageEnvelope<WorkerTaskPayload> message,
            CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("[AudioWorkerAgent] Processing audio transcript & vocal acoustics...");

            await Task.Delay(160, cancellationToken);

            var responsePayload = new WorkerTaskPayload
            {
                TaskDescription = message.Payload!.TaskDescription,
                Modality = "audio",
                ConfidenceScore = 0.93,
                OutputData = new
                {
                    AcousticTone = "High Urgency / Executive Escalation",
                    SpeakerIdentified = "IT Director (Marcus Vance)",
                    TranscriptSummary = "Customer blocked by SSL E-509 timeout during renewal before midnight lock out."
                },
                Reasoning = new List<string>
                {
                    "Analyzed voice pitch stress markers and speaking pace (165 WPM).",
                    "Extracted critical SLA deadline ('midnight lockout')."
                }
            };

            return new A2AMessageEnvelope<WorkerTaskPayload>
            {
                CorrelationId = message.CorrelationId,
                Sender = AgentRole,
                Recipient = message.Sender,
                Intent = "AUDIO_WORKER_RESULT",
                Payload = responsePayload
            };
        }

        protected override Task OnMessageReceivedAsync<T>(A2AMessageEnvelope<T> message, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
