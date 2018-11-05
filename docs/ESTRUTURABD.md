[Voltar](../README.md)

# Banco de Dados

Para o projeto apresentado foi criado somente uma tabela chamada Kill, nela contém informações suficientes para montar o retorno da API.

1. Utilizado dois providers SQLite e SqlServer para facilitar utilização em ambientes Windows e Linux
2. Foi utilizado Migrations para criação do Banco de Dados
3. Para alterar entre os Bancos de Dados basta informar o valor "sqlite" ou "sqlserver" na propriedade "DataBase" no arquivo appsetting.json que fica dentro da pasta do projeto LogQuake.API

Abaixo segue estrutura de criação do Banco de Dados em SQLite:

![bd sqlite](https://user-images.githubusercontent.com/44147082/47764130-9c4e7400-dca2-11e8-9b9c-3ecc1eca9cce.PNG)


Abaixo segue a mesma estrutura para criação do Banco de Dados Sql Server:

![bd sql server](https://user-images.githubusercontent.com/44147082/47976967-98ed2b00-e09c-11e8-9a70-7d62c0cc658d.PNG)
