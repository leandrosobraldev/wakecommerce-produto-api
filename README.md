# Wake Commerce - API de Produtos

API REST em .NET 8 para CRUD de Produtos, desenvolvida como solução de avaliação.

## Estrutura da Solution

| Projeto | Descrição |
|--------|-----------|
| **WakeCommerce.API** | Camada de apresentação: controllers, middleware e configuração da aplicação. Expõe os endpoints REST. |
| **WakeCommerce.Application** | Regras de negócio, DTOs, serviços de aplicação e mapeamentos (AutoMapper). Orquestra o uso do domínio e da infraestrutura. |
| **WakeCommerce.Domain** | Entidades, interfaces de repositório e contratos do domínio. Não depende de frameworks externos. |
| **WakeCommerce.Infrastructure** | Implementação de persistência: Entity Framework Core, repositórios, contexto e migrations. |
| **WakeCommerce.CrossCutting (WakeCommerce.IoC)** | Injeção de dependência: registro de serviços, DbContext e repositórios. |
| **Aplication.Test** | Projeto de testes unitários (xUnit). |
| **WakeCommerce.API.IntegrationTests** | Projeto de testes de integração (WebApplicationFactory, banco em memória). |

## Tecnologias

- **.NET 8**
- **Entity Framework Core 8** (abordagem **code-first**)
- **SQL Server** (configurável via connection string)
- **AutoMapper** para mapeamento entidade ↔ DTO
- **Swagger/OpenAPI** para documentação da API
- **JWT Bearer** para autenticação em produção
- **Health Checks** (incl. verificação do banco)
- **CORS** configurável via `Cors:AllowedOrigins`
- **Rate limiting** (100 req/min por IP)
- **Logging estruturado** (ILogger em Service e Repository)
- **CancellationToken** propagado em toda a stack

## Entity Framework – Abordagem Code-First

O projeto utiliza **Entity Framework Core em modo code-first**:

1. As entidades são definidas no projeto **WakeCommerce.Domain** (ex.: `Produto`).
2. O **AppDbContext** no projeto **WakeCommerce.Infrastructure** declara os `DbSet` e configurações (precision, etc.).
3. As **migrations** são geradas a partir do modelo (ex.: `CriacaoInicial`).
4. Ao subir a aplicação, a base é criada/atualizada via migrations e **5 produtos iniciais são inseridos** (seed) se a tabela estiver vazia.

Para aplicar migrations manualmente:

```bash
cd src/WakeCommerce.Infrastructure
dotnet ef database update --startup-project ../WakeCommerce.API
```

## Executar com Docker (para recrutadores)

Requisitos: **Docker** e **Docker Compose** instalados.

Na raiz do repositório:

```bash
docker compose up -d
```

- **API**: `http://localhost:5000`
- **Swagger**: `http://localhost:5000/swagger`
- **SQL Server**: porta `1433` (usuário `sa`, senha `StrongPassword@123`, banco `WakeCommerceDb`)

Na primeira subida o SQL Server pode levar 20–40 segundos para ficar pronto; a API faz retentativas automáticas. Se aparecer erro ao acessar, aguarde um pouco e tente de novo.

Para obter um token JWT no Swagger: **Authorize** → em "Value" informe só o token (sem a palavra "Bearer"). O token é obtido em:

```http
POST http://localhost:5000/api/v1/Auth/token
Content-Type: application/json

{"userName": "dev"}
```

Use o valor do campo `token` da resposta no Authorize do Swagger.

Parar os containers: `docker compose down`.

---

## Como executar (local, sem Docker)

1. Configure a connection string em `appsettings.json` (ou User Secrets) em **WakeCommerce.API**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=WakeCommerce;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

2. Execute a API:

```bash
cd src/WakeCommerce.API
dotnet run
```

3. Acesse o Swagger em: `https://localhost:<porta>/swagger`

### Autenticação (JWT)

Os endpoints de produtos exigem **Bearer JWT**. Para obter um token em desenvolvimento:

```http
POST /api/v1/Auth/token
Content-Type: application/json

{"userName": "dev"}
```

Use o `token` retornado no header: `Authorization: Bearer <token>`. No Swagger, clique em **Authorize** e informe `Bearer <token>`.

Em **produção**, altere `Jwt:SecretKey` em `appsettings.json` (ou variáveis de ambiente) e prefira um identity provider (ex.: IdentityServer, Auth0).

### Health e CORS

- **Health**: `GET /health` — verifica a saúde da API e do banco.
- **CORS**: origens permitidas em `Cors:AllowedOrigins` (ex.: frontends em localhost:3000, localhost:5173).

## Endpoints principais

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/v1/Auth/token` | Gera JWT (desenvolvimento; [AllowAnonymous]) |
| GET | `/api/v1/Produtos` | Lista produtos (paginação, ordenação, busca por nome) |
| GET | `/api/v1/Produtos/{id}` | Busca produto por id |
| POST | `/api/v1/Produtos` | Cria produto (valor não pode ser negativo) |
| PUT | `/api/v1/Produtos/{id}` | Atualiza produto |
| DELETE | `/api/v1/Produtos/{id}` | Remove produto |

## Testes

- **Testes unitários**: projeto `Aplication.Test` (xUnit).
- **Testes de integração**: projeto `WakeCommerce.API.IntegrationTests` (WebApplicationFactory, EF InMemory).

Execute todos os testes:

```bash
dotnet test WakeCommerce.slnx
```

## GitHub Actions

O repositório inclui um workflow em `.github/workflows/tests.yml` que executa **restore**, **build** e **testes** (unitários e de integração) em cada push e pull request para `main`/`master`.

## Requisitos atendidos

- **Funcionais**: CRUD completo; listar com ordenação e busca por nome (lista vazia retorna 200); valor do produto não pode ser negativo (validação no domínio e nos DTOs); POST retorna 201 com recurso criado e `Location`; DELETE retorna 204 No Content.
- **Não funcionais**: .NET 8, Entity Framework (code-first), entidade Produto com Nome, Estoque e Valor (Preço), seed com 5 produtos, projeto de testes unitários, README com explicação da Solution e do uso do EF.
- **Bônus**: padrões de projeto (Repository, Service, DTOs), GitHub Actions para testes, testes de integração.
- **Boas práticas aplicadas**: `DomainExceptionValidation` retorna **422 Unprocessable Entity**; logging estruturado (ILogger); **CancellationToken** em toda a stack; **Health Checks** (incl. DbContext); **JWT** para produção; **CORS** e **rate limiting** (100 req/min por IP).
