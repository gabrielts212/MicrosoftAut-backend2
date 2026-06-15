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
