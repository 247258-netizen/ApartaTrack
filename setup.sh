#!/bin/bash
# ============================================================
# Apartment Management - Setup Script for .NET 10
# Run: bash setup.sh
# ============================================================
set -e

echo "==> Checking .NET version..."
dotnet --version

echo ""
echo "==> Installing EF Core packages..."
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.*
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.*
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.*

echo ""
echo "==> Installing EF Core CLI tool (if not installed)..."
dotnet tool install --global dotnet-ef 2>/dev/null || dotnet tool update --global dotnet-ef

echo ""
echo "==> Creating first migration..."
dotnet ef migrations add InitialCreate

echo ""
echo "==> Applying migration (creates apartment.db)..."
dotnet ef database update

echo ""
echo "==> Build check..."
dotnet build

echo ""
echo "============================================"
echo "  Setup complete! Run the app with:"
echo "  dotnet run"
echo "  OR press F5 in VS Code"
echo "============================================"
