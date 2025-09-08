  # Desafio Arquitetura de Solução - Verity
  
  ## Fluxo de Caixa Diário — Clean Architecture + CQRS + JWT (NET 9)

  Controle **lançamentos diários** (Débitos/Créditos) e gere o **saldo diário consolidado**.
  Este monorepo contém **API .NET 9**, **Web (React + Vite + Tailwind)** e **PostgreSQL**, com **Serilog** e **OpenTelemetry**.

  > Toda a documentação está **neste README**, com **diagramas Mermaid** renderizáveis no GitHub.

  ---

  ## Índice
  1. [Requisitos de Negócio](#requisitos-de-negócio)
  2. [Domínios Funcionais & Capacidades](#domínios-funcionais--capacidades)
  3. [Requisitos Funcionais e Não Funcionais](#requisitos-funcionais-e-não-funcionais)
  4. [Arquitetura Alvo (com Diagramas Mermaid)](#arquitetura-alvo-com-diagramas-mermaid)
  5. [Justificativas Técnicas](#justificativas-técnicas)
  6. [Operação & Observabilidade](#operação--observabilidade)
  7. [Como Rodar Localmente](#como-rodar-localmente)
  8. [Endpoints Principais](#endpoints-principais)
  9. [Licença](#licença)

  ---

  ## Requisitos de Negócio

  **Serviços**:
  - **Serviço de Lançamentos**: cadastro, listagem e remoção.
  - **Serviço de Consolidado Diário**: apuração por data (débitos, créditos, saldo do dia e acumulado).

  **Regras essenciais**:
  - Cada lançamento pertence a **um usuário** e possui **data**, **tipo** (1=Débito, 2=Crédito), **valor** e **descrição**.
  - O relatório consolida por **dia** no intervalo `[de, até]`, com **saldo inicial** opcional.
  - Autenticação obrigatória; o usuário vê **apenas seus próprios dados**.

  ---

  ## Domínios Funcionais & Capacidades

  **Domínios**:
  - **Identidade & Acesso**: cadastro, login, JWT, refresh, logout.
  - **Lançamentos**: registrar, listar, excluir.
  - **Relatórios**: consolidação diária; futuros CSV/PDF.
  - **Plataforma & Observabilidade**: logs, métricas, traces, health.
  - **Segurança & Conformidade**: hashing de senha, CORS/TLS, proteção de dados.

  **Capacidades de Negócio**:
  - **Gerir Usuário** (onboarding, autenticação, sessão).
  - **Gerir Lançamentos** (CRUD básico).
  - **Apurar Saldo Diário** (consolidação por data + acumulado).
  - **Disponibilizar Relatórios** (UI + gráfico).
  - **Operar com Confiabilidade** (migrations, monitoramento, auditoria mínima).

  ---

  ## Requisitos Funcionais e Não Funcionais

  **Funcionais**
  1. **Cadastro de Usuário** (`POST /auth/register`).
  2. **Login** (`POST /auth/login`) → retorna `accessToken`, `refreshToken`, `expiresInSeconds`.
  3. **Refresh** (`POST /auth/refresh`) com **rotação** segura.
  4. **Logout** (`POST /auth/logout`) — atual ou **todos dispositivos**.
  5. **Criar Lançamento** (`POST /lancamentos`) — data, tipo (1=Débito, 2=Crédito), valor, descrição.
  6. **Listar Lançamentos** (`GET /lancamentos?de&ate`).
  7. **Remover Lançamento** (`DELETE /lancamentos/{id}`).
  8. **Relatório de Saldo Diário** (`GET /relatorios/saldo-diario?de&ate&saldoInicial`).

  **Não Funcionais**
  - **Segurança**: JWT + refresh rotativo; PBKDF2 (100k iterações+); CORS por ambiente; TLS em produção.
  - **Desempenho**: p95 CRUD < 200ms; relatório < 400ms em períodos de até 90 dias.
  - **Escalabilidade**: API stateless (horizontal); Postgres com tuning/replicas; materiais/ETL futuro para janelas extensas.
  - **Resiliência**: health-checks, logs estruturados, métricas e traces OTel; migrations automatizadas controladas.
  - **Observabilidade**: Serilog (console/JSON), OpenTelemetry (AspNetCore, HttpClient, EF).
  - **Manutenibilidade**: Clean Architecture + CQRS, testes do domínio (saldo).
  - **Portabilidade**: Docker Compose e CLI (`dotnet`, `npm`).

  ---

  ## Arquitetura Alvo (com Diagramas Mermaid)

  ### Contexto (alto nível)
  ```mermaid
  flowchart LR
    U[Usuário] -->|SPA| Web[WebApp (React/Vite)]
    Web -->|HTTP + JWT| Api[API .NET 9
(Clean Arch + CQRS)]
    Api -->|EF Core (Npgsql)| DB[(PostgreSQL)]
    Api -->|OTLP| OTel[(OpenTelemetry Collector/APM)]
  ```

  ### Contêineres
  ```mermaid
  flowchart TB
    subgraph Browser
      U[Usuário]
    end

    subgraph Frontend["WebApp (React + Vite)"]
      FE[SPA]
    end

    subgraph Backend["API (.NET 9)"]
      CTRL[Controllers]
MED[MediatR]
APP[Application]
DOM[Domain]
INF[Infrastructure]
    end

    subgraph Data["PostgreSQL"]
      DB[(fluxodb)]
    end

    subgraph Observ["Observabilidade"]
      OT[OpenTelemetry]
SG[Serilog]
    end

    U --> FE --> Backend
    Backend --> DB
    Backend --> OT
    Backend --> SG
  ```

  ### Componentes (Clean Architecture)
  ```mermaid
  flowchart LR
    subgraph API
      AuthCtrl[AuthController]
      LancCtrl[LancamentosController]
      RelCtrl[RelatoriosController]
    end

    subgraph Application
      Med[MediatR]
      Cmd[Commands]
      Qry[Queries]
      Val[Validators]
    end

    subgraph Domain
      User[User]
      Lanc[Lancamento]
      RT[RefreshToken]
      Tipo[TipoLancamento]
    end

    subgraph Infrastructure
      Db[AppDbContext]
      Jwt[JwtTokenGenerator]
      RTSvc[RefreshTokenService]
      Pwd[PasswordHasher]
    end

    AuthCtrl --> Med
    LancCtrl --> Med
    RelCtrl  --> Med
    Med --> Cmd
    Med --> Qry
    Cmd --> Db
    Qry --> Db
    Db --> User
    Db --> Lanc
    Db --> RT
    Pwd -. usado por .-> Cmd
    Jwt -. usado por .-> AuthCtrl
    RTSvc -. usado por .-> AuthCtrl
  ```

  ### Modelo de Dados (ER)
  ```mermaid
  erDiagram
      USERS {
        uuid Id PK
        string UserName
        string Email
        string PasswordHash
        timestamptz CreatedAt
      }

      LANCAMENTOS {
        uuid Id PK
        uuid UserId FK
        date Data
        int Tipo  "1=Débito, 2=Crédito"
        numeric Valor "14,2"
        text Descricao
      }

      REFRESH_TOKENS {
        uuid Id PK
        uuid UserId FK
        text TokenHash
        timestamptz ExpiresAt
        timestamptz CreatedAt
        timestamptz RevokedAt
        text ReplacedByTokenHash
        text UserAgent
        text IpAddress
      }

      USERS ||--o{ LANCAMENTOS : "possui"
      USERS ||--o{ REFRESH_TOKENS : "emite"
  ```

  ### Sequência — Criar Lançamento
  ```mermaid
  sequenceDiagram
    autonumber
    actor User as Usuário
    participant Web as WebApp
    participant API
    participant DB as PostgreSQL

    User->>Web: Preenche (data, tipo, valor, desc) e envia
    Web->>API: POST /api/v1/lancamentos (Bearer)
    API->>DB: INSERT lancamentos
    DB-->>API: OK (id)
    API-->>Web: 201 Created
    Web-->>User: Atualiza tabela
  ```

  ### Sequência — Relatório de Saldo Diário
  ```mermaid
  sequenceDiagram
    autonumber
    actor User as Usuário
    participant Web as WebApp
    participant API
    participant DB as PostgreSQL

    User->>Web: Solicita (de/até, saldoInicial)
    Web->>API: GET /api/v1/relatorios/saldo-diario
    API->>DB: SELECT lancamentos (group by data)
    DB-->>API: Débitos/Créditos por dia
    API->>API: Calcula saldo do dia e acumulado
    API-->>Web: JSON diário
    Web-->>User: Tabela + Gráfico
  ```

  ### Deployment — Dev e Produção
  ```mermaid
  flowchart LR
    subgraph Dev
      Docker[(Docker Compose)]
      WebD[Web:5173]
      ApiD[API:8080]
      PgD[(PostgreSQL local)]
      Docker --> WebD & ApiD
      ApiD --> PgD
    end

    subgraph Prod
      Ingress[Ingress/LB]
      WebP[Web (Nginx)]
      ApiP[API (replicas)]
      PgP[(PostgreSQL Gerenciado)]
      APM[(OTLP / APM)]
      Ingress --> WebP
      Ingress --> ApiP
      ApiP --> PgP
      ApiP --> APM
    end
  ```

  ---

  ## Justificativas Técnicas

  - **.NET 9 + Clean Architecture + CQRS** → testabilidade, separação de responsabilidades, evolução segura.
  - **PostgreSQL** → robusto, `numeric(14,2)`, bom suporte EF/Npgsql.
  - **React + Vite + Tailwind** → DX rápida, UI moderna e consistente.
  - **Serilog** → logs estruturados e roteáveis.
  - **OpenTelemetry (OTLP)** → padrão aberto para traces/métricas/logs.
  - **Docker Compose** → bootstrap local simples.
  - **PBKDF2** → hashing de senhas com parâmetros fortes.
  - **JWT + Refresh** → sessão curta com renovação segura e logout granular.

  ---

  ## Operação & Observabilidade

  - **Health**: `GET /health`
  - **Migrations**: aplicadas em startup via `Database.Migrate()` (sem `EnsureCreated`).
  - **Logs**: Serilog (Console); sugere-se JSON em prod.
  - **Traces/Métricas**: OTel (AspNetCore, HttpClient, EF, Runtime).
  - **Segurança Operacional**: DataProtection Keys persistidas; segredos via env/secret-manager; CORS por ambiente; TLS em produção.

  ---

  ## Como Rodar Localmente

  ### Com Docker Compose
  ```bash
  cd build/docker
  docker compose up -d --build
  # Web:  http://localhost:5173
  # API:  http://localhost:8080/swagger
  ```

  ### Sem Docker
  **Banco**: Postgres local (5432) e DB `fluxodb` (user/pass `app`/`app` ou ajuste a string).

  **API**
  ```bash
  dotnet restore
  dotnet tool restore
  cd src/Desafio.FluxoCaixa.Api
  dotnet run
  # http://localhost:8080/swagger
  ```

  **Web**
  ```bash
  cd src/WebApp
  npm ci
  npm run dev
  # http://localhost:5173
  ```

  ---

  ## Endpoints Principais

  - `POST /api/v1/auth/register` — { userName, email, password }
  - `POST /api/v1/auth/login` — { userNameOrEmail, password } → { accessToken, refreshToken, expiresInSeconds }
  - `POST /api/v1/auth/refresh` — { refreshToken } → rotação
  - `POST /api/v1/auth/logout` — { refreshToken, allDevices? } (se `allDevices=true`, requer Autorização)
  - `GET /api/v1/lancamentos?de=YYYY-MM-DD&ate=YYYY-MM-DD`
  - `POST /api/v1/lancamentos`
  - `DELETE /api/v1/lancamentos/{id}`
  - `GET /api/v1/relatorios/saldo-diario?de=YYYY-MM-DD&ate=YYYY-MM-DD&saldoInicial=0`

  ---

  ## Licença

  MIT — ajuste conforme a necessidade da organização.
