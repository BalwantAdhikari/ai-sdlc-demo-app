# Tasks: Role-Based Approval Limits

## MVP execution plan

- [ ] T001 [US1] Review the approval flow and define the manager/director threshold rules and shared validation path in backend/src/InvoiceApp.Domain/ApprovalService.cs
- [ ] T002 [P] [US1] Add failing unit tests for valid manager approval, valid director approval, manager over-limit rejection, duplicate approver rejection, and large-invoice second-director requirement in backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs
- [ ] T003 [US1] Implement the approval policy in backend/src/InvoiceApp.Domain/ApprovalService.cs: manager up to 50,000 inclusive, director up to 500,000 inclusive, invoice above 500,000 requires manager first and a different director second, and the same user cannot count twice
- [ ] T004 [P] Validate API contract semantics and run the repository regression suite with `dotnet test backend/InvoiceApp.sln`, confirming invalid approvals return a clear reason and leave the invoice unchanged in backend/src/InvoiceApp.Api/Controllers/ApprovalsController.cs and backend/tests/InvoiceApp.Tests/

## Why this is the right scope

- These four tasks cover the full approval-limit story without expanding into unrelated work.
- The plan keeps the business rule in the domain layer, where the app already enforces approval lifecycle logic.
- The tests are written before or alongside the implementation to validate the acceptance criteria without guessing at edge cases.
