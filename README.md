# Blog API

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13-239120?logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-7-DC382D?logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)
![License](https://img.shields.io/badge/License-Educational-blue)

**API RESTful para gerenciamento de blog construída com .NET 9 e Clean Architecture**

Esta é uma API completa para gerenciar um blog, incluindo posts, categorias e usuários. O projeto foi desenvolvido como parte do meu portfólio profissional para demonstrar habilidades em .NET 9, Clean Architecture, padrões de design e boas práticas de desenvolvimento.

A aplicação implementa recursos avançados como cache distribuído com Redis, rate limiting inteligente, autenticação JWT com autorização baseada em roles, validações de domínio usando FluentValidation e uma suite abrangente de testes unitários. Toda a infraestrutura está containerizada com Docker, permitindo deployment rápido e consistente.

## Características Principais

- **Clean Architecture** com separação clara em 4 camadas (Domain, Application, Infrastructure, API)
- **Autenticação JWT** com autorização baseada em roles (Author, Admin)
- **CRUD completo** de Posts e Categories com validações de negócio
- **Relacionamento Many-to-Many** entre Posts e Categories
- **Cache distribuído** com Redis usando cache-aside pattern (GetOrSetAsync)
- **Rate limiting distribuído** por IP e usuário autenticado
- **FluentValidation** para validações de domínio centralizadas
- **Password hashing** com BCrypt para segurança
- **Paginação** de resultados com metadata de paginação
- **Global exception handling** seguindo RFC 7807 (Problem Details)
- **Entity Framework Core** com PostgreSQL e migrations automáticas
- **Repository pattern** para abstração de acesso a dados
- **Testes unitários** com xUnit, Moq e FluentAssertions (17+ testes)
- **Documentação OpenAPI/Swagger** com interface Scalar UI
- **Docker Compose** com health checks para PostgreSQL, Redis e API
- **Graceful degradation** - aplicação funciona mesmo se Redis estiver offline

## Tecnologias Utilizadas

### Backend Framework
- .NET 9.0
- ASP.NET Core Web API
- C# 13

### Database & Caching
- PostgreSQL 16 (Alpine)
- Redis 7 (Alpine)
- Entity Framework Core 9.0
- StackExchange.Redis 2.8.16

### Segurança
- JWT Bearer Authentication
- BCrypt.Net-Next
- Role-based Authorization

### Bibliotecas
- AutoMapper 13.0 (mapeamento de objetos)
- FluentValidation 11.10 (validações de domínio)
- xUnit 2.9 (framework de testes)
- Moq 4.20 (mocking)
- FluentAssertions 6.12 (assertions expressivas)

### DevOps
- Docker & Docker Compose
- Multi-stage Dockerfile para otimização de imagem

### Documentação
- OpenAPI
- Scalar UI

## Arquitetura

Este projeto segue os princípios da **Clean Architecture**, garantindo separação de responsabilidades, testabilidade e manutenibilidade. O fluxo de dependências vai sempre das camadas externas para as internas, com o Domain layer no centro sem nenhuma dependência externa.

```
┌─────────────────────────────────────────────┐
│           API Layer (Presentation)          │
│  Controllers, Middleware, Configuration     │
└────────────────┬────────────────────────────┘
                 │ depends on
                 ▼
┌─────────────────────────────────────────────┐
│        Application Layer (Use Cases)        │
│     Services, DTOs, Mappings, Interfaces    │
└────────────────┬────────────────────────────┘
                 │ depends on
                 ▼
┌─────────────────────────────────────────────┐
│           Domain Layer (Core)               │
│   Entities, Validators, Domain Exceptions   │
│        (No external dependencies)           │
└─────────────────────────────────────────────┘
                 ▲
                 │ depends on
┌────────────────┴────────────────────────────┐
│       Infrastructure Layer (External)       │
│  Repositories, DbContext, Redis, JWT, etc.  │
└─────────────────────────────────────────────┘
```

### Camadas

**Domain Layer (Core)**
Contém as entidades de negócio (Post, Category, User), validadores FluentValidation e exceções de domínio. Esta camada não possui dependências externas e representa as regras de negócio puras da aplicação.

**Application Layer (Use Cases)**
Implementa os casos de uso através de serviços (PostService, CategoryService, AuthService). Inclui DTOs para transferência de dados, profiles do AutoMapper e interfaces de serviços. Esta camada orquestra o fluxo de dados entre as camadas externa e interna.

**Infrastructure Layer (External)**
Implementa os detalhes técnicos: repositórios usando Entity Framework Core, DbContext, serviços externos (Redis cache, JWT token service, BCrypt password service) e registro de dependências.

**API Layer (Presentation)**
Expõe a aplicação através de controllers REST, middleware de tratamento de erros global, configurações (Program.cs) e serviços específicos da camada de apresentação (rate limiters).

## Estrutura do Projeto

```
blog-api/
├── src/
│   ├── BlogAPI.API/              # Presentation Layer
│   │   ├── Controllers/          # API endpoints
│   │   ├── Configuration/        # Settings, handlers
│   │   └── Services/             # Rate limiters
│   ├── BlogAPI.Application/      # Application Layer
│   │   ├── DTOs/                 # Data transfer objects
│   │   ├── Mappings/             # AutoMapper profiles
│   │   ├── Services/             # Business logic services
│   │   └── Common/               # Cache keys, shared
│   ├── BlogAPI.Domain/           # Domain Layer
│   │   ├── Entities/             # Post, Category, User
│   │   ├── Validators/           # FluentValidation
│   │   ├── Interfaces/           # Repository interfaces
│   │   └── Exceptions/           # Domain exceptions
│   └── BlogAPI.Infrastructure/   # Infrastructure Layer
│       ├── Data/                 # DbContext, Configurations
│       ├── Repositories/         # EF Core repositories
│       └── Services/             # Redis, JWT, BCrypt
├── tests/
│   └── BlogAPI.UnitTests/        # Unit tests (xUnit)
├── docker-compose.yml             # Container orchestration
├── Dockerfile                     # Multi-stage build
```

## Como Executar o Projeto

### Pré-requisitos

- **Opção 1 (Recomendado)**: Docker e Docker Compose
- **Opção 2**: .NET 9.0 SDK + PostgreSQL 16 + Redis 7

### Opção 1: Docker Compose (Recomendado)

Esta é a forma mais rápida de executar o projeto. O Docker Compose irá subir todos os serviços necessários (PostgreSQL, Redis e a API) com um único comando.

```bash
# Clone o repositório
git clone https://github.com/vittordeaguiar/blog-api.git
cd blog-api

# Inicie todos os containers (API + PostgreSQL + Redis)
docker-compose up -d

# A API estará disponível em:
# - API: http://localhost:8080
# - Documentação Scalar: http://localhost:8080/scalar/v1
# - Swagger JSON: http://localhost:8080/swagger/v1/swagger.json

# Para ver logs da aplicação
docker-compose logs -f api

# Para parar todos os containers
docker-compose down
```

### Opção 2: Execução Local

Se preferir executar localmente sem Docker, certifique-se de ter PostgreSQL e Redis instalados e rodando.

```bash
# Configure as connection strings em appsettings.Development.json:
# - PostgreSQL: localhost:5432
# - Redis: localhost:6379

# Restaure as dependências
dotnet restore

# Execute as migrations (cria o banco automaticamente)
dotnet ef database update --project src/BlogAPI.Infrastructure --startup-project src/BlogAPI.API

# Execute a aplicação
dotnet run --project src/BlogAPI.API

# Acesse http://localhost:8080
```

## Endpoints da API

A API está organizada em três grupos principais de endpoints:

### Autenticação (`/v1/auth`)

- `POST /v1/auth/register` - Registrar novo usuário (role: Author ou Admin)
- `POST /v1/auth/login` - Fazer login e obter token JWT

### Posts (`/api/v1/posts`)

- `GET /api/v1/posts` - Listar posts paginados (público, com cache de 5 minutos)
- `GET /api/v1/posts/{slug}` - Obter post por slug (público, com cache de 10 minutos)
- `POST /api/v1/posts` - Criar novo post (requer autenticação)
- `PUT /api/v1/posts/{id}` - Atualizar post (apenas owner ou admin)
- `DELETE /api/v1/posts/{id}` - Deletar post (apenas owner ou admin)

### Categories (`/v1/categories`)

- `GET /v1/categories` - Listar todas categorias (público, cache de 60 minutos)
- `POST /v1/categories` - Criar categoria (somente admin)
- `DELETE /v1/categories/{id}` - Deletar categoria (somente admin)

Para explorar todos os endpoints interativamente, acesse a documentação Scalar em `/scalar/v1` após iniciar a aplicação.

## Autenticação e Autorização

A API utiliza JWT (JSON Web Tokens) para autenticação stateless. Aqui está um fluxo completo de como usar:

### 1. Registrar um novo usuário

```bash
curl -X POST http://localhost:8080/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao@example.com",
    "password": "SenhaSegura123!",
    "role": "Author"
  }'
```

### 2. Fazer login e obter o token

```bash
curl -X POST http://localhost:8080/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@example.com",
    "password": "SenhaSegura123!"
  }'
```

Resposta: `{"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."}`

### 3. Usar o token em requisições autenticadas

```bash
curl -X POST http://localhost:8080/api/v1/posts \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Meu Primeiro Post",
    "content": "Conteúdo do post aqui...",
    "categoryIds": ["uuid-da-categoria"]
  }'
```

### Roles Disponíveis

- **Author**: Pode criar posts e editar/deletar seus próprios posts
- **Admin**: Pode gerenciar categorias e editar/deletar qualquer post

## Cache e Performance

### Estratégia de Cache com Redis

A aplicação implementa cache distribuído usando Redis com diferentes TTLs (Time To Live) baseados no tipo de dado:

- **Categories**: 60 minutos - categorias mudam raramente e têm alta taxa de reutilização
- **Posts (lista paginada)**: 5 minutos - balanceia freshness dos dados vs performance
- **Posts (detalhe individual)**: 10 minutos - posts específicos mudam menos que listas

O cache utiliza o **cache-aside pattern** através do método `GetOrSetAsync`, que verifica o cache primeiro e, em caso de miss, consulta o banco de dados e armazena o resultado automaticamente.

A **invalidação automática** do cache ocorre em todas as operações de escrita (create, update, delete), garantindo que os dados em cache estejam sempre consistentes com o banco de dados.

### Rate Limiting Distribuído

O rate limiting é implementado usando Redis Sorted Sets com o algoritmo sliding window, permitindo que múltiplas instâncias da API compartilhem os mesmos contadores.

**Limites configurados:**

- **Por IP**: 1000 requests/minuto (ambiente dev), 100 requests/minuto (produção)
- **Por Usuário Autenticado**: 2000 requests/minuto (ambiente dev), 200 requests/minuto (produção)

Quando um limite é excedido, a API retorna HTTP 429 (Too Many Requests) com o header `Retry-After` indicando quando o cliente pode tentar novamente.

A implementação usa **graceful degradation**: se o Redis estiver offline, as requisições são permitidas (fail-open) para garantir que a API continue funcionando.

## Testes

O projeto possui uma suite abrangente de testes unitários cobrindo as camadas Domain, Application e Infrastructure.

### Executar os testes

```bash
# Executar todos os testes
dotnet test

# Executar com detalhes de cobertura
dotnet test /p:CollectCoverage=true
```

### Cobertura de Testes

**Domain Layer**
Testes das entidades de domínio, incluindo comportamentos de publish/unpublish de posts e gerenciamento de categorias.

**Application Layer**
Mais de 17 testes no PostService cobrindo:
- Criação de posts (validações, author válido/inválido)
- Listagem paginada (parâmetros válidos/inválidos)
- Busca por ID e slug
- Atualização com verificação de autorização (owner/admin)
- Deleção com verificação de autorização
- Publish/Unpublish de posts
- Vinculação de posts com categorias

**Infrastructure Layer**
Testes do BCryptPasswordService verificando hash e verificação de passwords.

### Frameworks de Teste

- **xUnit**: Framework de testes com suporte a teoria e fixtures
- **Moq**: Mocking de dependências (repositories, services) para testes isolados
- **FluentAssertions**: Assertions expressivas e legíveis que facilitam a compreensão dos testes

## Configuração

### Variáveis de Ambiente (Docker Compose)

```yaml
# PostgreSQL
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=blogapi_dev;Username=postgres;Password=postgres

# JWT
JwtSettings__SecretKey=sua-chave-secreta-aqui

# Redis
RedisSettings__ConnectionString=blogapi-redis:6379
RedisSettings__Password=redis_dev_password
RedisSettings__Enabled=true

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_HTTP_PORTS=8080
```

### Arquivos de Configuração

**appsettings.json**
Configurações para ambiente de produção com valores padrão seguros.

**appsettings.Development.json**
Configurações específicas para desenvolvimento:
- CORS liberado para localhost (portas 3000, 3001, 4200, 5173, 8080)
- Logging detalhado do Entity Framework Core
- Rate limits mais permissivos para facilitar testes
- Configurações de conexão para ambiente local

## Progresso do Projeto

Este projeto está **90% completo**, com 9 das 10 issues principais finalizadas:

- Estrutura Clean Architecture
- Entity Framework Core com PostgreSQL
- Middleware de tratamento de erros e documentação Swagger/Scalar
- Domínio de usuário com criptografia BCrypt
- Autenticação JWT com roles
- CRUD de Categories e Posts
- Regras de negócio (publish/unpublish, many-to-many)
- Controllers da API
- Testes unitários abrangentes
- Dockerização (em finalização)

Para acompanhar o progresso detalhado, consulte o [ROADMAP.md](./ROADMAP.md) e o [GitHub Project](https://github.com/vittordeaguiar/blog-api/projects).

## Aprendizados e Decisões Técnicas

### Clean Architecture

Escolhi implementar Clean Architecture para garantir separação clara de responsabilidades e facilitar testes. O Domain layer não possui dependências externas, o que permite testar as regras de negócio de forma completamente isolada. Essa arquitetura também facilitou a substituição de implementações - por exemplo, o NullCacheService serve como fallback automático quando o Redis está offline, sem afetar nenhuma outra parte do código.

### Redis para Cache e Rate Limiting

Implementar cache distribuído com Redis foi fundamental para permitir escalabilidade horizontal da API. Múltiplas instâncias podem compartilhar o mesmo cache e os mesmos contadores de rate limiting, algo impossível com cache in-memory tradicional. A decisão de implementar graceful degradation significa que a API continua funcionando mesmo se o Redis falhar - ela simplesmente opera sem cache até que o Redis volte.

### FluentValidation no Domain Layer

Centralizar validações de negócio nas entidades garante que as regras sejam respeitadas independentemente de onde a entidade é instanciada. Os validadores são reutilizáveis, testáveis e mantêm o código limpo ao separar a lógica de validação da lógica de negócio.

### Repository Pattern

A abstração do acesso a dados através do Repository Pattern facilita enormemente os testes (podemos fazer mock dos repositories) e nos dá flexibilidade para trocar o ORM ou adicionar outras fontes de dados sem afetar as camadas superiores da aplicação.

### JWT Bearer Authentication

Optei por JWT para implementar autenticação stateless, eliminando a necessidade de sessões no servidor. Os tokens JWT carregam claims (user ID, role) e são validados a cada requisição. Esta abordagem é ideal para APIs RESTful e arquiteturas distribuídas, permitindo que qualquer instância da API valide tokens sem consultar um armazenamento centralizado de sessões.

### Cache-Aside Pattern (GetOrSetAsync)

O padrão cache-aside implementado através do método `GetOrSetAsync` simplifica significativamente o código. Em vez de repetir a lógica "verifica cache, se não tem consulta DB, armazena em cache" em cada método, encapsulamos isso em um único método reutilizável.

### Rate Limiting Distribuído

Implementei rate limiting usando Redis Sorted Sets com o algoritmo sliding window. Esta abordagem garante que os limites sejam respeitados de forma precisa mesmo com múltiplas instâncias da API rodando simultaneamente, algo crítico para ambientes de produção escaláveis.

## Contato

GitHub: [vittordeaguiar](https://github.com/vittordeaguiar)

## Licença

Este projeto foi desenvolvido para fins educacionais e como parte do meu portfólio profissional. Sinta-se livre para explorar o código e usar como referência para seus próprios projetos.

## Screenshots
<img width="1892" height="943" alt="image" src="https://github.com/user-attachments/assets/d000ecc9-3d66-4a2a-8b9d-d88e74000acf" />
<img width="1421" height="860" alt="image" src="https://github.com/user-attachments/assets/c0f1ff96-ca01-4aac-adbe-f1e79bed84d0" />

