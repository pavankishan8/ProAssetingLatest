# Fix Port 4200 Already in Use

## Quick Solution

If port 4200 is in use, you have these options:

### Option 1: Kill the Process Using Port 4200

**Find the process:**
```powershell
netstat -ano | findstr :4200
```

**Kill the process (replace PID with actual process ID):**
```powershell
taskkill /F /PID <PID>
```

**Or using PowerShell:**
```powershell
$pid = (Get-NetTCPConnection -LocalPort 4200).OwningProcess
Stop-Process -Id $pid -Force
```

### Option 2: Use a Different Port

When prompted, type `Y` and Angular will use the next available port (usually 4201):

```powershell
ng serve
# Port 4200 is already in use.
# Would you like to use a different port? (Y/n)
# Type: Y
```

Then update your API CORS settings to allow the new port.

### Option 3: Find and Kill All Node/Angular Processes

```powershell
# Kill all Node processes (this will kill all Node apps)
Get-Process node -ErrorAction SilentlyContinue | Stop-Process -Force

# Or kill specific Angular CLI processes
Get-Process | Where-Object {$_.ProcessName -like "*node*" -or $_.ProcessName -like "*ng*"} | Stop-Process -Force
```

### Option 4: Use a Specific Port

Start Angular on a specific port:

```powershell
ng serve --port 4201
```

Then update `ProAssetin.API/Program.cs` CORS to allow:
```csharp
policy.WithOrigins("http://localhost:4201")
```

## Verify Port is Free

After killing the process, verify port 4200 is free:

```powershell
netstat -ano | findstr :4200
```

If no output, the port is free.

## Common Causes

- Previous Angular dev server didn't shut down properly (Ctrl+C didn't work)
- Multiple Angular instances running
- Background process still active
- VS Code terminal session still running

