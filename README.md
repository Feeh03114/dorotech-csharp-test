## Implementacao entregue

API de livraria desenvolvida em .NET 6 com ASP.NET Core, Entity Framework Core e PostgreSQL. A aplicacao expoe Swagger UI com autenticacao JWT Bearer para testar os endpoints protegidos.

### Stack

- .NET 6
- ASP.NET Core Web API
- Entity Framework Core 6
- PostgreSQL
- Swagger / Swashbuckle
- JWT Bearer
- Serilog
- xUnit

### Como executar

1. Suba um PostgreSQL local:

```bash
docker run --name bookstore-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=bookstore -p 5432:5432 -d postgres:16
```

2. Restaure as ferramentas e pacotes:

```bash
dotnet tool restore
dotnet restore Bookstore.sln
```

3. Aplique a migration:

```bash
dotnet tool run dotnet-ef -- database update --project src/Bookstore.Infrastructure --startup-project src/Bookstore.Api
```

4. Rode a API:

```bash
dotnet run --project src/Bookstore.Api
```

5. Acesse o Swagger:

```text
https://localhost:7087/swagger
```

Se a porta HTTPS variar, confira `src/Bookstore.Api/Properties/launchSettings.json`.

### Autenticacao

Endpoint:

```http
POST /api/auth/login
```

Payload:

```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

Copie o `accessToken` retornado e use o botao **Authorize** do Swagger com:

```text
Bearer <accessToken>
```

### Endpoints

Publicos:

- `GET /api/books`
- `GET /api/books/{id}`

Protegidos por JWT de administrador:

- `POST /api/books`
- `PUT /api/books/{id}`
- `DELETE /api/books/{id}`

Filtros de consulta em `GET /api/books`:

- `pageNumber`
- `pageSize`
- `name`
- `author`
- `isbn`
- `publisher`
- `publicationYear`

### Testes

```bash
dotnet test Bookstore.sln
```

### Observacoes

- A connection string padrao esta em `src/Bookstore.Api/appsettings.json`.
- A aplicacao executa migrations e seed de dados na inicializacao.
- Logs sao escritos no console e em `logs/bookstore-api-.log`.
- .NET 6 foi usado conforme solicitado, embora esteja fora de suporte upstream desde 12 de novembro de 2025.

---

## Desafio para Back-end Developer na DoroTech - C# .NET

#### Requisitos Gerais:

Uma livraria da cidade teve um aumento no número de seus exemplares e está com um problema para identificar todos os livros que possui em estoque. 
Para ajudar a livraria foi solicitado a você desenvolver uma aplicação web para gerenciar estes exemplares. Requisitos:


* O sistema deverá mostrar todos os livros cadastrados ordenados de forma ascendente pelo nome.
* Ao persistir, validar se o livro já foi cadastrado.
* O sistema deverá permitir consultar, criar, editar e excluir um livro.
* Os livros devem ser persistidos em um banco de dados.
* Criar algum mecanismo de log de registro e de erro.

#### Requisitos Técnicos:

* Configurar o Swagger na aplicação (fundamental), pois será usado para testes.
* Incluir mecanismo de autenticação no Swagger, usando Token JWT (Bearer).
* Para a persistência dos dados deve ser utilizado o Entity Framework.
* Como banco de dados, pode ser usado MySQL, PostgreSQL ou SQL Server.
* Utilizar migrations ou Gerar Scripts e disponibilizá-los um uma pasta.
* Incluir git.ignore no repositório para não subir arquivos de compilação.


#### Observações:
* O sistema deverá ser desenvolvido na plataforma .NET com C#.
	(preferêncialmente 5.0+, caso for usado outra versão, informar no pull-request)
* Deve conter autenticação com dois níveis de acesso, um administrador e um público, o usuário de nível 
	público não terá autenticação, ou seja, terá acesso livre a consulta de livros
* Atenção aos princípio do SOLID.
* Não é necessária a criação de front-end, o teste será feito pelo Swagger UI.

#### Diferenciais do desafio:
* Aplicação das boas práticas do DDD, TDD, Design Patterns, SOLID e Clean Code.
* A modelagem dos dados não será fornecida, de propósito. Desejamos avaliar a sua capacidade de abstração.
* A API deverá realizar tratamento de entrada de dados e retornar códigos de erro quando aplicáveis.
* Criar massa de dados para que seja possível verificar o funcionamento das lógicas propostas.
* Incluir parâmetros de paginação e campos de filtro nos métodos de consulta (GET).
* Documentar, via código-fonte, os campos, parâmetros e dados de retorno da API para exibição no Swagger.


## Como deverá ser entregue:

    1. Faça um fork deste repositório;
    2. Realize o teste;
    3. Adicione seu currículo na raiz do repositório;
    4. Envie-nos o PULL-REQUEST para que seja avaliado.



## C# Back-end Challenge (English)

#### General requirements:

A bookstore in town has had an increase in the number of its copies and is having a problem identifying all the books it has in stock.
To help the bookstore, you were asked to develop a web application to manage these copies. Requirements:


* The system should show all registered books sorted in ascending order by name.
* When persisting, validate if the book has already been registered.
* The system should allow consulting, creating, editing and deleting books.
* Books must be persisted in a database.
* Create some logging and error logging mechanism.

#### Technical requirements:

* Configure Swagger in the application (fundamental), as it will be used for testing.
* Include authentication mechanism in Swagger, using JWT Token (Bearer).
* For data persistence, Entity Framework must be used.
* As a database, MySQL, PostgreSQL or SQL Server can be used.
* Use migrations or Generate Scripts and make them available in a folder.
* Include git.ignore in the repository to avoid uploading deployment files.


#### Comments:
* The system must be developed on the .NET platform with C#.
(preferably 5.0+, if another version is used, inform the pull-request)
* Must contain authentication with two levels of access, an administrator and a public, user level
public will not have authentication, that is, it will have free access to consult books
* Attention to the principles of SOLID.
* No front-end creation required, testing will be done by Swagger UI.

#### Challenge differentials:
* Application of DDD, TDD, Design Patterns, SOLID and Clean Code best practices.
* Data modeling will not be provided on purpose. We wish to assess your capacity for abstraction.
* The API must perform data entry handling and return error codes when applicable.
* Create mass of data so that it is possible to verify the functioning of the proposed logics.
* Include pagination parameters and filter fields in query (GET) methods.
* Document, via source code, the API fields, parameters and return data for display in Swagger.


## How it should be delivered:

    1. Fork this repository;
    2. Carry out the test;
    3. Add your CV to the repository root;
    4. Send us the PULL-REQUEST to be evaluated.
