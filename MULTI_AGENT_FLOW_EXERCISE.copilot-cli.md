# Exercise: Multi-Agent Feature Flow — **GitHub Copilot CLI edition**

You'll add a `priority` field (`low | medium | high`) to tasks — persisted in the backend, shown as a colored badge in the UI, used as a secondary sort key. You'll drive it through a coordinated workflow of **built-in + custom subagents**, **lifecycle hooks** that enforce the boring stuff, and **per-phase commits** so the git log tells the story.

**Scripts below are written for Windows / PowerShell.**

---

## Part 1 — Create four custom subagents

| Agent | Role | Tools |
|-------|------|-------|
| `frontend-dev` | React/TS half | `read, edit, search, execute` |
| `backend-dev` | .NET half | `read, edit, search, execute` |
| `manual-tester` | Drives the live app via the chrome-devtools MCP | `read, execute, mcp-servers` (chrome-devtools server) |
| `code-reviewer` | Read-only review of the diff | `read, search, execute` |

### 1.1 Pick the folder

All four files go in **`.github/agents/`** at the repo root. Filename stem only allows `a-z A-Z 0-9 . - _`. Extension must be `.agent.md` (or plain `.md`).

```text
.github/agents/frontend-dev.agent.md
.github/agents/backend-dev.agent.md
.github/agents/manual-tester.agent.md
.github/agents/code-reviewer.agent.md
```

### 1.2 Add `frontend-dev`

`.github/agents/frontend-dev.agent.md`:

```markdown
---
name: frontend-dev
description: React + TypeScript developer. Owns frontend/src/. Reads PLAN.md, implements its frontend section, runs `npm run lint`. Stays out of backend/.
tools: read, edit, search, execute
---

You implement the frontend half of the feature in PLAN.md.
- Never touch files under backend/.
- Use existing i18n keys (frontend/src/locales/). Add new keys for all 4 languages if needed.
- Run `npm run lint` before reporting done.
- Before reporting done, stage your changes: `git add <your files>`. Do NOT commit — the orchestrator handles commits.
- Return one summary line + files changed.
```

### 1.3 Add `backend-dev`

`.github/agents/backend-dev.agent.md`:

```markdown
---
name: backend-dev
description: .NET 9 minimal-API developer. Owns backend/. Reads PLAN.md, implements its backend section, makes `dotnet build` and `dotnet test` pass.
tools: read, edit, search, execute
---

You implement the backend half of the feature in PLAN.md.
- Never touch files under frontend/.
- Update tests under backend/SimpleTaskBackend.Tests/ to cover new behavior.
- `dotnet build` and `dotnet test` must pass.
- Before reporting done, stage your changes: `git add <your files>`. Do NOT commit — the orchestrator handles commits.
- Return one summary line + files changed.
```

### 1.4 Add `manual-tester` (uses the chrome-devtools MCP)

Verify that the chrome-devtools MCP already installed and enabled.

`.github/agents/manual-tester.agent.md`:

```markdown
---
name: manual-tester
description: Manual QA tester. Drives the running app via the chrome-devtools MCP server.
tools: read, execute
mcp-servers:
  - chrome-devtools
---

Test the feature in a real browser.
1. Make sure backend (`dotnet run --project backend`) and frontend (`cd frontend; npm run dev`) are up.
2. Open http://localhost:5173.
3. One screenshot per scenario:
   - Create a task; set each priority; verify the colored badge.
   - Refresh; verify priority persists.
   - Sort by priority; verify order.
   - Mark a high-priority task done; verify the badge stays.
4. Return a markdown table: scenario | pass/fail | screenshot | notes.
```

### 1.5 Add `code-reviewer`

`.github/agents/code-reviewer.agent.md`:

```markdown
---
name: code-reviewer
description: Senior reviewer. Read-only. Bugs, security, missing tests, accessibility, i18n gaps.
tools: read, search, execute
---

Read the diff (`git diff main...HEAD`) and:
- Run `dotnet build` and `npm run lint`.
- Group findings: P0 (must fix), P1 (should fix), P2 (nit).
- One summary message. No file edits. Do not stage or commit.
```

> The CLI ships with a **built-in** `code-review` agent that does roughly the same thing (and is invoked by `/review`). We're authoring our own anyway so we control the scope and so it shows up in `/agent` for parallel `/fleet` use.

### 1.6 Restart and verify the agents loaded

---

## Part 2 — Add lifecycle hooks

### 2.1 `.github\hooks\hooks.json`

```json
{
  "version": 1,
  "hooks": {
    "postToolUse": [
      {
        "type": "command",
        "powershell": ".github/hooks/scripts/format.ps1",
        "timeoutSec": 60
      }
    ],
    "preToolUse": [
      {
        "type": "command",
        "powershell": ".github/hooks/scripts/guard-tool.ps1",
        "timeoutSec": 10
      }
    ],
    "subagentStop": [
      {
        "type": "command",
        "powershell": ".github/hooks/scripts/notify.ps1",
        "timeoutSec": 10
      }
    ]
  }
}
```

### 2.2 Three PowerShell scripts

Create `.github\hooks\scripts\` and save each file. All three append a line to `.github\hooks\hooks.log`.

**`.github\hooks\scripts\format.ps1`** — formats the changed surfaces after every edit (coarse but bulletproof; the per-tool `toolArgs` shape varies):

```powershell
$raw = [Console]::In.ReadToEnd()
$ts  = (Get-Date).ToString("HH:mm:ss")

try {
    $payload = $raw | ConvertFrom-Json
    $tool    = [string]$payload.toolName
} catch {
    $tool = "?"
}

# Only run formatters after edit-class tools.
if ($tool -notmatch '^(edit|write|multiedit|notebookedit|Edit|Write|MultiEdit|NotebookEdit)$') {
    exit 0
}

npx --yes prettier --write "frontend/**/*.{ts,tsx,js,json,md}" *> $null
Push-Location backend
dotnet format *> $null
Pop-Location

Add-Content -Path ".github\hooks\hooks.log" -Value "$ts [format]  repo formatted (trigger=$tool)"
Write-Output "formatted repo"
exit 0
```

**`.github\hooks\scripts\guard-tool.ps1`** — blocks dangerous tool calls. Uses the documented `permissionDecision: deny` response so the agent gets a reason:

```powershell
$raw = [Console]::In.ReadToEnd()
$ts  = (Get-Date).ToString("HH:mm:ss")

try {
    $payload  = $raw | ConvertFrom-Json
    $combined = "$($payload.toolName) $($payload.toolArgs)"
} catch {
    $combined = $raw
}

if ($combined -match 'rm\s+-rf|sudo\s|git\s+push\s+(--force|-f)|DROP\s+TABLE|Remove-Item.*-Recurse.*-Force') {
    Add-Content -Path ".github\hooks\hooks.log" -Value "$ts [guard]   BLOCKED: $combined"
    @{
        permissionDecision       = "deny"
        permissionDecisionReason = "Blocked dangerous operation by guard-tool.ps1: $combined"
    } | ConvertTo-Json -Compress
    exit 0
}

exit 0
```

**`.github\hooks\scripts\notify.ps1`** — Windows toast when a subagent finishes:

```powershell
$ts  = (Get-Date).ToString("HH:mm:ss")
$msg = "Copilot subagent finished"

Add-Content -Path ".github\hooks\hooks.log" -Value "$ts [notify]  subagent finished"

try {
    Add-Type -AssemblyName System.Windows.Forms
    Add-Type -AssemblyName System.Drawing
    $ni = New-Object System.Windows.Forms.NotifyIcon
    $ni.Icon = [System.Drawing.SystemIcons]::Information
    $ni.BalloonTipTitle = "Copilot CLI"
    $ni.BalloonTipText  = $msg
    $ni.Visible = $true
    $ni.ShowBalloonTip(3000)
    Start-Sleep -Milliseconds 1500
    $ni.Dispose()
} catch {
    Write-Host "[Copilot CLI] $msg"
}
exit 0
```

### 2.3 Restart and verify the hooks loaded

---

## Part 3 — Run the flow

### 3.1 Open a fresh session and paste the prompt below

> **Feature**: add a `priority` field (`low | medium | high`) to tasks — persisted in the backend, shown as a colored badge, used as a secondary sort key. All four languages need new i18n keys.
>
> Before you start, clear the hooks log so this run is isolated:
> ```
> Remove-Item .github\hooks\hooks.log -ErrorAction SilentlyContinue
> ```
>
> Run this exact flow, **one phase at a time**. Commit at each phase boundary so the git log tells the story.
>
> 1. **Explore** — use the built-in `explore` subagent to map: where tasks are defined, how the frontend talks to the backend, where i18n keys live, where the task list is rendered. (No commit.)
>
> 2. **Plan** — run `/plan` to produce a one-page implementation plan, then save the result to `PLAN.md`. Then commit:
>    ```
>    git add PLAN.md
>    git commit -m "plan: priority field implementation plan"
>    ```
>
> 3. **Implement (parallel)** — invoke `/fleet`, dispatching `@frontend-dev` and `@backend-dev` in the same prompt:
>    ```
>    /fleet implement PLAN.md.
>    @backend-dev owns the backend half. @frontend-dev owns the frontend half.
>    Each must `git add` only its own files and not commit.
>    ```
>    Watch progress with `/tasks`. When both finish, make **two separate commits** to preserve authorship:
>    ```
>    git reset                                                  # unstage everything first
>    git add backend/
>    git commit -m "feat(backend): add priority field, endpoint, tests"
>    git add frontend/
>    git commit -m "feat(frontend): add priority badge, sort, i18n"
>    ```
>
> 4. **Verify (parallel)** — another `/fleet`:
>    ```
>    /fleet verify the change.
>    @manual-tester runs the browser scenarios.
>    @code-reviewer runs `git diff main...HEAD` and reports P0/P1/P2 findings.
>    ```
>    (No commit — output is in chat.) You can also use the built-in `/review` slash command instead of `@code-reviewer` if you prefer.
>
> 5. **Iterate** — for each P0/P1 finding, dispatch the relevant dev (who runs `git add` again). Then commit each fix:
>    ```
>    git commit -m "fix(backend): <what changed>"
>    # or
>    git commit -m "fix(frontend): <what changed>"
>    ```
>    Re-run the reviewer when fixes land.
>
> 6. **Report what fired** — at the end, print the full hooks log so we can see what actually ran across every context (parent + subagents):
>    ```
>    Get-Content .github\hooks\hooks.log
>    ```
>    Summarize: how many `[format]` lines? How many `[notify]` (subagent finishes)? Any `[guard] BLOCKED`?
