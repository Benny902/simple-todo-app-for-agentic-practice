# Simple Task Manager Workshop Exercises

This repository contains a simple "Simple Task Manager" application (React Frontend + .NET Backend) designed as a base for practicing agentic coding tools.

## Frontend Tasks

**Goal**: Refactor the naive data fetching implementation.

1.  **Refactor to React Query & Axios**:
    *   **Context**: The current implementation uses native `fetch` and `useEffect` for data fetching.
    *   **Task**: Refactor the application to use `@tanstack/react-query` and `axios` instead.
    *   **Requirements**:
        *   Replace all manual state management (loading, error, data) with React Query hooks.
        *   Create a custom hook for task management.
        *   Ensure optimistic updates or invalidation works correctly for creating and updating tasks.

## Backend Tasks

**Goal**: Extend the backend functionality and data model.

1.  **Implement Soft Delete**:
    *   **Context**: Currently, there is no DELETE functionality.
    *   **Task**: Implement a "Soft Delete" feature where tasks are marked as deleted instead of being permanently removed.
    *   **Requirements**:
        *   Update the data model to support soft deletion.
        *   Update API endpoints to filter out deleted tasks and handle the delete action.
        *   The filtering should happen on the backend.

2.  **Add Task Description**:
    *   **Task**: Add a description field to the task entity.
    *   **Requirements**:
        *   Update the backend model and API endpoints (create/update) to support a new optional description field.
        *   Update the Frontend UI to allow users to add, view, and edit the description.

