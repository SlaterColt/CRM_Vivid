Here is the **Raw Markdown** for `CONTRIBUTING.md`.

Just like before, delete the current plain text in VS Code and paste this block in. This will give you the nice checkboxes, bold headers, and code snippets on GitHub.

````markdown
# Contributing to CRM_Vivid

Welcome to the engineering team.

This document outlines the standards, workflows, and "Hard Rules" required to contribute to the CRM_Vivid codebase. We are building a distributed system for high-stakes entertainment logistics. Precision is not optional.

---

## 👥 The Squad Roles

- **Slater Colt (Lead Architect):** System Integrity, Core Architecture, Code Review, Prompt Engineering.
- **Marc-Edwin (Integration Specialist):** External APIs, Cloud Infrastructure, Financial Logic.
- **Carl-Edwin (Guardian):** Security (Auth/RBAC), Testing Strategy, Compliance.

---

## ⚔️ The Workflow: Vertical Slices

We do not split work by "Frontend vs. Backend." We split by **Feature**.
If you are assigned "Financials," you own the Database Schema, the API Endpoint, and the React Component.

### The "Genesis Hand-off" Protocol

To maintain architectural consistency, we use a centralized context (Master Chat) to initialize features.

1.  **Assignment:** Slater generates a **Genesis Prompt** defining the Phase objectives.
2.  **Execution:** You (The Dev) take that prompt into your environment/AI context to generate the initial boilerplate.
3.  **Refinement:** You manually refine the code, implementing specific business logic and handling edge cases.
4.  **PR:** You submit a Pull Request.

---

## 📜 The Engineering Doctrine (Non-Negotiable)

Violating these protocols will result in immediate PR rejection.

### 1. The "No Canvas" Rule

**Markdown Only.** Do not use proprietary "Canvas" or "Notebook" features for code generation. All output must be standard Markdown code blocks to ensure portability.

### 2. Protocol 6: Read Before Write

**Never** overwrite a file without first understanding its current context. We have custom logic in `DependencyInjection.cs` and `Program.cs`. Blind copy-pasting destroys this.

### 3. Protocol 7: The Schema Sync (CRITICAL)

The Backend is the Source of Truth.

- **Constraint:** Any change to a Backend Entity or DTO...
- **Action:** ...MUST be immediately followed by an update to `frontend_harness/src/types.ts`.
- **Reason:** Failure to do this breaks the frontend build and causes "drift."

### 4. Protocol 10: The Task Alias

We use a domain entity named `Task`. This conflicts with C#'s `System.Threading.Tasks.Task`.
**Rule:** In any file importing `CRM_Vivid.Core.Entities`, add this alias:

```csharp
using Task = System.Threading.Tasks.Task;
```
````

### 5\. Protocol 13: Test Symmetry

The `tests/` directory structure must mirror `src/` exactly.

- Target **.NET 9**.
- Match NuGet versions precisely.
- Use `../../` relative paths for imports.

### 6\. Protocol 14: Infrastructure Awareness

When adding capabilities that touch the HTTP Pipeline (Middleware, Static Files, Auth, CORS), you **must** request context on `Program.cs` first. Do not guess where middleware goes.

---

## 🏗️ The 13-Step Feature Workflow

Do not innovate on the process. Innovate on the logic.

1.  **Entity:** Define the Core data structure (`Core/Entities`).
2.  **Config:** EF Core `IEntityTypeConfiguration` (`Infrastructure/Persistence/Configurations`).
3.  **DbContext:** Register the `DbSet` (`Infrastructure/Persistence/ApplicationDbContext.cs`).
4.  **DTO:** Define the data transfer object (`Application/Common/Models`).
5.  **Command/Query:** Define the Request record (`Application/[Feature]/Commands`).
6.  **Handler:** Implement the logic using MediatR.
7.  **Mapping:** Update `MappingProfile.cs` (AutoMapper).
8.  **Validator:** Implement FluentValidation (`Application/[Feature]/Validators`).
    - _Constraint:_ Validators must check DB state (e.g., duplicates) inside the pipeline.
9.  **Controller:** Expose the endpoint (`Api/Controllers`).
10. **Migration:** `dotnet ef migrations add [Name]` -\> `dotnet ef database update`.
11. **Types:** Update Frontend `types.ts`.
12. **Service:** Update Frontend `apiClient.ts`.
13. **UI:** Build the Component.

---

## 🐙 Git Strategy

- **`main`**: Production Ready. Protected. No direct commits.
- **`develop`**: The integration branch. PRs target this.
- **`feature/[user]/[feature-name]`**: Your workspace.
  - Ex: `feature/marc/cloud-storage`
  - Ex: `feature/carl/unit-tests`

### Pull Request Checklist

Before requesting a review from Slater:

- [ ] Does the code compile?
- [ ] Did you run the `tests/` project?
- [ ] Did you update `types.ts`?
- [ ] Did you check for the `Task` alias conflict?
- [ ] Is there any commented-out legacy code? (Delete it).

---

**"We don't just write code. We build the memory of the business."**

```

```
