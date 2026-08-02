using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using A2A.Protocol;
using Microsoft.Extensions.Logging;

namespace A2A.Core
{
    public interface IA2AAgent
    {
        string AgentId { get; }
        string AgentRole { get; }
        Task SendMessageAsync<T>(A2AMessageEnvelope<T> message, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Base class for asynchronous A2A agents utilizing high-throughput bounded System.Threading.Channels.
    /// </summary>
    public abstract class A2AAgentBase : IA2AAgent
    {
        private readonly Channel<object> _inbox;
        protected readonly ILogger Logger;

        public string AgentId { get; }
        public string AgentRole { get; }

        protected A2AAgentBase(string agentId, string agentRole, ILogger logger)
        {
            AgentId = agentId;
            AgentRole = agentRole;
            Logger = logger;
            _inbox = Channel.CreateBounded<object>(new BoundedChannelOptions(500)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true
            });
        }

        public async Task SendMessageAsync<T>(A2AMessageEnvelope<T> message, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("[A2A Message] {Sender} -> {Recipient} | Intent: {Intent}",
                message.Sender, message.Recipient, message.Intent);

            await _inbox.Writer.WriteAsync(message, cancellationToken);
            await OnMessageReceivedAsync(message, cancellationToken);
        }

        protected abstract Task OnMessageReceivedAsync<T>(A2AMessageEnvelope<T> message, CancellationToken cancellationToken);
    }
}
