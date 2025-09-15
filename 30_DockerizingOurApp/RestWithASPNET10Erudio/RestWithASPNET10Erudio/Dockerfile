FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RestWithASPNET10Erudio/RestWithASPNET10Erudio.csproj", "RestWithASPNET10Erudio/"]
RUN dotnet restore "./RestWithASPNET10Erudio/RestWithASPNET10Erudio.csproj"
COPY . .
WORKDIR "/src/RestWithASPNET10Erudio"
RUN dotnet build "./RestWithASPNET10Erudio.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RestWithASPNET10Erudio.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RestWithASPNET10Erudio.dll"]