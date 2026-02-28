FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/WakeCommerce.API/WakeCommerce.API.csproj", "WakeCommerce.API/"]
COPY ["src/WakeCommerce.CrossCutting/WakeCommerce.IoC.csproj", "WakeCommerce.CrossCutting/"]
COPY ["src/WakeCommerce.Application/WakeCommerce.Application.csproj", "WakeCommerce.Application/"]
COPY ["src/WakeCommerce.Domain/WakeCommerce.Domain.csproj", "WakeCommerce.Domain/"]
COPY ["src/WakeCommerce.Infrastructure/WakeCommerce.Infrastructure.csproj", "WakeCommerce.Infrastructure/"]

RUN dotnet restore "WakeCommerce.API/WakeCommerce.API.csproj"

COPY src/ .

WORKDIR "/src/WakeCommerce.API"
RUN dotnet build "WakeCommerce.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/WakeCommerce.API"
RUN dotnet publish "WakeCommerce.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WakeCommerce.API.dll"]