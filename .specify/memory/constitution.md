# Invoice Approval Demo App Constitution

<!--
Sync Impact Report
- Version change: placeholder scaffold -> 1.0.0
- Modified principles: new baseline constitution
- Added sections: Quality by Construction; Security and Trust; Verified Behavior with 100% Coverage; Data Integrity and Auditability; API Contract Discipline; Controlled Change and Clarification; Additional Constraints; Development Workflow
- Removed sections: template placeholder sections
- Deferred items: ratification date was not provided; confirm the original adoption date if the project has an official governance record.
-->

## Core Principles

### I. Quality by Construction
Code quality MUST be treated as a first-class requirement. Every change MUST be readable, maintainable, and consistent with the existing design; complexity MUST be justified by a clear requirement or risk. Small, reviewable changes are preferred over broad refactors, and each change MUST solve one problem without expanding scope unnecessarily. This keeps the application easier to reason about, reduces defect risk, and makes review decisions clearer.

### II. Security and Trust
Security is NON-NEGOTIABLE. Secrets, credentials, tokens, and customer data MUST never be committed to the repository or logged in plaintext. Access MUST be restricted to the smallest required scope, and every protected action MUST enforce the appropriate role and authentication checks. This reduces the risk of unauthorized access and protects the integrity of the demo environment.

### III. Verified Behavior with 100% Coverage
Every behavior that can be changed by a code path MUST have automated tests. New or modified logic MUST be covered by tests before merge, and the project MUST maintain 100% statement and branch coverage in the domain and service layers unless an exception is explicitly approved with written rationale. When a requirement is ambiguous, the team MUST ask for clarification instead of guessing. This prevents regressions and makes correctness measurable.

### IV. Data Integrity and Auditability
All state transitions and data changes MUST preserve integrity and remain traceable. Invoices MUST move through valid lifecycle states, invalid transitions MUST be rejected, and important state changes MUST be recorded through logs or audit trails that capture who changed what and why. The project MUST avoid silent data loss and MUST surface validation errors clearly. This prevents inconsistent states and supports debugging and review.

### V. API Contract Discipline
API behavior MUST be explicit, consistent, and backward-aware. Request validation, status codes, and error semantics MUST match the contract defined by the requirement, and response payloads MUST not expose internal-only data or inconsistent states. Breaking changes require explicit approval and clear migration guidance. This keeps integrations stable and reduces brittle client behavior.

### VI. Controlled Change and Clarification
Ambiguous requirements MUST be raised as clarifying questions before implementation. Changes MUST remain small, reviewable, and limited to the current scope; the simplest design that satisfies the specification MUST be preferred over clever or speculative alternatives. If a decision depends on trade-offs, the team MUST document the rationale before proceeding. This keeps delivery predictable and prevents hidden assumptions from becoming defects.

## Additional Constraints
The project MUST keep the application simple enough to run locally without infrastructure unrelated to the business scenario. Business logic MUST live in the domain layer; controllers and infrastructure code MUST orchestrate, not define rules. Logging MUST be concise, structured, and safe; it MUST not record secrets, credentials, or sensitive customer content. The project MUST prefer deterministic logic and explicit validation over hidden side effects.

## Development Workflow
All work MUST start from a clear requirement or acceptance criterion. Ambiguities MUST be resolved through questions before code is written. Changes MUST be small enough to review in a single session and MUST include the relevant tests with the code. Every review and merge MUST confirm that the change aligns with this constitution, especially for security, validation, coverage, and API contract expectations. This creates a repeatable workflow that reduces churn and strengthens reliability.

## Governance
This constitution supersedes informal practices whenever it conflicts with them. Any amendment MUST document the reason, describe the impact on the project, and identify the affected principles or workflow expectations. Proposed changes MUST be reviewed for clarity, testability, and compatibility before adoption. Compliance is expected at code review time and during release validation.

**Version**: 1.0.0 | **Ratified**: 2026-11-01 | **Last Amended**: 2026-11-01
