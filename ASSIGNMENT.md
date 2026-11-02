# Assignment: AI Interaction Traceability and Quality Evidence

## repo link

https://github.com/BalwantAdhikari/ai-sdlc-demo-app

## PR link

https://github.com/BalwantAdhikari/ai-sdlc-demo-app/pull/1


## video link

https://nagarro-my.sharepoint.com/:v:/p/balwant_adhikari/IQAu2eRpLmXdSrtvq1YsXC0pAfn08TIte36J80rQ58hqL-0?nav=eyJyZWZlcnJhbEluZm8iOnsicmVmZXJyYWxBcHAiOiJPbmVEcml2ZUZvckJ1c2luZXNzIiwicmVmZXJyYWxBcHBQbGF0Zm9ybSI6IldlYiIsInJlZmVycmFsTW9kZSI6InZpZXciLCJyZWZlcnJhbFZpZXciOiJNeUZpbGVzTGlua0NvcHkifX0&e=CNH4df

---

# AI Interaction Traceability Write-up

## Objective

This repository now includes a lightweight, hook-based mechanism to capture AI-assisted development activity with evidence and accountability. The goal is to improve traceability across the SDLC by recording how AI was used, what it did, what tools it accessed, what validation was performed, and where human oversight was applied.

The log is designed to answer questions such as:

- Which AI tool and model were used?
- What agent mode and agent type were active?
- Which prompts and follow-up requests were submitted?
- Which skills, sub-agents, or MCP integrations were used?
- What changes were made, and how were they verified?
- Did a human make corrections or approve the result?
- Was the outcome successful, partial, or in need of rework?

## Why this matters

AI tooling is valuable only when it is transparent and auditable. For engineering teams, the main risks are not just incorrect output, but the inability to explain how the output was produced, how it was validated, and whether a human reviewed it. This logger addresses that gap by capturing a durable chain of evidence.

It supports:

- quality review of AI-assisted work
- better debugging of failed or partial prompts
- accountability for human validation
- reproducibility of AI-assisted decisions and deliverables
- clearer handoff between participants and reviewers

## Design

The implementation follows the repository’s existing Spec Kit-style hook pattern instead of relying on manual documentation alone.

### Components

- `.specify/extensions.yml` registers the hook points used to capture events.
- `.specify/scripts/bash/log-ai-interaction.sh` appends structured JSONL records and refreshes a summary markdown file.
- `.github/skills/ai-interaction-logging/SKILL.md` defines the expected behavior for AI trace capture.
- `logs/ai-interactions.jsonl` stores the structured event log.
- `logs/ai-interactions-summary.md` provides a readable overview for human review.

## Log format

Each event is written as a JSON object in JSONL form. The record includes fields for:

- id and timestamp
- workflow stage
- agent and agent_name
- agent_type, ai_tool, model, mode
- related_ids and parent_task
- context and request
- skills_used, subagents_used, mcp_tools_used, tools_used
- suggestion and human_decision
- action_verification and limitation
- status, outcome, and artifacts

This format was chosen because it supports both machine processing and human review without requiring a heavyweight database or custom platform.

## Example record

```json
{
  "id": "20261101T063542Z-agent-requirement-transcription",
  "timestamp": "2026-10-19T06:35:42Z",
  "stage": "setup",
  "agent": "agent",
  "agent_name": "agent",
  "agent_type": "built-in",
  "ai_tool": "GitHub Copilot",
  "model": "MAI-Code-1.1-Flash",
  "mode": "agent",
  "related_ids": [],
  "parent_task": null,
  "context": "Assignment for AI in SDLC",
  "request": "Analyse Assignment requirement",
  "skills_used": [
    {
      "name": "ai-interaction-logging",
      "type": "custom",
      "purpose": "Capture AI traceability evidence",
      "invocation": "automatic"
    }
  ],
  "subagents_used": [],
  "mcp_tools_used": [],
  "tools_used": ["bash", "git", "workspace"],
  "suggestion": "",
  "human_decision": null,
  "action_verification": "Verified by running the script and checking the generated summary.",
  "limitation": null,
  "status": "applied",
  "artifacts": []
}
```

## Mutation score and validation evidence

In addition to the interaction log itself, this repository also captures code-quality evidence through mutation testing. The approval rules were validated with Stryker.NET, and the measured mutation score improved to 83.67%.

This matters because it shows that the AI-assisted changes were not only recorded, but were also exercised against fault-injection style checks. In practice, mutation testing strengthens the evidence that the rule logic is resilient and that the tests are meaningful rather than merely passing by coincidence.

The score can be represented as:

- Mutation score before improvement: 57.14%
- Mutation score after improvement: 83.67%

This improvement demonstrates that the validation layer was not just documenting the work, but actively increasing confidence in the correctness of the business rules and the AI-generated implementation.

## How it helps the team

This traceability layer helps ensure that AI-assisted work is not treated as opaque or unreviewed. It creates a record of:

- what work was requested
- which tool and model handled it
- which capabilities were used
- what the AI actually produced
- how the output was evaluated and by whom
- whether the work was accepted, reworked, or left partial
- what confidence level was achieved through validation such as mutation testing

That makes the AI contribution visible, auditable, and easier to assess for quality and effectiveness.

## Current repository usage

The repository is set up to write interaction evidence during AI-assisted workflows, and the summary file can be refreshed after meaningful milestones. This is particularly useful for handoff, review, and post-mortem analysis during AI-in-SDLC exercises.

## Expected outcome

The project now combines the normal engineering workflow with a durable AI evidence trail. That gives teams better insight into AI utilization quality, transparency, and human oversight without forcing teams to maintain separate manual logs for every prompt.

---

## Repository references

- AI hook config: `.specify/extensions.yml`
- Log generator: `.specify/scripts/bash/log-ai-interaction.sh`
- Skill definition: `.github/skills/ai-interaction-logging/SKILL.md`
- Structured logs: `logs/ai-interactions.jsonl`
- Summary: `logs/ai-interactions-summary.md`
