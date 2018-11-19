using LogQuake.CrossCutting;
using LogQuake.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TechTalk.SpecFlow;

namespace API.Test.steps
{
    [Binding]
    public sealed class GamesConsultarPartidasStep
    {
        private Dictionary<string, Game> gameResponse;
        private DtoResponseBase notification;

        [Given(@"que a url do endpoint é '(.*)'")]
        public void DadoQueAUrlDoEndpointE(string urlAPIGame)
        {
            ScenarioContext.Current["Endpoint"] = urlAPIGame;
        }

        [Given(@"o verbo http é '(.*)'")]
        public void DadoOMetodoE(string verbo)
        {
            var metodo = Method.POST;

            switch (verbo.ToUpper())
            {
                case "POST":
                    metodo = Method.POST;
                    break;
                case "GET":
                    metodo = Method.GET;
                    break;
                case "PUT":
                    metodo = Method.PUT;
                    break;
                case "DELETE":
                    metodo = Method.DELETE;
                    break;
                case "PATCH":
                    metodo = Method.PATCH;
                    break;
                default:
                    Assert.Fail("Método Http não esperado");
                    break;
            }

            ScenarioContext.Current["HttpMethod"] = metodo;
        }

        [When(@"chamar o serviço")]
        public void QuandoChamarOServico()
        {
            var endpoint = (string)ScenarioContext.Current["Endpoint"];
            string IdGame = "";

            if (ScenarioContext.Current.ContainsKey("IdGame"))
                IdGame = Convert.ToString(ScenarioContext.Current["IdGame"]);

            ExecutarRequest(endpoint + (string.IsNullOrEmpty(IdGame)? "": IdGame.ToString()));
        }

        [Given(@"o ID do jogo será '(.*)'")]
        public void DadoOIDDoJogoSera(int idGame)
        {
            ScenarioContext.Current["IdGame"] = idGame;
        }


        [Then(@"o statuscode da resposta deverá ser '(.*)'")]
        public void EntaoOStatuscodeDaRespostaDeveraSer(string statusCode)
        {
            var response = (IRestResponse)ScenarioContext.Current["Response"];

            string errorMessage;

            switch (response.StatusCode)
            {
                case HttpStatusCode.Forbidden:
                    var auth = response.Request.Parameters.Where(x => x.Name == "Authorization").FirstOrDefault();
                    errorMessage = "Authorization: " + (auth != null ? auth.Value : "none");
                    break;
                case HttpStatusCode.NotFound:
                    errorMessage = "ReponseURI:" + response.ResponseUri;
                    break;
                case HttpStatusCode.Unauthorized:
                    errorMessage = "ReponseURI:" + response.ResponseUri;
                    break;
                default:
                    errorMessage = response.Content;
                    break;
            }


            Assert.AreEqual(statusCode, response.StatusCode.ToString(), errorMessage);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                notification = JsonConvert.DeserializeObject<DtoResponseBase>(response.Content);
                Assert.IsTrue(notification.Notifications[0].ErrorCode == 2);
                Assert.IsTrue(notification.Notifications[0].Description == "Nenhum item encontrado com os parâmetros informados.");
                Assert.IsTrue(notification.Success == false);
            }
        }


        [Then(@"um total de '(.*)' kills:")]
        public void EntaoUmTotalDeKills(int countKills)
        {
            var response = (IRestResponse)ScenarioContext.Current["Response"];
            gameResponse = JsonConvert.DeserializeObject<Dictionary<string, Game>>(response.Content);
            Assert.AreEqual(countKills, gameResponse["game_5"].TotalKills);
        }

        [Then(@"conter a lista abaixo de players:")]
        public void EntaoConterAListaAbaixoDePlayers(Table table)
        {
            var response = (IRestResponse)ScenarioContext.Current["Response"];
            gameResponse = JsonConvert.DeserializeObject<Dictionary<string, Game>>(response.Content);

            string[] expectedLines =  table.Rows.Select(r => r["players"]).ToArray();
            string[] actualLines = gameResponse["game_5"].Players.ToArray();

            CollectionAssert.AreEqual(expectedLines, actualLines);
        }

        [Then(@"conter a lista abaixo de kills:")]
        public void EntaoConterUmaLisDeKills(Table table)
        {
            var response = (IRestResponse)ScenarioContext.Current["Response"];
            gameResponse = JsonConvert.DeserializeObject<Dictionary<string, Game>>(response.Content);
            Dictionary<string, int> expectedKills = table.Rows.ToDictionary(r => r["player"], r => Convert.ToInt32(r["countkill"]));

            CollectionAssert.AreEqual(expectedKills, gameResponse["game_5"].Kills);
        }

        [Given(@"o arquivo de log está na pasta corrente do projeto '(.*)'")]
        public void DadoOArquivoDeLogEstaNaPastaCorrenteDoProjeto(string p0)
        {
            //ScenarioContext.Current.Pending();
        }

        [When(@"chamar o serviço de upload")]
        public void QuandoChamarOServicoDeUpload()
        {
            string endpoint = (string)ScenarioContext.Current["Endpoint"];
            Method metodo = (Method)ScenarioContext.Current["HttpMethod"];

            //Token token = ObterToken();
            Token token = (Token)ScenarioContext.Current["token"];

            var client = new RestClient(endpoint);
            var request = new RestRequest(metodo);

            string fileLog = Directory.GetCurrentDirectory() + "\\Log\\games.log";

            request.AddFile("file", fileLog);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("key", "value", ParameterType.GetOrPost);

            request.AddParameter("Authorization", "Bearer " + token.access_token, ParameterType.HttpHeader);
            IRestResponse response = client.Execute(request);

            ScenarioContext.Current["Response"] = response;
        }


        [Then(@"a quantidade de resgistro inseridos deve ser '(.*)'")]
        public void EntaoAQuantidadeDeResgistroInseridosDeveSer(int registrosInseridos)
        {
            var response = (IRestResponse)ScenarioContext.Current["Response"];

            Assert.IsTrue(response.Content.Contains(registrosInseridos.ToString()));
        }

        [When(@"chamar o serviço de Servidor de Identidade")]
        public void QuandoChamarOServicoDeServidorDeIdentidade()
        {
            var client = new RestClient((string)ScenarioContext.Current["Endpoint"]);
            var request = new RestRequest();

            request.Method = (Method)ScenarioContext.Current["HttpMethod"];

            //add GetToken() API method parameters
            //utilizando dados fixos a princípio, pois já está cadastrado o aplicativo abaixo
            //com as permissões necessárias
            request.Parameters.Clear();
            if (ScenarioContext.Current["grant_type"].Equals("client_credentials"))
            {
                request.AddParameter("grant_type", ScenarioContext.Current["grant_type"]);
                request.AddParameter("client_secret", ScenarioContext.Current["client_secret"]);
                request.AddParameter("scope", ScenarioContext.Current["scope"]);
                request.AddParameter("client_id", ScenarioContext.Current["client_id"]);
            }
            else
            {
                request.AddParameter("grant_type", ScenarioContext.Current["grant_type"]);
                request.AddParameter("client_id", ScenarioContext.Current["client_id"]);
                request.AddParameter("client_secret", ScenarioContext.Current["client_secret"]);
                request.AddParameter("username", ScenarioContext.Current["username"]);
                request.AddParameter("password", ScenarioContext.Current["password"]);
                request.AddParameter("scope", ScenarioContext.Current["scope"]);
            }

            //make the API request and get the response
            IRestResponse response = client.Execute(request);

            ScenarioContext.Current["Response"] = response;

            //return an AccessToken
            Token token = JsonConvert.DeserializeObject<Token>(response.Content);

            ScenarioContext.Current["token"] = token;
        }

        [Given(@"o GrantType '(.*)'")]
        public void DadoOGrantType(string p0)
        {
            ScenarioContext.Current["grant_type"] = p0;
        }

        [Given(@"a senha do Client é '(.*)'")]
        public void DadoASenhaDoClientE(string p0)
        {
            ScenarioContext.Current["client_secret"] = p0;
        }

        [Given(@"o scope é '(.*)'")]
        public void DadoOScopeE(string p0)
        {
            ScenarioContext.Current["scope"] = p0;
        }

        [Given(@"o Id do Cliente '(.*)'")]
        public void DadoOIdDoCliente(string p0)
        {
            ScenarioContext.Current["client_id"] = p0;
        }

        [Given(@"o UserName é '(.*)'")]
        public void DadoOUserNameE(string p0)
        {
            ScenarioContext.Current["username"] = p0;
        }

        [Given(@"a senha do UserName é '(.*)'")]
        public void DadoASenhaDoUserNameE(string p0)
        {
            ScenarioContext.Current["password"] = p0;
        }

        [Given(@"obter o Token")]
        public void DadoObterOToken()
        {
            var client = new RestClient("http://localhost:59329/connect/token");
            var request = new RestRequest(Method.POST);

            //add GetToken() API method parameters
            //utilizando dados fixos a princípio, pois já está cadastrado o aplicativo abaixo
            //com as permissões necessárias
            request.Parameters.Clear();
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_secret", "secret2");
            request.AddParameter("scope", "LogQuake");
            request.AddParameter("client_id", "client2");

            //make the API request and get the response
            IRestResponse response2 = client.Execute(request);

            //return an AccessToken
            Token token = JsonConvert.DeserializeObject<Token>(response2.Content);

            ScenarioContext.Current["token"] = token;
        }

        [Given(@"informar um Token inválido")]
        public void DadoInformarUmTokenInvalido()
        {
            Token token = new Token
            {
                access_token = "9347kdfgn938orwkfnow4893oi4hjoewriuf",
                expires_in = 3600,
                token_type = "Bearer"
            };
            ScenarioContext.Current["token"] = token;
        }


        #region Métodos privados
        private void ExecutarRequest(string endpoint)
        {
            var url = endpoint;

            var request = new RestRequest();

            request.Method = (Method)ScenarioContext.Current["HttpMethod"];

            request.Parameters.Clear();

            if(request.Method == Method.POST || request.Method == Method.PUT || request.Method == Method.PATCH)
            {
                var json = (string)ScenarioContext.Current["data"];

                if (!String.IsNullOrEmpty(json))
                {
                    request.AddParameter("application/json", json, ParameterType.RequestBody);
                }
            }

            var restClient = new RestClient(url);

            //Token token = ObterToken();

            if (ScenarioContext.Current.ContainsKey("token")){
                Token token = (Token)ScenarioContext.Current["token"];

                request.AddParameter("Authorization", "Bearer " + token.access_token, ParameterType.HttpHeader);
            }
            var response = restClient.Execute(request);

            ScenarioContext.Current["Response"] = response;
        }
        #endregion
    }

    //Simula o objeto retornado quando efetua uma geração de Token
    public class Token 
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }

    }
    
}
