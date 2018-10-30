[Voltar](../README.md)

# Como utilizar a Solution LogQuake

Após abrir a Solution LogQuake, e rodar a Solution no VS2017, será abeto o navegador no seguinte http://localhost:65080/swagger esta página contém os contratos Swagger utilizar na API.

Serão exibidos duas tags "Games/Consulta" e "Games/Upload", dentro dessas tags existem os recursos disponíveis na API.

Abaixo segue um print a ser exibido:
![swagger](https://user-images.githubusercontent.com/44147082/47685927-5072d080-dbb7-11e8-9238-e548273941b1.PNG)

# Consumir a API
Abaixo serão apresentados exemplos de como consumir a API.

## Consultar uma partida por IdGame

Podemos consumir este recurso, por exemplo, com Postman seguindo os passos listados abaixo: 

1. Crie um novo Request
2. Informe um nome para o Request
3. Adicione esse Request em uma Collection
4. Informe o verbo GET
5. Informe o local da API: http://localhost:65080/api/games/5
6. Clique no botão Send

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