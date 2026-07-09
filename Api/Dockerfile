FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY nuget.config .
COPY Directory.Build.props* ./
COPY Directory.Build.targets* ./
COPY Directory.Packages.props* ./
COPY global.json* ./

COPY Api/Api.csproj Api/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Shared/Shared.csproj Shared/

RUN dotnet restore Api/Api.csproj \
    --configfile nuget.config \
    --verbosity normal

COPY . .

WORKDIR /src/Api
RUN dotnet publish Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore


FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]