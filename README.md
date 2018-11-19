![quake3arena](https://user-images.githubusercontent.com/44147082/47756229-0a357400-dc80-11e8-8b9f-37dc9e054c42.jpeg)

# Log - Quake III Arena
É um projeto de estudo, que tem como finalidade efetuar a leitura de um log gerado pelo jogo Quake III Arena, inserir em um banco de dados e depois disponibilizar consulta através de API RESTFul. Também é possível efetuar um upload do aquivo de log para atualizar os dados das partidas.

## Dê uma estrela! :star:
Se você gostou do projeto ou se ele te ajudou, por favor dê um estrela.

## Como usar:
- Você precisará do Visual Studio 2017 (15.6) e do .NET Core SDK (2.0).
- O SDK e as ferramentas mais recentes podem ser baixados em https://dot.net/core.

Você poderá rodar o projeto em Windows

## Tecnologias utilizadas:
- ASP.NET Core 2.0 (with .NET Core)
- ASP.NET WebApi Core
- Entity Framework Core 2.1.4
- .NET Core Native DI
- FluentValidator
- Swagger UI
- Logging
- Multiplos providers de Banco de Dados (SQLite, Sql Server)
- Cache de Memória
- SpecFlow (obs.: ainda não está compatível com .Net Core, foi utilizado o Framework 4.6.1)
- IdentityServer4

## Arquitetura:
- DDD - Domain Driven Design 
- BDD - Behavior Driven Development
- Repository and Generic Repository
- Domain Notification
- Unit Of Work

## Referências:
- https://docs.microsoft.com/en-us/dotnet/core/porting/project-structure
- https://docs.microsoft.com/pt-br/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.1&tabs=visual-studio%2Cvisual-studio-xml
- https://docs.microsoft.com/pt-br/dotnet/core/testing/unit-testing-with-mstest
- https://docs.microsoft.com/pt-br/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/net-core-microservice-domain-model
- https://docs.microsoft.com/pt-br/ef/core/get-started/aspnetcore/existing-db
- https://docs.microsoft.com/en-us/dotnet/csharp/codedoc
- https://docs.microsoft.com/pt-br/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1
- https://docs.microsoft.com/pt-br/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1
- https://docs.microsoft.com/pt-br/ef/core/managing-schemas/migrations/providers
- https://docs.microsoft.com/pt-br/aspnet/core/performance/caching/memory?view=aspnetcore-2.1
- https://docs.microsoft.com/pt-br/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implemenation-entity-framework-core
- https://specflow.org/getting-started/
- https://specflow.org/documentation/SpecFlow-Assist-Helpers/


## Manual do Desenvolvedor:
- [Estrutura de pastas da Solution](docs/ESTRUTURA.md)
- [Banco de Dados](docs/ESTRUTURABD.md)
- [Como abrir a Solution com Visusal Studio 2017](docs/VS2017.md)
- [Como utilizar a Solution LogQuake](docs/UTILIZAR.md)


## News:

**v1.4 - 2018/11/19**
- Criação de Servidor de Autenticação e incluído proteção na API LogQuake para solicitar autenticação. Como exmeplo, foi solicitado autenticação nos recursos de Upload e Consulta de partidas por Id.
- Ajustes nos testes de BDD (LogQuake.API.Test.BDD) para poder consumir API de Token


**v1.3 - 2018/11/11**
- Implementação de Testes utilizando conceito BDD, com SpecFlow.


**v1.2 - 2018/11/09**
- Criação de Unit Of Work
- Implementação de Cache de Memória (IMemoryCache)
  - Criados três novos recursos GET para a API LogQuake, todos apresentados em Swagger.
    - http://localhost:65080/api/games/CacheController (efetuando Cache de memória na camada de API)
    - http://localhost:65080/api/games/CacheService (efetuando Cache de memória na camada de Services)
    - http://localhost:65080/api/games/CacheRepository (efetuando Cache de memória na camada de Repository)


**v1.1 - 2018/11/05**
- Banco de Dados com múltiplos Providers (SQLite e SqlServer)
- Gravação de log através do provider Microsoft Logging
- Criação de ResponseBase com lista de notificações/erros para devolver ao usuário e sistema, mensagens retornadas da API
- Melhorias no contrato Swagger, criação de exemplos json para retorno e melhor detalhamento dos parâmetros de request.
