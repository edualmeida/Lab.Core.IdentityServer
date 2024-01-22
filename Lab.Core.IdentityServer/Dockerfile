#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Lab.Core.IdentityServer/Lab.Core.IdentityServer.csproj", "Lab.Core.IdentityServer/"]
RUN dotnet restore "./Lab.Core.IdentityServer/./Lab.Core.IdentityServer.csproj"
COPY . .
WORKDIR "/src/Lab.Core.IdentityServer"
RUN dotnet build "./Lab.Core.IdentityServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Lab.Core.IdentityServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN mkdir -p /var/app/keys
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lab.Core.IdentityServer.dll"]