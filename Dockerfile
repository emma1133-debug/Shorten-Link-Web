# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app


COPY . ./


RUN dotnet restore "UrlShortener.csproj"

RUN dotnet publish "UrlShortener.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "UrlShortener.dll"]
