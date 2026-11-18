# DDD Mentor AI

Plataforma de estudo de Domain-Driven Design. O usuário abre sessões de estudo por tópico e nível e conversa com um mentor. O projeto também serve de exemplo prático dos conceitos que ensina: agregado, value objects, portas e adaptadores, separação entre escrita e leitura.

> **Status:** em desenvolvimento. Autenticação, sessões e mensagens estão prontas. O mentor de IA é a próxima fase (ver [Roadmap](#roadmap)).

## Arquitetura

```
backend/src
├── DddMentorAI.Domain          Agregado StudySession, value objects, porta IStudySessionRepository
├── DddMentorAI.Application     Casos de uso, DTOs, portas (IUnitOfWork, IStudySessionQueries, IEmailSender)
├── DddMentorAI.Infrastructure  EF Core/PostgreSQL, Identity, repositório, consultas projetadas
└── DddMentorAI.API             Controllers, JWT, composição (DI)
```

As dependências apontam para dentro: `API → Infrastructure → Application → Domain`.

- **Escrita:** o caso de uso carrega o agregado pelo repositório, chama um método de domínio (`StudySession.Start`, `AddUserMessage`) e confirma com `IUnitOfWork`.
- **Leitura:** as consultas projetam direto para DTO, sem carregar nem rastrear o agregado.
- **Tempo:** `TimeProvider` é injetado, então o domínio nunca chama `DateTime.UtcNow`.
- **Isolamento por usuário:** toda leitura e escrita filtra pelo dono da sessão. Uma sessão de outro usuário responde como "não encontrada".

As decisões e os trade-offs estão em [`docs/adr`](docs/adr).

## Stack

.NET 8 · ASP.NET Core · EF Core 8 · PostgreSQL 16 · ASP.NET Identity + JWT · xUnit · React + Vite (frontend, ainda inicial)

## Como rodar

Pré-requisitos: .NET 8 SDK e Docker.

```bash
docker compose up -d
```

```bash
dotnet tool restore
```

```bash
dotnet user-secrets set "Jwt:Key" "<uma chave aleatória com 32+ caracteres>" --project backend/src/DddMentorAI.API
```

```bash
dotnet ef database update -p backend/src/DddMentorAI.Infrastructure -s backend/src/DddMentorAI.API
```

```bash
dotnet run --project backend/src/DddMentorAI.API
```

O Swagger fica disponível em `/swagger` no ambiente de desenvolvimento. Em dev, os e-mails de confirmação vão para o log (`DevEmailSender`).

A API não sobe sem `Jwt:Key`. Fora do ambiente de desenvolvimento, use a variável `Jwt__Key`.

## Testes

```bash
dotnet test backend/DddMentorAI.sln
```

- **Domínio:** invariantes do agregado e dos value objects.
- **Aplicação:** casos de uso com fakes e relógio fixo.
- **Persistência:** mapeamento EF do agregado com o provider InMemory. Isso cobre change tracking e conversões, mas não SQL nem constraints do PostgreSQL.

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/auth/register` | Cadastro (envia confirmação de e-mail) |
| POST | `/api/auth/login` | Login, retorna JWT |
| GET | `/api/studysessions` | Sessões do usuário |
| POST | `/api/studysessions` | Cria sessão |
| GET | `/api/studysessions/{id}` | Detalhes |
| GET | `/api/studysessions/{id}/messages` | Mensagens |
| POST | `/api/studysessions/{id}/messages` | Envia mensagem |

## Roadmap

- [x] Autenticação com confirmação de e-mail
- [x] Sessões e mensagens
- [x] Agregado rico, separação escrita/leitura e testes
- [ ] Mentor de IA: porta `IMentorAssistant`, prompt por nível, limite de custo por usuário
- [ ] Frontend: login, sessões, chat
- [ ] CI (GitHub Actions) e testes de integração com PostgreSQL real (Testcontainers)
- [ ] Concorrência otimista na sessão
