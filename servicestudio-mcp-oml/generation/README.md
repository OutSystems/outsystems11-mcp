# `docs/*.Generated.cs` provenance

This folder records **how the namespace doc files in `../docs/` are produced**, so the
corpus is reproducible rather than hand-maintained. It exists because the original
`.Generated.cs` files shipped with no generator provenance.

## Method (D1)

The docs are reflected from the **live `OutSystems.Model.V1` assembly** loaded inside the
Service Studio process, via the skill's own `applyModelApiCode` tool (any module open
— a reflection dump does not mutate `eSpace`, so it is a deliberate no-op and is never
merged). The generator:

1. Enumerates `public interface` types in the target namespace.
2. **Excludes** `*Descriptor` types (the reflection-pattern descriptors), matching the existing
   `OutSystems.Model.UI.Mobile*.Generated.cs` curation.
3. **Drops** the per-interface `Descriptor` accessor property (it points at an excluded type).
4. Marks a member `new` when a base interface declares a member with the same name
   (properties) or same name + parameter types (methods) — reproducing the corpus's
   covariant-hiding style.
5. Short-names types in the file's own namespace and in `System` / `System.Collections.Generic`
   (both `using`-imported); fully-qualifies everything else — matching corpus style.
6. Emits a one-line header comment + file-scoped `namespace` + a small curated set of
   `/// <summary>` comments (see `summaries` in the generator) for security/structure-critical
   members.

## Files here

| File | What it is |
|---|---|
| `generate-web-docs.cs` | The generator body. Paste into `applyModelApiCode` wrapped in a full `eSpace => { ... }` lambda (no `eSpace.Save`) with an OutSystems module open to regenerate the two Web docs. |
| `OutSystems.Model.UI.Web.reflection-dump.txt` | Raw pre-curation dump of `OutSystems.Model.UI.Web` (Descriptor types excluded, no `new`, fully-qualified). The source-of-truth artifact behind `docs/OutSystems.Model.UI.Web.Generated.cs`. |
| `OutSystems.Model.UI.Web.Widgets.reflection-dump.txt` | Same, for `OutSystems.Model.UI.Web.Widgets`. |

## Verification

Both generated docs were confirmed to **parse as valid C#** with a real C# parser
(reached via reflection over the host
AppDomain) — zero error-severity diagnostics.

> Note: the event interfaces referenced by the widgets (`OnClick`, `OnChange`, …) live in a
> third namespace, `OutSystems.Model.UI.Web.Events`, which is intentionally **not** shipped as a
> doc here — the corpus addition is scoped to exactly two files.
