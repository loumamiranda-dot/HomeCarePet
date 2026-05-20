# HomeCarePet

Sistema de agendamento para pet sitter, feito como projeto de estudo.

## Sobre

Esse projeto é um sistema web para a Vanessa Terceti Pet Sitter. Tem uma landing page publica, area do cliente pra agendar servicos pros pets, e um painel admin pra gerenciar tudo.

Estou aprendendo React e .NET então pode ter coisa pra melhorar ainda.

## Estrutura do projeto

```
HomeCarePet/
├── backend/          # API em .NET 8
│   ├── HomeCare.Api/
│   ├── HomeCare.Aplicacao/
│   ├── HomeCare.Dominio/
│   ├── HomeCare.Repositorio/
│   └── HomeCare.sln
└── frontend/         # React 18 + Vite
    ├── src/
    ├── public/
    ├── index.html
    ├── package.json
    └── vite.config.js
```

## Tecnologias

**Frontend:** React 18, Vite, React Router DOM, Axios, CSS Modules

**Backend:** .NET 8, Entity Framework Core, Dapper, SQL Server, JWT, BCrypt

Tambem tem integracao com a API da Groq (llama 3.3) pra sugerir pacotes de servicos pro cliente.

## Como rodar

### Backend
Precisa do SQL Server rodando. Configurar a connection string no `appsettings.json`.

```bash
cd backend
dotnet run --project HomeCare.Api
```

### Frontend
```bash
cd frontend
npm install
npm run dev
```
Roda em http://localhost:3000

## Funcionalidades

- Login e cadastro com JWT
- CRUD de clientes, pets, servicos e agendamentos
- Dashboard com metricas (admin)
- Sugestoes de pacotes por IA (cliente)
- Landing page com info da pet sitter

## TODO

- [ ] Adicionar testes
- [ ] Melhorar tratamento de erros
- [ ] Fazer deploy
- [ ] Adicionar upload de foto dos pets
