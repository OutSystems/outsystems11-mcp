# MCP contracts

Service Studio's MCP server has a **contract number**. It goes up whenever this skill has to
describe something new: a tool added, renamed or removed, an argument or output that changed, or a
field whose meaning changed. This skill is written for one contract, named in
[`SKILL.md`](../SKILL.md) § 0. Use this file when the server reports a different one.

## Which contract the server speaks

The first line of the server's instructions names it:

```
Service Studio <version>, MCP contract 2.
```

The second line says where to get the latest skill (`https://github.com/OutSystems/outsystems11-mcp`)
and when to offer it to the user: when this skill is written for an older contract than the server's.
Install it only if the user agrees. It loads in the next new session.

- **No such line** means contract 1: every Service Studio release from before the number existed.
- **Your client doesn't show the server's instructions?** Some don't. `createSessionToken`'s
  description says the same: `This server speaks MCP contract <N>.`, then the skill pointer. On
  contract 1 it says neither.

## Contract 1

Every release from 11.55.91 until the first one that reports a contract. These builds don't all
have the same tools, so **check the live tool list** before relying on one, and where a tool's live
description differs from this skill, trust the description. They come in three tool sets:

| The server advertises | Builds | What differs from contract 2 |
| --- | --- | --- |
| `listApps` | **Early contract 1 builds**: before 11.55.98.65437 | Everything in [Early contract 1 builds](#early-contract-1-builds) |
| `listOpenModules` and `searchApps`, but no `openModule` | From 11.55.98.65437 | No way to open or close a module: ask the user to open it in Service Studio |
| `listOpenModules`, `searchApps`, `openModule` and `closeModule` | From 11.55.99.65452 | The module tools work as this skill describes, plus reference rows (see below) |

What to do with each tool set: [`module-discovery.md`](module-discovery.md) § "Which tools your
Service Studio has".

On any contract 1 build, a merge that sets `Theme.IconLibrary` can fail with *"Update Service
Studio to merge the module. The feature Theme Icon Library is not supported in this version."*
Never set it ([`SKILL.md`](../SKILL.md) § 3.1).

On any contract 1 build, `listOpenModules` (`listApps` on early builds) can also return rows
with `isReference: true`. They are other modules Service Studio has loaded, not the open module's
references: don't use them.

### Early contract 1 builds

Where this skill says "early contract 1 builds", it means these:

- `listApps` lists the open modules, with its rows under `apps` instead of `modules`.
- Pass a row's `tabId` as `eSpaceName` when rows carry one, and its `name` when they don't.
- There is no `searchApps`, `openModule` or `closeModule`: ask the user to open the module in
  Service Studio.
- `omlPublish` on a module that isn't open fails with *"Module 'X' is not open in Service Studio.
  Open it before calling omlPublish."* instead of `MODULE_NOT_LOADED`.
