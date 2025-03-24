FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . ./

RUN dotnet restore click-flow-backend.sln

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# 6. Cấu hình cổng và chạy ứng dụng
EXPOSE 7087
ENTRYPOINT ["dotnet", "ClickFlow.API.dll"]