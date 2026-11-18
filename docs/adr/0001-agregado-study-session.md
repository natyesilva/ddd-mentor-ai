# ADR 0001 — StudySession como agregado rico, com leituras por projeção

- **Status:** aceito
- **Data:** 2026-11-17

## Contexto

O MVP tinha `StudySession` e `Message` como classes só com setters públicos. A lógica (criar sessão, adicionar mensagem, atualizar `UpdatedAt`, checar dono) ficava em `Infrastructure/Services/StudySessionService`, misturando caso de uso com EF Core. Para um projeto cujo assunto é DDD, o modelo anêmico contradiz o que o produto ensina. Também faltava validação no domínio: um `Level` fora do enum (ex.: `99`) era gravado.

## Opções consideradas

1. **Agregado rico mapeado direto pelo EF Core.** Setters privados, construtores de fábrica, coleção `_messages` mapeada por campo, value objects via `HasConversion`.
2. **Modelo de domínio puro e modelo de persistência separado, com mapeadores.** Domínio sem nenhuma concessão ao ORM.

## Decisão

Opção 1.

- `StudySession` é a raiz do agregado. `Message` só nasce por `StudySession.AddUserMessage`.
- `SessionTitle`, `StudyTopic` e `MessageContent` são value objects com as regras de tamanho que antes viviam só nos DTOs.
- O horário vem de `TimeProvider`, injetado na Application e passado ao domínio, o que torna os testes determinísticos.
- **Escrita** passa pelo agregado (`IStudySessionRepository`, definido no Domain). **Leitura** usa projeções direto para DTO (`IStudySessionQueries`, definido na Application), sem carregar o agregado.
- Os casos de uso foram para `Application/Services`. A Infrastructure só implementa as portas.

## Consequências

- (+) As invariantes ficam num lugar só e são testadas sem banco.
- (+) As leituras continuam baratas: uma query projetada, sem tracking.
- (−) O domínio faz concessões ao EF: construtor privado vazio e `ValueGeneratedNever` nos Ids. Esse último é obrigatório, porque sem ele o EF trata a mensagem nova como `Modified` e emite um UPDATE. Um teste cobre esse caso.
- (−) Adicionar mensagem carrega a sessão com **todas** as mensagens. É aceitável porque sessões de estudo são curtas e o mentor de IA (fase 2) vai precisar do histórico como contexto. Se as sessões crescerem, vale paginar ou resumir o histórico, ou parar de carregar a coleção e validar a regra por consulta.
- (−) Não há controle de concorrência otimista. Duas mensagens simultâneas na mesma sessão funcionam, mas o `UpdatedAt` fica com a última gravação. Candidato: `xmin` do PostgreSQL como row version.
- A migration `RichStudySessionAggregate` não altera tabelas. Ela só cria `IX_StudySessions_UpdatedAt`, que estava configurado no `DbContext` mas nunca tinha sido gerado em migration.

## Por que não a opção 2

Ela dobra o número de classes e de mapeamentos para proteger o domínio de um acoplamento que hoje se resume a um construtor privado e a um campo de coleção. A troca só compensaria se o modelo de persistência divergisse bastante do domínio, o que não acontece aqui.
