# Tour Management System (PSW 2025/2026)

University project for "Projektovanje softvera" at FTN, Novi Sad. Web-based tour management system with three roles: Tourist, Guide, Administrator.

## Tech Stack
- **Backend:** .NET 8, ASP.NET Core Web API
- **Frontend:** Angular (separate project)
- **Database:** PostgreSQL with EF Core (Npgsql)
- **Architecture:** DDD (Domain-Driven Design), CQRS via MediatR
- **Auth:** JWT Bearer tokens
- **Email:** MailKit (SMTP)
- **Maps:** Leaflet (frontend)

## Solution Structure (DDD Layers)
```
TourManagement.API/              → Presentation (controllers, middleware)
TourManagement.Application/      → Application (commands, queries, DTOs, validators)
TourManagement.Domain/           → Domain (entities, value objects, aggregates, events, repo interfaces)
TourManagement.Infrastructure/   → Infrastructure (EF DbContext, repositories, email, auth)
TourManagement.Tests/            → Tests (xUnit, mirrors main project structure)
```

**Dependency rules:** API → Application, Infrastructure | Application → Domain | Infrastructure → Domain, Application | Domain → nothing

```
TourManagement.Client/               → Angular frontend (standalone project)
  src/app/core/                      → Guards, interceptors, models, services
  src/app/features/                  → Feature modules (auth, tours, cart, admin)
  src/app/shared/                    → Shared components, pipes, directives
```

## Commands
```bash
# Backend - Build
dotnet build TourManagement.API/TourManagement.API.sln

# Backend - Run API (Swagger at http://localhost:5106/swagger)
dotnet run --project TourManagement.API/TourManagement.API

# Backend - Run tests
dotnet test TourManagement.API/TourManagement.API.sln

# Backend - EF Core migrations
dotnet ef migrations add <Name> --project TourManagement.API/TourManagement.Infrastructure --startup-project TourManagement.API/TourManagement.API
dotnet ef database update --project TourManagement.API/TourManagement.Infrastructure --startup-project TourManagement.API/TourManagement.API

# Frontend - Install dependencies
cd TourManagement.Client && npm install

# Frontend - Run dev server (http://localhost:4200)
cd TourManagement.Client && npx ng serve

# Frontend - Build
cd TourManagement.Client && npx ng build
```

## Development Principles

### TDD (Test-Driven Development)
- **Always write tests first**, then implement the code to make them pass
- Workflow: Red (failing test) → Green (make it pass) → Refactor
- Tests go in `TourManagement.Tests/` mirroring the main project structure
- Unit tests for domain logic, integration tests for infrastructure

### DDD (Domain-Driven Design)
- Domain layer is the core — no dependencies on infrastructure or frameworks
- Use aggregates, entities, value objects, domain events, and repository interfaces
- Ubiquitous language: class/method names should reflect the business domain

### SOLID Principles
- **S** — Single Responsibility: one reason to change per class
- **O** — Open/Closed: extend via new classes, not modifying existing ones
- **L** — Liskov Substitution: subtypes must be substitutable for base types
- **I** — Interface Segregation: small, focused interfaces
- **D** — Dependency Inversion: depend on abstractions (interfaces in Domain, implementations in Infrastructure)

## Coding Conventions
- **Naming:** PascalCase for public members, `_camelCase` for private fields
- **Controllers:** Thin — delegate to MediatR handlers
- **Repository pattern:** Interface in Domain, implementation in Infrastructure
- **Validation:** FluentValidation in Application layer
- **DTOs:** Record types preferred
- **Value Objects:** Record types in Domain layer
- **One aggregate root per file**
- **Code identifiers in English**, Serbian acceptable in comments only
- **Event sourcing** required for problem/issue status tracking

## Workflow Reminders
- **Migrations:** Whenever domain entities are added, removed, or modified (new properties, changed relationships, renamed fields), remind the user to run EF Core migrations. The user runs these manually from the `TourManagement.API/` directory:
  ```bash
  dotnet ef migrations add <DescriptiveName> --project TourManagement.Infrastructure --startup-project TourManagement.API
  dotnet ef database update --project TourManagement.Infrastructure --startup-project TourManagement.API
  ```
- **Do not** start Angular dev server or .NET API — the user handles that manually
