## 1. OMLs being worked on live in a temp folder

```
%TEMP%\ServiceStudio.MCPServer\<runId>-in.oml     input snapshot
%TEMP%\ServiceStudio.MCPServer\<runId>-out.oml    mutated result (successful writes only)
```

A clean `applyModelApiCode` returns the `-out.oml` path as `mutatedOmlPath`. That is
the file to open by hand, or pass to `omlMerge`, if you need to recover a change.

`omlReset` does **not** delete the oml file — a discarded chain stays on disk. A reset with
nothing in flight does nothing at all.


## 2. Starting Service Studio wipes %TEMP%\ServiceStudio.MCPServer

Recover anything you need **before** restarting Service Studio. Skipped while two or more Service
Studio instances are running, so leftovers accumulate instead.


## 3. Your mid-chain edits are invisible to the agent

The first `applyModelApiCode` takes a copy of your module and edits that; 
your real module is untouched. Everything the agent reads afterwards comes off its
copy, so anything you change in the UI meanwhile is invisible to it.

Two things put the agent back in sync, both by throwing that copy away so the next
tool call takes a fresh one off your live module:

- `omlReset` — discards the chain outright.
- `omlMerge` — opens the Compare-and-Merge window, then discards it.

`omlMerge` discards the copy when the window **opens**, not when you accept. So a
merge you reject or simply close still puts the agent back on your live module; its
chain is gone from its side either way. The file itself is left on disk at the
`mutatedOmlPath` you passed, so nothing is lost — pass that same path to `omlMerge`
again to bring it back.


## 4. Each open module is tracked separately

Chains on different modules do not interfere. A module can sit on an unmerged copy
indefinitely — merge or reset it explicitly.


## 5. Every connection needs approval

You get an "allow this agent?" prompt on every MCP handshake, including reconnects
of an agent you already approved. Per-connection, by design.


## 6. Loopback-only, but it runs arbitrary C#

Bound to `127.0.0.1` and gated by #5 — but once approved, `applyModelApiCode`
compiles and runs whatever C# the agent sends, with Service Studio's privileges.
Only approve agents you trust, pointed at modules you own.


## 7. Timeouts and port

Writes timeout at 300s, reads at 120s; the default server listens on `41820`,
moving forward if the port is busy by up to 20 retries.


## 8. The server starts on demand, not on login

Service Studio does not start the MCP server automatically — the user starts
it. A client that connects (or reconnects) before that gets `ConnectionRefused`
at the endpoint (e.g. `Failed to reconnect to servicestudio: ConnectionRefused
at http://127.0.0.1:41820`); that is not a broken install, just the server not
started yet.
