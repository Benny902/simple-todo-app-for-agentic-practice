# Exercise: Multi-Agent Feature Flow — **Claude Code edition**

You'll add a `priority` field (`low | medium | high`) to tasks — persisted in the backend, shown as a colored badge in the UI, used as a secondary sort key. You'll drive it through a coordinated workflow of built-in + custom sub-agents, with lifecycle hooks enforcing the boring stuff, and per-phase commits so the git log tells the story.

**Scripts below are written for Windows / PowerShell.**

---

## Part 1 — Create four custom sub-agents

| Agent | Role | Model | Tools |
|-------|------|-------|-------|
| `frontend-dev` | React/TS half | sonnet | Read, Edit, Write, Glob, Grep, Bash |
| `backend-dev` | .NET half | sonnet | Read, Edit, Write, Glob, Grep, Bash |
| `manual-tester` | Drives the live app via chrome-devtools MCP | haiku | Read, Bash, `mcp__chrome-devtools__*` |
| `code-reviewer` | Read-only review of the diff | opus | Read, Grep, Glob, Bash |

Different models on purpose — feel the cost/quality trade-off.

### 1.1 Pick the folder

- **Claude Code:** `.claude/agents/<name>.md`

### 1.2 Add `frontend-dev`

```markdown
---
name: frontend-dev
description: React + TypeScript developer. Owns frontend/src/. Reads PLAN.md, implements its frontend section, runs `npm run lint`. Stays out of backend/.
tools: Read, Edit, Write, Glob, Grep, Bash
model: sonnet
---

You implement the frontend half of the feature in PLAN.md.
- Never touch files under backend/.
- Use existing i18n keys (frontend/src/locales/). Add new keys for all 4 languages if needed.
- Run `npm run lint` before reporting done.
- Before reporting done, stage your changes: `git add <your files>`. Do NOT commit — the orchestrator handles commits.
- Return one summary line + files changed.
```

### 1.3 Add `backend-dev`

```markdown
---
name: backend-dev
description: .NET 9 minimal-API developer. Owns backend/. Reads PLAN.md, implements its backend section, makes `dotnet build` and `dotnet test` pass.
tools: Read, Edit, Write, Glob, Grep, Bash
model: sonnet
---

You implement the backend half of the feature in PLAN.md.
- Never touch files under frontend/.
- Update tests under backend/SimpleTaskBackend.Tests/ to cover new behavior.
- `dotnet build` and `dotnet test` must pass.
- Before reporting done, stage your changes: `git add <your files>`. Do NOT commit — the orchestrator handles commits.
- Return one summary line + files changed.
```

### 1.4 Add `manual-tester`

Verify that the chrome-devtools MCP already installed and enabled.

```markdown
---
name: manual-tester
description: Manual QA tester. Drives the running app via the chrome-devtools MCP.
tools: Read, Bash, mcp__chrome-devtools__*
model: haiku
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

```markdown
---
name: code-reviewer
description: Senior reviewer. Read-only. Bugs, security, missing tests, accessibility, i18n gaps.
tools: Read, Grep, Glob, Bash
model: opus
---

Read the diff (`git diff main...HEAD`) and:
- Run `dotnet build` (from `backend/`) and `npm run lint` (from `frontend/`).
- Group findings: P0 (must fix), P1 (should fix), P2 (nit).
- One summary message. No edits.
```

### 1.6 Restart your session and verify the agents loaded

---

## Part 2 — Add lifecycle hooks

### 2.1 Claude Code — `.claude\settings.json`

```json
{
  "hooks": {
    "PostToolUse": [
      { "matcher": "Edit|Write|MultiEdit|NotebookEdit", "hooks": [{ "type": "command", "command": "powershell -NoProfile -ExecutionPolicy Bypass -File .claude/scripts/format.ps1" }] }
    ],
    "PreToolUse": [
      { "matcher": "Bash", "hooks": [{ "type": "command", "command": "powershell -NoProfile -ExecutionPolicy Bypass -File .claude/scripts/guard-bash.ps1" }] }
    ],
    "SubagentStop": [
      { "hooks": [{ "type": "command", "command": "powershell -NoProfile -ExecutionPolicy Bypass -File .claude/scripts/notify.ps1" }] }
    ]
  }
}
```

### 2.2 Claude Code — write three PowerShell scripts

Create the folder `.claude\scripts\` and save each file below. All three append a line to `.claude\hooks.log` so you can inspect what fired afterwards.

**`.claude\scripts\format.ps1`** — formats the file Claude just edited:

```powershell
$payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
$file = $payload.tool_input.file_path
if (-not $file) { exit 0 }

$ts = (Get-Date).ToString("HH:mm:ss")

if ($file -match '\.(ts|tsx|js|json|md)$') {
    npx --yes prettier --write $file *> $null
    Add-Content -Path ".claude\hooks.log" -Value "$ts [format]  $file -> prettier"
    Write-Output "formatted $file with prettier"
}
elseif ($file -match '\.cs$') {
    Push-Location backend
    dotnet format --include $file *> $null
    Pop-Location
    Add-Content -Path ".claude\hooks.log" -Value "$ts [format]  $file -> dotnet format"
    Write-Output "formatted $file with dotnet format"
}
exit 0
```

**`.claude\scripts\guard-bash.ps1`** — blocks dangerous bash before it runs:

```powershell
$payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
$cmd = $payload.tool_input.command
$ts = (Get-Date).ToString("HH:mm:ss")

if ($cmd -match 'rm\s+-rf|sudo\s|git\s+push\s+(--force|-f)|Remove-Item.*-Recurse.*-Force') {
    Add-Content -Path ".claude\hooks.log" -Value "$ts [guard]   BLOCKED: $cmd"
    [Console]::Error.WriteLine("Blocked dangerous command: $cmd")
    exit 2
}
exit 0
```

**`.claude\scripts\notify.ps1`** — Windows toast when a sub-agent finishes:

```powershell
$msg = "Claude sub-agent finished"
$ts = (Get-Date).ToString("HH:mm:ss")

Add-Content -Path ".claude\hooks.log" -Value "$ts [notify]  sub-agent finished"

try {
    Add-Type -AssemblyName System.Windows.Forms
    Add-Type -AssemblyName System.Drawing
    $ni = New-Object System.Windows.Forms.NotifyIcon
    $ni.Icon = [System.Drawing.SystemIcons]::Information
    $ni.BalloonTipTitle = "Claude Code"
    $ni.BalloonTipText  = $msg
    $ni.Visible = $true
    $ni.ShowBalloonTip(3000)
    Start-Sleep -Milliseconds 1500
    $ni.Dispose()
} catch {
    Write-Host "[Claude Code] $msg"
}
exit 0
```

### 2.3 Restart your session and verify the hooks loaded

---

## Part 3 — Run the flow

### 3.1 Open a fresh session and paste the prompt below

> **Feature**: add a `priority` field (`low | medium | high`) to tasks — persisted in the backend, shown as a colored badge, used as a secondary sort key. All four languages need new i18n keys.
>
> Before you start, clear the hooks log so this run is isolated:
> ```
> Remove-Item .claude\hooks.log -ErrorAction SilentlyContinue
> ```
>
> Run this exact flow, **one phase at a time**. Commit at each phase boundary so the git log tells the story.
>
> 1. **Explore** — use the built-in `Explore` sub-agent to map: where tasks are defined, how the frontend talks to the backend, where i18n keys live, where the task list is rendered. (No commit.)
>
> 2. **Plan** — use the built-in `Plan` sub-agent to produce a one-page plan. **Save it to `PLAN.md`.** Then commit:
>    ```
>    git add PLAN.md
>    git commit -m "plan: priority field implementation plan"
>    ```
>
> 3. **Implement** — invoke `frontend-dev` and `backend-dev` **in parallel, in a single message**. Each reads `PLAN.md`, implements its half, and runs `git add <its files>` (no commit). Wait for both to return. Then make **two separate commits** to preserve authorship:
>    ```
>    git reset                                                  # unstage everything first
>    git add backend/
>    git commit -m "feat(backend): add priority field, endpoint, tests"
>    git add frontend/
>    git commit -m "feat(frontend): add priority badge, sort, i18n"
>    ```
>
> 4. **Verify** — invoke `manual-tester` and `code-reviewer` **in parallel, in a single message**. (No commit — output is in chat.)
>
> 5. **Iterate** — for each P0/P1 finding, dispatch the relevant dev (who runs `git add` again). Then commit each fix:
>    ```
>    git commit -m "fix(backend): <what changed>"
>    # or
>    git commit -m "fix(frontend): <what changed>"
>    ```
>    Re-run the reviewer when fixes land.
>
> 6. **Report what fired** — at the end, print the full hooks log so we can see what actually ran across every context (parent + sub-agents):
>    ```
>    Get-Content .claude\hooks.log
>    ```
>    Summarize: how many `[format]` lines? How many `[notify]` (sub-agent finishes)? Any `[guard] BLOCKED`?
