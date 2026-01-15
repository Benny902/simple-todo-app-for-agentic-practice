# Phase 0: Research & Design Decisions

**Branch**: `001-init-simptask`
**Date**: 2026-01-15

## Decisions

### Decision 1: Backend Architecture - Endpoints vs Controllers
- **Decision**: Use Minimal APIs with a dedicated `Endpoints` class/group structure.
- **Rationale**: Minimal APIs are the modern default for .NET, offering less boilerplate than Controllers. Grouping them in a class keeps `Program.cs` clean while avoiding the heaviness of full MVC Controllers.
- **Alternatives considered**:
    - *Controllers*: Too verbose for a simple CRUD app.
    - *Inline Lambdas in Program.cs*: Harder to test and maintain as the app grows (even slightly).

### Decision 2: Frontend State Management
- **Decision**: Local component state (`useState`) lifted to `App.tsx` or a custom hook (`useTasks`).
- **Rationale**: The app is small enough that a global store (Redux/Zustand) is overkill. Prop drilling will be minimal (max 1 level).
- **Alternatives considered**:
    - *Context API*: Acceptable, but maybe unnecessary for just 3 components.
    - *Redux/Zustand*: Explicitly avoided to keep the "before" state simple for the workshop refactoring task.

### Decision 3: API Client
- **Decision**: Native `fetch` with a thin wrapper service.
- **Rationale**: The workshop goal explicitly mentions refactoring *to* Axios/React Query. Starting with raw `fetch` provides the contrasting "naive" implementation.

### Decision 4: CSS/Styling
- **Decision**: Standard CSS Modules or basic CSS file from Vite template.
- **Rationale**: Keeps the focus on logic/React flow rather than styling libraries (Tailwind/MUI), unless the user specifically requested the Vite default styling (which they did).

## Clarifications Resolved
*(None required - scope is well defined)*
