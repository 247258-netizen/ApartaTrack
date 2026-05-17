# 🏢 ApartaTrack1 System
### Blazor Server + EF Core + SQLite | .NET 10

---

## Quick Start (VS Code mein)

### Step 1 — Folder open karo
```bash
cd ApartaTrack
code .
```

### Step 2 — Packages install karo
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.*
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.*
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.*
```

### Step 3 — EF CLI tool install karo
```bash
dotnet tool install --global dotnet-ef
```

### Step 4 — Migration banao aur DB create karo
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 5 — Run karo
```bash
dotnet run
```
Browser mein kholo: **https://localhost:5001**

---

## Ya ek command mein sab:
```bash
bash setup.sh
```

---

## Features
- ✅ Apartments — Add, Edit, Delete, Available/Occupied
- ✅ Tenants — CNIC, Phone, Emergency Contact
- ✅ Leases — Tenant ko apartment se link karo
- ✅ Payments — PKR amounts, Paid/Pending/Late status
- ✅ Dashboard — Summary cards + quick links
- ✅ SQLite — Koi server install nahi chahiye

## Project Structure
```
ApartaTrack/
├── Models/         → Apartment, Tenant, Lease, Payment
├── Data/           → AppDbContext (EF Core)
├── Services/       → Business logic (CRUD)
├── Pages/          → Blazor Razor pages
├── Shared/         → NavMenu, MainLayout
├── wwwroot/        → CSS
└── .vscode/        → launch.json, tasks.json
```
