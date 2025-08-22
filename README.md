# JsonPlaceholderApi

API Web **.NET 8** que integra com a API pública **JSONPlaceholder**, busca posts externos e persiste no **MySQL**. Inclui **Swagger** para testar os endpoints.

---

## 🔎 Visão Geral

- **/api/posts/fetch** → busca posts em `https://jsonplaceholder.typicode.com/posts` e **salva** no MySQL.
- **/api/posts** → **lista** os posts já salvos no banco.
- **Swagger UI** para testes interativos.

> Este projeto foi feito como avaliação de **Junior Backend**: integração com API externa, transformação/armazenamento e documentação básica.

---

## 🧰 Tecnologias

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core
- MySQL (provider: Pomelo)
- Swagger / OpenAPI

---

## ✅ Pré-requisitos

- **.NET 8 SDK** instalado (`dotnet --version` deve retornar 8.x)
- **MySQL** (Community Server) com um usuário/senha válidos
- **VS Code** com extensão **C# Dev Kit** (ou outra IDE de sua preferência)

_(Opcional, mas recomendado para migrações)_

- Ferramenta CLI do EF: `dotnet tool install --global dotnet-ef`
  > No Windows, **feche e reabra** o terminal/VS Code após instalar para atualizar o PATH.

---

## ⚙️ Configuração

1. **Criar o banco** (se ainda não existir):

```sql
CREATE DATABASE jsonplaceholderdb;
```

## ▶️ Como rodar localmente

1. Restaurar dependências e compilar:

```powershell
dotnet restore
dotnet build
```

2. Aplicar migrações (certifique-se que o banco está disponível):

```powershell
dotnet ef database update
```

3. Rodar a API:

```powershell
dotnet run --project e:\\Projetos\\JsonPlaceholderApi\\JsonPlaceholderApi.csproj
```

4. Abrir Swagger (em dev):

https://localhost:{port}/swagger

## Endpoints

- GET /api/posts/fetch → importa posts da API externa e salva no DB
- GET /api/posts → lista posts salvos
- GET /api/posts/{id} → obtém um post por id
- POST /api/posts → cria um post (json: { userId, title, body })
- PUT /api/posts/{id} → atualiza um post
- DELETE /api/posts/{id} → deleta um post

## Docker (opcional)

Também existe suporte via Docker (ex.: criar `docker-compose.yml` para API + MySQL). Veja a pasta `devops` se existir ou crie um `docker-compose.yml` seguindo as práticas comuns.

## CI

Um workflow de CI (GitHub Actions) foi adicionado em `.github/workflows/ci.yml` com passos para restore, build e testes.

## Testes

O projeto inclui testes unitários para o importador (`Tests/PostImporterTests.cs`). Para executar os testes localmente:

```powershell
dotnet test
```

Observação: em alguns ambientes Windows locais o `dotnet test` pode apresentar avisos de cópia de arquivos durante execução paralela; recomenda-se limpar `bin/` e `obj/` caso ocorra erro e reexecutar.
