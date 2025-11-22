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

## 🔐 Secrets & Setup (First Time Run)

We practice **Zero Trust**. No secrets are in the repo. You must configure your local environment before the app will boot.

### Step 1: Docker Secrets (Root)

Create a file named `.env` in the **root directory**.
_(Ask Slater for the `CRM_Vivid_Local` password or set your own)._

```ini
POSTGRES_USER=postgres
POSTGRES_PASSWORD=YOUR_LOCAL_PASSWORD_HERE
```

## Step 2: Backend Vault (User Secrets)

## We use .NET User Secrets to inject credentials. Run these commands in src/Api:

dotnet user-secrets init

# 1. Database Connection (Must match the password in Step 1)

dotnet user-secrets set "ConnectionStrings:DefaultConnection" 'Host=localhost;Port=5432;Database=CRM_Vivid_Dev;Username=postgres;Password=YOUR_LOCAL_PASSWORD_HERE'

# 2. SendGrid (Ask Slater/Marc for Key, or generate your own)

dotnet user-secrets set "SendGrid:ApiKey" "SG.YOUR_API_KEY"

# Go to Clerk Dashboard -> API Keys -> Publishable Key

## Step 3: Frontend Secrets

Create a file named .env.local in frontend_harness/

VITE_CLERK_PUBLISHABLE_KEY=pk_test_YOUR_KEY_HERE

#### 🚀 Launch Sequence

## 1. Start Infrastructure

Spin up Postgres and Redis (Uses your root .env file):

docker-compose up -d

# 2. Apply Database Schema

## Ensure your local DB has the latest tables:

cd src/Api
dotnet ef database update

## 3. Run Backend

dotnet run

# Swagger UI: https://localhost:7066/swagger

#### 4. Run Frontend_Harness for Testing

Open a new terminal:

cd frontend_harness
npm install
npm run dev

# UI: http://localhost:5173

#### 🤝 Contribution Workflow

We use a Vertical Slice assignment model. You own the Feature from Database to CSS.

# 1. Branching:

## main: Production Ready (Protected).

**develop: Integration Branch.**

feature/[your-name]/[feature-name]: Your workspace.

# 2. Pull Requests:

## All PRs must be reviewed by the Lead Architect (Slater).

**PRs failing Protocol 7 (Type Sync) will be rejected.**
