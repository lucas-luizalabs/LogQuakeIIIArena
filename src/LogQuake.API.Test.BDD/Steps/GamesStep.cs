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

            var client = new RestClient(endpoint);
            var request = new RestRequest(metodo);

            string fileLog = Directory.GetCurrentDirectory() + "\\Log\\games.log";

            request.AddFile("file", fileLog);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("key", "value", ParameterType.GetOrPost);

            IRestResponse response = client.Execute(request);

            ScenarioContext.Current["Response"] = response;
        }

        [Then(@"a quantidade de resgistro inseridos deve ser '(.*)'")]
        public void EntaoAQuantidadeDeResgistroInseridosDeveSer(int registrosInseridos)
        {
            var response = (IRestResponse)ScenarioContext.Current["Response"];

            Assert.IsTrue(response.Content.Contains(registrosInseridos.ToString()));
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

            var response = restClient.Execute(request);

            ScenarioContext.Current["Response"] = response;
        }
        #endregion
    }
}
