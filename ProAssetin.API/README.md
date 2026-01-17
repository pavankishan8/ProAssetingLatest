# ProAssetin API

Multi-tenant Asset Management System API built with .NET 8

## Features

- **Multi-tenancy**: Database isolation based on email domain
- **JWT Authentication**: Secure token-based authentication
- **ASP.NET Core Identity**: User and role management
- **Entity Framework Core**: Database access with dynamic connection strings
- **Swagger/OpenAPI**: API documentation
- **Serilog**: Structured logging

## Prerequisites

- .NET 8 SDK
- SQL Server (Express or full version)
- Visual Studio 2022 or VS Code

## Setup

1. **Update Connection String**

   Edit `appsettings.json` and update the connection string:
   ```json
   "ConnectionStrings": {
     "MasterConnection": "Server=localhost\\SQLEXPRESS;Database=ProAssetinDev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true",
    "TenantConnectionTemplate": "Server=localhost\\SQLEXPRESS;Database={DatabaseName};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```

2. **Update JWT Secret Key**

   For production, change the JWT secret key in `appsettings.json`:
   ```json
   "JwtSettings": {
     "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!"
   }
   ```

3. **Run Migrations**

   ```bash
   dotnet ef database update
   ```

   If migrations don't exist yet:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Run the API**

   ```bash
   dotnet run
   ```

   The API will be available at:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/swagger`

## Multi-Tenancy

The system automatically resolves tenant database from user email domain:

- `user@infosys.com` → Database: `ProAsset_Infosys`
- `user@wipro.com` → Database: `ProAsset_Wipro`

Tenant databases are created automatically on first use.

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/me` - Get current user (requires authentication)

### Assets
- `GET /api/assets` - Get assets (with pagination and filtering)
- `GET /api/assets/{id}` - Get asset by ID
- `POST /api/assets` - Create asset
- `PUT /api/assets/{id}` - Update asset
- `DELETE /api/assets/{id}` - Delete asset
- `GET /api/assets/categories` - Get asset categories
- `GET /api/assets/statuses` - Get asset statuses

### Dashboard
- `GET /api/dashboard` - Get dashboard data

### Reports
- `GET /api/reports/summary` - Get asset summary
- `GET /api/reports/category-stats` - Get statistics by category
- `GET /api/reports/status-stats` - Get statistics by status

## Testing

Use Swagger UI to test endpoints. First, register a user or login to get a JWT token, then use the "Authorize" button in Swagger to set the token.

## License

Proprietary

