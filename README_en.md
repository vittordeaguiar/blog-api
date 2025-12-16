# Blog API

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13-239120?logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-7-DC382D?logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)
![License](https://img.shields.io/badge/License-Educational-blue)

**RESTful API for blog management built with .NET 9 and Clean Architecture**

This is a complete API for managing a blog, including posts, categories, and users. The project was developed as part of my professional portfolio to demonstrate skills in .NET 9, Clean Architecture, design patterns, and development best practices.

The application implements advanced features such as distributed caching with Redis, intelligent rate limiting, JWT authentication with role-based authorization, domain validations using FluentValidation, and a comprehensive suite of unit tests. The entire infrastructure is containerized with Docker, allowing for fast and consistent deployment.

## Key Features

- **Clean Architecture** with clear separation into 4 layers (Domain, Application, Infrastructure, API)
- **JWT Authentication** with role-based authorization (Author, Admin)
- **Full CRUD** for Posts and Categories with business validations
- **Many-to-Many Relationship** between Posts and Categories
- **Distributed Caching** with Redis using cache-aside pattern (GetOrSetAsync)
- **Distributed Rate Limiting** by IP and authenticated user
- **FluentValidation** for centralized domain validations
- **Password Hashing** with BCrypt for security
- **Pagination** of results with pagination metadata
- **Global Exception Handling** following RFC 7807 (Problem Details)
- **Entity Framework Core** with PostgreSQL and automatic migrations
- **Repository Pattern** for data access abstraction
- **Unit Tests** with xUnit, Moq, and FluentAssertions (17+ tests)
- **OpenAPI/Swagger Documentation** with Scalar UI interface
- **Docker Compose** with health checks for PostgreSQL, Redis, and API
- **Graceful Degradation** - application works even if Redis is offline

## Technologies Used

### Backend Framework
- .NET 9.0
- ASP.NET Core Web API
- C# 13

### Database & Caching
- PostgreSQL 16 (Alpine)
- Redis 7 (Alpine)
- Entity Framework Core 9.0
- StackExchange.Redis 2.8.16

### Security
- JWT Bearer Authentication
- BCrypt.Net-Next
- Role-based Authorization

### Libraries
- AutoMapper 13.0 (object mapping)
- FluentValidation 11.10 (domain validations)
- xUnit 2.9 (testing framework)
- Moq 4.20 (mocking)
- FluentAssertions 6.12 (expressive assertions)

### DevOps
- Docker & Docker Compose
- Multi-stage Dockerfile for image optimization

### Documentation
- OpenAPI
- Scalar UI

## Architecture

This project follows **Clean Architecture** principles, ensuring separation of concerns, testability, and maintainability. The dependency flow always goes from external layers to internal ones, with the Domain layer at the center having no external dependencies.

```
┌─────────────────────────────────────────────┐
│           API Layer (Presentation)          │
│  Controllers, Middleware, Configuration     │
└────────────────┬────────────────────────────┘
                 │ depends on
                 ▼
┌─────────────────────────────────────────────┐
│        Application Layer (Use Cases)        │
│     Services, DTOs, Mappings, Interfaces    │
└────────────────┬────────────────────────────┘
                 │ depends on
                 ▼
┌─────────────────────────────────────────────┐
│           Domain Layer (Core)               │
│   Entities, Validators, Domain Exceptions   │
│        (No external dependencies)           │
└─────────────────────────────────────────────┘
                 ▲
                 │ depends on
┌────────────────┴────────────────────────────┐
│       Infrastructure Layer (External)       │
│  Repositories, DbContext, Redis, JWT, etc.  │
└─────────────────────────────────────────────┘
```

### Layers

**Domain Layer (Core)**
Contains business entities (Post, Category, User), FluentValidation validators, and domain exceptions. This layer has no external dependencies and represents the pure business rules of the application.

**Application Layer (Use Cases)**
Implements use cases through services (PostService, CategoryService, AuthService). Includes DTOs for data transfer, AutoMapper profiles, and service interfaces. This layer orchestrates data flow between the external and internal layers.

**Infrastructure Layer (External)**
Implements technical details: repositories using Entity Framework Core, DbContext, external services (Redis cache, JWT token service, BCrypt password service), and dependency registration.

**API Layer (Presentation)**
Exposes the application via REST controllers, global error handling middleware, configurations (Program.cs), and presentation-specific services (rate limiters).

## Project Structure

```
blog-api/
├── src/
│   ├── BlogAPI.API/              # Presentation Layer
│   │   ├── Controllers/          # API endpoints
│   │   ├── Configuration/        # Settings, handlers
│   │   └── Services/             # Rate limiters
│   ├── BlogAPI.Application/      # Application Layer
│   │   ├── DTOs/                 # Data transfer objects
│   │   ├── Mappings/             # AutoMapper profiles
│   │   ├── Services/             # Business logic services
│   │   └── Common/               # Cache keys, shared
│   ├── BlogAPI.Domain/           # Domain Layer
│   │   ├── Entities/             # Post, Category, User
│   │   ├── Validators/           # FluentValidation
│   │   ├── Interfaces/           # Repository interfaces
│   │   └── Exceptions/           # Domain exceptions
│   └── BlogAPI.Infrastructure/   # Infrastructure Layer
│       ├── Data/                 # DbContext, Configurations
│       ├── Repositories/         # EF Core repositories
│       └── Services/             # Redis, JWT, BCrypt
├── tests/
│   └── BlogAPI.UnitTests/        # Unit tests (xUnit)
├── docker-compose.yml             # Container orchestration
├── Dockerfile                     # Multi-stage build
```

## How to Run the Project

### Prerequisites

- **Option 1 (Recommended)**: Docker and Docker Compose
- **Option 2**: .NET 9.0 SDK + PostgreSQL 16 + Redis 7

### Option 1: Docker Compose (Recommended)

This is the fastest way to run the project. Docker Compose will spin up all necessary services (PostgreSQL, Redis, and the API) with a single command.

```bash
# Clone the repository
git clone https://github.com/vittordeaguiar/blog-api.git
cd blog-api

# Start all containers (API + PostgreSQL + Redis)
docker-compose up -d

# The API will be available at:
# - API: http://localhost:8080
# - Scalar Documentation: http://localhost:8080/scalar/v1
# - Swagger JSON: http://localhost:8080/swagger/v1/swagger.json

# To view application logs
docker-compose logs -f api

# To stop all containers
docker-compose down
```

### Option 2: Local Execution

If you prefer to run locally without Docker, make sure you have PostgreSQL and Redis installed and running.

```bash
# Configure connection strings in appsettings.Development.json:
# - PostgreSQL: localhost:5432
# - Redis: localhost:6379

# Restore dependencies
dotnet restore

# Run migrations (automatically creates the database)
dotnet ef database update --project src/BlogAPI.Infrastructure --startup-project src/BlogAPI.API

# Run the application
dotnet run --project src/BlogAPI.API

# Access http://localhost:8080
```

## API Endpoints

The API is organized into three main groups of endpoints:

### Authentication (`/v1/auth`)

- `POST /v1/auth/register` - Register new user (role: Author or Admin)
- `POST /v1/auth/login` - Login and obtain JWT token

### Posts (`/api/v1/posts`)

- `GET /api/v1/posts` - List paginated posts (public, 5-minute cache)
- `GET /api/v1/posts/{slug}` - Get post by slug (public, 10-minute cache)
- `POST /api/v1/posts` - Create new post (requires authentication)
- `PUT /api/v1/posts/{id}` - Update post (owner or admin only)
- `DELETE /api/v1/posts/{id}` - Delete post (owner or admin only)

### Categories (`/v1/categories`)

- `GET /v1/categories` - List all categories (public, 60-minute cache)
- `POST /v1/categories` - Create category (admin only)
- `DELETE /v1/categories/{id}` - Delete category (admin only)

To explore all endpoints interactively, access the Scalar documentation at `/scalar/v1` after starting the application.

## Authentication and Authorization

The API uses JWT (JSON Web Tokens) for stateless authentication. Here is a complete flow of how to use it:

### 1. Register a new user

```bash
curl -X POST http://localhost:8080/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "password": "SecurePassword123!",
    "role": "Author"
  }'
```

### 2. Login and get the token

```bash
curl -X POST http://localhost:8080/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePassword123!"
  }'
```

Response: `{"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."}`

### 3. Use the token in authenticated requests

```bash
curl -X POST http://localhost:8080/api/v1/posts \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "title": "My First Post",
    "content": "Post content here...",
    "categoryIds": ["category-uuid"]
  }'
```

### Available Roles

- **Author**: Can create posts and edit/delete their own posts
- **Admin**: Can manage categories and edit/delete any post

## Cache and Performance

### Caching Strategy with Redis

The application implements distributed caching using Redis with different TTLs (Time To Live) based on data type:

- **Categories**: 60 minutes - categories change rarely and have high reuse rate
- **Posts (paginated list)**: 5 minutes - balances data freshness vs performance
- **Posts (individual detail)**: 10 minutes - specific posts change less than lists

The cache uses the **cache-aside pattern** via the `GetOrSetAsync` method, which checks the cache first and, in case of a miss, queries the database and stores the result automatically.

**Automatic cache invalidation** occurs on all write operations (create, update, delete), ensuring cached data is always consistent with the database.

### Distributed Rate Limiting

Rate limiting is implemented using Redis Sorted Sets with the sliding window algorithm, allowing multiple API instances to share the same counters.

**Configured Limits:**

- **Per IP**: 1000 requests/minute (dev environment), 100 requests/minute (production)
- **Per Authenticated User**: 2000 requests/minute (dev environment), 200 requests/minute (production)

When a limit is exceeded, the API returns HTTP 429 (Too Many Requests) with the `Retry-After` header indicating when the client can try again.

The implementation uses **graceful degradation**: if Redis is offline, requests are allowed (fail-open) to ensure the API continues to function.

## Tests

The project has a comprehensive suite of unit tests covering the Domain, Application, and Infrastructure layers.

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage details
dotnet test /p:CollectCoverage=true
```

### Test Coverage

**Domain Layer**
Tests for domain entities, including post publish/unpublish behaviors and category management.

**Application Layer**
Over 17 tests in PostService covering:
- Post creation (validations, valid/invalid author)
- Paginated listing (valid/invalid parameters)
- Search by ID and slug
- Update with authorization check (owner/admin)
- Deletion with authorization check
- Post publish/unpublish
- Linking posts with categories

**Infrastructure Layer**
Tests for BCryptPasswordService verifying password hashing and verification.

### Test Frameworks

- **xUnit**: Testing framework with support for theory and fixtures
- **Moq**: Mocking dependencies (repositories, services) for isolated tests
- **FluentAssertions**: Expressive and readable assertions that facilitate test understanding

## Configuration

### Environment Variables (Docker Compose)

```yaml
# PostgreSQL
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=blogapi_dev;Username=postgres;Password=postgres

# JWT
JwtSettings__SecretKey=your-secret-key-here

# Redis
RedisSettings__ConnectionString=blogapi-redis:6379
RedisSettings__Password=redis_dev_password
RedisSettings__Enabled=true

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_HTTP_PORTS=8080
```

### Configuration Files

**appsettings.json**
Configurations for production environment with secure default values.

**appsettings.Development.json**
Specific configurations for development:
- CORS enabled for localhost (ports 3000, 3001, 4200, 5173, 8080)
- Detailed Entity Framework Core logging
- More permissive rate limits to facilitate testing
- Connection settings for local environment

## Project Progress

This project is **90% complete**, with 9 out of 10 main issues finished:

- Clean Architecture Structure
- Entity Framework Core with PostgreSQL
- Error handling middleware and Swagger/Scalar documentation
- User domain with BCrypt encryption
- JWT Authentication with roles
- Categories and Posts CRUD
- Business rules (publish/unpublish, many-to-many)
- API Controllers
- Comprehensive Unit Tests
- Dockerization (finishing up)

To track detailed progress, see [ROADMAP.md](./ROADMAP.md) and the [GitHub Project](https://github.com/vittordeaguiar/blog-api/projects).

## Learnings and Technical Decisions

### Clean Architecture

I chose to implement Clean Architecture to ensure clear separation of concerns and facilitate testing. The Domain layer has no external dependencies, allowing business rules to be tested in complete isolation. This architecture also facilitated the substitution of implementations - for example, the NullCacheService serves as an automatic fallback when Redis is offline, without affecting any other part of the code.

### Redis for Cache and Rate Limiting

Implementing distributed caching with Redis was fundamental to allow horizontal scalability of the API. Multiple instances can share the same cache and rate limiting counters, something impossible with traditional in-memory cache. The decision to implement graceful degradation means the API continues to work even if Redis fails - it simply operates without cache until Redis returns.

### FluentValidation in Domain Layer

Centralizing business validations in entities ensures that rules are respected regardless of where the entity is instantiated. Validators are reusable, testable, and keep the code clean by separating validation logic from business logic.

### Repository Pattern

Abstracting data access through the Repository Pattern greatly facilitates testing (we can mock repositories) and gives us flexibility to change the ORM or add other data sources without affecting the upper layers of the application.

### JWT Bearer Authentication

I opted for JWT to implement stateless authentication, eliminating the need for server-side sessions. JWT tokens carry claims (user ID, role) and are validated on every request. This approach is ideal for RESTful APIs and distributed architectures, allowing any API instance to validate tokens without consulting a centralized session store.

### Cache-Aside Pattern (GetOrSetAsync)

The cache-aside pattern implemented via the `GetOrSetAsync` method significantly simplifies the code. Instead of repeating the "check cache, if not present query DB, store in cache" logic in every method, we encapsulate this in a single reusable method.

### Distributed Rate Limiting

I implemented rate limiting using Redis Sorted Sets with the sliding window algorithm. This approach ensures limits are respected accurately even with multiple API instances running simultaneously, which is critical for scalable production environments.

## Contact

GitHub: [vittordeaguiar](https://github.com/vittordeaguiar)

## License

This project was developed for educational purposes and as part of my professional portfolio. Feel free to explore the code and use it as a reference for your own projects.
