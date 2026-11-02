# Implementation Plan: Role-Based Approval Limits

**Branch**: `001-approval-limits` | **Date**: 2026-11-01 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-approval-limits/spec.md`

## Summary

Add role-based invoice approval controls to the existing domain approval flow. The feature keeps the approval decision logic inside the domain layer, enforces inclusive role thresholds (Manager: up to 50,000; Director: up to 500,000), and requires a distinct dual-approval path for amounts above 500,000. Rejected attempts must fail cleanly without mutating invoice state.

## Technical Context

**Language/Version**: C# / .NET 8

**Primary Dependencies**: ASP.NET Core Web API, xUnit, in-memory repository

**Storage**: In-memory repository for the demo app; no persistence layer changes required for v1

**Testing**: xUnit unit tests in `backend/tests/InvoiceApp.Tests` and demo validation flows through the API and UI

**Target Platform**: Local web application and API running on .NET 8

**Project Type**: web-service

**Performance Goals**: Support demo-scale invoice approvals with no degradation in the in-memory workflow; no explicit throughput target required for this release

**Constraints**: Domain rules must remain in `InvoiceApp.Domain`; approval flows and authorization checks must stay role-aware; rejected attempts must not alter invoice state

**Scale/Scope**: Single-tenant demo app with a small in-memory data set and a limited approval workflow

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Pass: Quality by Construction — the feature stays within the existing domain/service pattern and avoids broad refactors.
- Pass: Security and Trust — the business logic continues to rely on existing role-based authorization and avoids secret or sensitive logging.
- Pass: Verified Behavior with 100% Coverage — the policy decisions and rejection paths require explicit test coverage for each threshold and validation branch.
- Pass: Data Integrity and Auditability — invalid approvals must fail without mutating invoice data and the decision trail remains auditable.
- Pass: API Contract Discipline — the controller contract remains explicit and maps service failures to 400/404/409 responses without leaking internal state.
- Pass: Controlled Change and Clarification — all ambiguities were resolved before design work began.

## Project Structure

### Documentation (this feature)

```text
specs/001-approval-limits/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # API contract documentation
├── checklist/           # Quality checklist file
└── spec.md              # Requirement specification
```

### Source Code (repository root)

```text
backend/
├── src/
│   ├── InvoiceApp.Api/
│   │   ├── Controllers/
│   │   ├── Auth/
│   │   └── wwwroot/
│   └── InvoiceApp.Domain/
│       ├── ApprovalService.cs
│       ├── Invoice.cs
│       ├── InvoiceStatus.cs
│       └── IInvoiceRepository.cs
└── tests/
    └── InvoiceApp.Tests/
        ├── ApprovalServiceTests.cs
        └── FakeInvoiceRepository.cs
```

**Structure Decision**: Keep the feature in the existing layered structure: domain validation in `InvoiceApp.Domain`, controller integration in `InvoiceApp.Api`, and tests in `backend/tests/InvoiceApp.Tests`.

## Complexity Tracking

No constitution violations were identified for this feature. The design remains within the app’s current layered architecture and does not require a new project or cross-cutting system.
