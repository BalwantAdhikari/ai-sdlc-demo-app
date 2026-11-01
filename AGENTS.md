# Project conventions for AI agents

Invoice approval demo app: ASP.NET Core 8 Web API with a small static web UI.

## Layout

- `backend/src/InvoiceApp.Domain`: business rules, no web or storage code. `ApprovalService` owns the invoice lifecycle (Draft, PendingApproval, Approved, Rejected).
- `backend/src/InvoiceApp.Api`: controllers, sign-in (`Auth/`), in-memory repository, and the web UI under `wwwroot/`.
- `backend/tests/InvoiceApp.Tests`: xUnit tests, one test class per class under test, named `<ClassUnderTest>Tests`.
- `e2e`: Playwright end-to-end tests (`tests/`) and page objects (`pages/`).
- `docs/use-cases`: short briefs for features we may build.

## Conventions

- Controllers live in `Controllers/`, one per resource, route `api/[controller]`. Request bodies are `record` types declared inside the controller.
- Controllers inherit `ApiControllerBase` and use `Handle(...)` so service errors map to 404, 409 and 400.
- Every endpoint is behind sign-in. Restrict actions with `[RequireRole("...")]`: destructive actions are Admin only, approvals are Approver or Admin.
- Business rules go in the Domain project, never in controllers.
- Register services in `Program.cs`. Repositories are interfaces in Domain with an in-memory implementation in Api.
- Every new rule or endpoint needs tests. Use `FakeInvoiceRepository` in tests.
- Use clear names; no one-letter variables or `flag` parameters.
- End-to-end tests use page objects and role-based locators (`getByRole`, `getByLabel`), never CSS or XPath selectors.
- AI-assisted work must preserve traceability: record the tool, model, agent type, prompt, validation evidence, relevant files, human decisions, and final outcome.
- Prefer automatic logging via the repo hook and skill workflow instead of relying on manual notes alone.

## AI interaction logging

- Structured AI interaction records are stored in `logs/ai-interactions.jsonl`.
- A human-readable summary is maintained in `logs/ai-interactions-summary.md`.
- The hook-backed script is `.specify/scripts/bash/log-ai-interaction.sh` and the hook configuration is in `.specify/extensions.yml`.
- When an AI session uses skills or sub-agents, include them in the record with purpose and status.
- Capture tool usage such as bash/git/workspace actions and MCP external integrations whenever they occur.
- Include verification evidence such as test commands, build status, changed files, and commit/session references when available.
- Record human intervention separately so the log shows where the user corrected, validated, or approved the AI output.
- If a task is partial, failed, or reworked, reflect that honestly in the `status` and `outcome` fields.

## Commands

- Run the app: `dotnet run --project backend/src/InvoiceApp.Api` then open http://localhost:5000
- Run the tests: `dotnet test backend/InvoiceApp.sln`
- Run end-to-end tests: `cd e2e && npm install && npx playwright install chromium && npm test`

## Demo accounts (not real credentials)

- `demo.approver` / `ChangeMe123!` (role Approver)
- `demo.admin` / `ChangeMe123!` (role Admin)

## Never

- Commit real credentials, tokens or customer data.
- Add business logic to `Program.cs` or controllers.
