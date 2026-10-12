# Impact analysis: Customer audit persistence

## Current state

- Target: `BFUE_Monitoring`, a Reactive module.
- Existing local entities: `ProcessFlowDecision`,
  `ProcessFlowDecisionStatus`, `AzureBlobConfig`, and `BlobDocument`.
- There is no `Customer` entity or customer CRUD action in this module.
- The module already has workflow/procedure audit-related actions and an
  `Audit_InternalAPI` REST integration. They serve existing workflows and are
  outside this change's scope.
- Current validation baseline: 27 warnings and no errors. Warnings include
  existing unused elements, login-security notices, and SQL-query cautions;
  none should be changed as part of this work.

## Architecture

- Add one independent server entity for customer audit rows and a reusable
  server action that inserts one row.
- Keep the action separate from the existing workflow audit actions and REST
  integration; this request does not describe customer CRUD flows to instrument.
- Callers will invoke the action explicitly for create, update, or delete events.
  No automatic event capture is possible within this module as it has no
  customer data lifecycle.

## Database

- Adding `CustomerAudit` changes the module's persistent schema and will require
  the normal OutSystems deployment/database synchronization when published.
- Because no customer entity is present, a customer identifier value should be
  stored without a local foreign-key relationship. The agreed format is a
  64-bit numeric value.
- The agreed before/after payloads contain only changed fields as serialized
  JSON, capped at 2,000 characters per value to match the supported text
  attribute length. Larger payloads must be rejected or this design revisited.
  Large or sensitive payloads can increase storage and expose data
  unnecessarily.
- The entity uses optimized/changed-attribute update behavior to avoid rewriting
  its large text attributes on partial updates.
- A customer/time index should be considered only if a read/query use case is
  added; no query surface was requested.

## APIs and dependencies

- The agreed callers are within this module, so the recording server action can
  remain private; no new public API or dependency refresh is needed.
- No new external API, integration, or module dependency is needed for a local
  insert.
- Use the existing entity-persistence pattern; do not route customer audit rows
  through `Audit_InternalAPI`, which is an existing unrelated integration.

## Security

- Audit data may contain personal or otherwise sensitive values. Persist only
  fields necessary for the audit purpose, restrict read access, and define
  retention outside this schema change.
- Deriving the changed-by value from runtime identity avoids trusting a
  caller-supplied actor. Confirm that interactive and background execution
  contexts map to the intended user/system identity; attribution is unreliable
  if they do not.
- Validate the event/action against create, update, and delete, and reject
  payloads over 2,000 characters. JSON syntax is not parsed; callers are
  responsible for supplying valid JSON.
- Do not expose the action publicly unless cross-module callers are required.

## Performance

- A single audit insert per explicit caller invocation is a bounded operation
  and does not introduce an N+1 read pattern.
- Snapshot size and event volume determine storage growth. Avoid unnecessary
  full snapshots, and add indexes or retention/archival behavior only when
  supported by an agreed read and retention requirement.

## Backward compatibility

- No existing entity, action, or integration needs to change, so existing
  callers should be unaffected.
- The new entity requires a database schema addition on deployment. Rollback
  would require removing the new entity/action and handling any audit rows
  already written; do not assume those rows can be discarded.
- Existing validation warnings are pre-existing and should remain distinguishable
  from any new validation messages after implementation.
- The new private recording action is not called by any current customer flow;
  expect an `UnusedUserAction` warning until a caller is added.

## Prioritized actions

1. Implement the agreed numeric identifier, 2,000-character changed-fields
   JSON values, private same-module recording action, runtime actor policy, and
   optimized entity update behavior.
2. Verify the actual runtime identity in interactive and background contexts
   before relying on the audit actor for accountability.
3. After approval, implement only the entity and recording action, then verify
   the model, persisted attributes, and validation-message delta.
