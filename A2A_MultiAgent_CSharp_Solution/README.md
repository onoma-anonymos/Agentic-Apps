# A2A Multi-Agent Framework (.NET 9 C# Reference Solution)

This project demonstrates an enterprise-grade **Agent-to-Agent (A2A)** multi-agent application in C# using Microsoft's Agent Framework & Semantic Kernel design patterns.

## Architecture & Agent Roles
1. **RouterAgent**: Evaluates incoming multimodal requests and classifies required modalities.
2. **OrchestratorAgent**: Decomposes the task and dispatches parallel A2A messages via async channels.
3. **TextWorkerAgent**: Analyzes text content, sentiment, entity extraction, and reasoning.
4. **ImageWorkerAgent**: Analyzes visual images, OCR extraction, and UI defect inspection.
5. **AudioWorkerAgent**: Analyzes speech transcripts, acoustic urgency markers, and audio metadata.
6. **SupervisorAgent**: Aggregates all worker results, verifies cross-modal consistency, and synthesizes the unified final response.

## Execution Flow

The following sequence diagram illustrates how the multi-agent system processes a multimodal user query:

```mermaid
sequenceDiagram
    participant User
    participant Router as RouterAgent
    participant Orchestrator as OrchestratorAgent
    participant TextWorker as TextWorkerAgent
    participant ImageWorker as ImageWorkerAgent
    participant AudioWorker as AudioWorkerAgent
    participant Supervisor as SupervisorAgent

    User->>Router: RouteUserQueryAsync(query, hasImage, hasAudio)
    Router->>Router: Classify modalities & extract intent
    Router->>Orchestrator: Dispatch decomposed task
    
    Orchestrator->>TextWorker: Analyze ticket #8849 (text analysis)
    Orchestrator->>ImageWorker: Check payment error screenshot (OCR & defect detection)
    Orchestrator->>AudioWorker: Listen to customer voicemail (transcription & urgency)
    
    par Parallel Processing
        TextWorker->>TextWorker: Entity extraction, sentiment analysis
        ImageWorker->>ImageWorker: Visual analysis, UI defect inspection
        AudioWorker->>AudioWorker: Speech transcription, acoustic markers
    end
    
    TextWorker-->>Supervisor: Text analysis results
    ImageWorker-->>Supervisor: Image analysis results
    AudioWorker-->>Supervisor: Audio analysis results
    
    Supervisor->>Supervisor: Aggregate results & verify consistency
    Supervisor->>Supervisor: Synthesize unified response
    Supervisor-->>Router: Final AgentResponse
    Router-->>User: Intent + Confidence + OutputData
```

## Quick Start
```bash
# 1. Restore NuGet dependencies
dotnet restore

# 2. Build and execute the sample A2A workflow
dotnet run
```
