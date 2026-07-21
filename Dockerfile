# ==========================================
# 1. ETAPA DE BASE (O que vai rodar em produção)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# ==========================================
# 2. ETAPA DE BUILD E RESTAURAÇÃO
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiando o arquivo de Solução (.slnx) e os arquivos de projeto (.csproj)
COPY ["IvanWeb.slnx", "."]
COPY ["IvanWeb.Api/IvanWeb.Api.csproj", "IvanWeb.Api/"]
COPY ["IvanWeb.Application/IvanWeb.Application.csproj", "IvanWeb.Application/"]
COPY ["IvanWeb.Domain/IvanWeb.Domain.csproj", "IvanWeb.Domain/"]
COPY ["IvanWeb.Infrastructure/IvanWeb.Infrastructure.csproj", "IvanWeb.Infrastructure/"]

# Restaurando as dependências usando a solution
RUN dotnet restore "IvanWeb.slnx"

# Copiando todo o resto do código da sua máquina para o container
COPY . .

# Mudando para a pasta da API e compilando
WORKDIR "/src/IvanWeb.Api"
RUN dotnet build "./IvanWeb.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ==========================================
# 3. ETAPA DE PUBLICAÇÃO
# ==========================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IvanWeb.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ==========================================
# 4. ETAPA FINAL (Montando a imagem enxuta)
# ==========================================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IvanWeb.Api.dll"]