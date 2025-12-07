FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

# Install Node.js, npm, and pnpm
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_lts.x | bash - && \
    apt-get install -y nodejs && \
    npm install -g pnpm && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["HmctsDevChallenge.Backend/HmctsDevChallenge.Backend.csproj", "HmctsDevChallenge.Backend/"]
COPY ["HmctsDevChallenge.Frontend/HmctsDevChallenge.Frontend.esproj", "HmctsDevChallenge.Frontend/"]
COPY ["HmctsDevChallenge.Frontend/package.json", "HmctsDevChallenge.Frontend/"]
COPY ["HmctsDevChallenge.Frontend/pnpm-lock.yaml", "HmctsDevChallenge.Frontend/"]
COPY ["HmctsDevChallenge.Frontend/pnpm-workspace.yaml", "HmctsDevChallenge.Frontend/"]
WORKDIR /src/HmctsDevChallenge.Frontend
RUN pnpm install --frozen-lockfile
WORKDIR /src
RUN dotnet restore "HmctsDevChallenge.Backend/HmctsDevChallenge.Backend.csproj"
COPY . .
WORKDIR "/src/HmctsDevChallenge.Backend"
RUN dotnet build "./HmctsDevChallenge.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HmctsDevChallenge.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HmctsDevChallenge.Backend.dll"]
