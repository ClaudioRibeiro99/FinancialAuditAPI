FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar os arquivos .csproj e restaurar as dependências
COPY ["FinancialAudit.Api/FinancialAudit.Api.csproj", "FinancialAudit.Api/"]
COPY ["FinancialAudit.Application/FinancialAudit.Application.csproj", "FinancialAudit.Application/"]
COPY ["FinancialAudit.Domain/FinancialAudit.Domain.csproj", "FinancialAudit.Domain/"]
COPY ["FinancialAudit.Infrastructure/FinancialAudit.Infrastructure.csproj", "FinancialAudit.Infrastructure/"]

RUN dotnet restore "FinancialAudit.Api/FinancialAudit.Api.csproj"

# Copiar o restante dos arquivos e compilar a aplicação
COPY . .
WORKDIR "/src/FinancialAudit.Api"
RUN dotnet build "FinancialAudit.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FinancialAudit.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "FinancialAudit.Api.dll"]
