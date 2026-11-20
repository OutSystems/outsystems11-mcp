# Finding, opening and closing modules

Every tool that reads or changes a module names it in `input.eSpaceName`. This file
covers how to choose that value, and the four tools that find, open and close modules:
`listOpenModules`, `searchApps`, `openModule` and `closeModule`.

> **Source of truth:** `Tools/ListOpenModulesTool.cs`, `Tools/SearchAppsTool.cs`,
> `Tools/OpenModuleTool.cs`, `Tools/CloseModuleTool.cs`, `Tools/ModuleResolver.cs`,
> `Apps/EnvironmentModules.cs`, `Apps/ModuleOpener.cs` and `Apps/ModuleCloser.cs` in the
> Service Studio MCP server. `openModule` and `closeModule` were verified live on 2026-11-18:
> an opening held by Studio's dialogs, a second call joining the same opening, an
> `environment` Studio isn't logged in to, and closes where Studio asked the developer to save.

## Which tools your Service Studio has

Use the tool names the server advertises in its tool list.

| The server advertises | What it means |
|---|---|
| `listOpenModules`, `searchApps`, `openModule`, `closeModule` | Everything below applies. A `listApps` call fails as an unknown tool. |
| `listOpenModules` and `searchApps`, but no `openModule` | A later contract 1 build ([`contracts.md`](contracts.md)). Choose and search for modules as below, but there is no way to open or close one: ask the user to open it in Service Studio. |
| `listApps` | An early contract 1 build ([`contracts.md`](contracts.md)). `listApps` is the same open-module list, with rows under `apps` instead of `modules`. When its rows carry `tabId`, address modules as below. When they don't, address them by `name`. There is no way to search or open a module: ask the user to open it in Service Studio. |

The server's own instructions say the same: where guidance says `listApps` for the
open modules, call `listOpenModules`.

## Choosing `eSpaceName`

`eSpaceName` takes the `tabId`, `key` or `name` of a `listOpenModules` row with
`isOpen: true`. The server tries them in that order:

1. **`tabId`**: one tab, compared ignoring case. **Prefer it.** It is the only value that
   can't be ambiguous.
2. **`key`**: the module's own key, a GUID. It stays the same when the module is closed and
   reopened, so use it to recognise a module across calls.
3. **`name`**: compared exactly, case included.

When a key or name matches modules open in several tabs, the call fails with
**`MODULE_AMBIGUOUS`**: `Several open modules match '<value>'; pass the tabId of the one to
work on as eSpaceName. Candidates: Orders (tabId '…'), Orders (tabId '…').` Retry with one of
those tab ids. If you can't tell which one the user means, ask them.

When nothing matches, the call fails with **`MODULE_NOT_LOADED`**: `No open module matches
'<value>'. Call listOpenModules and pass the tabId, key or name of a row with isOpen: true.
If the module isn't open, find it with searchApps and open it with openModule.`

Reference rows (`isReference: true`) are never accepted as `eSpaceName`.

A `tabId` names the module **in that tab**. Once the tab holds another module, or is closed,
it stops matching. When the user may have closed or reopened modules since you last listed
them, call `listOpenModules` again before you reuse a `tabId`.

## `listOpenModules`

`{ "sessionToken": "<token>" }`. It takes no `input`.

```jsonc
{
  "modules": [
    {
      "key": "9803f00d-…",        // the module's own key, stable across reopens
      "tabId": "…",               // open rows only: the Studio tab it is open in
      "isSelected": true,         // open rows only: the tab in front of its Studio window
      "name": "FloorPlan",
      "kind": "eSpace",
      "version": "…",
      "description": "…",
      "isOpen": true,
      "isReference": false,
      "moduleType": "Traditional" // see module-style-support.md
    },
    { "key": "…", "name": "OutSystemsUI", "kind": "eSpace", "version": "…", "isOpen": false, "isReference": true, "moduleType": "ReactiveLibrary" }
  ]
}
```

- **One row per open tab.** A module open in two tabs is listed twice, with different
  `tabId`s and the same `key`.
- **`isSelected`** marks the module the developer is looking at in each Studio window. Use it
  to resolve "this module" or "the module I have open". With several Studio windows,
  several rows can have it. Then ask the user which one.
- **Reference rows** list the loaded references of the open modules, without `tabId` or
  `isSelected`. Identical rows are listed once, and a module that is also open is listed only as open.
- **The list doesn't say which environment a module came from.** Modules open from two
  environments appear in one list. `searchApps` marks only the tabs connected to the
  environment it lists (below).

## `searchApps`: modules that aren't open

Searches the modules of the environment Service Studio is logged in to, as Open from
Environment lists them, grouped by application. Every input field is optional:

```jsonc
{ "input": { "search": "orders", "application": "Order Management", "moduleType": "Reactive" }, "sessionToken": "<token>" }
```

| Field | Meaning |
|---|---|
| `search` | Words, each of which must appear in the module's name, its application's name or the name of who last changed it. A trailing `.oml` is ignored. Words of 4 or more letters and digits also match a name written without its spaces, so `OrderManagement` finds `Order Management`. |
| `application` | Only that application's modules: its key, or its whole name in any case, with or without spaces, punctuation and accents. |
| `moduleType` | Only that exact type, one of the 8 `moduleType` values. `Traditional` leaves out `TraditionalLibrary`. |
| `environment` | The host name of the environment to search, as the result's `environment` and the `ENVIRONMENT_REQUIRED` message give it, in any case. Needed only when Service Studio's windows are logged in to different environments. **Ask the user which one**; don't choose for them. |

```jsonc
{
  "environment": "dev.example.com",
  "apps": [
    {
      "key": "…", "name": "Order Management",
      "modules": [
        { "key": "…", "name": "OrderManagement", "moduleType": "Reactive",
          "lastChangedBy": "…", "lastChangedAt": "2026-11-10T09:12:00+00:00",
          "isOpen": true, "tabIds": ["…"] }
      ]
    }
  ],
  "hasMore": false,
  "message": ""
}
```

- **It returns at most 50 modules, as whole applications.** The first application is
  listed whole even when it alone has more than 50. When `hasMore` is true, `message` names
  the applications it left out. **Don't page.** Narrow with `search`, or pass one of those
  names as `application`.
- **A miss suggests close names** in `message`, for the argument that missed. You can pass a
  suggestion back as it is.
- **`isOpen` and `tabIds`** mark modules open in a Studio window connected to *this*
  environment. Pass a `tabId` as `eSpaceName` without opening anything.
- It doesn't list system or protected modules, extensions, or other environments' modules,
  just as Open from Environment doesn't.
- Errors:
  - **`ENVIRONMENT_REQUIRED`** when Studio's windows are logged in to several environments
    and you passed no `environment`. The message names them. Ask the user which one, then
    call again with it as `environment`.
  - **`NOT_LOGGED_IN`** when no Studio window is logged in to an environment (ask the
    developer to log in), or when `environment` names one that no window is logged in to
    (the message names the ones that are).
  - **`SERVER_REQUEST_FAILED`** when the server didn't return its modules.

## `openModule`

```jsonc
{ "input": { "moduleKey": "<key from searchApps>", "environment": "dev.example.com", "waitSeconds": 45 }, "sessionToken": "<token>" }
// environment and waitSeconds are optional
// → { "status": "opened", "message": "…", "module": { /* a listOpenModules row */ }, "inNewWindow": true }
```

**Open a module only when the user's request names one that isn't open.** Find its key with
`searchApps`, searching for the name. When several modules match, or none, ask the user
which one and open nothing. `moduleKey` must be a module key from `searchApps`, not a name.

**Pass `environment` when you passed one to `searchApps`**, with the same value. Studio's
windows are then logged in to several environments, and without it `openModule` fails with
`ENVIRONMENT_REQUIRED`, as `searchApps` does. With one environment logged in, leave it out.

Studio opens the module in a new tab, as its start page does, **without** moving the
developer off the tab and window they are on. The developer sees a notification naming your
`clientName` and the module, with a *Show* button.

| `status` | Meaning | Next step |
|---|---|---|
| `opened` | Studio opened it. `module` is the new tab's row. | Pass `module.tabId` as `eSpaceName`. **Tell the user you opened it.** |
| `alreadyOpen` | A tab connected to that environment already has it, or another agent asked first. `module` is that tab's row. | Pass `module.tabId`. You didn't open it, so you may not close it. |
| `opening` | Studio is still downloading or loading it, or is waiting for the developer in one of its dialogs. This can take a minute or more. Studio shows a dialog of a tab that isn't in front only once the developer clicks that tab, and `message` says when it holds one. | Tell the user, and when `message` says a dialog is waiting, ask them to click the module's tab and answer it. Then call `openModule` again with the **same `moduleKey` and `environment`**. It waits for the same opening instead of starting another. |
| `notOpened` | Studio didn't open it, and `message` says why: the download failed, the developer canceled a dialog, or the tab closed again. | Tell the user. Don't retry unless they ask. |

- `waitSeconds` is how long one call waits before it returns `opening`: at most 50, 45 by default.
- A Studio window holds 12 tabs. Past that, Studio opens the tab in a new window and
  `inNewWindow` is true. The field is absent otherwise.
- Errors: `INVALID_ARGUMENT` for a `moduleKey` that isn't a key `searchApps` lists,
  `ENVIRONMENT_REQUIRED`, `NOT_LOGGED_IN` and `SERVER_REQUEST_FAILED` as for `searchApps`, and `INTERNAL` when the
  opening failed unexpectedly (tell the user; Service Studio's log has the details).
- An opening that ended without any call returning it is forgotten after two minutes. A
  later `openModule` of that module starts a new opening.

## `closeModule`

```jsonc
{ "input": { "eSpaceName": "<tabId>", "closeEmptyWindow": true, "waitSeconds": 45 }, "sessionToken": "<token>" }
// → { "status": "closed", "message": "…", "changes": "saved", "emptyWindow": "closed" }
```

**Close the modules you opened** once you are done with them, and before opening more in a
long task, because a window holds 12 tabs. **Never close a module the user opened.**
`closeModule` refuses any tab that this session token didn't open with `openModule`,
including one another agent opened.

**Tell the user before you call it when the module has unsaved changes**, such as changes you
merged with `omlMerge`. Studio then brings the module's tab forward and asks the developer
to *Save*, *Don't Save* or *Keep Open*.

| `status` | Meaning | Next step |
|---|---|---|
| `closed` | Studio closed the tab. `changes` is `saved` or `discarded` when the developer was asked: `saved` means saved to a local file, not published; `discarded` means the unsaved changes are gone, including the ones you merged. `emptyWindow` is `closed` or `leftOpen` when the tab was its window's last. | Report it. |
| `closing` | The developer hasn't answered yet. | Call `closeModule` again with the **same `eSpaceName`**. It finds the same close even after the tab has gone. |
| `notClosed` | Studio didn't close it. `message` says why and what to do. | Follow the message. If the developer chose *Keep Open*, don't call it again unless the user asks. |

`closeModule` returns `notClosed` without asking anyone, in this order, when:

1. Studio holds a dialog in that tab, which it shows only once the tab is in front. Ask the
   user to click the module's tab and answer the dialog, then call again.
2. Studio is otherwise busy in that tab (a dialog may be open, or an operation running).
   Try again later.
3. An OML edit session is open on the module. The message says to call `endOmlEditSession`,
   but that discards unmerged edits for good. If you started the session, merge what you want
   to keep (`merged: true`), then end it. If you didn't, ask the user first (SKILL.md § 2.1).
4. The module has changes from `applyModelApiCode` that aren't merged. Merge them with
   `omlMerge`, or discard them with `omlReset`.
5. A server operation, such as a publish, is running. Wait for it to end.
6. The tab is the last one in a window the user opened. Ask the user whether to close that
   window too, then call again with **`closeEmptyWindow`** set to their answer. **Never pass
   `closeEmptyWindow` without asking.** A window Studio opened for one of your openings closes
   when it empties, unless you pass `closeEmptyWindow: false`. Studio's only window is never
   closed, because that would quit Studio: its last tab closes, and the window stays open
   with `emptyWindow: "leftOpen"`.

A tab that has already closed fails with `MODULE_NOT_LOADED`. The developer sees a
notification that your `clientName` closed the module. A close that ended without any call
returning it is forgotten after two minutes.

## A typical sequence

The user asks to "add a `Discount` attribute to `Order` in the OrderManagement module", and
it isn't open:

1. `listOpenModules`: no open row is `OrderManagement`.
2. `searchApps` with `search: "OrderManagement"`: one match. Note its `key`. If it had
   `isOpen: true` with a single `tabIds` entry, you would use that tab and skip step 3.
   With several entries, the module is open in more than one tab: pick the one the user
   means, as for `MODULE_AMBIGUOUS` above, and ask them if you can't tell. If it fails with
   `ENVIRONMENT_REQUIRED`, ask the user which environment, then call it again with that
   `environment`.
3. `openModule` with that `moduleKey`, plus the `environment` from step 2 if you passed one.
   On `opening`, tell the user and call again with the same arguments. On `opened`, tell the
   user you opened it, then keep `module.tabId`.
4. Read with that `tabId` as `eSpaceName`. Then call `startOmlEditSession`, apply and merge
   with the same `tabId`, and call `endOmlEditSession` once the merge returns `merged: true`
   (SKILL.md §§ 2–3.1). Without a session, `applyModelApiCode` is refused.
5. If you opened the module in step 3, close it. A module that was already open isn't yours
   to close, and `closeModule` refuses it. Tell the user that closing the module will make
   Studio ask them to save the merged changes. Then call `closeModule` with the `tabId`. On `closing`, call again. While the edit session
   is open, `closeModule` returns `notClosed`, which is why step 4 ends it.
