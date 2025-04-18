# FightingGameServer_Rest Docker 배포 가이드 (외부 DB 사용)

## 필수 요구 사항
- Docker 설치
- Docker Compose 설치
- 외부 MySQL 데이터베이스 서버 접근 정보

## 배포 단계

### 1. 환경 설정

`docker-compose.yml` 파일에서 환경 설정이 개발 환경으로 되어 있는지 확인하세요:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
```

Development 환경으로 설정하면 Swagger UI가 활성화되어 API를 쉽게 테스트할 수 있습니다.

외부 데이터베이스 서버 주소, 포트, 사용자 이름 및 비밀번호를 실제 값으로 변경하세요.

### 2. 컨테이너 빌드 및 실행

프로젝트 디렉토리에서 다음 명령어를 실행하세요:

```bash
docker-compose up --build -d
```

이 명령어는 백그라운드에서 컨테이너를 빌드하고 실행합니다.

### 3. 로그 확인

컨테이너 로그를 확인하려면 다음 명령어를 사용하세요:

```bash
docker-compose logs -f
```

### 4. 컨테이너 상태 확인

컨테이너 상태를 확인하려면 다음 명령어를 사용하세요:

```bash
docker-compose ps
```

### 5. 컨테이너 중지

컨테이너를 중지하려면 다음 명령어를 사용하세요:

```bash
docker-compose down
```

## 포트 정보

- 웹 API: http://localhost:8080 (HTTP), https://localhost:8443 (HTTPS)
- Swagger UI: http://localhost:8080/swagger (Development 환경에서만 사용 가능)

## 배포 시 주의사항

1. 프로덕션 환경에서는 데이터베이스 연결 문자열의 비밀번호를 환경 변수나 Docker 비밀(secrets)을 사용하여 관리하는 것이 좋습니다.
2. 외부 데이터베이스 서버에 대한 방화벽 설정이 올바르게 구성되어 있는지 확인하세요.
3. 필요한 경우 SSL 인증서를 설정하여 HTTPS를 적절히 구성하세요. 