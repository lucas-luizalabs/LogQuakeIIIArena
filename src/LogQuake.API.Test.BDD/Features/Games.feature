#language: pt-br

Funcionalidade: Dado que sou um jogador de Quake
	Eu quero efetuar upload de log de partidias efetuadas, e inserir essas niformações em banco de dados
	E consultar os dados das partidas efetuadas
	Para eu possa analisar as informações de cada partida, e verificar os participantes, total de mortes, mortes para cada jogador.


Cenário: 0 - obter Token por Client_Credentials
	Dado que a url do endpoint é 'http://localhost:59329/connect/token'
	E o verbo http é 'POST'
	E o GrantType 'client_credentials'
	E a senha do Client é 'secret2'
	E o scope é 'LogQuake'
	E o Id do Cliente 'client2'
	Quando chamar o serviço de Servidor de Identidade
	Então o statuscode da resposta deverá ser 'OK'

Cenário: 1 - efetuar upload de arquivo de log do jogo Quake III Arena
	Dado que a url do endpoint é 'http://localhost:65080/api/games/upload'
	E o verbo http é 'POST'
	E o arquivo de log está na pasta corrente do projeto 'Log\games.log'
	E obter o Token
	Quando chamar o serviço de upload
	Então o statuscode da resposta deverá ser 'OK'
	E a quantidade de resgistro inseridos deve ser '1058'

Cenário: 2 - listar todas as partidas de forma paginada
	Dado que a url do endpoint é 'http://localhost:65080/api/games/'
	E o verbo http é 'GET'
	E obter o Token
	Quando chamar o serviço
	Então o statuscode da resposta deverá ser 'OK'

Cenário: 3 - obter jogo através de um identificador
	Dado que a url do endpoint é 'http://localhost:65080/api/games/'
	E o verbo http é 'GET'
	E o ID do jogo será '5'
	E obter o Token
	Quando chamar o serviço
	Então o statuscode da resposta deverá ser 'OK'
	E um total de '130' kills:
	E conter a lista abaixo de players:
	| players        |
	| Isgalamido     |
	| Oootsimo       |
	| Zeh            |
	| Assasinu Credi |
	| Mal            |
	| Dono da Bola   |
	| Chessus        |
	E conter a lista abaixo de kills:
    | player         | countkill |
    | Zeh            | -7        |
    | Isgalamido     | 5         |
    | Oootsimo       | 5         |
    | Dono da Bola   | -12       |
    | Assasinu Credi | 3         |
    | Mal            | -19       |
    | Chessus        | -2        |

Cenário: 4 - buscar uma partida inexistente através de um identificador
	Dado que a url do endpoint é 'http://localhost:65080/api/games/'
	E o verbo http é 'GET'
	E o ID do jogo será '999999'
	E obter o Token
	Quando chamar o serviço
	Então o statuscode da resposta deverá ser 'NotFound'

Cenário: 5 - obter jogo através de um identificador, mas informando um Token inválido
	Dado que a url do endpoint é 'http://localhost:65080/api/games/'
	E o verbo http é 'GET'
	E o ID do jogo será '5'
	E informar um Token inválido
	Quando chamar o serviço
	Então o statuscode da resposta deverá ser 'InternalServerError'

Cenário: 6 - obter Token por Password
	Dado que a url do endpoint é 'http://localhost:59329/connect/token'
	E o verbo http é 'POST'
	E o GrantType 'password'
	E o Id do Cliente 'rop.client'
	E a senha do Client é 'secret'
	E o UserName é 'isgalamido'
	E a senha do UserName é 'password'
	E o scope é 'LogQuake'
	Quando chamar o serviço de Servidor de Identidade
	Então o statuscode da resposta deverá ser 'OK'
