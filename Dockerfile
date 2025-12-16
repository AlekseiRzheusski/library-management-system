# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY LibraryManagement.Api/*.csproj LibraryManagement.Api/
COPY LibraryManagement.Application/*.csproj LibraryManagement.Application/
COPY LibraryManagement.Domain/*.csproj LibraryManagement.Domain/
COPY LibraryManagement.Infrastructure/*.csproj LibraryManagement.Infrastructure/
COPY LibraryManagement.Contract/*.csproj LibraryManagement.Contract/
COPY LibraryManagement.Shared/*.csproj LibraryManagement.Shared/
COPY LibraryManagement.Migrations.PostgreSql/*.csproj LibraryManagement.Migrations.PostgreSql/
COPY LibraryManagement.Integration.Tests/*.csproj LibraryManagement.Integration.Tests/
COPY LibraryManagement.Migrations.Sqlite/*.csproj LibraryManagement.Migrations.Sqlite/

RUN dotnet restore

COPY . .

RUN dotnet publish LibraryManagement.Api/LibraryManagement.Api.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV DOTNET_URLS=http://0.0.0.0:5112;http://0.0.0.0:5113

EXPOSE 5112
EXPOSE 5113

ENTRYPOINT ["dotnet", "LibraryManagement.Api.dll"]
