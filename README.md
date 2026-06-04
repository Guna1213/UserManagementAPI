# User Management API

A RESTful API built with **ASP.NET Core (.NET 8)** for managing users. Includes CRUD endpoints, input validation, logging middleware, and API key authentication middleware.

---

## Features

| Feature | Details |
|---|---|
| CRUD Endpoints | GET, POST, PUT, DELETE for users |
| Input Validation | Data annotations: Required, Email, Range, Regex |
| Logging Middleware | Logs every request and response with timing |
| Auth Middleware | API key validation via `X-API-Key` header |
| Swagger UI | Interactive API docs at `/swagger` |

---

## Project Structure

```
UserManagementAPI/
├── Controllers/
│   └── UsersController.cs     # CRUD endpoints
├── Middleware/
│   ├── LoggingMiddleware.cs   # Request/response logging
│   └── AuthMiddleware.cs      # API key authentication
├── Models/
│   └── User.cs                # User model with validation
├── Program.cs                 # App setup + middleware pipeline
└── UserManagementAPI.csproj
```

---

## Running Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Steps

```bash
git clone https://github.com/YOUR_USERNAME/UserManagementAPI.git
cd UserManagementAPI
dotnet run
```

Open browser: `http://localhost:5000/swagger`

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create new user |
| PUT | `/api/users/{id}` | Update existing user |
| DELETE | `/api/users/{id}` | Delete user |
| GET | `/health` | Health check |

---

## Authentication

All API endpoints (except `/swagger` and `/health`) require an API key header:

```
X-API-Key: my-secret-api-key-123
```

---

## Example Request

### Create a User (POST)

```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -H "X-API-Key: my-secret-api-key-123" \
  -d '{
    "name": "Guna D",
    "email": "guna@example.com",
    "age": 21,
    "role": "User"
  }'
```

### Response
```json
{
  "id": 4,
  "name": "Guna D",
  "email": "guna@example.com",
  "age": 21,
  "role": "User"
}
```

---

## Validation Rules

| Field | Rules |
|-------|-------|
| Name | Required, 2–100 characters |
| Email | Required, valid email format, unique |
| Age | Required, 1–120 |
| Role | Required, must be Admin / User / Manager |

---

## Middleware Pipeline

```
Request → LoggingMiddleware → AuthMiddleware → Controller → Response
```

1. **LoggingMiddleware** — logs method, path, status code, and duration
2. **AuthMiddleware** — checks `X-API-Key` header, returns 401 if missing/invalid

---

## Built With

- ASP.NET Core (.NET 8)
- Swashbuckle (Swagger)
- GitHub Copilot (for debugging and enhancements)
