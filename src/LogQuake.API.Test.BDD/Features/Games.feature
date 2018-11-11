﻿#language: pt-br

Funcionalidade: Dado que sou um jogador de Quake
	Eu quero efetuar upload de log de partidias efetuadas, e inserir essas niformações em banco de dados
	E consultar os dados das partidas efetuadas
	Para eu possa analisar as informações de cada partida, e verificar os participantes, total de mortes, mortes para cada jogador.


Cenário: 0 - efetuar upload de arquivo de log do jogo Quake III Arena
	Dado que a url do endpoint é 'http://localhost:65080/api/games/upload'
	E o verbo http é 'POST'
	E o arquivo de log está na pasta corrente do projeto 'Log\games.log'
	Quando chamar o serviço de upload
	Então o statuscode da resposta deverá ser 'OK'
	E a quantidade de resgistro inseridos deve ser '1058'

Cenário: 1 - listar todas as partidas de forma paginada
	Dado que a url do endpoint é 'http://localhost:65080/api/games/'
	E o verbo http é 'GET'
	Quando chamar o serviço
	Então o statuscode da resposta deverá ser 'OK'

Cenário: 2 - obter jogo através de um identificador
	Dado que a url do endpoint é 'http://localhost:65080/api/games/'
	E o verbo http é 'GET'
	E o ID do jogo será '5'
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

Cenário: 3 - buscar uma partida inexistente através de um identificador
	Dado que a url do endpoint é 'http://localhost:65080/api/games/'
	E o verbo http é 'GET'
	E o ID do jogo será '999999'
	Quando chamar o serviço
	Então o statuscode da resposta deverá ser 'NotFound'