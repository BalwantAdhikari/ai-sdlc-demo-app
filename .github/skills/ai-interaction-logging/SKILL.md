---
name: "ai-interaction-logging"
description: "Record structured AI interaction evidence for traceability, human oversight, and verification in this repository."
compatibility: "Works with any GitHub Copilot or agent session; hook-compatible with Spec Kit workflows."
metadata:
  author: "project-maintainers"
  category: "quality"
---

## Purpose

Use this skill to create a persistent, machine-readable record of AI-assisted work in this repository. The log must support traceability across the SDLC, including prompts, skills used, tools invoked, verification evidence, and human decisions.

## Required behavior

When this skill is invoked, or when a task is started in this repository, do all of the following:

1. Capture the interaction as a JSONL record in `logs/ai-interactions.jsonl`.
2. Include the fields defined in the project log format: timestamp, stage, agent, agent_name, agent_type, ai_tool, model, mode, related_ids, parent_task, context, request, skills_used, subagents_used, mcp_tools_used, tools_used, suggestion, human_decision, action_verification, limitation, status, outcome, and artifacts.
3. Record both the current request and the validation evidence that followed, such as test commands, build results, changed files, and commit references.
4. Prefer structured evidence over generic descriptions. A useful record must describe what was actually done and how it was validated.
5. If human input corrected or approved the AI output, record that under `human_decision` and include the validation result under `action_verification`.
6. If a sub-agent or skill is used, include it in `subagents_used` or `skills_used`, with invocation status and purpose.
7. Update the human-readable summary in `logs/ai-interactions-summary.md` after each significant task or completion milestone.

## Hook integration

This repository supports a Spec Kit-style hook configuration via `.specify/extensions.yml`. The hook should invoke `.specify/scripts/bash/log-ai-interaction.sh` before and after AI-assisted workflows, and refresh the summary after implementation steps finish.

## Output expectations

- Append one record per interaction to `logs/ai-interactions.jsonl`.
- Keep the log file compact and JSONL-formatted for traceability and automation.
- Ensure that the summary markdown remains readable for reviewers and stakeholders.
- Record incomplete or reworked interactions honestly with `status` values such as `partial`, `rework`, or `failed`.

## Example invocation

```bash
bash .specify/scripts/bash/log-ai-interaction.sh setup agent "AI requirement analysis" applied
bash .specify/scripts/bash/log-ai-interaction.sh --summary
```
