# RetroGames

Loja virtual de jogos retrô usados, desenvolvida como projeto de estudo de desenvolvimento web.

O sistema permite consultar jogos, visualizar detalhes, realizar cadastro e login, administrar o catálogo e efetuar compras.

## 1. Tecnologias utilizadas

### Backend
- .NET 10
- ASP.NET Core Web API
- C#
- ADO.NET com MySqlConnector
- MySQL 8
- FluentValidation
- xUnit v3 e Moq

### Frontend
- React 19
- Vite 8
- JavaScript
- Bootstrap 5
- React Router 7
- Axios 1.x

## 2. Arquitetura

O projeto é dividido em três camadas principais no backend:

- **Controller:** recebe as requisições HTTP e encaminha as operações para os Services.
- **Service:** concentra as regras de negócio e a composição dos dados.
- **Repository:** realiza o acesso ao banco de dados utilizando SQL parametrizado.

O frontend é uma aplicação React que consome a API por meio de requisições HTTP.

## 3. Estrutura do projeto

### Projetos .NET

- `RetroGames.Model`: entidades e modelos compartilhados.
- `RetroGames.Repository`: acesso ao banco de dados.
- `RetroGames.Services`: regras de negócio.
- `RetroGames.WebApi.ApiModels`: modelos de entrada e saída da API.
- `RetroGames.WebApi`: API ASP.NET Core.
- `RetroGames.WebApi.UnitTests`: testes automatizados.

### Frontend

- `retrogames.web`: aplicação React com Vite.

## 4. Funcionalidades

### Catálogo
- Listagem de jogos.
- Busca textual.
- Filtros por plataforma e gênero.
- Paginação.
- Visualização dos detalhes de um jogo.

### Administração
- Cadastro, consulta, alteração e desativação de jogos.
- Associação de jogos a gêneros.
- Cadastro, consulta, alteração e exclusão de plataformas.

### Autenticação
- Cadastro de clientes.
- Login e logout.
- Consulta da sessão atual.
- Controle de acesso por perfil.

### Compras
- Carrinho de compras armazenado no navegador.
- Adição e remoção de jogos.
- Alteração de quantidades.
- Finalização de pedidos.
- Baixa de estoque.
- Consulta dos pedidos do cliente.

## 5. Principais endpoints

| Método | Endpoint | Finalidade |
|---|---|---|
| POST | `/api/Autenticacao/cadastrar` | Cadastro de cliente |
| POST | `/api/Autenticacao/login` | Login |
| POST | `/api/Autenticacao/logout` | Logout |
| GET | `/api/Autenticacao/eu` | Consultar usuário autenticado |
| GET | `/api/Jogo` | Listar jogos |
| GET | `/api/Jogo/{id}` | Consultar jogo |
| GET | `/api/Plataforma` | Listar plataformas |
| GET | `/api/Genero` | Listar gêneros |
| GET | `/api/CondicaoJogo` | Listar condições |
| POST | `/api/Pedido` | Finalizar compra |
| GET | `/api/Pedido/meus` | Listar pedidos do cliente |

As operações administrativas de manutenção exigem autorização de administrador.

## 6. Banco de dados

O projeto utiliza MySQL 8.

Os scripts SQL de criação e atualização da estrutura ficam no diretório `database/`.

O módulo de compras utiliza as tabelas:
- `tb_situacao_pedido`
- `tb_pedido`
- `tb_item_pedido`

## 7. Configuração do ambiente

### Pré-requisitos

- .NET SDK 10
- Node.js 22 LTS
- MySQL 8
- Git

### Configuração do backend

Configure as variáveis necessárias no arquivo `.env`, na raiz do repositório.

Utilize o `.env.example` como referência.

Não versione o arquivo `.env` nem compartilhe credenciais.

### Configuração do banco

Crie o banco de dados conforme os scripts SQL disponíveis no diretório `database/`.

Execute os scripts na ordem indicada pelos seus nomes.

### Configuração do frontend

Entre na pasta do frontend:

```bash
cd retrogames.web
```

Instale as dependências:

```bash
npm install
```

## 8. Executando o projeto

Na raiz da solução, restaure as dependências:

```bash
dotnet restore RetroGames.sln
```

Compile a solução:

```bash
dotnet build RetroGames.sln
```

Execute a API:

```bash
dotnet run --project RetroGames.WebApi
```

Em outro terminal, entre na pasta do frontend:

```bash
cd retrogames.web
npm run dev
```

O frontend utiliza o Vite na porta 5173 e encaminha as requisições `/api` para a API na porta 5080.

## 9. Testes

Para executar os testes automatizados do backend, utilize na raiz da solução:

```bash
dotnet test
```

Os testes utilizam xUnit v3 e Moq para verificar as regras de negócio sem depender diretamente do banco de dados.

Para gerar a versão de produção do frontend:

```bash
cd retrogames.web
npm run build
```
