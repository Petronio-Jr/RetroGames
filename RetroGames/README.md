# RetroGames

Solução inicial com seis projetos .NET 10, conforme as seções 2, 3.1 e 3.2 da especificação.

## Projetos

- `RetroGames.Model`: biblioteca de entidades e tipos compartilhados.
- `RetroGames.Repository`: biblioteca de acesso a dados ADO.NET/MySqlConnector.
- `RetroGames.Services`: biblioteca de regras de negócio e composição.
- `RetroGames.WebApi.ApiModels`: biblioteca de modelos de entrada e saída da API.
- `RetroGames.WebApi`: API ASP.NET Core com controllers.
- `RetroGames.WebApi.UnitTests`: testes unitários com xUnit v3 e Moq.

## Comandos

Na raiz da solução:

```bash
dotnet restore RetroGames.sln
dotnet build RetroGames.sln
dotnet test RetroGames.sln
```

Configure connection strings e credenciais usando `dotnet user-secrets` ou variáveis de ambiente. Não coloque segredos em arquivos versionados.
