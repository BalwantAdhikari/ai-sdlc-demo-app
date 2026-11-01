# Tasks: Role-Based Approval Limits

## MVP execution plan

- [X] T001 [US1] Review the current approval service and confirm where the single-approver limit rules belong in backend/src/InvoiceApp.Domain/ApprovalService.cs
- [X] T002 [P] [US1] Add failing unit tests for manager approval at 50,000 inclusive and director approval at 500,000 inclusive in backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs
- [X] T003 [US1] Implement the single-approver limit rule in backend/src/InvoiceApp.Domain/ApprovalService.cs so manager approvals are allowed up to 50,000 inclusive and director approvals are allowed up to 500,000 inclusive
- [X] T004 [P] [US2] Add failing unit tests for duplicate approver rejection, invalid second-approver rejection, and large-invoice dual-approval success in backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs
- [X] T005 [US2] Implement the two-approver rule in backend/src/InvoiceApp.Domain/ApprovalService.cs requiring a manager first, a different director second, and rejecting any duplicate or out-of-policy approver combination
- [ ] T006 [P] Validate the API contract and run the full regression suite with dotnet test backend/InvoiceApp.sln in backend/src/InvoiceApp.Api/Controllers/ApprovalsController.cs and backend/tests/InvoiceApp.Tests/

## Why the split is intentional

- The single-approver limits are the first business gate and can be validated independently before large-invoice enforcement is added.
- The two-approver rule is a separate policy layer with its own failing tests, clearer guardrails, and easier rollback if the large-invoice path needs adjustment.
- This keeps the implementation incremental, reviewable, and aligned with the requirement to land the simpler approval limits first and then the stricter dual-approval rule.
