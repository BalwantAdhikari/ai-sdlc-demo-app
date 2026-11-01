# UC-3: Approval limits by role

## Background

The invoice approval service in ai-sdlc-demo-app currently has no limits tied to who is approving. Higher-value invoices should get more oversight than routine ones, so what an approver is allowed to sign off should depend on their role.

## The idea

Add approval limits by role to the invoice approval service. Each role can approve up to a set amount, and very large invoices need two different approvers.

## What the business asked for

- A Manager may approve invoices up to 50,000.
- A Director may approve invoices up to 500,000.
- Anything above 500,000 needs two distinct approvers, and the second approver must be a Director.
- The same user may never count as both approvers on one invoice.
- A rejected attempt returns a clear reason, and nothing about the invoice changes.

## Out of scope for the first release

- To be confirmed with the business.

## Open questions (good prompts for clarifying questions)

- Is "up to" inclusive? For example, can a Manager approve an invoice of exactly 50,000?
- For invoices above 500,000, can the first approver be a Manager, or must it also be a Director?
- What currency do the limits apply to, and are invoices in other currencies converted?
- What happens if a user has an unsupported role, or more than one role?
- Where does an invoice sit while it waits for its second approver, and can the first approval be withdrawn?
- What should the rejection reason look like (error code, message text, who sees it)?