# Baseline: Customer audit persistence

## Current behavior

- The open `BFUE_Monitoring` Reactive module has no `Customer` entity and no
  customer create/update/delete actions.
- It contains four local entities: `ProcessFlowDecision`,
  `ProcessFlowDecisionStatus`, `AzureBlobConfig`, and `BlobDocument`.
- Existing audit-named actions and `Audit_InternalAPI` support existing workflow
  behavior; they do not provide the requested customer audit storage.
- The current validation snapshot contains 27 warnings and no errors.
- No module changes have been made for this request.

## Expected behavior

- `CustomerAudit` stores a 64-bit numeric customer identifier, event/action,
  changed-by user, event timestamp, and before/after changed fields as serialized
  JSON, with each JSON value limited to 2,000 characters.
- A reusable server action inserts an audit row when explicitly invoked for a
  create, update, or delete event.
- The action is private to the module, derives the actor from runtime identity,
  records its timestamp at write time, and rejects invalid event data rather
  than reporting a successful write.
- JSON inputs longer than 2,000 characters are rejected before persistence.
- Changed-fields JSON is stored as supplied; callers are responsible for
  passing valid JSON syntax.
- Existing workflow, blob, and integration behavior remains unchanged.
- No automatic customer lifecycle auditing is expected; there is no customer
  lifecycle in this module to hook into.

## Regression scenarios

1. **Audit insert:** invoke the recording action with valid create, update, and
   delete payloads and verify each produces one persisted row with the supplied
   audit values and a server-generated timestamp.
2. **Optional snapshots:** verify create events can omit before values and
   delete events can omit after values, as described in the RFC.
3. **Invalid event data:** verify an unsupported action or invalid required
   input does not create a row or return success.
4. **Existing model stability:** confirm all four existing entities, existing
   audit workflow actions, and `Audit_InternalAPI` remain unchanged.
5. **Validation delta:** compare post-change validation messages with the
   baseline; pre-existing warnings are not regressions, but new validation
   errors or warnings attributable to this change must be reviewed.

## Baseline limitation

The background runtime identity must be verified before relying on it for
accountability. The agreed caller scope is same-module only; each serialized
changed-fields JSON value is capped at 2,000 characters.
