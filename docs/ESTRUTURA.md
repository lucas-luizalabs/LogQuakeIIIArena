[Voltar](../README.md)

# Estrutura de pastas do projeto

Abaixo segue uma visão da estrutura de pastas criadas no projeto:

![estrutura de pastas](https://user-images.githubusercontent.com/44147082/47684066-53b68e00-dbb0-11e8-8468-1a0049a7ef51.PNG)

## Projetos:
1. **LogQuake.API.Test**
    - Projeto responsável por efetuar os testes da camada de API.
2. **LogQuake.API**
    - Projeto responsável por expor os recursos disponíveis para Consultar os logs do jogo e efetuar Upload de um novo log.
3. **LogQuake.Domain**
    - Projeto responsável por armazenar o Domínio do projeto.
4. **LogQuake.Service.Test**
    - Projeto responsável por efetuar os testes da camada de Serviço, validando as Consultas e Leitura de arquios.
5. **LogQuake.Service**
    - Projeto responsável por conter as regras de negócio do projeto.
6. **LogQuake.Infra.Test**
    - Projeto responsável efetuar os testes da camada de acesso ao Banco de Dados.
7. **LogQuake.Infra**
    - Projeto responsável por efetuar a leitura e escrita no Banco de Dados.
8. **LogQuake.CrossCutting**
    - Projeto responsável por conter rotinas que sejam utilizadas por qualquer camada do projeto.
