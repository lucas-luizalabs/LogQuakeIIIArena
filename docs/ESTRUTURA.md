[Voltar](../README.md)

# Estrutura de pastas da Solution

Foi criado uma separação de pastas por projetos de "fonte" e "testes", seguindo o modelo proposto pela Microsoft https://docs.microsoft.com/en-us/dotnet/core/porting/project-structure com esse modelo de estruturação, facilita a visualização dos projetos de fonte, não misturando com os projetos de testes.

Abaixo segue uma visão da estrutura de pastas criadas para a solution LogQuake:

![estrutura de pastas 1 - 2](https://user-images.githubusercontent.com/44147082/47756338-6d270b00-dc80-11e8-8eef-008f1cbf8a86.PNG)

Abaixo segue uma visão expandida:

![estrutura de pastas 2 - 2 novo](https://user-images.githubusercontent.com/44147082/48319133-0e06b600-e5f1-11e8-9299-b7bfdc1a48e9.PNG)

## Projetos:
1. **LogQuake.API.Test**
    - Projeto responsável por efetuar os testes da camada de API, validando os recurso de Consultas e Upload.
2. **LogQuake.API**
    - Projeto responsável por expor os recursos disponíveis para Consultar os logs do jogo e efetuar Upload de um novo log.
3. **LogQuake.Domain.Test**
    - Projeto responsável por efetuar os testes da camada de Domínio.
4. **LogQuake.Domain**
    - Projeto responsável por armazenar o Domínio do projeto.
5. **LogQuake.Service.Test**
    - Projeto responsável por efetuar os testes da camada de Serviço, validando as Consultas e Leitura de arquivos.
6. **LogQuake.Service**
    - Projeto responsável por conter as regras de negócio do projeto.
7. **LogQuake.Infra.Test**
    - Projeto responsável efetuar os testes da camada de acesso ao Banco de Dados.
8. **LogQuake.Infra**
    - Projeto responsável por efetuar a leitura e escrita no Banco de Dados.
9. **LogQuake.CrossCutting**
    - Projeto responsável por conter rotinas que sejam utilizadas por qualquer camada do projeto.
