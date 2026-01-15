---
description: "Task list for SimpTask implementation"
---

# Tasks: Init SimpTask

**Input**: Design documents from `/specs/001-init-simptask/`
**Prerequisites**: plan.md, spec.md, data-model.md, contracts/openapi.yaml, research.md, quickstart.md

**Tests**: 
- Backend: xUnit Integration Tests (low priority, last phase)
- Frontend: Cypress E2E Tests (low priority, last phase)

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Adjust solution structure: Add existing `backend` project to solution (if not already added)
- [ ] T002 Clean Backend Template: Remove WeatherForecast sample code from `backend/`
- [ ] T003 Clean Frontend Template: Remove default Vite assets and example code from `frontend/src/`
- [ ] T004 [P] Add necessary NuGet packages to Backend: `Microsoft.EntityFrameworkCore.InMemory`
- [ ] T005 [P] Setup Frontend structure: Create directories `frontend/src/components`, `frontend/src/types`, `frontend/src/services`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T006 Define Task Entity: Create `backend/Models/Task.cs`
- [ ] T007 Setup DbContext: Create `backend/Data/AppDbContext.cs` with `DbSet<Task>`
- [ ] T008 Configure Services: Register In-Memory DbContext in `backend/Program.cs`
- [ ] T009 Define Frontend Types: Create `frontend/src/types/index.ts` with `Task` interface
- [ ] T010 Setup API Client: Create `frontend/src/services/api.ts` with base fetch configuration

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Add and View Tasks (Priority: P1) 🎯 MVP

**Goal**: Users can create new tasks and see a list of them.

**Independent Test**: Open app, type "Test Task", click Add, see "Test Task" in list.

### Implementation for User Story 1

- [ ] T011 [US1] Backend Endpoint: Implement `GET /api/tasks` in `backend/Endpoints/TaskEndpoints.cs`
- [ ] T012 [US1] Backend Endpoint: Implement `POST /api/tasks` in `backend/Endpoints/TaskEndpoints.cs`
- [ ] T013 [P] [US1] Frontend Service: Add `getTasks` and `createTask` to `frontend/src/services/api.ts`
- [ ] T014 [US1] Frontend Component: Create `frontend/src/components/TaskInput.tsx`
- [ ] T015 [US1] Frontend Component: Create `frontend/src/components/TaskList.tsx` (Read-only view initially)
- [ ] T016 [US1] Frontend Integration: Wire up state and effects in `frontend/src/App.tsx`
- [ ] T017 [US1] Enable CORS in `backend/Program.cs` to allow frontend requests

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Complete Task (Priority: P2)

**Goal**: Users can mark tasks as completed.

**Independent Test**: Click checkbox on a task, verify visual change.

### Implementation for User Story 2

- [ ] T018 [US2] Backend Endpoint: Implement `PATCH /api/tasks/{id}` in `backend/Endpoints/TaskEndpoints.cs`
- [ ] T019 [P] [US2] Frontend Service: Add `updateTask` to `frontend/src/services/api.ts`
- [ ] T020 [US2] Frontend Component: Update `frontend/src/components/TaskItem.tsx` to include checkbox/toggle
- [ ] T021 [US2] Frontend Integration: Add toggle handler in `frontend/src/App.tsx`

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - Delete Task (Priority: P3)

**Goal**: Users can remove tasks.

**Independent Test**: Click delete button, verify task removal.

### Implementation for User Story 3

- [ ] T022 [US3] Backend Endpoint: Implement `DELETE /api/tasks/{id}` in `backend/Endpoints/TaskEndpoints.cs`
- [ ] T023 [P] [US3] Frontend Service: Add `deleteTask` to `frontend/src/services/api.ts`
- [ ] T024 [US3] Frontend Component: Add Delete button to `frontend/src/components/TaskItem.tsx`
- [ ] T025 [US3] Frontend Integration: Add delete handler in `frontend/src/App.tsx`

**Checkpoint**: All user stories should now be independently functional

---

## Phase 6: Tests (Low Priority / Final Phase)

**Purpose**: Adding test coverage as requested (Integration & E2E)

- [ ] T026 Create Test Project: `dotnet new xunit -n SimpTask.Tests`
- [ ] T027 Configure Integration Tests: Setup `WebApplicationFactory` in `backend/SimpTask.Tests/`
- [ ] T028 [US1] Backend Test: Write integration test for GET/POST tasks
- [ ] T029 [US2] Backend Test: Write integration test for PATCH task
- [ ] T030 [US3] Backend Test: Write integration test for DELETE task
- [ ] T031 Setup Cypress: `npm install cypress --save-dev` in `frontend/`
- [ ] T032 [US1] E2E Test: Write Cypress spec for adding/viewing tasks
- [ ] T033 [US2] [US3] E2E Test: Write Cypress spec for completing and deleting tasks

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T034 [P] Update `frontend/src/App.css` to match standard Vite template styling
- [ ] T035 Code cleanup: Remove unused imports and types
- [ ] T036 Verify `quickstart.md` instructions work as expected

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
- **Tests (Phase 6)**: Can happen anytime after respective stories, but prioritized last per user request

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational
- **User Story 2 (P2)**: Independent logic, but UI depends on List component from US1
- **User Story 3 (P3)**: Independent logic, but UI depends on List component from US1

### Parallel Opportunities

- Backend and Frontend setup tasks can run in parallel
- Once `backend/Models` and `frontend/types` are defined, Backend API and Frontend Service work can happen in parallel
- Integration tests can be written in parallel with E2E tests

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → MVP!
3. Add User Story 2 → Test independently
4. Add User Story 3 → Test independently
