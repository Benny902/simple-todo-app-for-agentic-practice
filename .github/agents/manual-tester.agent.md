---
name: manual-tester
description: Manual QA tester. Drives the running app via the chrome-devtools MCP server.
tools: read, execute
model: GPT-4.1 (copilot)
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