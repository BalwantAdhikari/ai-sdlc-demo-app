# AI interaction logging

This repository includes a lightweight, hook-friendly mechanism for recording AI-assisted work in a traceable way.

## Files

- `.specify/extensions.yml` defines the hook points for AI workflow steps.
- `.specify/scripts/bash/log-ai-interaction.sh` appends structured JSONL events.
- `logs/ai-interactions.jsonl` stores the actual interaction trace.
- `logs/ai-interactions-summary.md` provides a readable summary for reviewers.

## Why this matters

The goal is to capture more than prompt counts. These logs preserve:

- the AI tool and model used
- the agent type and execution mode
- the request and any follow-up interaction
- the skills and sub-agents invoked
- the tools or MCP integrations used
- verification evidence and affected files
- human corrections or decisions
- the final outcome and whether the work required rework

## Hooked workflow

The project’s Spec Kit-style hook configuration calls the logger before and after key workflow stages so the records stay current without requiring manual documentation.

## Example JSONL entry

```json
{
  "id": "20261101T000000Z-agent-requirement-transcription",
  "timestamp": "2026-11-01T00:00:00Z",
  "stage": "setup",
  "agent": "agent",
  "agent_name": "agent",
  "agent_type": "built-in",
  "ai_tool": "GitHub Copilot",
  "model": "MAI-Code-1.1-Flash",
  "mode": "agent",
  "context": "Assignment for AI in SDLC",
  "request": "Analyse Assignment requirement",
  "skills_used": [],
  "subagents_used": [],
  "mcp_tools_used": [],
  "tools_used": [],
  "status": "applied",
  "outcome": "applied"
}
```

## Refreshing the summary

```bash
bash .specify/scripts/bash/log-ai-interaction.sh --summary
```
