# Research: Role-Based Approval Limits

## Decision

Approval limits will be enforced in the domain service, not in controllers or the UI. The domain should validate the invoice amount against the approver role, enforce the two-step approval rule for amounts above 500,000, and reject duplicate approvers before any state change is applied.

## Rationale

The current project already follows a domain-first design: `ApprovalService` owns invoice lifecycle transitions, while controllers only map HTTP requests to service calls. This creates a clean, business-rule-centered design that matches the constitution and avoids leaking policy logic into request handling.

## Alternatives considered

- Controller-level validation: rejected because it would duplicate business rules and make the API contract more fragile.
- UI-only blocking: rejected because approval rules must be enforced consistently even when the same user bypasses the interface.
- Database-backed policy table: rejected for this demo because the app intentionally uses an in-memory store and a single, small approval workflow.

## Resolved decisions

- Manager approval threshold: 50,000 inclusive.
- Director approval threshold: 500,000 inclusive.
- Large invoice rule: amounts above 500,000 require a manager as the first approver and a different director as the second approver.
- Duplicate approvers are disallowed on the same invoice.
- Rejected approval attempts must return a clear reason and leave the invoice unchanged.
- For this release, approval limits are evaluated in the base accounting currency used by the invoice workflow.

## Implementation implications

- `ApprovalService` should evaluate both amount and role before changing status.
- Rejections should raise explicit domain errors such as `InvalidOperationException` or `ArgumentException` with human-readable reasons.
- The API should continue returning 409 for invalid state transitions and 400 for invalid request values, per the existing controller base class.
- Each new rule should be backed by unit tests covering edge cases and successful approval flows.
