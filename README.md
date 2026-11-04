# RetroGames

Solução inicial com seis projetos .NET 10 e um front-end React/Vite, conforme as seções 2, 3.1, 3.2 e 8 da especificação.

## Projetos .NET

- `RetroGames.Model`: biblioteca de entidades e tipos compartilhados.
- `RetroGames.Repository`: biblioteca de acesso a dados ADO.NET/MySqlConnector.
- `RetroGames.Services`: biblioteca de regras de negócio e composição.
- `RetroGames.WebApi.ApiModels`: biblioteca de modelos de entrada e saída da API.
- `RetroGames.WebApi`: API ASP.NET Core com controllers.
- `RetroGames.WebApi.UnitTests`: testes unitários com xUnit v3 e Moq.

## Front-end

- `retrogames.web`: SPA em JavaScript com React 19, Vite 8, Bootstrap 5, React Router 7 e Axios 1.x.
- O Vite atende na porta 5173 e encaminha `/api` para `http://localhost:5080`.

## Comandos .NET

Na raiz da solução:

```bash
dotnet restore RetroGames.sln
dotnet build RetroGames.sln
dotnet test RetroGames.sln
```

## Comandos do front-end

Dentro de `retrogames.web/`:

```bash
npm install
npm run dev
```

Para gerar a versão de produção:

```bash
npm run build
```

Configure connection strings e credenciais usando `dotnet user-secrets` ou variáveis de ambiente. Não coloque segredos em arquivos versionados.
