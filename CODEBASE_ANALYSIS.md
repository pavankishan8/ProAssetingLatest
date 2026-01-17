# ProAssetin - Comprehensive Codebase Analysis

## 📋 Project Overview

**ProAssetin** is a comprehensive Enterprise Asset Management (EAM) system built with Angular 15 frontend and ASP.NET Web API (C#) backend. The application manages IT assets throughout their lifecycle, including creation, allocation, tracking, maintenance, repair, disposal, and reporting.

---

## 🏗️ Architecture & Technology Stack

### Frontend (Angular Application)
- **Framework**: Angular 15.2.7
- **Language**: TypeScript 4.9.4
- **Styling**: SCSS, Bootstrap 5.3.2, Angular Material 15.2.9
- **State Management**: Services with RxJS (BehaviorSubject, Subject)
- **HTTP Client**: Angular HttpClient
- **Build Tool**: Angular CLI 15.2.7

### Backend (API)
- **Framework**: ASP.NET Web API 2 (Target Framework: .NET 4.6.1)
- **Language**: C# 
- **Database**: SQL Server (Connection: `AL230377-BLR\SQLEXPRESS`)
- **Architecture Pattern**: RESTful API with Controller-Service-Repository pattern
- **CORS**: Enabled for all origins (`*`)

### Key Libraries & Dependencies

#### Frontend Dependencies:
- **UI Libraries**: 
  - Angular Material (Components, Forms, Dialog, Stepper, Pagination)
  - Bootstrap 5.3.2
  - CoreUI Angular 4.5.25
  - PrimeNG 17.12.0
  - FontAwesome 6.4.2
- **Charts**: Chart.js 4.4.2, ng2-charts 6.0.0
- **Notifications**: angular2-notifications, ngx-toastr 17.0.2
- **Utilities**: 
  - jsPDF 2.5.1 (PDF generation)
  - html2canvas 1.4.1
  - File-saver 2.0.5
  - Signature Pad 4.0.2
- **Pagination**: ngx-pagination 6.0.3

#### Backend Dependencies:
- **EPPlus 6.2.10** - Excel file processing
- **MailKit 4.2.0** - Email functionality
- **Newtonsoft.Json 13.0.1** - JSON serialization
- **Microsoft.AspNet.WebApi 5.3.0** - Web API framework

---

## 📁 Project Structure

```
ProAssetin/
├── Mine/
│   └── Proassetin/              # Angular Frontend
│       ├── src/
│       │   └── app/
│       │       ├── Assets/      # Asset management modules
│       │       ├── Components/  # UI components
│       │       ├── Purchase/    # Purchase order modules
│       │       ├── Settings/    # User/system settings
│       │       ├── Services/    # Business logic services
│       │       └── Environments/# Environment configuration
│       └── dist/                # Production build
│
└── Mine API/
    └── ProAssetin-API/
        └── Test/                # ASP.NET Web API Backend
            ├── Controllers/     # API endpoints
            ├── Models/          # Data models
            ├── Utils/           # Utility classes
            └── packages/        # NuGet packages
```

---

## 🎯 Core Features & Modules

### 1. **Authentication & Authorization**
- Email/password-based login
- Session management (20-minute timeout)
- Role-based access control
- AuthGuard for route protection
- Session expiration handling with activity listeners

**Components**: `LoginComponent`, `AuthGuard`, `SessionService`

### 2. **Asset Management**
Core asset lifecycle management with multiple statuses:

- **In-Stock**: Available assets ready for allocation
- **Allocated**: Assets assigned to employees
- **Repair**: Assets sent for repair/maintenance
- **Sold**: Assets sold/disposed
- **Damaged**: Assets marked as damaged
- **E-Waste**: Assets scheduled for electronic waste disposal

**Features**:
- Manual asset creation
- Bulk import from Excel (XLSX)
- Asset allocation to employees
- Asset search by ID or Employee ID
- Asset status tracking and updates
- Custom asset ID prefixes per user
- Pre-defined asset ID configuration

**Components**: 
- `AssetsPageComponent`, `AssetsCreateComponent`, `AssetsSearchComponent`, `AssetsReportsComponent`
- `AssetMDashboardComponent`

### 3. **Employee Management**
- Employee registration with company details
- Employee profile management
- Role assignment and permissions
- Employee search and details retrieval
- Image upload for employee profiles
- Domain account integration

**API Endpoints**: `/api/Register`, `/api/GetEmployeeDetails`, `/api/getAllEmployees`

### 4. **Ticket Management System**
- Create, update, and track tickets
- Ticket status management (Pending, Closed)
- Employee-based ticket assignment
- Ticket dashboard with counts and graphs
- Task tracking with priorities

**Components**: 
- `CreateTicketComponent`, `PendingTicketsComponent`, `ClosedTicketsComponent`
- `TicketToolDashboardComponent`

### 5. **Purchase Management**
- Purchase order creation (`PurCreateComponent`)
- Purchase order management (`POComponent`)
- Vendor management integration

### 6. **Dashboard Modules**
Multiple specialized dashboards:
- **Asset Management Dashboard** (`AssetMDashboardComponent`)
- **Invoice Management** (`InvoiceMDashboardComponent`)
- **Purchase Management** (`PurchaseMDashboardComponent`)
- **Budget Management** (`BudgetMDashboardComponent`)
- **Vendor Management** (`VendorMDashboardComponent`)
- **E-Waste Management** (`EWasteMDashboardComponent`)
- **Security Management** (`SecurityMDashboardComponent`)
- **VAPT (Vulnerability Assessment & Penetration Testing)** (`VaptMDashboardComponent`)
- **Project Management** (`ProjectMDashboardComponent`)
- **SAML Management** (`SamlMDashboardComponent`)

### 7. **Settings & Configuration**
Comprehensive settings module with nested routing:

- **User Settings** (`UserSettingsComponent`)
  - Currency & Timezone configuration
  - User management page
  - Account creation
  - Account permissions
  - Account roles
  - SMTP configuration
  - Pre-defined ID configuration

**Components**: `UserSettingsComponent`, `CurrencyTimeZoneComponent`, `UserSPageComponent`, 
`AccountCreationComponent`, `AccountPermissionsComponent`, `AccountRolesComponent`, 
`SMTPConfigComponent`, `PreDefinedIDComponent`

### 8. **Reports & Analytics**
- Asset reports (`AssetsReportsComponent`)
- Inventory reports
- Asset counts and statistics
- Ticket reports and graphs

### 9. **Inventory Scanning**
- Scan inventory functionality (`ScanInventoryComponent`)
- Inventory data retrieval by type

---

## 🔌 API Architecture

### API Base URL
- **Development**: `http://localhost:53417/`
- **CORS**: Enabled for all origins, headers, and methods

### Controllers

#### 1. **LoginController** (`/api/login`)
- `POST /api/login` - User authentication

#### 2. **AssetsController** 
- `POST /api/assets/add` - Add asset manually
- `POST /api/assets/import` - Import assets from Excel
- `GET /api/inventory/{type}` - Get inventory by type
- `GET /api/asset/search?assetId={id}` - Search asset by ID
- `POST /api/allocate-asset` - Allocate asset to employee
- `GET /api/asset-counts` - Get asset statistics
- `GET /api/searchByEmpID?employeeId={id}` - Get assets by employee ID

#### 3. **TicketsToolController**
- `GET /api/getTicketsCount` - Get ticket summary
- `GET /api/getTicketsData?employeeId={id}` - Get tickets by employee
- `POST /api/AddTicket` - Create ticket
- `POST /api/UpdateTicket?taskId={id}` - Update ticket
- `GET /api/getAllTickets` - Get all tickets
- `GET /api/TaskAssignedToNameCounts` - Get ticket statistics

#### 4. **UserSettingsController**
- Employee management endpoints
- Configuration management
- SMTP settings

#### 5. **PurchaseController**
- Purchase order management endpoints

---

## 🗄️ Database Schema (Inferred)

### Key Tables:

1. **Employees**
   - EmployeeID, DomainAccount, EmployeeType, Username
   - FirstName, LastName, Email, Password
   - RegisterDate, LastLoginDate, Role
   - PhoneNumber, Location, ProjectName, Team
   - CustomerName, WorkType, ReportingManager
   - CompanyID

2. **Company**
   - CompanyID, CompanyName, Address
   - PhoneNumber, Industry

3. **Assets** (Main asset table)
   - AssetID, AssetType, Make, Model, Status
   - RAM, Processor, OS, Serial_Number, IMEI_Number
   - Purchase_Cost, Purchase_Year, MonthsInUse
   - NextRecycleDate, AssignedToUserID

4. **InStock**
   - Same structure as Assets (for in-stock items)

5. **Allocated**
   - AssetID, AssetType, Make, Processor, RAM, Status
   - Serial_Number, AssignedToUserID
   - Email, Username, PhoneNumber, Location, ReportingManager

6. **Repair** (Repair items table)
   - AssetID, Vendor, SentDate, ReceiveDate
   - Repair_Cost, RepairStatus, Tracking
   - RepairNotes, DeliveredBy

7. **Sold** (Sold items table)
   - AssetID, Sold_Cost, Sold_Year
   - SoldNotes, SoldTo, Approvals

8. **Damaged** (Damaged items table)
   - AssetID, DamagedNotes

9. **EWaste** (E-waste items table)
   - AssetID, EWaste_Vendor, EWasteNotes, EWasteApprovals

10. **TicketsTable**
    - TaskID, TaskTitle, TaskState, Priority
    - TaskAssignedToID, TaskAssignedToName

11. **Configuration** (Employee configuration)
    - ConfigurationID, PreDefinedAssetID, GSTNumber, Image

---

## 🔒 Security Features

### Authentication
- Session-based authentication using `sessionStorage`
- Session timeout: 20 minutes of inactivity
- Activity listeners (mouse, keyboard, touch, scroll) reset timer
- AuthGuard protects routes

### Security Concerns Identified:
⚠️ **CRITICAL**: Passwords stored in **plain text** in database
⚠️ No HTTPS enforcement mentioned
⚠️ CORS enabled for all origins (`*`) - security risk in production

### Recommendations:
- Implement password hashing (bcrypt, Argon2)
- Enforce HTTPS in production
- Restrict CORS to specific domains
- Add JWT tokens for stateless authentication
- Implement CSRF protection

---

## 🔄 Data Flow & Services

### Frontend Services

1. **ApiService** (`api.service.ts`)
   - Centralized HTTP service for all API calls
   - Handles asset, employee, ticket operations

2. **AuthService** (`auth.service.ts`)
   - Login authentication
   - User session management

3. **SessionService** (`session.service.ts`)
   - Session timeout management
   - Activity monitoring
   - Session expiration events

4. **SharedDataService** (`shared-data.service.ts`)
   - Shared state management using RxJS
   - Selected value tracking
   - Card click events

5. **NotificationService** (`notification.service.ts`)
   - Success, warning, error, and info notifications
   - Configurable toast notifications

---

## 🎨 UI/UX Architecture

### Layout Structure
- **Header Component**: Top navigation bar
- **Side Navigation Bar**: Left sidebar menu
- **Footer Component**: Page footer
- **Home Component**: Main container with router outlet

### Routing Structure
```
/Login → LoginComponent (Public)
/Home → HomeComponent (Protected)
  ├── Home-Page → HomePageComponent
  ├── Create → CreatePageComponent
  ├── View → ViewPageComponent
  ├── Delete → DeletePageComponent
  ├── Reports → ReportsPageComponent
  ├── ScanInv → ScanInventoryComponent
  ├── CreateTick → CreateTicketComponent
  ├── ClosedTick → ClosedTicketsComponent
  ├── PendingTick → PendingTicketsComponent
  ├── AssetMDash → AssetMDashboardComponent
  ├── InvoiceMDash → InvoiceMDashboardComponent
  ├── PurchaseMDash → PurchaseMDashboardComponent
  ├── samlMDash → SamlMDashboardComponent
  ├── BudgetMDash → BudgetMDashboardComponent
  ├── VendorMDash → VendorMDashboardComponent
  ├── EwasteMDash → EWasteMDashboardComponent
  ├── SecurityMDash → SecurityMDashboardComponent
  ├── vaptMDash → VaptMDashboardComponent
  ├── ProjectMDash → ProjectMDashboardComponent
  ├── TTMDash → TicketToolDashboardComponent
  ├── AssetPage → AssetsPageComponent
  ├── AssetCreate → AssetsCreateComponent
  ├── AssetSearch → AssetsSearchComponent
  ├── AssetReports → AssetsReportsComponent
  ├── PurCreate → PurCreateComponent
  └── PurO → POComponent

/UserSettings → UserSettingsComponent (Protected)
  ├── CurTime → CurrencyTimeZoneComponent
  ├── UserS → UserSPageComponent
  ├── AccCreation → AccountCreationComponent
  ├── AccPermissions → AccountPermissionsComponent
  ├── AccRoles → AccountRolesComponent
  ├── SMTPCon → SMTPConfigComponent
  └── PreID → PreDefinedIDComponent
```

---

## 📦 Key Functionalities

### Asset Creation
- Supports multiple asset statuses (In-Stock, Repair, Sold, Damaged, E-Waste)
- Form validation based on status
- Asset ID auto-generation with custom prefixes
- Excel import for bulk operations

### Asset Allocation
- Search employee by ID
- Allocate asset to employee
- Moves asset from InStock to Allocated table
- Updates asset status in main Assets table
- Tracks employee details (Email, Username, Location, Reporting Manager)

### Excel Import
- Uses EPPlus library for Excel processing
- Bulk asset creation from XLSX files
- Supports file upload with FormData

### Email Integration
- MailKit library for email functionality
- SMTP configuration in settings
- Email notifications (implied from SMTP config)

### PDF Generation
- jsPDF for PDF creation
- html2canvas for HTML to image conversion
- Report generation capabilities

### Charts & Visualization
- Chart.js integration
- Dashboard widgets for statistics
- Ticket count graphs
- Asset count displays

---

## 🛠️ Development Configuration

### Environment Configuration
```typescript
// environment.ts
export const environment = {
  baseUrl: "http://localhost:53417/", // API base URL
};
```

### Database Connection
```xml
<!-- Web.config -->
<connectionStrings>
  <add name="Test" 
       connectionString="Server=AL230377-BLR\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;MultipleActiveResultSets=True;" />
</connectionStrings>
```

### Build Configuration
- **Development**: Source maps enabled, no optimization
- **Production**: All output hashing, optimization enabled
- Budget limits: 5MB for initial and component styles

---

## 📊 Component Breakdown

### Total Components: 41+

**Asset Management**: 4 components
**Dashboard Modules**: 11 components  
**Settings**: 9 components
**Core UI**: 4 components (Header, Footer, SideNav, Home)
**Purchase**: 2 components
**Ticket Management**: 3 components
**Other**: 8 components (Create, View, Delete, Reports, etc.)

---

## 🔍 Code Quality Observations

### Strengths
✅ Modular component structure
✅ Service-based architecture
✅ Comprehensive feature set
✅ Reusable UI components (Angular Material)
✅ Centralized API service
✅ Session management implementation
✅ Route guards for authentication

### Areas for Improvement
⚠️ Passwords in plain text (CRITICAL SECURITY ISSUE)
⚠️ Wide-open CORS policy
⚠️ Debugger statements in code (`debugger` in login.component.ts)
⚠️ Hard-coded connection strings in Web.config
⚠️ No error logging framework visible
⚠️ Mixed naming conventions (some components use `-m-`, others don't)
⚠️ Large component files (assets-create.component.ts has 438 lines)

---

## 🚀 Deployment Considerations

### Frontend
- Production build: `ng build --configuration production`
- Output directory: `dist/proassetin`
- Serves static files
- Requires API backend running

### Backend
- ASP.NET Web API application
- Requires IIS or hosting environment
- SQL Server database connection
- Port: 53417 (development)

### Dependencies
- Node.js and npm for frontend
- .NET Framework 4.6.1 runtime
- SQL Server database
- IIS or compatible hosting

---

## 📝 Summary

ProAssetin is a **feature-rich Enterprise Asset Management system** with:
- Comprehensive asset lifecycle management
- Multi-module dashboard architecture
- Employee and ticket management
- Purchase order integration
- Extensive settings and configuration
- Report generation capabilities

The application follows Angular best practices for frontend architecture and uses ASP.NET Web API for the backend. However, **critical security improvements** are needed, particularly for password storage and CORS configuration, before production deployment.

**Technology Maturity**: Production-ready architecture with security hardening needed
**Code Organization**: Well-structured and modular
**Feature Completeness**: Comprehensive EAM solution
**Security Status**: Requires immediate attention for password handling and CORS

---

*Analysis generated from codebase review*
*Last Updated: Based on current repository state*



