using LogQuake.Infra.Data.Contexto;
using LogQuake.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LogQuake.Infra.Test
{
    [TestClass]
    public class KillRepositoryTest
    {

        private LogQuakeContext _logQuakeContext;
        private KillRepository _killRepository;
        //private ServiceBase<Kill> _serviceBase;

        public KillRepositoryTest()
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
            //_serviceBase = new ServiceBase<Kill>(_killRepository);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InserirRegistro()
        {
            _killRepository.Add(null);
        }
    }
}
