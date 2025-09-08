  # Desafio Arquitetura de Solução - Verity
  
  ## Fluxo de Caixa Diário — Clean Architecture + CQRS + JWT (NET 9)

  Aplicação **full-stack** para controle do **fluxo de caixa diário** por usuário. Permite registrar **lançamentos** de **Débito/Crédito**, consultar por período e gerar **relatório consolidado** com **saldo do dia** e **saldo acumulado**, com autenticação segura e telemetria pronta para ambientes de produção.

  - **Backend:** .NET 9 (ASP.NET Core) com **Clean Architecture + CQRS**, **EF Core (PostgreSQL)** e **migrations**.
  - **Segurança:** **JWT** com **refresh rotativo** e logout individual/global; senhas com **PBKDF2**; **CORS** por ambiente.
  - **Observabilidade:** **Serilog** (logs estruturados) + **OpenTelemetry** (traces/métricas via OTLP).
  - **Frontend:** **React + Vite + TypeScript + Tailwind**, com **componentes reutilizáveis** e gráfico do saldo acumulado.
  - **Execução:** **Docker Compose** (API, Web e Postgres) ou execução local via `dotnet run` e `npm run dev`.
  - **Qualidade:** validações (FluentValidation), testes de domínio/aplicação, e documentação com diagramas Mermaid no README.

  ### Funcionalidades
  - **Lançamentos:** criar, listar (por intervalo de datas) e remover.
  - **Relatórios:** obter o **saldo diário consolidado** (débitos, créditos, saldo do dia e acumulado) com **saldo inicial** opcional.
  - **Autenticação:** **cadastro, login, refresh e logout** para manter sessões curtas e seguras.

  ---

  ## Índice
  1. [Requisitos de Negócio](#requisitos-de-negócio)
  2. [Domínios Funcionais & Capacidades](#domínios-funcionais--capacidades)
  3. [Requisitos Funcionais e Não Funcionais](#requisitos-funcionais-e-não-funcionais)
  4. [Arquitetura Alvo (com Diagramas Mermaid)](#arquitetura-alvo-com-diagramas-mermaid)
  5. [Justificativas Técnicas](#justificativas-técnicas)
  6. [Operação e Observabilidade](#operação-e-observabilidade)
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
  - **Identidade e Acesso**: cadastro, login, JWT, refresh, logout.
  - **Lançamentos**: registrar, listar, excluir.
  - **Relatórios**: consolidação diária; futuros CSV/PDF.
  - **Plataforma e Observabilidade**: logs, métricas, traces, health.
  - **Segurança e Conformidade**: hashing de senha, CORS/TLS, proteção de dados.

  **Capacidades de Negócio**:
  - **Gerir Usuário** (onboarding, autenticação, sessão).
  - **Gerir Lançamentos** (CRUD básico).
  - **Apurar Saldo Diário** (consolidação por data + acumulado).
  - **Disponibilizar Relatórios** (UI + gráfico).
  - **Operar com Confiabilidade** (migrations, monitoramento, auditoria mínima).

  ---

  ## Requisitos Funcionais e Não Funcionais

  **Funcionais**
  1. **Cadastro de Usuário** (`POST /auth/register`): cria conta com validação de `userName`, `email` e força mínima de senha; retorna 201 ou erros de validação.
  2. **Login** (`POST /auth/login`): autentica por `userNameOrEmail + password`; emite `accessToken` (curta duração) e `refreshToken` (longa duração) para manter sessão.
  3. **Refresh de Token** (`POST /auth/refresh`): renova a sessão de forma **rotativa** (novo refresh invalida o anterior), sem pedir credenciais novamente.
  4. **Logout** (`POST /auth/logout`): finaliza sessão revogando o refresh **atual** ou **todos** os dispositivos do usuário.
  5. **Criar Lançamento** (`POST /lancamentos`): registra **débitos** (1) e **créditos** (2) com `data`, `valor` numérico e `descrição`; vinculado ao usuário autenticado.
  6. **Listar Lançamentos** (`GET /lancamentos?de&ate`): consulta por período, ordenado; pensado para paginação futura sem quebrar contrato.
  7. **Remover Lançamento** (`DELETE /lancamentos/{id}`): exclusão lógica/física conforme estratégia; garante isolamento por usuário.
  8. **Relatório de Saldo Diário** (`GET /relatorios/saldo-diario?de&ate&saldoInicial`): consolida por dia (débitos, créditos, saldo do dia e **acumulado**) com **saldo inicial** 

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
    U[Usuario] -->|SPA| Web["WebApp (React/Vite)"]
    Web -->|HTTP + JWT| Api["API .NET 9 Clean Arch + CQRS"]
    Api -->|EF Core| DB["(PostgreSQL)"]
    Api -->|OTLP| OTel["OpenTelemetry Collector / APM"]
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
      Docker["Docker Compose"]
      WebD["Web : 5173"]
      ApiD["API : 8080"]
      PgD[("PostgreSQL local")]
      Docker --> WebD
      Docker --> ApiD
      ApiD --> PgD
    end
  
    subgraph Prod
      Ingress["Ingress-LB"]
      WebP["Web (Nginx)"]
      ApiP["API (replicas)"]
      PgP[("PostgreSQL Gerenciado")]
      APM["OTLP / APM"]
      Ingress --> WebP
      Ingress --> ApiP
      ApiP --> PgP
      ApiP --> APM
    end
  ```

  ---

  ## Justificativas Técnicas
  
  As escolhas foram guiadas pelos requisitos **funcionais** (cadastro/login, lançamentos e relatório consolidado) e **não funcionais** (segurança, desempenho, escalabilidade, resiliência, observabilidade, manutenibilidade e portabilidade). O objetivo é entregar valor rápido, com **qualidade arquitetural** e **rota de evolução** clara.
  
  ### Plataforma e Arquitetura
  - **.NET 9 + ASP.NET Core** — runtime performático, DI nativa, pipeline HTTP eficiente e ótimo suporte a JSON/containers.
  - **Clean Architecture + CQRS** — separa domínio/aplicação de infraestrutura/UI; facilita testes e evolução. CQRS permite otimizar caminhos de **escrita** (commands) e **leitura** (queries) sem acoplamento.
  - **MediatR + Behaviors** — centraliza **validação**, **telemetria** e demais *cross-cuttings* no pipeline, reduzindo repetição em controllers/handlers.
  - **Monólito modular (neste escopo)** — menor custo operacional inicial. O particionamento por domínios/capacidades permite extração futura para microserviços **sem reescrever** o core.
  
  ### Persistência e Acesso a Dados
  - **PostgreSQL** — relacional robusto/portável, com `numeric(14,2)` para valores monetários (sem erros de ponto flutuante).
  - **EF Core (Npgsql)** — produtividade com LINQ, **migrations** versionadas e rastreamento quando necessário. Consultas do relatório usam agregações SQL e **índice `(UserId, Data)`** para período.
  - **Migrations** — aplicadas automaticamente em Dev/Compose; em Produção, recomendadas via *init job* ou pipeline (janela controlada).
  - **Integridade & concorrência** — FKs explícitas, isolamento por usuário. Exclusão de lançamentos **física** (simples), com espaço para evoluir para **lógica** (auditoria) sem quebrar contrato.
  
  ### Segurança e Identidade
  - **JWT (HS256)** com `aud/iss/exp/iat/nbf/jti`; **access token** curto (~15 min) e **refresh token** (~7 dias) **rotativo** (novo refresh invalida o anterior). Refresh tokens são armazenados **hasheados** no banco.
  - **Senhas** — **PBKDF2-SHA256** com ≥150k iterações e *salt* único; parâmetros ajustáveis por ambiente (possível **Argon2id** futuro).
  - **CORS/TLS/HSTS** — em Dev libera `http://localhost:5173`; em Prod restringe ao domínio oficial, TLS obrigatório e **HSTS**.
  - **Data Protection Keys** persistidas fora do contêiner para permitir **scale-out**; **rate limiting** em login/APIs para reduzir brute force/abuso.
  - **Logout** por dispositivo (revoga refresh específico) ou **global** (revoga todos).
  - **Por que não OIDC externo agora?** Para o escopo do desafio, autenticação **in-app** é suficiente. A arquitetura aceita plugar IdP (AAD/Auth0/Keycloak) depois via **JWT Bearer/OpenIdConnect** sem grandes refatorações.
  
  ### Frontend
  - **React + Vite + TypeScript** — DX rápida, *hot reload* e *type safety* ponta a ponta; **Vite** produz builds leves com *code-splitting*.
  - **Tailwind** — consistência visual e produtividade; implementamos **componentes reutilizáveis** (Button, Input, Select, Card, Table) como base de um design system simples.
  - **Validação** — **Zod** no registro (feedback imediato); espaço para evoluir com máscaras de moeda, i18n e ARIA/a11y.
  - **Gráficos** — **Chart.js** cobre bem o saldo acumulado; pode migrar para Recharts/ECharts conforme necessidade.
  
  ### Observabilidade e Operação
  - **Serilog** — logs **estruturados** (JSON) com correlação (`traceId/spanId`) e campos de negócio (`userId`, `path`, `statusCode`, `elapsedMs`); fácil rotear para Elastic/CloudWatch/Splunk.
  - **OpenTelemetry** — instrumentação de **AspNetCore**, **HttpClient**, **EF Core** e **Runtime**, exportando via **OTLP** (Tempo/Jaeger/Grafana/Datadog) sem *vendor lock-in*.
  - **Health checks** (startup/liveness/readiness) e **graceful shutdown** compatíveis com K8s/ECS.
  - **12-Factor** — configuração por variáveis de ambiente (`Jwt__Key`, `ConnectionStrings__Postgres`, `OTEL_EXPORTER_OTLP_ENDPOINT`), `.env.example` e Dockerfile multi-stage.
  
  ### Escalabilidade e Evolução
  - **API stateless** atrás de **Load Balancer** — escala horizontal sem *sticky session*.
  - **Banco** — suporte a **read replicas** para leituras intensas e **materialized views/ETL** para janelas muito longas; quando surgirem integrações assíncronas, considerar **Outbox** e **idempotência**.
  - **Estratégia evolutiva** — começar **monolítico** para otimizar *lead time*; migrar para **microserviços por bounded context** quando métricas apontarem ganho real (evita complexidade prematura).
  
  ### Qualidade e Testes
  - **FluentValidation** em comandos; **Behaviors** do MediatR padronizam validação/telemetria.
  - **Testes de Domínio** (saldo diário/acumulado) garantem regra de negócio; **testes de aplicação** cobrem handlers; **integração** com Postgres em contêiner (ou Testcontainers) valida repositórios/EF.
  - **Padrões de código** — Analyzers do .NET, `EditorConfig`, ESLint/TS e (opcional) Prettier (`npm run fmt`).
  
  ### Alternativas consideradas (e por que não agora)
  - **Microserviços desde o início** — eleva custo de operação/observabilidade e orquestração sem necessidade clara neste escopo; **monólito modular** entrega mais rápido com caminho de extração futura.
  - **Event Sourcing** — poderoso para auditoria/temporalidade, mas aumenta complexidade (projeções, replays); requisitos atuais não justificam.
  - **NoSQL** — não há necessidade de esquema flexível nem latência sub-ms; o relacional atende melhor agregações e integridade.
  - **OIDC IdP externo** — agregaria gestão/custos; a solução **in-app** atende e continua interoperável com IdPs no futuro.

  ---

  ## Operação e Observabilidade

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
