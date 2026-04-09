# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["SmartJobRecommender.csproj", "./"]
RUN dotnet restore "SmartJobRecommender.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "SmartJobRecommender.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "SmartJobRecommender.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartJobRecommender.dll"]
