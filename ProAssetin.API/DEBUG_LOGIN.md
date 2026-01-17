# Debugging Login Issues

## Common Issues and Solutions

### 1. Check API is Running

Verify the API is running and accessible:
```powershell
# Check if API is running
curl https://localhost:5001/swagger -k
```

Or open in browser: `https://localhost:5001/swagger`

### 2. Check API URL in Angular

Verify `environment.ts` has correct API URL:
```typescript
apiUrl: 'https://localhost:5001/api'
```

### 3. Check CORS Configuration

Make sure CORS in `Program.cs` allows Angular origin:
```csharp
policy.WithOrigins("http://localhost:4200")
```

### 4. Test Login via Swagger First

Before testing from Angular, test directly via Swagger:
1. Open: `https://localhost:5001/swagger`
2. Use `POST /api/auth/login`
3. Try with:
   ```json
   {
     "email": "admin@infosys.com",
     "password": "Admin123"
   }
   ```

### 5. Check Browser Console

Open browser DevTools (F12) and check:
- **Console tab**: For JavaScript errors
- **Network tab**: For API request/response details
  - Check if request is being sent
  - Check response status code
  - Check response body for error details

### 6. Verify Users Were Created

Check database to confirm users exist:
```sql
USE [ProAssetinDev]
SELECT Email, FirstName, LastName, TenantId, IsActive, EmailConfirmed
FROM ProAssetinUsers
WHERE Email = 'admin@infosys.com';
```

### 7. Common Error Codes

- **400 Bad Request**: Invalid credentials or missing fields
- **401 Unauthorized**: Authentication failed
- **404 Not Found**: API endpoint not found (check URL)
- **500 Internal Server Error**: Server-side error (check API logs)

### 8. Test API Response Format

The API should return:
```json
// Success
{
  "token": "...",
  "user": {
    "id": "...",
    "email": "admin@infosys.com",
    "firstName": "Admin",
    "lastName": "Infosys",
    "tenantId": "infosys"
  }
}

// Error
{
  "message": "Invalid email or password"
}
```

## Quick Checklist

- [ ] API is running (`dotnet run` in ProAssetin.API)
- [ ] Database exists and tables are created
- [ ] Users were seeded (check console output)
- [ ] CORS allows `http://localhost:4200`
- [ ] Angular environment has correct API URL
- [ ] Test credentials: `admin@infosys.com` / `Admin123`
- [ ] Browser console shows no CORS errors
- [ ] Network tab shows API request is being made

