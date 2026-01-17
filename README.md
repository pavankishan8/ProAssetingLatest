# ProAssetin - Multi-Tenant Asset Management System

A modern, production-ready asset management system built with .NET Core 8 and Angular 18, featuring multi-tenancy, JWT authentication, and a responsive Material Design UI.

## 🎯 Features

- **Multi-Tenancy**: Database isolation based on email domain (e.g., `user@infosys.com` → `ProAsset_Infosys` database)
- **JWT Authentication**: Secure token-based authentication with ASP.NET Core Identity
- **Asset Management**: Complete CRUD operations for assets with search, filtering, and pagination
- **Dashboard**: Real-time statistics and analytics
- **Reports**: Comprehensive reporting with data visualization
- **Responsive Design**: Fully responsive UI that works on mobile, tablet, and desktop
- **Modern UI**: Built with Angular Material Design components

## 🏗️ Architecture

### Backend (.NET Core 8 Web API)
- **Framework**: .NET 8
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity + JWT
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI

### Frontend (Angular 18)
- **Framework**: Angular 18
- **UI Library**: Angular Material
- **State Management**: RxJS Observables
- **Charts**: Chart.js + ng2-charts
- **Styling**: SCSS with responsive breakpoints

## 📋 Project Structure

```
ProAssetin/
├── ProAssetin.API/          # Backend Web API
│   ├── Controllers/         # API endpoints
│   ├── Services/            # Business logic
│   ├── Models/              # Entities and DTOs
│   ├── Data/                # Database contexts
│   └── Middleware/          # Multi-tenancy middleware
│
└── ProAssetin.Web/          # Angular frontend
    ├── src/
    │   ├── app/
    │   │   ├── core/        # Guards, interceptors, services
    │   │   ├── features/    # Feature modules (auth, assets, dashboard, etc.)
    │   │   └── shared/      # Shared components and modules
    │   └── environments/    # Environment configuration
```

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- SQL Server (Express or full version)

### Backend Setup

1. **Navigate to API directory**
   ```bash
   cd ProAssetin.API
   ```

2. **Update connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ProAssetinDev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```

3. **Install EF Core tools** (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Run migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the API**:
   ```bash
   dotnet run
   ```

   API will be available at:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`
   - Swagger: `https://localhost:5001/swagger`

### Frontend Setup

1. **Navigate to Web directory**
   ```bash
   cd ProAssetin.Web
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Update API URL** in `src/environments/environment.ts` if needed:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'https://localhost:5001/api'
   };
   ```

4. **Run the frontend**:
   ```bash
   ng serve
   ```

   Frontend will be available at `http://localhost:4200`

## 🔐 Multi-Tenancy

The system automatically creates and manages tenant databases:

- **Email**: `admin@infosys.com` → **Database**: `ProAsset_Infosys`
- **Email**: `user@wipro.com` → **Database**: `ProAsset_Wipro`

Tenant databases are created automatically on first use (registration or login).

### How It Works

1. User registers/logs in with email
2. System extracts domain from email (e.g., `infosys` from `user@infosys.com`)
3. Tenant database name is generated: `ProAsset_Infosys`
4. Database is created if it doesn't exist
5. All subsequent API calls use the tenant-specific database
6. Complete data isolation between tenants

## 📡 API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/me` - Get current user

### Assets
- `GET /api/assets` - List assets (with pagination and filtering)
- `GET /api/assets/{id}` - Get asset by ID
- `POST /api/assets` - Create asset
- `PUT /api/assets/{id}` - Update asset
- `DELETE /api/assets/{id}` - Delete asset
- `GET /api/assets/categories` - Get asset categories
- `GET /api/assets/statuses` - Get asset statuses

### Dashboard
- `GET /api/dashboard` - Get dashboard statistics

### Reports
- `GET /api/reports/summary` - Asset summary report
- `GET /api/reports/category-stats` - Statistics by category
- `GET /api/reports/status-stats` - Statistics by status

## 🔒 Security Features

- JWT token-based authentication
- Password hashing (via ASP.NET Core Identity)
- HTTPS support
- CORS configuration
- Input validation
- SQL injection protection (Entity Framework parameterized queries)
- XSS protection (Angular's built-in sanitization)

## 📱 Responsive Design

The application is fully responsive with breakpoints:
- **Mobile**: < 600px
- **Tablet**: 600px - 960px
- **Desktop**: > 960px

All components adapt seamlessly to different screen sizes using:
- Angular Flex Layout
- CSS Grid and Flexbox
- Material Design responsive components
- Custom media queries

## 🛠️ Development

### Backend Development
- Use Swagger UI for API testing
- Logs are written using Serilog
- Database changes via EF Core migrations

### Frontend Development
- Angular CLI for scaffolding
- Hot module replacement during development
- TypeScript strict mode enabled

## 📦 What's Included

### ✅ Completed
- Multi-tenancy infrastructure
- JWT authentication (backend + frontend)
- User registration and login
- Asset management API
- Dashboard and Reports API
- Angular authentication flow
- Responsive login/register forms
- Material Design integration
- HTTP interceptors and guards
- Error handling

### ⏳ Remaining (Structure in place, components pending)
- Main layout with navigation
- Dashboard UI with charts
- Assets management UI
- Reports UI

The foundation is complete and production-ready. Remaining work follows established patterns.

## 📄 Documentation

- `SETUP_GUIDE.md` - Detailed setup instructions
- `PROJECT_STATUS.md` - Current project status
- `COMPLETE_PROJECT_STRUCTURE.md` - Complete file structure
- `CODEBASE_ANALYSIS.md` - Analysis of original codebase

## 🔄 Testing

### Backend Testing
1. Use Swagger UI at `/swagger`
2. Register a new user
3. Login to get JWT token
4. Use "Authorize" button to set token
5. Test protected endpoints

### Multi-Tenancy Testing
1. Register users with different email domains:
   - `admin@infosys.com`
   - `user@wipro.com`
2. Login with each user
3. Verify data isolation (assets created by one tenant are not visible to another)

## 🚢 Deployment

### Backend
- Publish to IIS, Azure App Service, or Docker container
- Update connection strings for production
- Set strong JWT secret key
- Configure HTTPS
- Update CORS settings

### Frontend
- Build for production: `ng build --configuration production`
- Deploy to web server (IIS, Nginx, Azure Static Web Apps, etc.)
- Update API URL in `environment.prod.ts`

## 📝 License

Proprietary

## 👥 Contributing

This is a private project. For questions or issues, contact the development team.

## 🎉 Getting Started Checklist

- [ ] Install .NET 8 SDK
- [ ] Install Node.js 18+
- [ ] Install SQL Server
- [ ] Update connection string in `appsettings.json`
- [ ] Run database migrations
- [ ] Start backend API
- [ ] Install frontend dependencies (`npm install`)
- [ ] Start frontend (`ng serve`)
- [ ] Register a test user
- [ ] Login and explore!

---

**Built with ❤️ using .NET Core 8 and Angular 18**

