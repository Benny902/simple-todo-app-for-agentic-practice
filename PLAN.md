# Priority Field Implementation Plan

## Goal
Add a `priority` field (`low | medium | high`) to tasks so it is:
- persisted in the backend,
- exposed through API contracts,
- editable/visible in the UI as a colored badge,
- used as a secondary sort key,
- translated in all supported locales (`en`, `fr`, `he`, `ru`).

## Scope
- Backend: data model, DTOs, validation, endpoint mapping, sorting, tests.
- Frontend: types, API contracts, creation UI, list item badge, sorting behavior, i18n keys.

## Backend Plan (@backend-dev)
1. Add `Priority` to task domain model and constrain allowed values (`low|medium|high`).
2. Extend request/response contracts:
   - `CreateTaskRequest` should accept priority (default to `medium` if omitted).
   - `UpdateTaskRequest` should optionally accept priority updates.
   - `TaskResponse` should include priority.
3. Update endpoint mapping/service logic:
   - persist priority on create/update,
   - include priority in responses,
   - apply sorting rule: primary existing order, secondary key by priority rank (`high`, `medium`, `low`) or as specified by current ordering behavior.
4. Add/adjust backend tests:
   - create persists priority,
   - update changes priority,
   - invalid priority rejected,
   - list sorting respects secondary priority ordering.
5. Run `dotnet build` and `dotnet test`.

## Frontend Plan (@frontend-dev)
1. Update TypeScript task types and API payload types to include `priority`.
2. Update task creation UI:
   - add priority selector with `low|medium|high`,
   - default selection aligned with backend default (`medium`).
3. Update task rendering:
   - show colored priority badge in each task item,
   - keep badge visible when task is completed.
4. Update list sorting behavior on client side if needed to match backend sorting contract and ensure stable ordering.
5. Add i18n keys in all locale files for priority labels and UI text needed by selector/badge.
6. Run `npm run lint`.

## Verify Plan
- Manual QA scenarios:
  - create tasks at each priority and verify badge color,
  - refresh and verify persistence,
  - verify sort order by priority as secondary key,
  - complete a high-priority task and verify badge remains visible.
- Reviewer checks: `git diff main...HEAD`, build/lint, and P0/P1/P2 findings.

## Commit Plan
1. `plan: priority field implementation plan`
2. `feat(backend): add priority field, endpoint, tests`
3. `feat(frontend): add priority badge, sort, i18n`
4. Optional iterative fix commits for reviewer findings.
