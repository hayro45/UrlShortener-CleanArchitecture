# URL Shortener API

A clean architecture URL shortening service built with .NET 8, demonstrating enterprise-level design patterns and best practices.

## 🏗️ Architecture

This project follows **Onion Architecture (Clean Architecture)** with strict layer separation:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│                 (ASP.NET Core Web API)                      │
├─────────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                      │
│              (EF Core, PostgreSQL, Repositories)            │
├─────────────────────────────────────────────────────────────┤
│                    Application Layer                        │
│       (CQRS Handlers, Validators, DTOs, Use Cases)          │
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer                           │
│            (Entities, Interfaces, Value Objects)            │
└─────────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

| Layer | Description | Dependencies |
|-------|-------------|--------------|
| **Domain** | Business entities and repository interfaces | None (Core) |
| **Application** | Use cases, CQRS commands/queries, validation | Domain |
| **Infrastructure** | Data access, external services | Domain, Application |
| **Presentation** | API controllers, configuration | All layers |

## 🛠️ Tech Stack

- **.NET 8** (LTS)
- **ASP.NET Core Web API**
- **Entity Framework Core** + **PostgreSQL**
- **MediatR** (CQRS pattern)
- **FluentValidation**
- **Docker & Docker Compose**
- **xUnit + Moq** (Testing)

## ✨ Production-Ready Features

### Error Handling
- **Global Exception Middleware** - Centralized error handling with ProblemDetails format
- **Custom Exception Types** - Domain-specific exceptions (NotFoundException, ConflictException)
- **Environment-Aware Responses** - Stack traces only in Development mode

### Logging
- **Serilog Integration** - Structured logging with console and file sinks
- **Request/Response Logging** - Automatic HTTP request logging with timing
- **Rolling File Logs** - Daily log files with 7-day retention
- **Enriched Context** - Correlation IDs and diagnostic context

### Health Checks
- **Database Health Check** - PostgreSQL connection monitoring
- **Multiple Endpoints** - `/health`, `/health/ready`, `/health/live`
- **JSON Response Format** - Detailed health status with timing metrics
- **Kubernetes-Ready** - Liveness and readiness probes

### CORS
- **Flexible Configuration** - Allow all origins for development
- **Production-Ready Comments** - Guidance for specific origin configuration
- **Frontend Integration** - Ready for React, Angular, Vue.js applications

### Rate Limiting
- **Global Rate Limiter** - 10 requests/minute per client
- **Fixed Window Algorithm** - Simple and effective rate limiting
- **429 Response** - Proper "Too Many Requests" error messages
- **DDoS Protection** - Prevents abuse and ensures fair usage

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Running with Docker Compose

```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

The API will be available at: `http://localhost:5000`

### Running Locally

```bash
# Start PostgreSQL (using Docker)
docker run -d --name postgres -p 5432:5432 \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=urlshortener_dev \
  postgres:16-alpine

# Run the API
cd src/UrlShortener.Api
dotnet run
```

## 📡 API Endpoints

### Create Short URL

```http
POST /api/urls
Content-Type: application/json

{
  "originalUrl": "https://www.example.com/very/long/path"
}
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "originalUrl": "https://www.example.com/very/long/path",
  "shortCode": "abc123X",
  "createdAt": "2024-01-19T12:00:00Z"
}
```

### Get Original URL

```http
GET /api/urls/{shortCode}
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "originalUrl": "https://www.example.com/very/long/path",
  "shortCode": "abc123X",
  "createdAt": "2024-01-19T12:00:00Z"
}
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📁 Project Structure

```
UrlShortener/
├── src/
│   ├── UrlShortener.Domain/          # Entities, Interfaces
│   ├── UrlShortener.Application/     # CQRS, Validators, DTOs
│   ├── UrlShortener.Infrastructure/  # EF Core, Repositories
│   └── UrlShortener.Api/             # Controllers, Startup
├── tests/
│   └── UrlShortener.Application.Tests/
├── docker-compose.yml
├── Dockerfile
└── UrlShortener.sln
```

## 🎯 Design Principles

- **SOLID Principles**
- **Clean Code**
- **DRY (Don't Repeat Yourself)**
- **Separation of Concerns**
- **Dependency Inversion**

## 📝 License

This project is licensed under the MIT License.
