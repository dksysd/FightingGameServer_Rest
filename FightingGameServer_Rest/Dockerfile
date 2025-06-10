# ASP.NET Core Runtime을 위한 베이스 이미지
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 환경 변수 설정
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# SDK 이미지를 사용하여 애플리케이션 빌드
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 프로젝트 파일들을 먼저 복사하여 캐시 최적화
COPY ["FightingGameServer_Rest.csproj", "FightingGameServer_Rest/"]
#COPY ["global.json", "./"]
RUN dotnet restore "FightingGameServer_Rest/FightingGameServer_Rest.csproj" --verbosity minimal

# 소스 코드 복사 및 빌드
COPY . .
WORKDIR "/src/FightingGameServer_Rest"
RUN dotnet build "FightingGameServer_Rest.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# 게시 단계 - 프로덕션 최적화
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FightingGameServer_Rest.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    --self-contained false \
    --no-restore \
    --verbosity minimal

# 최종 이미지 - 보안 강화
FROM base AS final
WORKDIR /app

# 비root 사용자 생성 및 권한 설정
RUN adduser --disabled-password --gecos '' --shell /bin/false appuser && \
    chown -R appuser:appuser /app
    
# 게시된 파일들 복사
COPY --from=publish --chown=appuser:appuser /app/publish .

# 비root 사용자로 실행
USER appuser

ENTRYPOINT ["dotnet", "FightingGameServer_Rest.dll"]