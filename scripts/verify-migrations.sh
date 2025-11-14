#!/bin/bash

# Скрипт для проверки миграций EF Core

set -e

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
INFRA_PROJECT="$PROJECT_ROOT/src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj"
STARTUP_PROJECT="$PROJECT_ROOT/src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj"

echo "=== Проверка миграций EF Core ==="
echo ""

echo "Список миграций:"
dotnet ef migrations list \
  --project "$INFRA_PROJECT" \
  --startup-project "$STARTUP_PROJECT" \
  --context ApplicationDbContext \
  --no-build

echo ""
echo "Генерация SQL-скрипта миграций:"
dotnet ef migrations script \
  --project "$INFRA_PROJECT" \
  --startup-project "$STARTUP_PROJECT" \
  --context ApplicationDbContext \
  --no-build \
  --idempotent

echo ""
echo "✅ Проверка завершена успешно"
