FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=Microsoft.AspNetCore.SpaProxy

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FightingGameServer_Rest.csproj", "./"]
RUN dotnet restore "FightingGameServer_Rest.csproj"
COPY . .
RUN dotnet build "FightingGameServer_Rest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FightingGameServer_Rest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FightingGameServer_Rest.dll"] 