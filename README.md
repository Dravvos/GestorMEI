# 🏢 GestorMEI — Sistema de Gestão para Microempreendedor Individual

API back-end completa para **gestão financeira e administrativa de MEIs (Microempreendedores Individuais)**, desenvolvida em **C# com ASP.NET Core**. O projeto segue uma arquitetura em camadas bem definida, com separação entre API, regras de negócio, acesso a dados e gerenciamento de identidade.

---

## ✨ Funcionalidades

- **Autenticação e autorização** de usuários (Identity)
- **Gestão de receitas e despesas** do MEI
- **Controle financeiro** com visão de entradas e saídas
- **Relatórios e consultas** de dados financeiros
- API RESTful pronta para consumo por qualquer front-end ou aplicativo mobile
- Pipeline de CI/CD configurado via **GitHub Actions**

---

## 🏗️ Arquitetura em Camadas

O projeto é dividido em camadas com responsabilidades bem separadas:

```
GestorMEI/
├── GestorMEI.API        → Endpoints REST (Controllers, middlewares, configurações)
├── GestorMEI.BLL        → Regras de negócio (Business Logic Layer)
├── GestorMEI.Data       → Acesso a dados (repositórios, EF Core, migrations)
├── GestorMEI.DTO        → Objetos de transferência de dados
└── GestorMEI.Identity   → Autenticação e gerenciamento de usuários
```

---

## 🛠️ Tecnologias Utilizadas

| Tecnologia              | Uso                                            |
|-------------------------|------------------------------------------------|
| C# / ASP.NET Core       | Framework principal da API                     |
| Entity Framework Core   | ORM para acesso ao banco de dados              |
| PostgreSQL              | Banco de dados relacional                      |
| JWT / ASP.NET Identity  | Autenticação e autorização                     |
| GitHub Actions          | Pipeline de CI/CD automatizado                 |

---

## 📋 Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) instalado e configurado
- [Visual Studio](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

---

## 🚀 Como Executar

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/Dravvos/GestorMEI.git
   cd GestorMEI
   ```

2. **Configure a string de conexão:**
   - Crie uma variável de ambiente chamada `MEIConn`
   - Atualize o valor dela com os dados do seu PostgreSQL

3. **Aplique as migrations:**
   ```bash
   dotnet ef database update --project GestorMEI.Data --startup-project GestorMEI.API
   ```

4. **Execute a aplicação:**
   ```bash
   cd GestorMEI.API
   dotnet run
   ```

5. **Acesse a documentação (Swagger):**
   - Navegue até `https://localhost:{porta}/swagger`

---

## 📌 Endpoints Principais

| Método | Rota                    | Descrição                             |
|--------|-------------------------|---------------------------------------|
| POST   | `/api/auth/register`    | Cadastro de novo usuário MEI          |
| POST   | `/api/auth/login`       | Login e geração de token JWT          |
| GET    | `/api/venda/{empresaId}`| Lista despesas da empresa             |
| POST   | `/api/venda`            | Registra um novo lançamento           |
| PUT    | `/api/venda/{id}`       | Atualiza um lançamento existente      |
| DELETE | `/api/venda/{id}`       | Remove um lançamento                  |
| GET    | `/api/relatorio`        | Consulta relatório financeiro         |

> Os endpoints requerem autenticação via `Bearer Token` no header `Authorization`.

---

## 📁 Estrutura do Projeto

```
GestorMEI/
├── .github/workflows/      # CI/CD (GitHub Actions)
├── GestorMEI.API/          # Camada de apresentação (Controllers, Startup)
├── GestorMEI.BLL/          # Regras de negócio
├── GestorMEI.Data/         # Repositórios e contexto do EF Core
├── GestorMEI.DTO/          # Objetos de transferência de dados
├── GestorMEI.Identity/     # Autenticação e gerenciamento de identidade
├── MEI.sln                 # Solution file
├── LICENSE.txt             # Licença MIT
└── README.md
```

---

## 🔐 Autenticação

A API utiliza **JWT (JSON Web Tokens)**. Após o login, inclua o token no header de todas as requisições protegidas:

```
Authorization: Bearer {seu_token_aqui}
```

---

## 🤝 Contribuições

Contribuições são bem-vindas! Para contribuir:

1. Faça um **fork** do repositório
2. Crie uma branch: `git checkout -b feature/minha-feature`
3. Commit: `git commit -m 'feat: descrição da melhoria'`
4. Push: `git push origin feature/minha-feature`
5. Abra um **Pull Request**

---

## 📄 Licença

Este projeto está licenciado sob a [MIT License](LICENSE.txt).

---

Desenvolvido por [Daniel Oliveira Dias (Dravvos)](https://github.com/Dravvos)
