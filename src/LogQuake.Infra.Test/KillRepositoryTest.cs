using LogQuake.Domain.Entities;
using LogQuake.Infra.Data.Contexto;
using LogQuake.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace LogQuake.Infra.Test
{
    [TestClass]
    public class KillRepositoryTest
    {
        #region Atributos
        private SQLiteLogQuakeContext _context;
        private KillRepository _killRepository;
        private IConfiguration _configuration;
        #endregion

        #region Construtor
        public KillRepositoryTest()
        {
            InitContext();
        }
        #endregion

        #region Criação do Contexto
        [TestInitialize]
        public void InitContext()
        {
            var builder = new DbContextOptionsBuilder<SQLiteLogQuakeContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database");

            _context = new SQLiteLogQuakeContext(builder.Options);

            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

            _killRepository = new KillRepository(_context, cache, _configuration);

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

        #region Criação do BD
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
                _context.Kills.AddRange(kills);
            }
            _context.SaveChanges();
        }
        #endregion

        #region Testes Unitários
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InserirRegistroNulo()
        {
            //arrange

            //action
            _killRepository.Add(null);
            _context.SaveChanges();

            //assert
        }

        [TestMethod]
        public void InserirRegistro()
        {
            //arrange
            PreparaBaseDeDados(false);

            //action
            Kill kill = new Kill();
            kill.IdGame = 1;
            kill.PlayerKilled = "Zeh";
            kill.PlayerKiller = "<world>";
            _killRepository.Add(kill);
            _context.SaveChanges();

            //assert
            Assert.IsTrue(_killRepository.Count() == 1);
        }

        [TestMethod]
        public void InserirDoisRegistros()
        {
            //arrange
            PreparaBaseDeDados(false);

            //action
            Kill kill;
            for (int i = 1; i <= 2; i++)
            {
                kill = new Kill();
                kill.IdGame = 1;
                kill.PlayerKilled = "Zeh";
                kill.PlayerKiller = "<world>";
                _killRepository.Add(kill);
            }
            _context.SaveChanges();

            //assert
            Assert.IsTrue(_killRepository.Count() == 2);
        }
        
        [TestMethod]
        public void InserirResgistroSemPlayerKilled()
        {
            //arrange
            PreparaBaseDeDados(false);

            //action
            Kill kill = new Kill();
            kill.Id = 0;
            kill.IdGame = 1;
            kill.PlayerKiller = "<world>";
            _killRepository.Add(kill);
            _context.SaveChanges();

            //assert
            Assert.IsTrue(_killRepository.Count() == 1);
        }

        [TestMethod]
        public void AlterarResgistro()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            Kill retorno = _killRepository.GetById(3);
            retorno.PlayerKiller = "Assassino";
            retorno.PlayerKilled = "Morto";

            _killRepository.Update(retorno);
            _context.SaveChanges();
            retorno = _killRepository.GetById(3);

            //assert
            Assert.IsTrue(retorno.Id == 3);
            Assert.IsTrue(retorno.IdGame == 1);
            Assert.IsTrue(retorno.PlayerKiller == "Assassino");
            Assert.IsTrue(retorno.PlayerKilled == "Morto");
        }

        [TestMethod]
        public void AlterarResgistroMudandoJogo()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            Kill retorno = _killRepository.GetById(3);
            retorno.IdGame = 9999;

            _killRepository.Update(retorno);
            _context.SaveChanges();
            retorno = _killRepository.GetById(3);

            //assert
            Assert.IsTrue(retorno.Id == 3);
            Assert.IsTrue(retorno.IdGame == 9999);
            Assert.IsTrue(retorno.PlayerKiller == "<world>");
            Assert.IsTrue(retorno.PlayerKilled == "Dono da Bola");
        }

        [TestMethod]
        public void ExcluirResgistro()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            Kill retorno = _killRepository.GetById(3);
            _killRepository.Remove(retorno);
            _context.SaveChanges();
            retorno = _killRepository.GetById(3);

            //assert
            Assert.IsNull(retorno);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExcluirResgistroNaoEncontrado()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            _killRepository.Remove(null);
            _context.SaveChanges();

            //assert
            //Assert.IsNull(null);
        }
        #endregion
    }
}
