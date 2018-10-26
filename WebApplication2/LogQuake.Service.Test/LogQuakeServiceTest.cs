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
            _logQuakeContext.Kills.AddRange(books);
            _logQuakeContext.SaveChanges();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 1;
            pageRequest.PageSize = 5;
            List<_Game> result = controller.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 5);
            Assert.IsTrue(result[0].TotalKills == 1);
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
            List<_Game> result = controller.GetAll(pageRequest);

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
            List<_Game> result = controller.GetAll(null);

            //assert
        }

    }
}
