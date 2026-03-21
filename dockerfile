# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY EnergyBoard.API/*.csproj ./EnergyBoard.API/
COPY EnergyBoard.application/*.csproj ./EnergyBoard.application/
COPY EnergyBoard.domain/*.csproj ./EnergyBoard.domain/
COPY EnergyBoard.infraestructure/*.csproj ./EnergyBoard.infraestructure/

RUN dotnet restore EnergyBoard.API/EnergyBoard.API.csproj

COPY . .

RUN dotnet publish EnergyBoard.API/EnergyBoard.API.csproj -c Release -o /app/out

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000

ENTRYPOINT ["dotnet", "EnergyBoard.API.dll"]