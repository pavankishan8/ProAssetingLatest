# Fix CORS "0 Unknown Error"

## The Issue

Error: `Http failure response for https://localhost:5001/api/auth/login: 0 Unknown Error`

This typically means:
1. **CORS not configured correctly** - API not allowing Angular origin
2. **API not running** - Can't reach the API endpoint
3. **Middleware order** - CORS must be before authentication

## Solution Applied

1. **CORS Configuration**: Updated to allow `http://localhost:4200`
2. **Middleware Order**: Moved `UseCors()` **before** `UseHttpsRedirection()`

## Additional Checks

### 1. Verify API is Running

```powershell
# Check if API is running on port 5001
netstat -ano | findstr :5001
```

Or open in browser: `https://localhost:5001/swagger`

### 2. Test API Directly

Try accessing the API directly from browser:
```
https://localhost:5001/api/auth/login
```

You should get a 405 Method Not Allowed (this is expected for GET).

### 3. Check Browser Console

Open DevTools (F12) → Network tab:
- Check if request is being sent
- Look for CORS-related errors
- Check the response headers

### 4. Test via Swagger

1. Open: `https://localhost:5001/swagger`
2. Test `/api/auth/login` endpoint directly
3. If this works, the issue is CORS configuration

### 5. Verify CORS Headers

After restarting the API, check the response headers include:
```
Access-Control-Allow-Origin: http://localhost:4200
Access-Control-Allow-Credentials: true
```

## If Still Not Working

### Option 1: Restart API
```powershell
# Stop the API (Ctrl+C)
# Then restart
cd ProAssetin.API
dotnet run
```

### Option 2: Clear Browser Cache
- Clear browser cache and cookies
- Try incognito/private window

### Option 3: Temporary - Allow All Origins (Development Only)

In `Program.cs`, temporarily use:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

**Note**: Remove `AllowCredentials()` when using `AllowAnyOrigin()` - they can't be used together.

