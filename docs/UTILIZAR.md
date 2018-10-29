# Como utilizar a Solution LogQuake

Após abrir a Solution LogQuake, e rodar a Solution no VS2017, será abeto o navegador no seguinte http://localhost:65080/swagger esta página contém os contratos Swagger utilizar na API.

Serão exibidos duas tags "Games/Consulta" e "Games/Upload", dentro dessas tags existem os recursos disponíveis na API.

Abaixo segue um print a ser exibido:
![swagger](https://user-images.githubusercontent.com/44147082/47685927-5072d080-dbb7-11e8-9238-e548273941b1.PNG)

# Consumir a API

## Consultar uma partida por IdGame

Podemos consumir este recurso, por exemplo, com Postman seguindo os passos listados abaixo: 

1. Crie um novo Request
2. Informe um nome
3. Adicione esse Request em uma Collection
4. Informe o verbo GET
5. informe o local da API: http://localhost:65080/api/games/5

## Consultar todas as partidas de forma paginada

## Upload de novo arquivo de log
