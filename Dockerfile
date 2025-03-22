# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy toàn bộ source vào container
COPY . ./

# Restore dependencies
RUN dotnet restore "UrlShortener.csproj"

# Publish project ra thư mục out
RUN dotnet publish "UrlShortener.csproj" -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files từ build stage
COPY --from=build /out .

# Mở cổng 5000 (nếu cần expose)
EXPOSE 5000

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "UrlShortener.dll"]
