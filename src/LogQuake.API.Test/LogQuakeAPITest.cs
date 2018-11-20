using LogQuake.API.Controllers;
using LogQuake.CrossCutting;
using LogQuake.Domain.Dto;
using LogQuake.Domain.Entities;
using LogQuake.Infra.Data.Contexto;
using LogQuake.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LogQuake.Infra.UoW;

namespace LogQuake.API.Test
{
    [TestClass]
    public class LogQuakeAPITest
    {
        #region Atributos
        private SQLiteLogQuakeContext _context;
        private ILogQuakeService _logQuakeService;
        private GamesController _controller;
        private ILogger<GamesController> _loggerGamesController;
        private ILogger<LogQuakeService> _loggerLogQuakeServices;
        private IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private IConfiguration _configuration;
        private UnitOfWork _unitOfWork;
        #endregion

        #region Criação do Contexto
        [TestInitialize]
        public void InitContext()
        {
            var builder = new DbContextOptionsBuilder<SQLiteLogQuakeContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database");

            _context = new SQLiteLogQuakeContext(builder.Options);

            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

            LoggerFactory loggerFactoryServices = new LoggerFactory();
            loggerFactoryServices.AddConsole(LogLevel.None);
            loggerFactoryServices.AddDebug(LogLevel.None);
            _loggerLogQuakeServices = new Logger<LogQuakeService>(loggerFactoryServices);

            _unitOfWork = new UnitOfWork(_context, cache, _configuration);

            _logQuakeService = new LogQuakeService(_unitOfWork, cache, _loggerLogQuakeServices, _configuration);

            LoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.None);
            loggerFactory.AddDebug(LogLevel.None);
            _loggerGamesController = new Logger<GamesController>(loggerFactory);

            _configuration = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _context = null;
        }
        #endregion

        #region Criação do Banco de Dados
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

            _controller = new GamesController(_logQuakeService, _loggerGamesController, _cache, _configuration);
        }
        #endregion

        #region Testes Unitários
        [TestMethod]
        public void ConsultaPaginada()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            DtoGameRequest dto = new DtoGameRequest();
            dto.PageNumber = 1;
            dto.PageSize = 2;
            var result = _controller.Get(dto) as OkObjectResult;
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
            DtoGameRequest dto = new DtoGameRequest();
            dto.PageNumber = 99999;
            dto.PageSize = 99;
            var result = _controller.Get(dto) as NotFoundObjectResult;
            DtoResponseBase notification = (DtoResponseBase)result.Value;

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsTrue(notification.Notifications[0].ErrorCode == 2);
            Assert.IsTrue(notification.Success == false);
        }

        [TestMethod]
        public void ConsultaPorIdGame()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            var result = _controller.Get(2) as OkObjectResult;
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
            var result = _controller.Get(-999999) as NotFoundObjectResult;
            DtoResponseBase notification = (DtoResponseBase)result.Value;

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsTrue(notification.Notifications[0].ErrorCode == 2);
            Assert.IsTrue(notification.Success == false);
        }
        #endregion
    }
}
