# Invoice Approval Contract

## Overview

This feature extends the current approval flow for submitted invoices. The service validates approval rules before changing invoice status and reports validation failures using the existing API error mapping.

## Endpoints

### POST /api/invoices/{id}/submit

Submits a draft invoice for approval.

- Success: HTTP 200 with the updated invoice
- Failure: HTTP 409 when the invoice is not in Draft status

### POST /api/approvals/{invoiceId}/approve

Approves a pending invoice using the current signed-in user.

- Success: HTTP 200 with the approved invoice
- Failure: HTTP 404 when no invoice is found
- Failure: HTTP 409 when the invoice is not PendingApproval or does not meet the selected role policy
- Failure: HTTP 400 when the request is invalid for the business rule

### POST /api/approvals/{invoiceId}/reject

Rejects a pending invoice with a reason.

Request body:

```json
{
  "reason": "Duplicate invoice"
}
```

- Success: HTTP 200 with the rejected invoice
- Failure: HTTP 400 when the reason is missing or blank
- Failure: HTTP 404 when no invoice is found
- Failure: HTTP 409 when the invoice is not PendingApproval

## Business rule contract

- Manager may approve values up to 50,000 inclusive.
- Director may approve values up to 500,000 inclusive.
- Amounts above 500,000 require a manager approval and a different director approval.
- The same user cannot serve as both approvers for a single invoice.
- A rejected approval attempt returns a clear reason and leaves invoice state unchanged.

## Error response shape

```json
{
  "error": "<human-readable validation or business rule message>"
}
```
