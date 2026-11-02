#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'EOF'
Usage:
  log-ai-interaction.sh [stage] [agent] [agent_name] [request] [status]
  log-ai-interaction.sh --summary

Environment variables for richer metadata:
  AI_INTERACTION_CONTEXT
  AI_INTERACTION_AI_TOOL
  AI_INTERACTION_MODEL
  AI_INTERACTION_MODE
  AI_INTERACTION_SKILLS
  AI_INTERACTION_SUBAGENTS
  AI_INTERACTION_MCP_TOOLS
  AI_INTERACTION_TOOLS
  AI_INTERACTION_HUMAN_DECISION
  AI_INTERACTION_ACTION_VERIFICATION
  AI_INTERACTION_LIMITATION
  AI_INTERACTION_OUTCOME
  AI_INTERACTION_STATUS
  AI_INTERACTION_PARENT_TASK
  AI_INTERACTION_RELATED_IDS
EOF
}

if [[ "${1:-}" == "-h" || "${1:-}" == "--help" ]]; then
  usage
  exit 0
fi

ROOT_DIR="${AI_LOG_ROOT:-$(git rev-parse --show-toplevel 2>/dev/null || pwd)}"
LOG_DIR="$ROOT_DIR/logs"
mkdir -p "$LOG_DIR"
LOG_FILE="$LOG_DIR/ai-interactions.jsonl"
SUMMARY_FILE="$LOG_DIR/ai-interactions-summary.md"

if [[ "${1:-}" == "--summary" ]]; then
  python3 - "$LOG_FILE" "$SUMMARY_FILE" <<'PY'
import json
import sys
from collections import Counter
from datetime import datetime, timezone
from pathlib import Path

log_file = Path(sys.argv[1])
summary_file = Path(sys.argv[2])
records = []
if log_file.exists():
    for line in log_file.read_text(encoding='utf-8').splitlines():
        line = line.strip()
        if line:
            try:
                records.append(json.loads(line))
            except json.JSONDecodeError:
                continue

status_counts = Counter(record.get('status', 'unknown') for record in records)
agent_counts = Counter(record.get('agent_name', 'unknown') for record in records)
latest = records[-1] if records else {}

summary_lines = [
    '# AI Interaction Log Summary',
    '',
    f"Generated: {datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ')}",
    f"Total records: {len(records)}",
    '',
    '## Status overview',
    ''
]

if status_counts:
    for status, count in status_counts.most_common():
        summary_lines.append(f"- {status}: {count}")
else:
    summary_lines.append('- No records yet.')

summary_lines.extend([
    '',
    '## Agent overview',
    ''
])
if agent_counts:
    for agent, count in agent_counts.most_common():
        summary_lines.append(f"- {agent}: {count}")
else:
    summary_lines.append('- No agents recorded.')

summary_lines.extend([
    '',
    '## Latest entry',
    ''
])
if latest:
    summary_lines.extend([
        f"- Timestamp: {latest.get('timestamp', 'n/a')}",
        f"- Stage: {latest.get('stage', 'n/a')}",
        f"- Agent: {latest.get('agent_name', 'n/a')}",
        f"- Request: {latest.get('request', 'n/a')}",
        f"- Status: {latest.get('status', 'n/a')}",
        f"- Outcome: {latest.get('outcome', 'n/a')}",
    ])
else:
    summary_lines.append('- No interaction recorded yet.')

summary_lines.extend([
    '',
    '## Recent entries',
    ''
])
if records:
    for record in records[-5:]:
        summary_lines.append(
            f"- {record.get('timestamp', 'n/a')} | {record.get('stage', 'n/a')} | {record.get('agent_name', 'n/a')} | {record.get('status', 'n/a')} | {record.get('request', 'n/a')}"
        )
else:
    summary_lines.append('- No entries recorded.')

summary_file.write_text('\n'.join(summary_lines) + '\n', encoding='utf-8')
print(f"Summary refreshed at {summary_file}")
PY
  exit 0
fi

STAGE="${1:-${AI_INTERACTION_STAGE:-setup}}"
AGENT="${2:-${AI_INTERACTION_AGENT:-agent}}"
AGENT_NAME="${3:-${AI_INTERACTION_AGENT_NAME:-unknown-agent}}"
REQUEST="${4:-${AI_INTERACTION_REQUEST:-AI interaction}}"
STATUS="${5:-${AI_INTERACTION_STATUS:-applied}}"

python3 - "$LOG_FILE" "$STAGE" "$AGENT" "$AGENT_NAME" "$REQUEST" "$STATUS" "$ROOT_DIR" <<'PY'
import json
import os
import sys
from datetime import datetime, timezone
from pathlib import Path

log_file = Path(sys.argv[1])
stage = sys.argv[2]
agent = sys.argv[3]
agent_name = sys.argv[4]
request = sys.argv[5]
status = sys.argv[6]
root_dir = Path(sys.argv[7])

record = {
    "id": f"{datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%SZ')}-{agent_name.lower().replace(' ', '-')}",
    "timestamp": datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ'),
    "stage": stage,
    "agent": agent,
    "agent_name": agent_name,
    "agent_type": os.environ.get("AI_INTERACTION_AGENT_TYPE", "built-in"),
    "ai_tool": os.environ.get("AI_INTERACTION_AI_TOOL", "GitHub Copilot"),
    "model": os.environ.get("AI_INTERACTION_MODEL", "MAI-Code-1.1-Flash"),
    "mode": os.environ.get("AI_INTERACTION_MODE", "agent"),
    "related_ids": json.loads(os.environ.get("AI_INTERACTION_RELATED_IDS", "[]")),
    "parent_task": os.environ.get("AI_INTERACTION_PARENT_TASK") or None,
    "context": os.environ.get("AI_INTERACTION_CONTEXT", "Assignment for AI in SDLC"),
    "request": request,
    "skills_used": json.loads(os.environ.get("AI_INTERACTION_SKILLS", "[]")),
    "subagents_used": json.loads(os.environ.get("AI_INTERACTION_SUBAGENTS", "[]")),
    "mcp_tools_used": json.loads(os.environ.get("AI_INTERACTION_MCP_TOOLS", "[]")),
    "tools_used": json.loads(os.environ.get("AI_INTERACTION_TOOLS", "[]")),
    "suggestion": os.environ.get("AI_INTERACTION_SUGGESTION", ""),
    "human_decision": os.environ.get("AI_INTERACTION_HUMAN_DECISION") or None,
    "action_verification": os.environ.get("AI_INTERACTION_ACTION_VERIFICATION", "Verified by the human reviewer or automated test command."),
    "limitation": os.environ.get("AI_INTERACTION_LIMITATION") or None,
    "status": status,
    "outcome": os.environ.get("AI_INTERACTION_OUTCOME", status),
    "artifacts": json.loads(os.environ.get("AI_INTERACTION_ARTIFACTS", "[]")),
    "repository": str(root_dir),
}

log_file.parent.mkdir(parents=True, exist_ok=True)
with log_file.open('a', encoding='utf-8') as fh:
    fh.write(json.dumps(record, ensure_ascii=False) + '\n')

print(f"Logged AI interaction to {log_file}")
PY
