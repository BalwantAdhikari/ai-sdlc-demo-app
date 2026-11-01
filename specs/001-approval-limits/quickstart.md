# Quickstart: Role-Based Approval Limits

## Prerequisites

- .NET 8 SDK installed
- Local repo cloned and restored
- Demo app can run locally

## Validation steps

1. Start the app:
   - `dotnet run --project backend/src/InvoiceApp.Api`
2. Open the app in a browser and sign in as a manager or director user.
3. Create or locate an invoice and submit it for approval.
4. Validate the following scenarios:
   - Manager approves an invoice at 50,000 and succeeds.
   - Director approves an invoice at 500,000 and succeeds.
   - Manager attempts to approve an invoice above 50,000 and receives a clear rejection.
   - Director attempts to approve an invoice above 500,000 without the required second approval and is rejected.
   - Large invoice receives a manager approval followed by a director approval and succeeds.
   - Same user attempts to count as both approvers and is rejected.
   - Invalid approval attempt leaves the invoice state unchanged.

## Automated verification

Run the existing test suite:

- `dotnet test backend/InvoiceApp.sln`

Expected outcome: all approval life-cycle and policy rules remain green, with new tests covering the threshold and dual-approval behavior.
