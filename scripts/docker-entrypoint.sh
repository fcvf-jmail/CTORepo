#!/bin/bash
set -e

echo "============================================"
echo "Запуск Web API приложения"
echo "============================================"

# Ожидание готовности базы данных и применение миграций с повторными попытками
echo "Ожидание готовности PostgreSQL..."
sleep 5

echo "Применение миграций базы данных..."
cd /src

# Попытки применить миграции с повторными попытками
MAX_RETRIES=30
RETRY_COUNT=0
until dotnet ef database update \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext \
  --no-build 2>&1 | tee /tmp/migration.log || [ $RETRY_COUNT -eq $MAX_RETRIES ]; do
  RETRY_COUNT=$((RETRY_COUNT+1))
  echo "Попытка $RETRY_COUNT из $MAX_RETRIES не удалась. Повторная попытка через 2 секунды..."
  sleep 2
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
  echo "Не удалось применить миграции после $MAX_RETRIES попыток"
  exit 1
fi

echo "Миграции успешно применены!"

# Возврат в рабочую директорию и запуск приложения
cd /app
echo "Запуск приложения..."
echo "============================================"
exec dotnet WebApi.Presentation.dll
