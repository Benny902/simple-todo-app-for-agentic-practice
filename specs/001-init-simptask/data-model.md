# Data Model: Init Simple Task Manager

**Branch**: `001-init-simptask`

## Entities

### Task

Represents a single item of work to be tracked.

| Field | Type | Required | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `Guid` | Yes | Unique identifier | Auto-generated |
| `Title` | `string` | Yes | The task description | Not empty/whitespace, Max 100 chars (reasonable limit) |
| `IsCompleted` | `boolean` | Yes | Completion status | Default: `false` |
| `CreatedAt` | `DateTime` | Yes | Timestamp of creation | Auto-generated |

## Validation Rules

1. **Title**:
   - Must not be null or empty string.
   - Must not contain only whitespace.
2. **State Transitions**:
   - `IsCompleted` can toggle from `false` <-> `true` freely.

## API Contracts

### Base URL
`/api/tasks`

### Endpoints

#### 1. GET /api/tasks
Retrieve all tasks.

- **Response**: `200 OK`
  ```json
  [
    {
      "id": "uuid",
      "title": "Buy milk",
      "isCompleted": false,
      "createdAt": "2026-01-15T10:00:00Z"
    }
  ]
  ```

#### 2. POST /api/tasks
Create a new task.

- **Request Body**:
  ```json
  {
    "title": "Walk dog"
  }
  ```
- **Response**: `201 Created`
  ```json
  {
    "id": "uuid",
    "title": "Walk dog",
    "isCompleted": false,
    "createdAt": "..."
  }
  ```
- **Error**: `400 Bad Request` if title is empty.

#### 3. PATCH /api/tasks/{id}
Update specific properties of a task.

- **Request Body** (Partial Update):
  ```json
  {
    "isCompleted": true
    // OR
    "title": "New Title"
  }
  ```
- **Response**: `200 OK` (returns updated task).
- **Behavior**: Updates only provided fields. `Id` and `CreatedAt` are immutable.

## Database Schema (In-Memory)

Table: `Tasks`
- `Id` (PK)
- `Title`
- `IsCompleted`
- `CreatedAt`
