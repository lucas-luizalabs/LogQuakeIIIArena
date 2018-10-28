using LogQuake.API.Controllers;
using LogQuake.Domain.Entities;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.CrossCuting;
using LogQuake.Infra.Data.Contexto;
using LogQuake.Infra.Data.Repositories;
using LogQuake.Service.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogQuake.Service.Test
{
    [TestClass]
    public class LogQuakeServiceTest
    {
        #region Atributos
        private LogQuakeContext _context;
        private KillRepository _killRepository;
        private LogQuakeService _logQuakeService;
        #endregion

        public LogQuakeServiceTest()
        {
            InitContext();
        }

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

        [TestMethod]
        public void BuscaPaginada()
        {
            //arrange
            var books = Enumerable.Range(1, 10).Select(i => new Kill { Id = i, IdGame = i, PlayerKilled = "Wrox Press" });
            _context.Kills.RemoveRange(_context.Kills);
            _context.Kills.AddRange(books);
            _context.SaveChanges();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 1;
            pageRequest.PageSize = 5;
            Dictionary<string, _Game> result = _logQuakeService.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 5);
            Assert.IsTrue(result.Values.First().TotalKills == 1);
        }

        [TestMethod]
        public void BuscaPrimeiraPagina()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 1;
            pageRequest.PageSize = 5;
            Dictionary<string, _Game> result = _logQuakeService.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 4);
            Assert.IsTrue(result.Values.First().TotalKills == 6);
            Assert.IsTrue(result.Values.First().Kills["Zeh"] == 2);
            Assert.IsTrue(result.Values.First().Kills["Isgalamido"] == 1);
            Assert.IsTrue(result.Values.First().Kills["Dono da Bola"] == -5);
        }

        [TestMethod]
        public void BuscaSegundaPagina()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 2;
            pageRequest.PageSize = 3;
            Dictionary<string, _Game> result = _logQuakeService.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.Values.First().TotalKills == 2);
            Assert.IsTrue(result.Values.First().Kills["Teste"] == 1);
            Assert.IsTrue(result.Values.First().Kills["Docinho"] == -1);
            Assert.IsTrue(result.Values.First().Kills["Dono da Bola"] == -1);
        }

        [TestMethod]
        public void BuscaNaoEncontrandoPagina200()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 200;
            pageRequest.PageSize = 3;
            Dictionary<string, _Game> result = _logQuakeService.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 0);
        }


        [TestMethod]
        public void BuscaPaginadaQueNaoEncontradaNada()
        {
            //arrange
            PreparaBaseDeDados(false);

            //limpando a tabela Kill
            //_context.Kills.RemoveRange(_context.Kills);
            //_context.SaveChanges();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 1;
            pageRequest.PageSize = 5;
            Dictionary<string, _Game> result = _logQuakeService.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result.Count == 0, "Erro");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuscaPaginadaNulo()
        {
            //arrange

            //action
            Dictionary<string, _Game> result = _logQuakeService.GetAll(null);

            //assert
        }

        [TestMethod]
        public void BuscaPorId()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            Dictionary<string, _Game> result = _logQuakeService.GetById(2);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Values.First().TotalKills == 2);
            Assert.IsTrue(result.Values.First().Kills["Zeh"] == 1);
            Assert.IsTrue(result.Values.First().Kills["Dono da Bola"] == -2);
        }

        [TestMethod]
        public void BuscaPorIdNaoencontrado()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            Dictionary<string, _Game> result = _logQuakeService.GetById(22);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Values.Count == 0);
        }

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
        }

    }
}
