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
        private LogQuakeContext _logQuakeContext;
        private KillRepository _killRepository;
        private ServiceBase<Kill> _serviceBase;

        public LogQuakeServiceTest()
        {
            InitContext();
        }

        public void InitContext()
        {
            var builder = new DbContextOptionsBuilder<LogQuakeContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database");

            _logQuakeContext = new LogQuakeContext(builder.Options);
            //var books = Enumerable.Range(1, 10)
            //    .Select(i => new Kill { Id = i, IdGame = i, PlayerKilled = "Wrox Press" });
            //context.Kills.AddRange(books);
            //int changed = context.SaveChanges();

            //_logQuakeContext = context;
            //_killRepository = context.Kills;
            _killRepository = new KillRepository(_logQuakeContext);
            _serviceBase = new ServiceBase<Kill>(_killRepository);
        }

        [TestMethod]
        public void BuscaPaginada()
        {
            //arrange
            LogQuakeService controller = new LogQuakeService(_killRepository);
            var books = Enumerable.Range(1, 10).Select(i => new Kill { Id = i, IdGame = i, PlayerKilled = "Wrox Press" });
            _logQuakeContext.Kills.RemoveRange(_logQuakeContext.Kills);
            _logQuakeContext.Kills.AddRange(books);
            _logQuakeContext.SaveChanges();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 1;
            pageRequest.PageSize = 5;
            Dictionary<string, _Game> result = controller.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 5);
            Assert.IsTrue(result.Values.First().TotalKills == 1);
        }

        [TestMethod]
        public void BuscaPrimeiraPagina()
        {
            //arrange
            LogQuakeService controller = new LogQuakeService(_killRepository);
            List<Kill> books = new List<Kill>();
            books.Add(new Kill { Id = 1, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Isgalamido" });
            books.Add(new Kill { Id = 2, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 3, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 4, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 5, IdGame = 1, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 6, IdGame = 1, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            _logQuakeContext.Kills.RemoveRange(_logQuakeContext.Kills);
            _logQuakeContext.Kills.AddRange(books);
            _logQuakeContext.SaveChanges();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 1;
            pageRequest.PageSize = 5;
            Dictionary<string, _Game> result = controller.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.Values.First().TotalKills == 6);
            Assert.IsTrue(result.Values.First().Kills["Zeh"] == 2);
            Assert.IsTrue(result.Values.First().Kills["Isgalamido"] == 1);
            Assert.IsTrue(result.Values.First().Kills["Dono da Bola"] == -5);
        }

        [TestMethod]
        public void BuscaSegundaPagina()
        {
            //arrange
            LogQuakeService controller = new LogQuakeService(_killRepository);
            List<Kill> books = new List<Kill>();
            books.Add(new Kill { Id = 1, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Isgalamido" });
            books.Add(new Kill { Id = 2, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 3, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 4, IdGame = 2, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 5, IdGame = 2, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 6, IdGame = 2, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            _logQuakeContext.Kills.RemoveRange(_logQuakeContext.Kills);
            _logQuakeContext.Kills.AddRange(books);
            _logQuakeContext.SaveChanges();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 2;
            pageRequest.PageSize = 3;
            Dictionary<string, _Game> result = controller.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.Values.First().TotalKills == 3);
            Assert.IsTrue(result.Values.First().Kills["Isgalamido"] == 2);
            Assert.IsTrue(result.Values.First().Kills["Dono da Bola"] == -3);
        }

        [TestMethod]
        public void BuscaNaoEncontrandoSegundaPagina()
        {
            //arrange
            LogQuakeService controller = new LogQuakeService(_killRepository);
            List<Kill> books = new List<Kill>();
            books.Add(new Kill { Id = 1, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Isgalamido" });
            books.Add(new Kill { Id = 2, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 3, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 4, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 5, IdGame = 1, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 6, IdGame = 1, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            _logQuakeContext.Kills.RemoveRange(_logQuakeContext.Kills);
            _logQuakeContext.Kills.AddRange(books);
            _logQuakeContext.SaveChanges();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 2;
            pageRequest.PageSize = 3;
            Dictionary<string, _Game> result = controller.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 0);
        }


        [TestMethod]
        public void BuscaPaginadaQueNaoEncontradaNada()
        {
            //arrange
            LogQuakeService controller = new LogQuakeService(_killRepository);
            //limpando a tabela Kill
            _logQuakeContext.Kills.RemoveRange(_logQuakeContext.Kills);
            _logQuakeContext.SaveChanges();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 1;
            pageRequest.PageSize = 5;
            Dictionary<string, _Game> result = controller.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result.Count == 0, "Erro");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuscaPaginadaNulo()
        {
            //arrange
            LogQuakeService controller = new LogQuakeService(_killRepository);

            //action
            Dictionary<string, _Game> result = controller.GetAll(null);

            //assert
        }

        [TestMethod]
        public void BuscaPorId()
        {
            //arrange
            LogQuakeService controller = new LogQuakeService(_killRepository);
            List<Kill> books = new List<Kill>();
            books.Add(new Kill { Id = 1, IdGame = 2, PlayerKiller = "Zeh", PlayerKilled = "Isgalamido" });
            books.Add(new Kill { Id = 2, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 3, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 4, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 5, IdGame = 2, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 6, IdGame = 1, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            _logQuakeContext.Kills.RemoveRange(_logQuakeContext.Kills);
            _logQuakeContext.Kills.AddRange(books);
            _logQuakeContext.SaveChanges();

            //action
            Dictionary<string, _Game> result = controller.GetById(2);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Values.First().TotalKills == 2);
            Assert.IsTrue(result.Values.First().Kills["Zeh"] == 1);
            Assert.IsTrue(result.Values.First().Kills["Dono da Bola"] == -1);
        }

        [TestMethod]
        public void BuscaPorIdNaoencontrado()
        {
            //arrange
            LogQuakeService controller = new LogQuakeService(_killRepository);
            List<Kill> books = new List<Kill>();
            books.Add(new Kill { Id = 1, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Isgalamido" });
            books.Add(new Kill { Id = 2, IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 3, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 4, IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 5, IdGame = 1, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            books.Add(new Kill { Id = 6, IdGame = 1, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            _logQuakeContext.Kills.RemoveRange(_logQuakeContext.Kills);
            _logQuakeContext.Kills.AddRange(books);
            _logQuakeContext.SaveChanges();

            //action
            Dictionary<string, _Game> result = controller.GetById(2);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Values.Count == 0);
        }
    }
}
