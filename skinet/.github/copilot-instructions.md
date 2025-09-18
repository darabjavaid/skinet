# Skinet E-commerce Application AI Instructions

## Project Architecture

### Backend (.NET Core)
- **Clean Architecture** with three projects:
  - `API`: Web API controllers and DTOs
  - `Core`: Domain entities and interfaces
  - `Infrastructure`: Data access and implementations

#### Key Patterns
1. **Repository Pattern**
   - Generic repository (`IGenericRepository<T>`) for CRUD operations
   - Base entity pattern (`BaseEntity`) for common properties
   - Example: `Infrastructure/Data/GenericRepository.cs`

2. **Specification Pattern**
   - Used for querying with filters and includes
   - Base spec in `Core/Specifications/BaseSpecification.cs`
   - Allows composable query criteria

3. **Entity Configuration**
   - EF Core configurations in `Infrastructure/Config`
   - Example: `ProductConfiguration.cs` for decimal precision

### Frontend (Angular)
- Standalone components architecture
- Uses Angular Material and TailwindCSS
- Global error handling via `provideBrowserGlobalErrorListeners`

## Development Workflow

### Backend Commands
```bash
# Database Operations
dotnet ef migrations add [Name] -s API -p Infrastructure
dotnet ef database update -s API -p Infrastructure
dotnet ef database drop -p Infrastructure -s API

# Running the API
dotnet run --project API/

# Docker for Database
docker compose up -d   # Start SQL Server
docker compose down   # Stop containers
```

### Frontend Commands
```bash
ng serve -o           # Start dev server
npm run build        # Production build
```

## Key Integration Points

1. **API-Database**
   - Connection string in `API/appsettings.json`
   - Auto-migration in `Program.cs`
   - Seed data in `Infrastructure/Data/StoreContextSeed.cs`

2. **API-Frontend**
   - CORS configured for `localhost:4200`
   - Global exception handling via `ExceptionMiddleware`

## Project Conventions

1. **Entity Structure**
   - All entities inherit from `BaseEntity`
   - Required fields marked with `required` keyword
   - Price properties use decimal(18,2)

2. **API Controllers**
   - Inherit from `BaseApiController`
   - Use `[ApiController]` and `[Route("api/[controller]")]`
   - Return paginated results using `CreatePagedResult<T>`

3. **Angular Components**
   - Use standalone components
   - Implement zone-based change detection
   - Follow Angular Material design patterns

## Common Tasks

1. **Adding New Entity**
   - Create entity class in `Core/Entities`
   - Add DbSet to `StoreContext`
   - Create configuration in `Infrastructure/Config`
   - Add migration and update database

2. **Adding New API Endpoint**
   - Add controller action in `API/Controllers`
   - Implement repository method if needed
   - Add specification if filtering required

3. **Error Handling**
   - Use `BuggyController` patterns for error responses
   - Client-side errors handled via global error listeners
