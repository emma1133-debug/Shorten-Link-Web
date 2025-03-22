# Stage 1: Build + EF Tools
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy all source files
COPY . .

# Cài dotnet-ef
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Restore dependencies
RUN dotnet restore "UrlShortener.csproj"

# Build & publish
RUN dotnet publish "UrlShortener.csproj" -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "UrlShortener.dll"]
