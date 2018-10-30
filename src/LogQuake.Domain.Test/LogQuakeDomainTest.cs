using LogQuake.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace LogQuake.Domain.Test
{
    [TestClass]
    public class LogQuakeDomainTest
    {
        #region Atributos
        private Game _game;
        #endregion

        #region Construtor
        public LogQuakeDomainTest()
        {
            InitContext();
        }
        #endregion

        #region Criação do Contexto
        [TestInitialize]
        public void InitContext()
        {
            _game = new Game();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _game = null;
        }
        #endregion

        [TestMethod]
        public void RegistraPlayers()
        {
            //arrange
            List<string> players = new List<string>();
            players.Add("Isgalamido");
            players.Add("Zeh");

            //action
            _game.RegistraPlayers(players);

            //assert
            Assert.IsTrue(_game.Players.Length == 2);
        }

        [TestMethod]
        public void RegistraPlayersComPlayerWorld()
        {
            //arrange
            List<string> players = new List<string>();
            players.Add("Isgalamido");
            players.Add("<world>");
            players.Add("Zeh");

            //action
            _game.RegistraPlayers(players);

            //assert
            Assert.IsTrue(_game.Players.Length == 2);
        }

        [TestMethod]
        public void RegistraPlayersComPlayerNulo()
        {
            //arrange
            List<string> players = new List<string>();
            players.Add("Isgalamido");
            players.Add(null);
            players.Add("Zeh");

            //action
            _game.RegistraPlayers(players);

            //assert
            Assert.IsTrue(_game.Players.Length == 2);
        }

        [TestMethod]
        public void RegistraPlayersNulo()
        {
            //arrange

            //action
            _game.RegistraPlayers(null);

            //assert
            Assert.IsTrue(_game.Players == null);
        }

        [TestMethod]
        public void RegistraMorte()
        {
            //arrange

            //action
            _game.RegistraMorte("<world>","Zeh");

            //assert
            Assert.IsTrue(_game.TotalKills == 1);
            Assert.IsTrue(_game.Kills.Count == 1);
            Assert.IsTrue(_game.Kills["Zeh"] == -1);
        }

        [TestMethod]
        public void RegistraMorteComDoisJogadoresValidos()
        {
            //arrange

            //action
            _game.RegistraMorte("Isgalamido", "Zeh");

            //assert
            Assert.IsTrue(_game.TotalKills == 1);
            Assert.IsTrue(_game.Kills.Count == 2);
            Assert.IsTrue(_game.Kills["Zeh"] == -1);
            Assert.IsTrue(_game.Kills["Isgalamido"] == 1);
        }

        [TestMethod]
        public void RegistraMorteComDoisJogadoresUmMatandoOOutro()
        {
            //arrange

            //action
            _game.RegistraMorte("Isgalamido", "Zeh");
            _game.RegistraMorte("Zeh", "Isgalamido");

            //assert
            Assert.IsTrue(_game.TotalKills == 2);
            Assert.IsTrue(_game.Kills.Count == 0);
        }

        [TestMethod]
        public void RegistraMorteComTresJogadores()
        {
            //arrange

            //action
            _game.RegistraMorte("Isgalamido", "Zeh");
            _game.RegistraMorte("<world>", "Zeh");
            _game.RegistraMorte("Zeh", "Docinho");

            //assert
            Assert.IsTrue(_game.TotalKills == 3);
            Assert.IsTrue(_game.Kills.Count == 3);
            Assert.IsTrue(_game.Kills["Isgalamido"] == 1);
            Assert.IsTrue(_game.Kills["Zeh"] == -1);
            Assert.IsTrue(_game.Kills["Docinho"] == -1);
        }

    }
}
