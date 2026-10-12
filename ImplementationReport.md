# Implementation report: Customer audit persistence

## Changes made

In the in-flight `BFUE_Monitoring` OML:

- Added the private server entity `CustomerAudit` with:
  - Auto-numbered `Long Integer` identifier.
  - Mandatory `CustomerId` (`Long Integer`).
  - Mandatory `EventType` (`Text`, length 10; accepted values are `Create`,
    `Update`, and `Delete`).
  - Mandatory `ChangedByUserId` foreign key to `(System).User`.
  - Mandatory `ChangedOn` (`Date Time`).
  - Optional `BeforeChangedFieldsJson` and `AfterChangedFieldsJson` (`Text`,
    length 2,000 each).
- Set the entity update behavior to `Optimized` (changed attributes) for its
  text fields.
- Added the private server action `RecordCustomerAudit` with mandatory
  `CustomerId` and `EventType` inputs and optional before/after JSON inputs.
- The action rejects unsupported event values and JSON inputs over 2,000
  characters using `InvalidCustomerAuditEvent`; it assigns the actor from
  `GetUserId()`, sets `ChangedOn` from `CurrDateTime()`, and persists through
  the generated `CreateCustomerAudit` entity action.
- No existing workflow audit actions or external integrations were changed.

## Implementation notes

- Service Studio rejects a null text-attribute `Length`; the approved
  2,000-character maximum was applied and is enforced before insertion.
- JSON syntax is not parsed. Callers must supply valid serialized JSON.
- The action has no current customer-flow callers, as the module has no
  customer CRUD operations.
- The action flow is positioned vertically with the invalid-input branch to the
  right; the serialized read-back confirms the Start/If/Assign/Create/End links.
