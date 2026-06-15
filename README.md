Backend (UserService) - .NET 10 + Dapper

Como usar (desenvolvimento):

1. Gere a solution e adicione o projeto (opcional):

   dotnet new sln -n MyApiSolution
   dotnet sln MyApiSolution.sln add UserService/UserService.csproj

2. Restaurar pacotes:

   cd UserService
   dotnet restore

3. Usando Docker Compose (recomendado em dev):

   cd ../
   docker-compose up --build
   - o SQL Server ficará disponível em localhost:1433
   - a API estará em http://localhost:5000 (mapeado para porta 80 no container)

4. Abrir no Visual Studio:
   - Abra `MyApiSolution.sln` (se você criou a solution) ou abra a pasta `UserService` como projeto.
   - Ajuste `appsettings.json` (AzureAd, ConnectionStrings) conforme necessário.

Notas:

- Dapper não fornece migrations — use scripts SQL na pasta `sql/` com ferramentas como DbUp ou Flyway.
- Configure as variáveis de ambiente sensíveis (senha do SA, Azure AD) no seu ambiente ou no container.

## API Endpoints

Base URL (dev): `http://localhost:5000`

1. GET /api/user/{id}
   - Protegido: requer `Authorization: Bearer <access_token>` (token obtido via MSAL no frontend)
   - Resposta 200: objeto JSON do usuário
   - Resposta 404: usuário não encontrado

   Exemplo (curl):

   ```bash
   curl -H "Authorization: Bearer $TOKEN" http://localhost:5000/api/user/1
   ```

2. POST /api/user
   - Cria um novo usuário (público no exemplo atual)
   - Corpo (JSON): `{ "name": "Nome", "email": "email@example.com", "passwordHash": "<hash>" }`
   - Resposta 201: retorna `Location` com a rota do novo recurso

   Exemplo (curl):

   ```bash
   curl -X POST http://localhost:5000/api/user \
     -H "Content-Type: application/json" \
     -d '{"name":"Teste","email":"teste@example.com","passwordHash":"hash"}'
   ```

## DB / Migrations

- Scripts SQL de criação estão em `backend/UserService/sql/` (`001_create_users.sql`).
- No ambiente de desenvolvimento com Docker Compose, o serviço aplica automaticamente os scripts via DbUp no startup (veja `Program.cs`).
- Em produção prefira aplicar scripts por pipeline (CI/CD) usando `DbUp`, `Flyway` ou ferramenta equivalente.

## Configurações importantes

- `appsettings.json` contém `ConnectionStrings:DefaultConnection` (substitua por sua string do Azure SQL em produção) e a seção `AzureAd` com `TenantId` e `ClientId`.
- Variáveis de ambiente úteis: `ConnectionStrings__DefaultConnection`, `FRONTEND_URL`, `AzureAd__TenantId`, `AzureAd__ClientId`.

## Onde editar/abrir

- Projeto: [backend/UserService/UserService.csproj](backend/UserService/UserService.csproj)
- Código principal: [backend/UserService/Program.cs](backend/UserService/Program.cs)
- Scripts SQL: [backend/UserService/sql/001_create_users.sql](backend/UserService/sql/001_create_users.sql)

## Deploy no Render (Docker)

Passo a passo mínimo para publicar no Render usando o `UserService/Dockerfile`:

1) Prepare o repositório
- Garanta que o `UserService/Dockerfile` esteja na pasta `UserService/` e que o `UserService.csproj` aponte para `net10.0` (ou a versão correta).

2) Não commite secrets
- Remova connection strings sensíveis do `docker-compose.yml` e `appsettings.*.json` antes de commitar. Use as Environment Variables do Render para a conexão com o Postgres.

3) Criar o Web Service no Render
- No painel do Render: New → Web Service → Environment: Docker.
- Aponte para o seu repositório e informe o caminho do Dockerfile se estiver em subpasta (ex.: `UserService/Dockerfile`).

4) Variáveis de ambiente (Render Dashboard → Environment)
- `ConnectionStrings__DefaultConnection` com o valor do Postgres (ex.:
  `Host=dpg-d8nk6u0k1i2s73de8tig-a.oregon-postgres.render.com;Port=5432;Database=minhaapp;Username=teste;Password=<SENHA>;SSL Mode=Require;Trust Server Certificate=true`)
- `ASPNETCORE_ENVIRONMENT=Production`

5) Comando de start / Migrations
- Se precisar aplicar migrations ou executar tarefas antes do start, configure o campo "Start Command" do Render para algo como:
  `dotnet UserService.dll` (ou um script que rode migrations e depois inicie a app).

6) Teste local antes do deploy
- Build e run local da imagem:
  `docker build -t userservice:local -f UserService/Dockerfile .`
  `docker run -e "ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=minhaapp;Username=teste;Password=YourLocalPassword;SSL Mode=Require;Trust Server Certificate=true" -p 5000:80 userservice:local`

7) Troubleshooting no Render
- Verifique os logs do serviço no painel do Render para erros na inicialização ou conexão com o banco.
- Confirme a string de conexão e parâmetros de SSL.

Se quiser, eu adiciono um `appsettings.Production.json` (sem secrets), um `.env.example` e instruções para automatizar migrations no startup.
