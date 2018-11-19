[Voltar](../README.md)

# Como utilizar a Solution LogQuake

Após abrir a Solution LogQuake, e rodar a Solution no VS2017, será aberto o navegador no seguinte endereço  http://localhost:65080/index.html esta página contém os contratos Swagger disponíveis na API.

Abaixo segue um print a ser exibido:
![swagger v2](https://user-images.githubusercontent.com/44147082/48732774-ac190280-ec27-11e8-9c91-ccc35c5c6ed2.PNG)


# Obtendo Token
Abaixo segue um exemplo de como gerar um Token do tipo (Grant_Type) Password.

Podemos consumir este recurso, por exemplo, com Postman seguindo os passos listados abaixo:
1. Crie um novo Request
2. Informe um nome para o Request
3. Adicione esse Request em uma Collection
4. Informe o verbo POST
5. Informe o local da API: http://localhost:59329/connect/token
6. Informe os parâmetros abaixo:
7. grant_type => password
8. client_id => rop.client
9. client_secret => secret
10. username => isgalamido
11. password => pasword
12. scope => LogQuake offline_access
13. Clique no botão Send

Segue exemplo abaixo:
![token 1-2](https://user-images.githubusercontent.com/44147082/48732953-4bd69080-ec28-11e8-9f89-dfe1d34fdda0.PNG)


# Consumir a API
Abaixo serão apresentados exemplos de como consumir a API.

## Consultar uma partida por IdGame

Podemos consumir este recurso, por exemplo, com Postman seguindo os passos listados abaixo: 

1. Crie um novo Request
2. Informe um nome para o Request
3. Adicione esse Request em uma Collection
4. Informe o verbo GET
5. Informe o local da API: http://localhost:65080/api/games/5
6. Selecione a opção "Authorization"
7. Selecione na combo Type a opção "Bearer Token"
8. Insira o Token recebido da API de Token no campo Token
9. Clique no botão Send

Segue exemplo abaixo:
![getbyid](https://user-images.githubusercontent.com/44147082/47686174-882e4800-dbb8-11e8-86fd-be8ca6487a6f.PNG)


## Consultar todas as partidas de forma paginada
Podemos consumir este recurso, por exemplo, com Postman seguindo os passos listados abaixo: 

1. Crie um novo Request
2. Informe um nome para o Request
3. Adicione esse Request em uma Collection
4. Informe o verbo GET
5. Informe o local da API: http://localhost:65080/api/games
6. Informe os campos PageNumber e PageSize
7. Clique no botão Send

Segue exemplo abaixo:
![getallpaginada](https://user-images.githubusercontent.com/44147082/47686284-13a7d900-dbb9-11e8-807c-f84c07437f6a.PNG)

## Upload de novo arquivo de log
Podemos consumir este recurso, por exemplo, com Postman seguindo os passos listados abaixo: 

1. Crie um novo Request
2. Informe um nome para o Request
3. Adicione esse Request em uma Collection
4. Informe o verbo POST
5. Informe o local da API: http://localhost:65080/api/games/upload
6. Selecione a opção Body
7. Selecione a opção form-data
8. Em Key, selecione File, e digite o nome file no campo KEY
9. No campo VALUE, selecione um arquivo de log do jogo Quake III Arena
10. Clique no botão Send

Segue exemplo abaixo:
![upload](https://user-images.githubusercontent.com/44147082/47686418-d132cc00-dbb9-11e8-9fd2-6fe943171903.PNG)
