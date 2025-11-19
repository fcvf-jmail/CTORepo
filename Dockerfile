# Этап 1: Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы решения и проектов для восстановления зависимостей
COPY ["WebApi.sln", "./"]
COPY ["global.json", "./"]
COPY ["src/WebApi.Domain/WebApi.Domain.csproj", "src/WebApi.Domain/"]
COPY ["src/WebApi.Application/WebApi.Application.csproj", "src/WebApi.Application/"]
COPY ["src/WebApi.Infrastructure/WebApi.Infrastructure.csproj", "src/WebApi.Infrastructure/"]
COPY ["src/WebApi.Presentation/WebApi.Presentation.csproj", "src/WebApi.Presentation/"]
COPY ["src/WebApi.ProjectStartUp/WebApi.ProjectStartUp.csproj", "src/WebApi.ProjectStartUp/"]
COPY ["tests/WebApi.Tests/WebApi.Tests.csproj", "tests/WebApi.Tests/"]

# Восстанавливаем зависимости
RUN dotnet restore "WebApi.sln"

# Копируем остальные файлы проекта
COPY . .

# Собираем проект
WORKDIR "/src/src/WebApi.ProjectStartUp"
RUN dotnet build "WebApi.ProjectStartUp.csproj" -c Release -o /app/build

# Этап 2: Публикация приложения
FROM build AS publish
RUN dotnet publish "WebApi.ProjectStartUp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Этап 3: Финальный образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Копируем опубликованное приложение
COPY --from=publish /app/publish .

# Открываем порт
EXPOSE 8080

# Настройка переменных окружения
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Запуск приложения
ENTRYPOINT ["dotnet", "WebApi.ProjectStartUp.dll"]
