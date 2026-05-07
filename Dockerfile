# Përdor SDK për të builduar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopjo csproj (direkt, pa nënfolder)
COPY ["CreditCard.csproj", "."]
RUN dotnet restore "CreditCard.csproj"

# Kopjo gjithë kodin dhe buildo
COPY . .
RUN dotnet publish "CreditCard.csproj" -c Release -o /app/publish

# Përdor runtime për të ekzekutuar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CreditCard.dll"]