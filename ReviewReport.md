# Review report: Customer audit persistence

## Review findings

| Severity | Finding | Impact and recommendation |
|---|---|---|
| Medium | `GetUserId()` is recorded as the actor, but the background execution identity has not been exercised at runtime. | Verify timer/system calls resolve to the intended designated identity before relying on those records for accountability. |
| Medium | Changed-fields JSON is persisted as caller-supplied text and its syntax is not parsed. | Require callers to serialize valid JSON and avoid including data that should not be retained. |
| Medium | Each before/after JSON value is limited to 2,000 characters. | Callers must handle over-limit input; revisit storage if audit payloads commonly exceed this limit. |
| Low | `RecordCustomerAudit` has no current call sites. | The module reports it as unused until a customer workflow invokes it; retain it as the requested reusable persistence action. |
| Low | The entity has generated update/delete operations available inside the module. | Keep callers on the recording action and apply access controls to any future audit-reading or maintenance actions. |

## Design review

- **Security:** the action is private and does not accept a caller-supplied actor;
  `ChangedByUserId` references the System User entity. Audit read access,
  retention, and background identity still require runtime/environment policy.
- **Performance:** the action performs one insert per call, no aggregate or
  N+1 read. The text fields are capped at 2,000 characters and the entity uses
  optimized update behavior. No customer/time index was added without a
  requested query use case.
- **Maintainability:** the new customer audit capability is separate from the
  existing workflow audit integration and actions.
- **OutSystems practices:** the entity has an auto-number identifier, uses a
  foreign key for the actor, and persists via its generated create action
  instead of Advanced SQL.

## Overall

The design matches the approved scope. No additional implementation change is
recommended before merge; the runtime identity and caller behavior are
deployment-level checks.
