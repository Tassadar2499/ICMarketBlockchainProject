FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["global.json", "./"]
COPY ["ICMarkets.Blockchains.sln", "./"]
COPY ["src/ICMarkets.Blockchains.Api/ICMarkets.Blockchains.Api.csproj", "src/ICMarkets.Blockchains.Api/"]
COPY ["src/ICMarkets.Blockchains.Application/ICMarkets.Blockchains.Application.csproj", "src/ICMarkets.Blockchains.Application/"]
COPY ["src/ICMarkets.Blockchains.Domain/ICMarkets.Blockchains.Domain.csproj", "src/ICMarkets.Blockchains.Domain/"]
COPY ["src/ICMarkets.Blockchains.Infrastructure/ICMarkets.Blockchains.Infrastructure.csproj", "src/ICMarkets.Blockchains.Infrastructure/"]
COPY ["tests/ICMarkets.Blockchains.UnitTests/ICMarkets.Blockchains.UnitTests.csproj", "tests/ICMarkets.Blockchains.UnitTests/"]
COPY ["tests/ICMarkets.Blockchains.IntegrationTests/ICMarkets.Blockchains.IntegrationTests.csproj", "tests/ICMarkets.Blockchains.IntegrationTests/"]

RUN dotnet restore "ICMarkets.Blockchains.sln"

COPY . .

RUN dotnet publish "src/ICMarkets.Blockchains.Api/ICMarkets.Blockchains.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN mkdir -p /app/data

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ICMarkets.Blockchains.Api.dll"]
