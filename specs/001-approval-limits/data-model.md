# Data Model: Role-Based Approval Limits

## Entities

### Invoice

Represents the invoice requiring approval and its current lifecycle state.

| Field | Type | Notes |
| --- | --- | --- |
| Id | string | Unique identifier for the invoice |
| CustomerName | string | Human-readable customer or vendor name |
| Amount | decimal | Invoice total for policy evaluation |
| Status | InvoiceStatus | Draft, PendingApproval, Approved, or Rejected |
| CreatedAt | DateTime | Creation timestamp |
| DecidedBy | string? | User who made the final decision |
| DecidedAt | DateTime? | Time when processing was completed |
| DecisionNote | string? | Optional rejection or decision rationale |

### Approval Policy

A conceptual rule set applied during approval validation.

| Field | Type | Notes |
| --- | --- | --- |
| ManagerLimit | decimal | 50,000 inclusive |
| DirectorLimit | decimal | 500,000 inclusive |
| LargeInvoiceThreshold | decimal | Greater than 500,000 |
| RequiresSecondApprover | bool | True for amounts above the director limit |
| SecondApproverRole | string | Director |

### Approval Decision

A single approval step recorded for an invoice when a user acts.

| Field | Type | Notes |
| --- | --- | --- |
| InvoiceId | string | Related invoice |
| Approver | string | Username of the person approving |
| Role | string | Manager or Director |
| Outcome | string | Approved or Rejected |
| Reason | string? | Required when rejected |
| DecisionTime | DateTime | Timestamp of decision |

## Relationships

- One invoice has zero or more approval decisions in its workflow history.
- For standard invoices, a single approval decision is sufficient if the approver’s role and amount are within the allowed threshold.
- For large invoices, the approval flow requires at least two distinct approval decisions, with the second one by a director.

## Validation rules

- Amounts equal to 50,000 or 500,000 are valid within the relevant role limit.
- Manager approval is allowed only when the total amount is less than or equal to 50,000.
- Director approval is allowed only when the total amount is less than or equal to 500,000.
- Amounts above 500,000 require a manager first and a different director second.
- Duplicate approvers are invalid on the same invoice.
- Rejected approval attempts must not alter the invoice state.

## State transitions

- Draft -> PendingApproval when an invoice is submitted.
- PendingApproval -> Approved when a valid approver decision passes the policy.
- PendingApproval -> Rejected when a decision is invalid or explicitly rejected with a reason.
- Invalid transitions remain blocked.
