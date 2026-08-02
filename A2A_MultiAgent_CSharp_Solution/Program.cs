using System;
using System.Threading.Tasks;
using A2A.Agents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace A2A
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("===================================================================");
            Console.WriteLine("    A2A MULTI-AGENT STUDIO (.NET 9 & AGENT FRAMEWORK IN C#)       ");
            Console.WriteLine("    Router -> Orchestrator -> [Text, Image, Audio] -> Supervisor  ");
            Console.WriteLine("===================================================================\n");

            var services = new ServiceCollection();
            services.AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Information));

            // Register A2A Specialized Worker Agents
            services.AddSingleton<TextWorkerAgent>();
            services.AddSingleton<ImageWorkerAgent>();
            services.AddSingleton<AudioWorkerAgent>();

            // Register Supervisor, Orchestrator, and Router Agents
            services.AddSingleton<SupervisorAgent>();
            services.AddSingleton<OrchestratorAgent>();
            services.AddSingleton<RouterAgent>();

            var serviceProvider = services.BuildServiceProvider();

            // Resolve Router Agent to initiate execution
            var router = serviceProvider.GetRequiredService<RouterAgent>();

            string sampleQuery = "Analyze ticket #8849. Check the payment error screenshot and listen to the customer voicemail.";
            string correlationId = Guid.NewGuid().ToString("N");

            Console.WriteLine($"[User Query] {sampleQuery}\n");

            var finalResponse = await router.RouteUserQueryAsync(
                userQuery: sampleQuery,
                hasImage: true,
                hasAudio: true,
                correlationId: correlationId
            );

            Console.WriteLine("\n===================================================================");
            Console.WriteLine("    SUPERVISOR AGGREGATED RESPONSE (VERIFIED)                      ");
            Console.WriteLine("===================================================================");
            Console.WriteLine($"Intent Code:      {finalResponse.Intent}");
            Console.WriteLine($"Confidence Score: {finalResponse.Payload?.ConfidenceScore:P0}");
            Console.WriteLine($"Synthesis Output: {System.Text.Json.JsonSerializer.Serialize(finalResponse.Payload?.OutputData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");
            Console.WriteLine("===================================================================\n");
        }
    }
}
