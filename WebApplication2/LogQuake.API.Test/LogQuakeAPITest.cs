using LogQuake.API.Controllers;
using LogQuake.Domain.Entities;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.CrossCuting;
using LogQuake.Infra.Data.Contexto;
using LogQuake.Infra.Data.Repositories;
using LogQuake.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace LogQuake.API.Test
{
    [TestClass]
    public class LogQuakeAPITest
    {
        #region Atributos
        private LogQuakeContext _context;
        private ILogQuakeService _logQuakeService;
        private IKillRepository _killRepository;
        private GamesController controller;
        #endregion

        #region Cria��o do Contexto
        [TestInitialize]
        public void InitContext()
        {
            var builder = new DbContextOptionsBuilder<LogQuakeContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database");

            _context = new LogQuakeContext(builder.Options);

            _killRepository = new KillRepository(_context);
            _logQuakeService = new LogQuakeService(_killRepository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _context = null;
        }
        #endregion

        #region Cria��o do Banco de Dados
        private void PreparaBaseDeDados(bool ComRegistros = true)
        {
            _context.Kills.RemoveRange(_context.Kills);
            if (ComRegistros)
            {
                List<Kill> kills = new List<Kill>();
                kills.Add(new Kill { Id = 1, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Isgalamido" });
                kills.Add(new Kill { Id = 2, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Dono da Bola" });
                kills.Add(new Kill { Id = 3, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
                kills.Add(new Kill { Id = 4, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
                kills.Add(new Kill { Id = 5, IdGame = 1, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
                kills.Add(new Kill { Id = 6, IdGame = 1, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });

                kills.Add(new Kill { Id = 7, IdGame = 2, PlayerKiller = "Zeh", PlayerKilled = "Dono da Bola" });
                kills.Add(new Kill { Id = 8, IdGame = 2, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });

                kills.Add(new Kill { Id = 9, IdGame = 3, PlayerKiller = "Zeh", PlayerKilled = "Dono da Bola" });
                kills.Add(new Kill { Id = 10, IdGame = 3, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });

                kills.Add(new Kill { Id = 11, IdGame = 4, PlayerKiller = "Teste", PlayerKilled = "Docinho" });
                kills.Add(new Kill { Id = 12, IdGame = 4, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
                _context.Kills.AddRange(kills);
            }
            _context.SaveChanges();

            controller = new GamesController(_logQuakeService);
        }
        #endregion

        #region Testes Unit�rios
        [TestMethod]
        public void ConsultaPaginada()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 1;
            pageRequest.PageSize = 2;
            var result = controller.Get(pageRequest) as OkObjectResult;
            Dictionary<string, Game> game = (Dictionary<string, Game>)result.Value;

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(game["game_1"].TotalKills == 6);
            Assert.IsTrue(game["game_1"].Players.Length == 3);
            Assert.IsTrue(game["game_1"].Players[0] == "Zeh");
            Assert.IsTrue(game["game_1"].Players[1] == "Isgalamido");
            Assert.IsTrue(game["game_1"].Players[2] == "Dono da Bola");
            Assert.IsTrue(game["game_1"].Kills.Count == 3);
            Assert.IsTrue(game["game_1"].Kills["Zeh"] == 2);
            Assert.IsTrue(game["game_1"].Kills["Isgalamido"] == 1);
            Assert.IsTrue(game["game_1"].Kills["Dono da Bola"] == -5);

            Assert.IsTrue(game["game_2"].TotalKills == 2);
            Assert.IsTrue(game["game_2"].Players.Length == 2);
            Assert.IsTrue(game["game_2"].Players[0] == "Zeh");
            Assert.IsTrue(game["game_2"].Players[1] == "Dono da Bola");
            Assert.IsTrue(game["game_2"].Kills.Count == 2);
            Assert.IsTrue(game["game_2"].Kills["Zeh"] == 1);
            Assert.IsTrue(game["game_2"].Kills["Dono da Bola"] == -2);
        }


        [TestMethod]
        public void ConsultaPaginadaNaoEncontrada()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 99999;
            pageRequest.PageSize = 99;
            var result = controller.Get(pageRequest) as NotFoundObjectResult;
            Dictionary<string, Game> game = (Dictionary<string, Game>)result.Value;

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsTrue(game.Count == 0);
        }

        [TestMethod]
        public void ConsultaPorIdGame()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            var result = controller.Get(2) as OkObjectResult;
            Dictionary<string, Game> game = (Dictionary<string, Game>)result.Value;

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(game["game_2"].TotalKills == 2);
            Assert.IsTrue(game["game_2"].Players.Length == 2);
            Assert.IsTrue(game["game_2"].Players[0] == "Zeh");
            Assert.IsTrue(game["game_2"].Players[1] == "Dono da Bola");
            Assert.IsTrue(game["game_2"].Kills.Count == 2);
            Assert.IsTrue(game["game_2"].Kills["Zeh"] == 1);
            Assert.IsTrue(game["game_2"].Kills["Dono da Bola"] == -2);
        }

        [TestMethod]
        public void ConsultaPorIdGameNaoEncontrado()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            var result = controller.Get(-999999) as NotFoundObjectResult;
            Dictionary<string, Game> game = (Dictionary<string, Game>)result.Value;

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsTrue(game.Count == 0);
        }
        #endregion
    }
}