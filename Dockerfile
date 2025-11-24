# Dockerfile for Kosova Dogana Moderne
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["KosovaDoganaModerne.csproj", "./"]
RUN dotnet restore "KosovaDoganaModerne.csproj"
COPY . .
RUN dotnet build "KosovaDoganaModerne.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KosovaDoganaModerne.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "KosovaDoganaModerne.dll"]
