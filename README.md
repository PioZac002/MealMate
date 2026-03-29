# 🍽️ MealMate+

**MealMate+** is a comprehensive household meal planning, smart fridge, shopping list, calorie tracking, and fitness tracking application.

## Tech Stack

| Layer | Technology |
|---|---|
| **Backend API** | ASP.NET Core 8 Web API (C#) |
| **Architecture** | Clean Architecture (Domain / Application / Infrastructure / API) |
| **ORM / Database** | Entity Framework Core 8 + PostgreSQL 16 |
| **Authentication** | ASP.NET Identity + JWT Tokens (access + refresh) |
| **Email** | MailKit (SMTP) for invite codes |
| **Frontend** | Next.js 14 (React) + TypeScript + Tailwind CSS |
| **Containerization** | Docker + Docker Compose |
| **Cache** | Redis 7 |
| **API Docs** | Swagger / OpenAPI |
| **Validation** | FluentValidation |
| **Mapping** | AutoMapper |
| **Tests** | xUnit |

## Project Structure

```
MealMate/
├── docker-compose.yml
├── README.md
├── backend/
│   ├── Dockerfile
│   ├── MealMate.sln
│   ├── src/
│   │   ├── MealMate.API/          # Controllers, Middleware, Program.cs
│   │   ├── MealMate.Application/  # Services, DTOs, Interfaces, Validators
│   │   ├── MealMate.Domain/       # Entities, Enums
│   │   └── MealMate.Infrastructure/ # EF DbContext, Migrations, Email, JWT
│   └── tests/
│       └── MealMate.Tests/
└── frontend/
    ├── Dockerfile
    └── src/
        ├── app/       # Next.js App Router pages
        ├── contexts/  # AuthContext
        └── services/  # API client
```

## How to Run

### With Docker

```bash
docker-compose up --build
# API:      http://localhost:8080
# Frontend: http://localhost:3000
# Swagger:  http://localhost:8080/swagger
```

### Local Development

```bash
# Backend
cd backend/src/MealMate.API
dotnet run

# Frontend
cd frontend
npm install && npm run dev
```

## API Endpoints

### Auth
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login |
| POST | `/api/auth/refresh` | Refresh token |
| POST | `/api/auth/revoke` | Logout |

### Households
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/households` | My households |
| POST | `/api/households` | Create |
| POST | `/api/households/{id}/invite` | Invite by email |
| POST | `/api/households/join` | Join with code |

### Ingredients
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/ingredients` | List with filtering |
| POST | `/api/ingredients` | Create |
| PUT | `/api/ingredients/{id}` | Update |
| DELETE | `/api/ingredients/{id}` | Delete |

### Recipes
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/recipes` | List with filtering |
| GET | `/api/recipes/{id}` | Detail with nutrition |
| POST | `/api/recipes` | Create |
| PUT | `/api/recipes/{id}` | Update |
| DELETE | `/api/recipes/{id}` | Delete |

## Security

- JWT Bearer authentication
- Invite codes verified by email match (security against code theft)
- Password: min 8 chars, uppercase, digit required
- Refresh token rotation with expiry

## Future Roadmap

- **Phase 2**: Smart Fridge + Shopping Lists
- **Phase 3**: Calorie Tracking + Fitness Tracker
- **Phase 4**: Meal Planning with auto shopping lists
- **Phase 5**: Receipt scanning with AI ingredient matching
- **Phase 6**: AI recommendations + analytics
