# Coucher Web API Manual Test Plan

## Overview

This document is a manual test plan for the current `Coucher.WebApi` surface.

It covers:
- authentication
- authorization
- GraphQL read access
- exercise writes
- exercise task writes
- admin writes
- mock seeding

Use this plan after local development changes, before Docker image publication, and before handing the API to frontend or QA consumers.

## Environment

- API base URL: `http://localhost:8080`
- Swagger UI: `http://localhost:8080/swagger`
- GraphQL endpoint: `http://localhost:8080/graphql`
- Session header: `session-id`

Recommended tools:
- Swagger UI for quick REST validation
- Postman or Bruno for repeatable manual scenarios
- GraphQL Playground, Altair, or Swagger-adjacent client for `/graphql`

## Important Notes

- Most resources do not have REST `GET` endpoints.
- Verify created or updated data mainly through GraphQL.
- Most write endpoints require authentication.
- Authorization failures should return:
  - `401` for missing or invalid session
  - `403` for authenticated but forbidden actions
- Successful delete endpoints should usually return `204 No Content`.

## Seed and Test Data Setup

### 1. Reset and seed mock data

Call:

`POST /api/mock/seed`

Suggested body:

```json
{
  "resetExisting": true,
  "userCount": 12,
  "exerciseCount": 3,
  "additionalParticipantsPerExercise": 4,
  "taskTemplateCount": 12,
  "tasksPerExercise": 10,
  "notificationsPerUser": 2
}
```

Expected:
- `200 OK`
- response contains counts for users, exercises, tasks, templates, notifications

### 2. Seeded login conventions

Mock users are generated with:
- `identityNumber`: `300000000`, `300000001`, `300000002`, ...
- `personalNumber`: `PN-1000`, `PN-1001`, `PN-1002`, ...

Use:
- admin user: `300000000` / `PN-1000`
- regular user examples: `300000001` / `PN-1001`, `300000002` / `PN-1002`

Current seeded roles:
- first user is `Admin`
- remaining users are `User`

No seeded `Auditor` is created by default. Create one by updating a user role as part of the admin tests below.

### 3. Collect ids for later tests

Use GraphQL to collect:
- user ids
- user role ids
- exercise ids
- participant ids
- task ids
- section ids
- influencer ids
- closed-list ids for exercise status, task status, series, category, sections, influencers

Suggested GraphQL query for admin:

```graphql
query BootstrapData {
  users {
    id
    identityNumber
    firstName
    lastName
  }
  userRoles {
    id
    userId
    role
  }
  exercises {
    id
    name
    createdByUserId
    participants {
      id
      userId
      role
    }
  }
  exerciseTasks {
    id
    exerciseId
    parentId
    name
  }
  closedListItems {
    id
    key
    value
    displayOrder
  }
}
```

## Authentication Tests

### A1. Login with valid admin credentials

Call:
- `POST /api/auth/login`

Body:

```json
{
  "identityNumber": "300000000",
  "passwordOrPersonalNumber": "PN-1000"
}
```

Expected:
- `200 OK`
- response contains `sessionId` and `userId`

Save the returned `sessionId` as `ADMIN_SESSION`.

### A2. Login with valid regular user credentials

Repeat login for:
- `300000001` / `PN-1001`
- `300000002` / `PN-1002`

Save returned session ids as `USER1_SESSION` and `USER2_SESSION`.

### A3. Login with wrong credentials

Expected:
- `401 Unauthorized`

### A4. Get current session with valid header

Call:
- `GET /api/auth/session`
- header: `session-id: <valid session>`

Expected:
- `200 OK`
- returned `sessionId` and `userId` match the logged-in user

### A5. Get current session without header

Expected:
- `401 Unauthorized`

### A6. Logout with valid header

Call:
- `POST /api/auth/logout`

Expected:
- `204 No Content`

### A7. Reuse logged-out session

Call:
- `GET /api/auth/session`

Expected:
- `401 Unauthorized`

## Authorization Bootstrap Tests

### B1. Promote one seeded user to Auditor

As admin:
- get a non-admin `userId`
- get that user’s existing `userRole` row id from GraphQL
- call `PUT /api/admin/user-roles/{id}`

Body example:

```json
{
  "userId": "<target-user-id>",
  "role": 1
}
```

Expected:
- `200 OK`
- role becomes `Auditor`

Save that user’s session as `AUDITOR_SESSION`.

### B2. Verify admin-only user-role update is blocked for non-admin

As regular user and as auditor:
- repeat the same `PUT /api/admin/user-roles/{id}`

Expected:
- `403 Forbidden`

## GraphQL Visibility Tests

Use the same query shape for all roles:

```graphql
query VisibilityCheck {
  exercises {
    id
    name
    createdByUserId
    participants {
      id
      userId
      role
    }
  }
  exerciseTasks {
    id
    exerciseId
    name
  }
}
```

### G1. Admin visibility

Expected:
- sees all exercises
- sees all tasks

### G2. Auditor visibility

Expected:
- sees all exercises
- sees all tasks

### G3. Regular user creator visibility

Use a regular user who created at least one exercise during this test plan.

Expected:
- sees exercises they created
- also sees exercises where they are a participant
- does not see unrelated exercises

### G4. Exercise manager visibility

Use a user who is manager in at least one exercise.

Expected:
- sees exercises where they are manager
- also sees exercises where they are a participant
- does not see unrelated exercises

### G5. Exercise participant visibility

Use a user added as participant only.

Expected:
- sees exercises where they are a participant
- does not see unrelated exercises

### G6. Admin-only GraphQL fields

As admin, query:

```graphql
query AdminReads {
  users { id identityNumber }
  userRoles { id userId role }
  taskTemplates { id name }
  taskTemplateDependencies { id templateId dependsOnId }
  taskTemplateInfluencers { id templateId influencerId }
}
```

Expected:
- admin gets data

Repeat as regular user and auditor.

Expected:
- GraphQL returns an authorization error for admin-only fields

### G7. User notifications are self-only

Query:

```graphql
query MyNotifications {
  userNotifications {
    id
    userId
    title
    exerciseId
    taskId
  }
}
```

Expected for any authenticated user:
- only notifications where `userId` equals current user id

## REST Authorization Matrix Tests

For each endpoint below, run:
- as admin
- as auditor
- as exercise manager
- as exercise participant
- as unrelated regular user when relevant

Expected results:
- admin: allowed unless explicitly read-only path does not apply
- auditor: `403` on all write endpoints
- manager: allowed only inside managed exercises
- participant: allowed only for partial task edits in assigned exercises
- unrelated regular user: `403`

## Exercise Tests

### E1. Regular user creates an exercise

Call:
- `POST /api/exercises`

Verify:
- `200 OK`
- response includes `createdByUserId` equal to caller
- GraphQL shows the new exercise for the creator

### E2. Auditor cannot create an exercise

Expected:
- `403 Forbidden`

### E3. Admin can create an exercise

Expected:
- `200 OK`

### E4. Exercise definition full update

Call:
- `PUT /api/exercises/{id}`

As manager/admin:
- expect `200 OK`

As participant/auditor/unrelated user:
- expect `403 Forbidden`

### E5. Exercise partial definition updates

Call each:
- `PUT /api/exercises/{id}/details`
- `PUT /api/exercises/{id}/end-date`
- `PUT /api/exercises/{id}/status`
- `PUT /api/exercises/{id}/trainee-unit`
- `PUT /api/exercises/{id}/trainer-unit`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

### E6. Add participant

Call:
- `POST /api/exercises/{id}/participants`
- raw string body of target user id

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

Verify in GraphQL:
- new participant row exists

### E7. Update participant role

Call:
- `PUT /api/exercises/participants/{participantId}/role`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

### E8. Reassign exercise manager

Call:
- `PUT /api/exercises/{id}/manager`
- raw string body of target user id

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

Verify:
- old manager becomes participant
- target becomes manager

### E9. Add and remove section

Calls:
- `POST /api/exercises/{id}/sections`
- `DELETE /api/exercises/sections/{sectionLinkId}`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

### E10. Add and remove influencer

Calls:
- `POST /api/exercises/{id}/influencers`
- `DELETE /api/exercises/influencers/{influencerLinkId}`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

### E11. Add, update, and remove contact

Calls:
- `POST /api/exercises/{id}/contacts`
- `PUT /api/exercises/contacts/{contactId}`
- `DELETE /api/exercises/contacts/{contactId}`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

### E12. Archive and unarchive exercise

Calls:
- `POST /api/exercises/{id}/archive`
- `POST /api/exercises/{id}/unarchive`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

Verify:
- `archiveTime` and `archivedByUserId` set on archive
- cleared on unarchive

## Exercise Task Tests

Use one exercise where the tester is manager and one where the tester is participant only.

### T1. Create root task

Call:
- `POST /api/exercise-tasks`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

Verify:
- default status id is set from config

### T2. Bulk create tasks

Call:
- `POST /api/exercise-tasks/bulk`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

Verify:
- dependencies by `taskKey` are created correctly

### T3. Create child task

Call:
- `POST /api/exercise-tasks/{id}/children`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

Verify:
- child created under the parent
- creating a grandchild under a child should fail

### T4. Full task update

Call:
- `PUT /api/exercise-tasks/{id}`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

### T5. Full task partial-split updates

Calls:
- `PUT /api/exercise-tasks/{id}/series`
- `PUT /api/exercise-tasks/{id}/category`
- `PUT /api/exercise-tasks/{id}/details`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

### T6. Participant partial task edits

Calls:
- `PUT /api/exercise-tasks/{id}/status`
- `PUT /api/exercise-tasks/{id}/due-date`
- `POST /api/exercise-tasks/{id}/responsible-users`
- `PUT /api/exercise-tasks/{id}/responsible-users`
- `DELETE /api/exercise-tasks/responsible-users/{responsibilityId}`
- `POST /api/exercise-tasks/{id}/responsible-users/bulk-delete`

Expected:
- participant in same exercise allowed
- manager/admin allowed
- auditor/unrelated user forbidden

Verify:
- status updates set `lastStatusUpdaterId`, `lastStatusUpdateTime`
- completion timestamp behavior matches status transitions

### T7. Dependency management

Calls:
- `POST /api/exercise-tasks/{id}/dependencies`
- `DELETE /api/exercise-tasks/dependencies/{dependencyId}`

Expected:
- add/delete allowed only for manager/admin
- participant/auditor/unrelated user forbidden

Negative checks:
- self-dependency rejected
- cross-exercise dependency rejected

### T8. Delete task

Call:
- `DELETE /api/exercise-tasks/{id}`

Expected:
- manager/admin allowed
- participant/auditor/unrelated user forbidden

Verify:
- deep delete removes descendants, dependencies, responsible users

## Admin Closed List Tests

Run all as admin, then repeat one representative case as auditor and regular user.

### C1. Create and bulk create closed-list items

Calls:
- `POST /api/admin/closed-list-items`
- `POST /api/admin/closed-list-items/bulk`

Expected:
- admin `200 OK`
- non-admin `403 Forbidden`

### C2. Update closed-list item

Calls:
- `PUT /api/admin/closed-list-items/{id}`
- `PUT /api/admin/closed-list-items/{id}/value`
- `PUT /api/admin/closed-list-items/{id}/description`
- `PUT /api/admin/closed-list-items/{id}/display-order`
- `PUT /api/admin/closed-list-items/display-orders`

Expected:
- admin `200 OK`
- non-admin `403 Forbidden`

### C3. Archive and unarchive closed-list item

Calls:
- `POST /api/admin/closed-list-items/{id}/archive`
- `POST /api/admin/closed-list-items/{id}/unarchive`

Expected:
- admin allowed
- non-admin forbidden

## Admin Task Template Tests

### TT1. Create template with nested children and dependency keys

Call:
- `POST /api/admin/task-templates`

Use payload with:
- root template
- direct child templates
- `templateKey`
- `dependsOnTemplateKeys`

Expected:
- admin `200 OK`
- non-admin `403 Forbidden`

Verify:
- root and children are created
- child-to-child dependency links resolve correctly
- grandchildren are not allowed

### TT2. Bulk create templates

Call:
- `POST /api/admin/task-templates/bulk`

Expected:
- admin only

### TT3. Update template

Calls:
- `PUT /api/admin/task-templates/{id}`
- `PUT /api/admin/task-templates/{id}/series`
- `PUT /api/admin/task-templates/{id}/category`
- `PUT /api/admin/task-templates/{id}/default-weeks-before-exercise-start`
- `PUT /api/admin/task-templates/{id}/details`

Expected:
- admin allowed
- non-admin forbidden

### TT4. Child, dependency, and influencer management

Calls:
- `POST /api/admin/task-templates/{id}/children`
- `POST /api/admin/task-templates/{id}/dependencies`
- `DELETE /api/admin/task-templates/dependencies/{dependencyId}`
- `POST /api/admin/task-templates/{id}/influencers`
- `DELETE /api/admin/task-templates/influencers/{influencerLinkId}`

Expected:
- admin allowed
- non-admin forbidden

### TT5. Archive and unarchive task template

Calls:
- `POST /api/admin/task-templates/{id}/archive`
- `POST /api/admin/task-templates/{id}/unarchive`

Expected:
- admin allowed
- non-admin forbidden

## Negative and Security Tests

### N1. Missing session header on protected endpoints

Test representative protected endpoints from:
- exercises
- exercise tasks
- admin

Expected:
- `401 Unauthorized`

### N2. Invalid session header

Expected:
- `401 Unauthorized`

### N3. Wrong-role access

Test representative cases:
- auditor updating an exercise
- participant archiving an exercise
- regular user deleting a task
- non-admin updating a user role

Expected:
- `403 Forbidden`

### N4. Resource not found

Use random GUIDs on representative endpoints.

Expected:
- not-found behavior remains consistent with current implementation
- usually `500`-style exception handling is not desired, so record any unexpected error shape as a defect

### N5. Invalid body formats

Examples:
- malformed GUID string body
- malformed primitive JSON for scalar endpoints
- invalid enum values

Expected:
- request rejected by model binding or service validation

## Final Regression Checklist

- login, logout, and session endpoints still work after seeding
- seeded admin can manage user roles, closed lists, and templates
- regular user can create an exercise
- auditor can read but cannot write
- creator visibility works even when creator is only a participant
- manager can fully manage exercise and task data inside their exercise
- participant can only perform partial task edits inside assigned exercises
- GraphQL data is filtered by current user role and exercise scope
- admin-only GraphQL fields are blocked for non-admin users
- notifications query only returns current user notifications
- Swagger still renders all current endpoints and scalar body shapes correctly
