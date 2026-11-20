# MCP host design choices & caveats

## 1. OMLs being worked on live in a temp folder

```
%TEMP%\ServiceStudio.MCPServer\<runId>-in.oml     input snapshot
%TEMP%\ServiceStudio.MCPServer\<runId>-out.oml    mutated result (successful writes only)
```

A clean `applyModelApiCode` returns the `-out.oml` path as `mutatedOmlPath`, and that
file becomes the edit session's working copy. `omlMerge` accepts only that tracked
working copy. An older `-out.oml`, or one left behind by `omlReset`, is refused with
`INVALID_ARGUMENT`, so the MCP tools can't bring discarded work back. You can still
open a leftover file by hand in Service Studio.

`endOmlEditSession` deletes the working copy. `omlReset` leaves it on disk. Both throw
away the edits that weren't merged. A reset with nothing unmerged does nothing at all.


## 2. Starting Service Studio wipes %TEMP%\ServiceStudio.MCPServer

Copy out anything you want to keep **before** restarting Service Studio. Skipped while
two or more Service Studio instances are running, so leftovers accumulate instead.


## 3. Your edits during an edit session are invisible to the agent

`startOmlEditSession` takes a copy of your module, and the agent's `applyModelApiCode`
calls edit that copy; your real module is untouched. Everything the agent reads while
the session is open comes off its copy, so anything you change in the UI meanwhile is
invisible to it.

A merge doesn't change that. After `omlMerge` the agent keeps working on the same
copy, so it still doesn't see your own edits. If you reject the merge, the copy still
holds the agent's edits, and it can merge again. If you leave some of its changes out of
a merge, the copy keeps those too, and its next merge proposes them again. Tell the
agent, so it starts a new session from what you accepted.

The agent is back on your live module only when the session ends:

- `endOmlEditSession` — ends the session and deletes the copy, along with any edits
  that weren't merged.
- `omlReset` — with unmerged edits, discards them and ends the session. With none, it
  does nothing, and the session stays open.


## 4. Each open module is tracked separately

Edit sessions on different modules do not interfere. A session stays open on its
module until `endOmlEditSession` (or an `omlReset` with unmerged edits) ends it. It
lives in Service Studio, so it outlasts the conversation that started it, and every
agent connected to Studio shares it.


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
