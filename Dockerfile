# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY CalikBackend.sln .
COPY src/CalikBackend.API/CalikBackend.API.csproj src/CalikBackend.API/
COPY src/CalikBackend.Domain/CalikBackend.Domain.csproj src/CalikBackend.Domain/
COPY src/CalikBackend.Application/CalikBackend.Application.csproj src/CalikBackend.Application/
COPY src/CalikBackend.Infrastructure/CalikBackend.Infrastructure.csproj src/CalikBackend.Infrastructure/
RUN dotnet restore

COPY . .
RUN dotnet publish src/CalikBackend.API/CalikBackend.API.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CalikBackend.API.dll"]
