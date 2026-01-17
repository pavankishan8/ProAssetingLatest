# Test Users for Login

## Test Credentials

All test users have the same password: **`Admin123`**

## Infosys Users (5 users)

| Email | Password | Role | Location | Project | Team |
|-------|----------|------|----------|---------|------|
| `admin@infosys.com` | Admin123 | Admin | Bangalore | Project Alpha | Team A |
| `pavan@infosys.com` | Admin123 | User | Hyderabad | Project Beta | Team B |
| `ravi@infosys.com` | Admin123 | User | Pune | Project Gamma | Team C |
| `priya@infosys.com` | Admin123 | User | Mumbai | Project Delta | Team A |
| `amit@infosys.com` | Admin123 | User | Delhi | Project Epsilon | Team B |

## Wipro Users (5 users)

| Email | Password | Role | Location | Project | Team |
|-------|----------|------|----------|---------|------|
| `admin@wipro.com` | Admin123 | Admin | Bangalore | Project X | Team 1 |
| `sanjay@wipro.com` | Admin123 | User | Chennai | Project Y | Team 2 |
| `neha@wipro.com` | Admin123 | User | Gurgaon | Project Z | Team 1 |
| `vikram@wipro.com` | Admin123 | User | Bangalore | Project A | Team 3 |
| `anjali@wipro.com` | Admin123 | User | Pune | Project B | Team 2 |

## How to Use

1. **Start the API**:
   ```powershell
   cd ProAssetin.API
   dotnet run
   ```

2. **Users are auto-created** when the API starts (in Development mode)

3. **Login via Swagger**:
   - Navigate to: `https://localhost:5001/swagger`
   - Use `/api/auth/login` endpoint
   - Test with any user credentials above

4. **Login via Angular App**:
   - Navigate to: `http://localhost:4200`
   - Use login page
   - Enter email and password (Admin123)

## Database Mapping

- **Infosys users** (`@infosys.com`) → Connect to `ProAsset_Infosys` database
- **Wipro users** (`@wipro.com`) → Connect to `ProAsset_Wipro` database

## Notes

- All users have `EmailConfirmed = true`
- All users are `IsActive = true`
- Passwords are hashed using ASP.NET Core Identity
- Users are created in `ProAssetinDev` (master database)
- Tenant databases are auto-created on first login

## Verification

To verify users were created:

```sql
USE [ProAssetinDev]
SELECT Email, FirstName, LastName, TenantId, Location, ProjectName, Team
FROM ProAssetinUsers
WHERE Email LIKE '%@infosys.com' OR Email LIKE '%@wipro.com'
ORDER BY TenantId, Email;
```

