# Tour Management System

University project for **Projektovanje softvera (PSW)** 2025/2026 at FTN, Novi Sad.

A web-based tour management platform with three user roles: **Tourist**, **Guide**, and **Administrator**. Tourists browse and purchase tours, leave reviews, and report problems. Guides create and manage tours, handle reported problems, and assign substitute guides. Administrators oversee user management and problem escalation.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | .NET 8, ASP.NET Core Web API |
| Frontend | Angular 21, TypeScript 5.9, Tailwind CSS |
| Database | PostgreSQL with EF Core 8 (Npgsql) |
| Architecture | DDD (Domain-Driven Design), CQRS via MediatR |
| Auth | JWT Bearer tokens, BCrypt password hashing |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Email | MailKit (SMTP) |
| Maps | Leaflet |
| Testing | xUnit (backend), Vitest (frontend) |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v18+) and npm
- [PostgreSQL](https://www.postgresql.org/download/) (v14+)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/psw-2026.git
cd psw-2026
```

### 2. Configure the database

Update the connection string in `TourManagement.API/TourManagement.API/appsettings.Development.json` to point to your local PostgreSQL instance.

### 3. Apply database migrations

```bash
dotnet ef database update --project TourManagement.API/TourManagement.Infrastructure --startup-project TourManagement.API/TourManagement.API
```

### 4. Run the backend

```bash
dotnet run --project TourManagement.API/TourManagement.API
```

The API will be available at `http://localhost:5106` with Swagger UI at `http://localhost:5106/swagger`.

### 5. Run the frontend

```bash
cd TourManagement.Client
npm install
npx ng serve
```

The frontend will be available at `http://localhost:4200`.

## Project Structure

```
TourManagement.API/
├── TourManagement.API/             Presentation layer (controllers, middleware)
├── TourManagement.Application/     Application layer (commands, queries, DTOs, validators)
├── TourManagement.Domain/          Domain layer (entities, value objects, aggregates, events)
├── TourManagement.Infrastructure/  Infrastructure layer (EF DbContext, repositories, email, auth)
└── TourManagement.Tests/           Test project (xUnit)

TourManagement.Client/
└── src/app/
    ├── core/                       Guards, interceptors, models, services
    ├── features/                   Feature modules (auth, tours, cart, guide, admin, etc.)
    └── shared/                     Shared components, pipes, directives
```

### Dependency Rules (DDD)

```
API → Application, Infrastructure
Application → Domain
Infrastructure → Domain, Application
Domain → (nothing)
```

The Domain layer is the core of the system and has zero external dependencies. Repository interfaces are defined in Domain; implementations live in Infrastructure.

## Domain Model

| Entity | Description |
|--------|-------------|
| **Tour** | A guided tour with key points, pricing, and status management |
| **KeyPoint** | Geographic waypoint belonging to a tour (lat/lng, name, description) |
| **TourReview** | Tourist rating and comment for a completed tour |
| **TourPurchase** | Record of a tourist purchasing a tour |
| **ShoppingCart** | Tourist's cart with cart items and bonus point tracking |
| **Problem** | Issue reported by a tourist, tracked via event sourcing |
| **User** | System user (Tourist, Guide, or Administrator) |

Problem status transitions are modeled with **event sourcing** — each state change (Created, SentToReview, ReturnedToGuide, Resolved, Rejected) is stored as an immutable event.

## API Endpoints

| Controller | Responsibility |
|-----------|---------------|
| `AuthController` | Registration, login, JWT token issuance |
| `TourController` | CRUD for tours and key points, publishing, archiving |
| `ShoppingCartController` | Cart management, checkout, bonus points |
| `TourReviewController` | Create and list tour reviews |
| `ProblemController` | Report, escalate, and resolve tour problems |
| `SubstituteController` | Guide substitute management |
| `ProfileController` | User profile operations |
| `AdminController` | User blocking, problem review |
| `ImageController` | Image upload and retrieval |

## Development Principles

### TDD (Test-Driven Development)

This project follows a strict TDD workflow:

1. **Red** — Write a failing test that defines the expected behavior
2. **Green** — Write the minimum code to make the test pass
3. **Refactor** — Clean up while keeping tests green

Tests are located in `TourManagement.Tests/` and mirror the main project structure. Run them with:

```bash
dotnet test TourManagement.API/TourManagement.API.sln
```

### DDD (Domain-Driven Design)

- The **Domain layer** is the heart of the application — pure business logic with no framework dependencies
- Business concepts are modeled as **Aggregates**, **Entities**, and **Value Objects**
- **Domain Events** capture meaningful state changes (especially for Problem status tracking)
- **Repository interfaces** are defined in Domain; EF Core implementations live in Infrastructure
- **Ubiquitous language**: class and method names reflect the tour management business domain

### SOLID Principles

- **Single Responsibility** — One reason to change per class
- **Open/Closed** — Extend via new classes, not modifying existing ones
- **Liskov Substitution** — Subtypes are substitutable for their base types
- **Interface Segregation** — Small, focused interfaces
- **Dependency Inversion** — Depend on abstractions (interfaces in Domain, implementations in Infrastructure)

## Useful Commands

```bash
# Build the backend
dotnet build TourManagement.API/TourManagement.API.sln

# Run backend tests
dotnet test TourManagement.API/TourManagement.API.sln

# Add a new EF Core migration
dotnet ef migrations add <MigrationName> --project TourManagement.API/TourManagement.Infrastructure --startup-project TourManagement.API/TourManagement.API

# Apply migrations
dotnet ef database update --project TourManagement.API/TourManagement.Infrastructure --startup-project TourManagement.API/TourManagement.API

# Frontend dev server
cd TourManagement.Client && npx ng serve

# Frontend production build
cd TourManagement.Client && npx ng build
```
