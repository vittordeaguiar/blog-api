FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

COPY BlogAPI.sln ./
COPY src/BlogAPI.API/BlogAPI.API.csproj src/BlogAPI.API/
COPY src/BlogAPI.Application/BlogAPI.Application.csproj src/BlogAPI.Application/
COPY src/BlogAPI.Domain/BlogAPI.Domain.csproj src/BlogAPI.Domain/
COPY src/BlogAPI.Infrastructure/BlogAPI.Infrastructure.csproj src/BlogAPI.Infrastructure/
COPY tests/BlogAPI.UnitTests/BlogAPI.UnitTests.csproj tests/BlogAPI.UnitTests/

RUN dotnet restore BlogAPI.sln

FROM restore AS build
WORKDIR /src

COPY . .

RUN dotnet build BlogAPI.sln -c Release --no-restore

FROM build AS publish
RUN dotnet publish src/BlogAPI.API/BlogAPI.API.csproj -c Release -o /app/publish --no-build

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "BlogAPI.API.dll"]
