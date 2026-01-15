# Implementation Plan: Init SimpTask

**Branch**: `001-init-simptask` | **Date**: 2026-01-15 | **Spec**: [Link to spec](../spec.md)
**Input**: Feature specification from `/specs/001-init-simptask/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Implement a minimalist To-Do list application "SimpTask" with a React Frontend and .NET WebAPI Backend. The app will support creating, viewing, completing, and deleting tasks. The backend will use an In-Memory database for simplicity (per requirements), and the frontend will use standard `fetch` and `useEffect` initially to serve as a baseline for future refactoring tasks.

## Technical Context

**Language/Version**: 
- Backend: C# / .NET 10
- Frontend: TypeScript 5.x / React 19.x

**Primary Dependencies**: 
- Backend: ASP.NET Core WebAPI, Microsoft.EntityFrameworkCore.InMemory
- Frontend: Vite, React, Native fetch (for initial "naive" implementation)

**Storage**: 
- In-Memory (EF Core InMemory provider)

**Testing**: 
- Backend: xUnit Integration Tests (using WebApplicationFactory)
- Frontend: Cypress (E2E tests)
- Note: Testing implementation is low priority (final phase).

**Target Platform**: 
- Localhost development environment (Cross-platform)

**Project Type**: 
- Web application (Frontend + Backend monorepo-style structure)

**Performance Goals**: 
- Minimal latency for local operations (<100ms API response).
- Instant UI feedback.

**Constraints**: 
- Minimal complexity.
- No authentication.
- In-memory persistence (data lost on restart).

**Scale/Scope**: 
- Single user, <100 tasks typical load.
- ~5 API endpoints.
- ~2 Main UI components (TaskForm, TaskList).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

*(Constitution file is generic/placeholder, so assuming standard best practices apply)*

- **Simplicity**: The design strictly adheres to the "minimalist" requirement.
- **Test-First**: Plan includes independent tests in the spec, and testing frameworks are defined.
- **Library/Component Based**: Frontend will be componentized (TaskItem, TaskList, TaskInput).

## Project Structure

### Documentation (this feature)

```text
specs/001-init-simptask/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
backend/
├── Models/            # Task entity
├── Data/              # AppDbContext
├── Services/          # TaskService (Business logic)
├── Endpoints/         # TaskEndpoints (API handlers)
├── Program.cs
└── SimpTask.Tests/    # Separate xUnit project (Integration Tests)

frontend/
├── src/
│   ├── components/
│   │   ├── TaskList.tsx
│   │   ├── TaskItem.tsx
│   │   └── TaskInput.tsx
│   ├── services/
│   │   └── api.ts         # Fetch wrappers
│   ├── types/
│   │   └── index.ts       # Task interface
│   └── App.tsx
└── tests/
```

**Structure Decision**: Standard separate Frontend/Backend directories to allow distinct learning tracks for each stack. Backend follows a lightweight Service/Endpoint pattern (Simpler than full Controller/MediatR for this scope).

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

*(No violations - simplicity is the primary goal)*
