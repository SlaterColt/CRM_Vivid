I've got you. The text you pasted lost all its "Markdown DNA" (the hashtags, backticks, and stars that tell GitHub how to style it).

Here is the **Raw Markdown** code.

**Action:** Open your `README.md` file in VS Code, delete everything currently in it, and paste this code block exactly as is.

````markdown
# CRM_Vivid

> **The Operating System for LCD Entertainment**

CRM_Vivid is a B2B SaaS Event Management Platform built to handle the complexity of high-stakes entertainment logistics. It transitions LCD Entertainment from scattered spreadsheets to a centralized, distributed system.

---

## 🛠️ The Tech Stack

We run on a modern, type-safe, and scalable stack.

### Backend (The Core)

- **Framework:** .NET 9 Web API
- **Architecture:** Clean Architecture (Onion) + CQRS (MediatR)
- **Database:** PostgreSQL (via EF Core)
- **Background Jobs:** Hangfire + Redis
- **Email:** SendGrid

### Frontend (The Harness)

- **Framework:** React + Vite
- **Language:** TypeScript
- **Styling:** CSS Modules / Standard CSS
- **State:** Context API (Lightweight)

### Infrastructure

- **Containerization:** Docker (Postgres, Redis)
- **Storage:** Local (Dev) / Cloud Ready (Abstraction Layer)

---

## 📜 The Engineering Doctrine (Non-Negotiable)

To maintain velocity and stability, every contributor (Human or AI) must adhere to these protocols.

### 1. The "No Canvas" Rule

We do not use proprietary "Canvas" or "Notebook" features for code generation. All code changes, documentation, and snippets must be standard **Markdown code blocks**. This ensures portability and prevents formatting lock-in.

### 2. Protocol 6: Read Before Write

**Never** overwrite a file without first understanding its current context. If you are using an AI assistant, force it to read the existing file before proposing edits to preserve custom logic.

### 3. Protocol 7: The Schema Sync

The Backend is the Source of Truth.

- If you change a **DTO** or **Entity** in .NET...
- You **MUST** immediately update `frontend_harness/src/types.ts`.
- _Failure to do this breaks the frontend build._

### 4. Protocol 10: The Task Alias

We use a customized `Task` entity. This conflicts with the C# `System.Threading.Tasks.Task`.

- **Rule:** In any file importing `CRM_Vivid.Core.Entities`, you must add this alias at the top:
  ```csharp
  using Task = System.Threading.Tasks.Task;
  ```

### 5. Protocol 13: Test Symmetry

The `tests/` directory is a mirror of the `src/` directory.

- Tests must target **.NET 9**.
- NuGet package versions in tests must match `src` exactly.
- Standard import depth is `../../`.

### 6. The 13-Step Feature Workflow

Do not skip steps. This is the "Run of Show" for adding a new feature:

1.  **Entity:** Define the Core data structure.
2.  **Config:** EF Core `IEntityTypeConfiguration`.
3.  **DbContext:** Register the `DbSet`.
4.  **DTO:** Define the data transfer object.
5.  **Command/Query:** Define the Request record.
6.  **Handler:** Implement the logic (MediatR).
7.  **Mapping:** Update `MappingProfile.cs` (AutoMapper).
8.  **Validator:** FluentValidation rules (Check DB state!).
9.  **Controller:** Expose the endpoint.
10. **Migration:** `dotnet ef migrations add [Name]` -> `dotnet ef database update`.
11. **Types:** Update Frontend `types.ts`.
12. **Service:** Update Frontend `apiClient.ts`.
13. **UI:** Build the Component.

---

## 📂 Project Structure

```text
.
├── frontend_harness/        # React Application
│   ├── src/types.ts         # The Holy Grail of Types
│   └── src/components/      # Vertical Slice Components
├── src/
│   ├── Api/                 # Entry Point (Controllers, Program.cs)
│   ├── Application/         # Business Logic (CQRS, Commands, Queries)
│   ├── Core/                # Domain Entities (Pure C#)
│   └── Infrastructure/      # Database, File Storage, Email Service
├── tests/                   # xUnit Test Suite
└── docker-compose.yml       # Infra Setup
```
````

---

## 🚀 Getting Started

### 0.\ Security & Auth

Run the below in your terminal with the real passwords

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=CRM_Vivid_Dev;Username=postgres;Password=THE_REAL_PASSWORD"

### 1\. Infrastructure (Docker)

Spin up Postgres and Redis:

```bash
docker-compose up -d
```

### 2\. Backend (.NET 9)

Navigate to the API and run:

```bash
cd src/Api
dotnet run
```

_Swagger UI:_ `https://localhost:7066/swagger`

### 3\. Frontend (Vite)

Open a new terminal:

```bash
cd frontend_harness
npm install
npm run dev
```

_UI:_ `http://localhost:5173`

---

## 🤝 Contribution Workflow

We use a **Vertical Slice** assignment model. You own the Feature from Database to CSS.

1.  **Branching:**
    - `main`: Production Ready (Protected).
    - `develop`: Integration Branch.
    - `feature/[your-name]/[feature-name]`: Your workspace.
2.  **Pull Requests:**
    - All PRs must be reviewed by the **Lead Architect (Slater)**.
    - PRs failing _Protocol 7_ (Type Sync) will be rejected.

---

**System Status:** `PHASE 20 COMPLETE` (The Archivist).
**Lead Architect:** Slater Colt

```

```
