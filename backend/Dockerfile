FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY MealMate.sln .
COPY src/MealMate.Domain/MealMate.Domain.csproj src/MealMate.Domain/
COPY src/MealMate.Application/MealMate.Application.csproj src/MealMate.Application/
COPY src/MealMate.Infrastructure/MealMate.Infrastructure.csproj src/MealMate.Infrastructure/
COPY src/MealMate.API/MealMate.API.csproj src/MealMate.API/
COPY tests/MealMate.Tests/MealMate.Tests.csproj tests/MealMate.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish src/MealMate.API/MealMate.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MealMate.API.dll"]
