# Feature Specification: Role-Based Approval Limits

**Feature Branch**: `001-approval-limits`

**Created**: 2026-11-01

**Status**: Draft

**Input**: User description: "Add approval limits by role to the invoice approval service. Each role can approve up to a set amount, and very large invoices need two different approvers. A Manager may approve invoices up to 50,000. A Director may approve invoices up to 500,000. Anything above 500,000 needs two distinct approvers, and the second approver must be a Director. The same user may never count as both approvers on one invoice. A rejected attempt returns a clear reason, and nothing about the invoice changes."

## Clarifications

### Session 2026-11-01

- Q: Are approval thresholds inclusive? → A: Yes, values equal to 50,000 and 500,000 are valid within the applicable role limit.
- Q: For invoices above 500,000, who must be the first and second approver? → A: Manager first, then a director.
- Q: Which currency applies to the thresholds? → A: The base accounting currency for the invoice workflow.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Manager approves a standard invoice within limit (Priority: P1)

A manager needs to approve routine invoice work efficiently without exceeding their authority. The approval flow should permit valid approvals within the manager threshold while keeping larger approvals under higher review.

**Why this priority**: This is the primary business flow for day-to-day invoice review and makes the new approval controls immediately usable for most transactions.

**Independent Test**: A manager can approve an invoice below the manager threshold and the invoice moves to the next valid state without requiring additional approval.

**Acceptance Scenarios**:

1. **Given** an invoice amount is within the manager approval limit, **When** a manager submits approval, **Then** the approval is accepted and the invoice advances according to the approval workflow.
2. **Given** an invoice amount is above the manager limit, **When** a manager attempts approval, **Then** the system rejects the action and explains the limit constraint.

---

### User Story 2 - Director approves an invoice within their higher authority (Priority: P1)

A director must be able to approve higher-value invoices without unnecessary escalation when the invoice stays within their policy ceiling. The policy should support director-only approvals for medium-sized transactions while protecting against over-approval.

**Why this priority**: Directors have higher approval authority, and the feature must allow valid approvals without creating friction for the appropriate role.

**Independent Test**: A director can approve an invoice up to the director limit and the system accepts the decision when all policy rules are satisfied.

**Acceptance Scenarios**:

1. **Given** an invoice amount is within the director approval limit, **When** a director approves it, **Then** the system accepts the approval and records the decision.
2. **Given** an invoice amount exceeds the director limit but remains below the dual-approval threshold, **When** a director attempts approval, **Then** the system rejects the action because the amount requires a different approval pattern.

---

### User Story 3 - Large invoices require dual approval with a distinct director (Priority: P1)

For very large invoice values, the business needs stronger oversight and a second reviewer. The process should require two distinct approvers and prevent a single person from approving the same invoice twice.

**Why this priority**: This is the highest-risk policy rule and protects the business from excessive exposure or self-approval on large values.

**Independent Test**: An invoice above the director threshold can only be approved when two different approvers satisfy the required role combination and the system rejects duplicate approvers.

**Acceptance Scenarios**:

1. **Given** an invoice exceeds the director threshold, **When** a valid manager and a director approve it in sequence, **Then** the invoice meets the required approval policy and proceeds normally.
2. **Given** an invoice exceeds the director threshold, **When** the same user attempts to serve as both approvers, **Then** the system rejects the second approval and the invoice remains unchanged.
3. **Given** an invoice exceeds the director threshold, **When** the second approval is supplied by a non-director, **Then** the action is rejected with a clear reason and no invoice state changes occur.

---

### Edge Cases

- What happens when an invoice amount is exactly equal to a role limit? The threshold is inclusive, so amounts equal to 50,000 or 500,000 are valid within the relevant role limit.
- What happens when the first approver and second approver are the same person? The system MUST reject the duplicate approval and leave the invoice unchanged.
- What happens when an invoice is submitted for a second approval after one approval is already recorded? The system MUST enforce the correct sequence and prevent invalid state transitions.
- What happens when a user has an unsupported or multiple roles? The approval policy applies only to the user’s active role in the current approval decision, and unsupported roles are treated as out of policy until the business defines a different rule.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST enforce approval limits by user role for invoice approval decisions.
- **FR-002**: A manager MUST be able to approve invoices up to the manager limit.
- **FR-003**: A director MUST be able to approve invoices up to the director limit.
- **FR-004**: Invoices above the director limit MUST require approval from two distinct users.
- **FR-005**: The second approver for a large invoice MUST be a director.
- **FR-006**: The system MUST prevent the same user from being counted as both approvers on a single invoice.
- **FR-007**: Rejected approval attempts MUST return a clear reason that explains why the approval was denied.
- **FR-008**: Rejected approval attempts MUST leave the invoice state and data unchanged.
- **FR-009**: The system MUST distinguish between approvals that are valid within the role policy and approvals that fail the role-based limits.
- **FR-010**: The system MUST support the documented business limits for manager and director approvals: manager up to 50,000 and director up to 500,000, with both thresholds treated as inclusive values.
- **FR-011**: For invoices above 500,000, the system MUST require a manager as the first approver and a director as the second approver.
- **FR-012**: The approval limits MUST be evaluated in the base accounting currency for the invoice workflow; no conversion or multi-currency rule is required in this release.

### Key Entities *(include if feature involves data)*

- **Invoice**: The business document requiring approval; includes the total amount, current status, approval history, and whether it is awaiting additional review.
- **Approver**: A user authorized to review or approve an invoice; carries role-based approval authority and identity.
- **Approval Decision**: A single approval action linked to a specific invoice, approver, and outcome, including the reason if the action is rejected.
- **Approval Policy**: The set of thresholds and role requirements that determine whether a given invoice is eligible for approval by one or two users.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Valid invoice approvals are accepted within the applicable role-based policy without manual override in at least 95% of standard approval transactions.
- **SC-002**: Zero invalid approval attempts are accepted when a user exceeds their role limit, reuses the same approver, or fails the required dual-approval pattern.
- **SC-003**: Every rejected approval attempt includes a clear reason and leaves the invoice in the same state it had before the rejected action.
- **SC-004**: Business users can tell whether an invoice is eligible for single-approval or requires second approval based on the policy without needing technical support.
- **SC-005**: The approval process remains understandable and auditable for invoice review teams, with each approval tied to the correct approver and role rule.

## Assumptions

- The invoice amount used for approval checks is the value that the business uses for policy enforcement in the current workflow.
- The approval policy is enforced consistently across all invoice processing paths that reach the approval stage.
- A rejected approval attempt does not partially update the invoice or approval history.
- Unsupported user roles are treated as out-of-policy until the business defines a specific fallback or exception behavior.
- The dual-approval requirement applies only to invoices above the director threshold unless a business exception is explicitly approved.
