//using LogQuake.API.Controllers;
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
using System.IO;
using System.Linq;
using System.Reflection;

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

        #region Construtor
        public LogQuakeServiceTest()
        {
            InitContext();
        }
        #endregion

        #region Criação do Contexto
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

        #region Criação do Banco de Dados
        private void PreparaBaseDeDados(bool ComRegistros = true)
        {
            _context.Kills.RemoveRange(_context.Kills);
            if (ComRegistros)
            {
                //var kills = Enumerable.Range(1, 10).Select(i => new Kill { Id = i, IdGame = i, PlayerKilled = "Wrox Press" });
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
        #endregion

        #region Testes Unitários
        [TestMethod]
        public void BuscaPaginada()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 1;
            pageRequest.PageSize = 5;
            Dictionary<string, Game> result = _logQuakeService.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 4);
            Assert.IsTrue(result.Values.First().TotalKills == 6);
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
            Dictionary<string, Game> result = _logQuakeService.GetAll(pageRequest);

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
            Dictionary<string, Game> result = _logQuakeService.GetAll(pageRequest);

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
            Dictionary<string, Game> result = _logQuakeService.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count == 0);
        }


        [TestMethod]
        public void BuscaPaginadaQueNaoEncontradaNada()
        {
            //arrange
            PreparaBaseDeDados(false);

            //action
            PageRequestBase pageRequest = new PageRequestBase();
            pageRequest.PageNumber = 1;
            pageRequest.PageSize = 5;
            Dictionary<string, Game> result = _logQuakeService.GetAll(pageRequest);

            //assert
            Assert.IsTrue(result.Count == 0, "Erro");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuscaPaginadaNulo()
        {
            //arrange

            //action
            Dictionary<string, Game> result = _logQuakeService.GetAll(null);

            //assert
        }

        [TestMethod]
        public void BuscaPorId()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            Dictionary<string, Game> result = _logQuakeService.GetById(2);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Values.First().TotalKills == 2);
            Assert.IsTrue(result.Values.First().Kills["Zeh"] == 1);
            Assert.IsTrue(result.Values.First().Kills["Dono da Bola"] == -2);
        }

        [TestMethod]
        public void BuscaPorIdNaoEncontrado()
        {
            //arrange
            PreparaBaseDeDados();

            //action
            Dictionary<string, Game> result = _logQuakeService.GetById(22);

            //assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Values.Count == 0);
        }

        [TestMethod]
        public void LeituraDeArquivoLogExistente()
        {
            //arrange
            string path = Path.Combine(Directory.GetCurrentDirectory(), "log", "games.log");

            //action
            List<string> linhas = _logQuakeService.LerArquivoDeLog(path);

            //assert
            Assert.IsTrue(linhas.Count == 5210);
        }

        [TestMethod]
        public void LeituraDeArquivoLogInexistente()
        {
            //arrange
            string path = Path.Combine(Directory.GetCurrentDirectory(), "log", "games.logx");

            //action
            List<string> linhas = _logQuakeService.LerArquivoDeLog(path);

            //assert
            Assert.IsTrue(linhas.Count == 0);
        }

        [TestMethod]
        public void LeituraDeArquivoLogInexistenteNulo()
        {
            //arrange

            //action
            List<string> linhas = _logQuakeService.LerArquivoDeLog(null);

            //assert
            Assert.IsTrue(linhas.Count == 0);
        }

        [TestMethod]
        public void LeituraDeArquivoLogInexistenteVazio()
        {
            //arrange

            //action
            List<string> linhas = _logQuakeService.LerArquivoDeLog("");

            //assert
            Assert.IsTrue(linhas.Count == 0);
        }

        [TestMethod]
        public void ConverterArquivoEmListaDeKillZerado()
        {
            //arrange
            List<string> linhas = new List<string>();

            //action
            List<Kill> kills = _logQuakeService.ConverterArquivoEmListaDeKill(linhas);

            //assert
            Assert.IsTrue(linhas.Count == 0);
        }

        [TestMethod]
        public void ConverterArquivoParcialEmListaDeKill()
        {
            //arrange
            List<string> linhas = new List<string>();
            linhas.Add(@"  0:00------------------------------------------------------------");
            linhas.Add(@"  0:00 InitGame: \sv_floodProtect\1\sv_maxPing\0\sv_minPing\0\sv_maxRate\10000\sv_minRate\0\sv_hostname\Code Miner Server\g_gametype\0\sv_privateClients\2\sv_maxclients\16\sv_allowDownload\0\dmflags\0\fraglimit\20\timelimit\15\g_maxGameClients\0\capturelimit\8\version\ioq3 1.36 linux - x86_64 Apr 12 2009\protocol\68\mapname\q3dm17\gamename\baseq3\g_needpass\0");
            linhas.Add(@"  0:25 ClientConnect: 2");
            linhas.Add(@"  0:25 ClientUserinfoChanged: 2 n\Dono da Bola\t\0\model\sarge / krusade\hmodel\sarge / krusade\g_redteam\\g_blueteam\\c1\5\c2\5\hc\95\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  0:27 ClientUserinfoChanged: 2 n\Mocinha\t\0\model\sarge\hmodel\sarge\g_redteam\\g_blueteam\\c1\4\c2\5\hc\95\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  0:27 ClientBegin: 2");
            linhas.Add(@"  0:29 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  0:35 Item: 2 item_armor_shard");
            linhas.Add(@"  0:35 Item: 2 item_armor_shard");
            linhas.Add(@"  0:35 Item: 2 item_armor_shard");
            linhas.Add(@"  0:35 Item: 2 item_armor_combat");
            linhas.Add(@"  0:38 Item: 2 item_armor_shard");
            linhas.Add(@"  0:38 Item: 2 item_armor_shard");
            linhas.Add(@"  0:38 Item: 2 item_armor_shard");
            linhas.Add(@"  0:55 Item: 2 item_health_large");
            linhas.Add(@"  0:56 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  0:57 Item: 2 ammo_rockets");
            linhas.Add(@"  0:59 ClientConnect: 3");

            //action
            List<Kill> kills = _logQuakeService.ConverterArquivoEmListaDeKill(linhas);

            //assert
            Assert.IsTrue(kills.Count == 0);
        }

        [TestMethod]
        public void ConverterArquivoEmListaDeKillCom1Jogo()
        {
            #region Arrange
            //arrange
            List<string> linhas = new List<string>();
            linhas.Add(@"  0:00------------------------------------------------------------");
            linhas.Add(@"  0:00 InitGame: \sv_floodProtect\1\sv_maxPing\0\sv_minPing\0\sv_maxRate\10000\sv_minRate\0\sv_hostname\Code Miner Server\g_gametype\0\sv_privateClients\2\sv_maxclients\16\sv_allowDownload\0\dmflags\0\fraglimit\20\timelimit\15\g_maxGameClients\0\capturelimit\8\version\ioq3 1.36 linux - x86_64 Apr 12 2009\protocol\68\mapname\q3dm17\gamename\baseq3\g_needpass\0");
            linhas.Add(@"  0:25 ClientConnect: 2");
            linhas.Add(@"  0:25 ClientUserinfoChanged: 2 n\Dono da Bola\t\0\model\sarge / krusade\hmodel\sarge / krusade\g_redteam\\g_blueteam\\c1\5\c2\5\hc\95\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  0:27 ClientUserinfoChanged: 2 n\Mocinha\t\0\model\sarge\hmodel\sarge\g_redteam\\g_blueteam\\c1\4\c2\5\hc\95\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  0:27 ClientBegin: 2");
            linhas.Add(@"  0:29 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  0:35 Item: 2 item_armor_shard");
            linhas.Add(@"  0:35 Item: 2 item_armor_shard");
            linhas.Add(@"  0:35 Item: 2 item_armor_shard");
            linhas.Add(@"  0:35 Item: 2 item_armor_combat");
            linhas.Add(@"  0:38 Item: 2 item_armor_shard");
            linhas.Add(@"  0:38 Item: 2 item_armor_shard");
            linhas.Add(@"  0:38 Item: 2 item_armor_shard");
            linhas.Add(@"  0:55 Item: 2 item_health_large");
            linhas.Add(@"  0:56 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  0:57 Item: 2 ammo_rockets");
            linhas.Add(@"  0:59 ClientConnect: 3");
            linhas.Add(@"  0:59 ClientUserinfoChanged: 3 n\Isgalamido\t\0\model\xian / default\hmodel\xian / default\g_redteam\\g_blueteam\\c1\4\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:01 ClientUserinfoChanged: 3 n\Isgalamido\t\0\model\uriel / zael\hmodel\uriel / zael\g_redteam\\g_blueteam\\c1\5\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:01 ClientBegin: 3");
            linhas.Add(@"  1:02 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  1:04 Item: 2 item_armor_shard");
            linhas.Add(@"  1:04 Item: 2 item_armor_shard");
            linhas.Add(@"  1:04 Item: 2 item_armor_shard");
            linhas.Add(@"  1:06 ClientConnect: 4");
            linhas.Add(@"  1:06 ClientUserinfoChanged: 4 n\Zeh\t\0\model\sarge / default\hmodel\sarge / default\g_redteam\\g_blueteam\\c1\5\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:08 Kill: 3 2 6: Isgalamido killed Mocinha by MOD_ROCKET");
            linhas.Add(@"  1:08 ClientUserinfoChanged: 4 n\Zeh\t\0\model\sarge / default\hmodel\sarge / default\g_redteam\\g_blueteam\\c1\1\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:08 ClientBegin: 4");
            linhas.Add(@"  1:10 Item: 3 item_armor_shard");
            linhas.Add(@"  1:10 Item: 3 item_armor_shard");
            linhas.Add(@"  1:10 Item: 3 item_armor_shard");
            linhas.Add(@"  1:10 Item: 3 item_armor_combat");
            linhas.Add(@"  1:11 Item: 4 weapon_shotgun");
            linhas.Add(@"  1:11 Item: 4 ammo_shells");
            linhas.Add(@"  1:16 Item: 4 item_health_large");
            linhas.Add(@"  1:18 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  1:18 Item: 4 ammo_rockets");
            linhas.Add(@"  1:26 Kill: 1022 4 22: <world> killed Zeh by MOD_TRIGGER_HURT");
            linhas.Add(@"  1:26 ClientUserinfoChanged: 2 n\Dono da Bola\t\0\model\sarge\hmodel\sarge\g_redteam\\g_blueteam\\c1\4\c2\5\hc\95\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:26 Item: 3 weapon_railgun");
            linhas.Add(@"  1:29 Item: 2 weapon_rocketlauncherv");
            linhas.Add(@"  1:29 Item: 3 weapon_railgun");
            linhas.Add(@"  1:32 Item: 3 weapon_railgun");
            linhas.Add(@"  1:32 Kill: 1022 4 22: <world> killed Zeh by MOD_TRIGGER_HURT");
            linhas.Add(@"  1:35 Item: 2 item_armor_shard");
            linhas.Add(@"  1:35 Item: 2 item_armor_shard");
            linhas.Add(@"  1:35 Item: 2 item_armor_shard");
            linhas.Add(@"  1:35 Item: 3 weapon_railgun");
            linhas.Add(@"  1:38 Item: 2 item_health_large");
            linhas.Add(@"  1:38 Item: 3 weapon_railgun");
            linhas.Add(@"  1:41 Kill: 1022 2 19: <world> killed Dono da Bola by MOD_FALLING");
            linhas.Add(@"  1:41 Item: 3 weapon_railgun");
            linhas.Add(@"  1:43 Item: 2 ammo_rockets");
            linhas.Add(@"  1:44 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  1:46 Item: 2 item_armor_shard");
            linhas.Add(@"  1:47 Item: 2 item_armor_shard");
            linhas.Add(@"  1:47 Item: 2 item_armor_shard");
            linhas.Add(@"  1:47 ShutdownGame:");
            #endregion

            //action
            List<Kill> kills = _logQuakeService.ConverterArquivoEmListaDeKill(linhas);

            //assert
            Assert.IsTrue(kills.Count == 4);

            Assert.IsTrue(kills[0].IdGame == 1);
            Assert.IsTrue(kills[0].PlayerKiller == "Isgalamido");
            Assert.IsTrue(kills[0].PlayerKilled == "Mocinha");

            Assert.IsTrue(kills[1].IdGame == 1);
            Assert.IsTrue(kills[1].PlayerKiller == "<world>");
            Assert.IsTrue(kills[1].PlayerKilled == "Zeh");

            Assert.IsTrue(kills[2].IdGame == 1);
            Assert.IsTrue(kills[2].PlayerKiller == "<world>");
            Assert.IsTrue(kills[2].PlayerKilled == "Zeh");

            Assert.IsTrue(kills[3].IdGame == 1);
            Assert.IsTrue(kills[3].PlayerKiller == "<world>");
            Assert.IsTrue(kills[3].PlayerKilled == "Dono da Bola");
        }

        [TestMethod]
        public void ConverterArquivoEmListaDeKillCom2Jogos()
        {
            #region Arrange
            //arrange
            List<string> linhas = new List<string>();
            linhas.Add(@"  0:00------------------------------------------------------------");
            linhas.Add(@"  0:00 InitGame: \sv_floodProtect\1\sv_maxPing\0\sv_minPing\0\sv_maxRate\10000\sv_minRate\0\sv_hostname\Code Miner Server\g_gametype\0\sv_privateClients\2\sv_maxclients\16\sv_allowDownload\0\dmflags\0\fraglimit\20\timelimit\15\g_maxGameClients\0\capturelimit\8\version\ioq3 1.36 linux - x86_64 Apr 12 2009\protocol\68\mapname\q3dm17\gamename\baseq3\g_needpass\0");
            linhas.Add(@"  0:25 ClientConnect: 2");
            linhas.Add(@"  0:25 ClientUserinfoChanged: 2 n\Dono da Bola\t\0\model\sarge / krusade\hmodel\sarge / krusade\g_redteam\\g_blueteam\\c1\5\c2\5\hc\95\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  0:27 ClientUserinfoChanged: 2 n\Mocinha\t\0\model\sarge\hmodel\sarge\g_redteam\\g_blueteam\\c1\4\c2\5\hc\95\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  0:27 ClientBegin: 2");
            linhas.Add(@"  0:29 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  0:35 Item: 2 item_armor_shard");
            linhas.Add(@"  0:35 Item: 2 item_armor_shard");
            linhas.Add(@"  0:35 Item: 2 item_armor_shard");
            linhas.Add(@"  0:35 Item: 2 item_armor_combat");
            linhas.Add(@"  0:38 Item: 2 item_armor_shard");
            linhas.Add(@"  0:38 Item: 2 item_armor_shard");
            linhas.Add(@"  0:38 Item: 2 item_armor_shard");
            linhas.Add(@"  0:55 Item: 2 item_health_large");
            linhas.Add(@"  0:56 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  0:57 Item: 2 ammo_rockets");
            linhas.Add(@"  0:59 ClientConnect: 3");
            linhas.Add(@"  0:59 ClientUserinfoChanged: 3 n\Isgalamido\t\0\model\xian / default\hmodel\xian / default\g_redteam\\g_blueteam\\c1\4\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:01 ClientUserinfoChanged: 3 n\Isgalamido\t\0\model\uriel / zael\hmodel\uriel / zael\g_redteam\\g_blueteam\\c1\5\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:01 ClientBegin: 3");
            linhas.Add(@"  1:02 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  1:04 Item: 2 item_armor_shard");
            linhas.Add(@"  1:04 Item: 2 item_armor_shard");
            linhas.Add(@"  1:04 Item: 2 item_armor_shard");
            linhas.Add(@"  1:06 ClientConnect: 4");
            linhas.Add(@"  1:06 ClientUserinfoChanged: 4 n\Zeh\t\0\model\sarge / default\hmodel\sarge / default\g_redteam\\g_blueteam\\c1\5\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:08 Kill: 3 2 6: Isgalamido killed Mocinha by MOD_ROCKET");
            linhas.Add(@"  1:08 ClientUserinfoChanged: 4 n\Zeh\t\0\model\sarge / default\hmodel\sarge / default\g_redteam\\g_blueteam\\c1\1\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:08 ClientBegin: 4");
            linhas.Add(@"  1:10 Item: 3 item_armor_shard");
            linhas.Add(@"  1:10 Item: 3 item_armor_shard");
            linhas.Add(@"  1:10 Item: 3 item_armor_shard");
            linhas.Add(@"  1:10 Item: 3 item_armor_combat");
            linhas.Add(@"  1:11 Item: 4 weapon_shotgun");
            linhas.Add(@"  1:11 Item: 4 ammo_shells");
            linhas.Add(@"  1:16 Item: 4 item_health_large");
            linhas.Add(@"  1:18 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  1:18 Item: 4 ammo_rockets");
            linhas.Add(@"  1:26 Kill: 1022 4 22: <world> killed Zeh by MOD_TRIGGER_HURT");
            linhas.Add(@"  1:26 ClientUserinfoChanged: 2 n\Dono da Bola\t\0\model\sarge\hmodel\sarge\g_redteam\\g_blueteam\\c1\4\c2\5\hc\95\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:26 Item: 3 weapon_railgun");
            linhas.Add(@"  1:29 Item: 2 weapon_rocketlauncherv");
            linhas.Add(@"  1:29 Item: 3 weapon_railgun");
            linhas.Add(@"  1:32 Item: 3 weapon_railgun");
            linhas.Add(@"  1:32 Kill: 1022 4 22: <world> killed Zeh by MOD_TRIGGER_HURT");
            linhas.Add(@"  1:35 Item: 2 item_armor_shard");
            linhas.Add(@"  1:35 Item: 2 item_armor_shard");
            linhas.Add(@"  1:35 Item: 2 item_armor_shard");
            linhas.Add(@"  1:35 Item: 3 weapon_railgun");
            linhas.Add(@"  1:38 Item: 2 item_health_large");
            linhas.Add(@"  1:38 Item: 3 weapon_railgun");
            linhas.Add(@"  1:41 Kill: 1022 2 19: <world> killed Dono da Bola by MOD_FALLING");
            linhas.Add(@"  1:41 Item: 3 weapon_railgun");
            linhas.Add(@"  1:43 Item: 2 ammo_rockets");
            linhas.Add(@"  1:44 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  1:46 Item: 2 item_armor_shard");
            linhas.Add(@"  1:47 Item: 2 item_armor_shard");
            linhas.Add(@"  1:47 Item: 2 item_armor_shard");
            linhas.Add(@"  1:47 ShutdownGame:");
            linhas.Add(@"  1:47 ------------------------------------------------------------");
            linhas.Add(@"  1:47 InitGame: \sv_floodProtect\1\sv_maxPing\0\sv_minPing\0\sv_maxRate\10000\sv_minRate\0\sv_hostname\Code Miner Server\g_gametype\0\sv_privateClients\2\sv_maxclients\16\sv_allowDownload\0\bot_minplayers\0\dmflags\0\fraglimit\20\timelimit\15\g_maxGameClients\0\capturelimit\8\version\ioq3 1.36 linux-x86_64 Apr 12 2009\protocol\68\mapname\q3dm17\gamename\baseq3\g_needpass\0");
            linhas.Add(@"  1:47 ClientConnect: 2");
            linhas.Add(@"  1:47 ClientUserinfoChanged: 2 n\Dono da Bola\t\0\model\sarge\hmodel\sarge\g_redteam\\g_blueteam\\c1\4\c2\5\hc\95\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:47 ClientBegin: 2");
            linhas.Add(@"  1:47 ClientConnect: 3");
            linhas.Add(@"  1:47 ClientUserinfoChanged: 3 n\Isgalamido\t\0\model\uriel/zael\hmodel\uriel/zael\g_redteam\\g_blueteam\\c1\5\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:47 ClientBegin: 3");
            linhas.Add(@"  1:47 ClientConnect: 4");
            linhas.Add(@"  1:47 ClientUserinfoChanged: 4 n\Zeh\t\0\model\sarge/default\hmodel\sarge/default\g_redteam\\g_blueteam\\c1\1\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  1:47 ClientBegin: 4");
            linhas.Add(@"  1:48 Item: 4 ammo_rockets");
            linhas.Add(@"  1:48 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  1:51 Item: 3 item_armor_shard");
            linhas.Add(@"  1:51 Item: 3 item_armor_combat");
            linhas.Add(@"  1:54 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  1:54 Item: 3 ammo_rockets");
            linhas.Add(@"  1:57 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  2:00 Kill: 1022 3 22: <world> killed Isgalamido by MOD_TRIGGER_HURT");
            linhas.Add(@"  2:02 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  2:04 Kill: 1022 2 19: <world> killed Dono da Bola by MOD_FALLING");
            linhas.Add(@"  2:04 Item: 4 item_armor_body");
            linhas.Add(@"  2:04 Kill: 1022 3 19: <world> killed Isgalamido by MOD_FALLING");
            linhas.Add(@"  2:07 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  2:11 Kill: 2 4 6: Dono da Bola killed Zeh by MOD_ROCKET");
            linhas.Add(@"  2:14 Item: 3 weapon_railgun");
            linhas.Add(@"  2:20 Item: 3 weapon_railgun");
            linhas.Add(@"  2:22 Kill: 3 2 10: Isgalamido killed Dono da Bola by MOD_RAILGUN");
            linhas.Add(@"  2:23 Item: 3 weapon_railgun");
            linhas.Add(@"  2:27 Item: 4 item_armor_combat");
            linhas.Add(@"  2:29 Kill: 3 4 10: Isgalamido killed Zeh by MOD_RAILGUN");
            linhas.Add(@"  2:32 Item: 3 item_quad");
            linhas.Add(@"  2:33 Item: 4 ammo_shells");
            linhas.Add(@"  2:33 Item: 4 weapon_shotgun");
            linhas.Add(@"  2:37 Kill: 3 2 10: Isgalamido killed Dono da Bola by MOD_RAILGUN");
            linhas.Add(@"  2:42 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  2:43 Kill: 3 4 10: Isgalamido killed Zeh by MOD_RAILGUN");
            linhas.Add(@"  2:45 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  2:45 Item: 3 item_armor_body");
            linhas.Add(@"  2:46 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  2:56 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  2:57 Kill: 3 4 10: Isgalamido killed Zeh by MOD_RAILGUN");
            linhas.Add(@"  3:01 Item: 4 weapon_shotgun");
            linhas.Add(@"  3:12 Item: 3 item_health_large");
            linhas.Add(@"  3:12 Kill: 2 4 6: Dono da Bola killed Zeh by MOD_ROCKET");
            linhas.Add(@"  3:13 Kill: 3 2 6: Isgalamido killed Dono da Bola by MOD_ROCKET");
            linhas.Add(@"  3:25 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  3:27 Kill: 1022 3 22: <world> killed Isgalamido by MOD_TRIGGER_HURT");
            linhas.Add(@"  3:29 Kill: 4 2 6: Zeh killed Dono da Bola by MOD_ROCKET");
            linhas.Add(@"  3:30 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  3:32 Kill: 3 4 6: Isgalamido killed Zeh by MOD_ROCKET");
            linhas.Add(@"  3:34 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  3:37 Kill: 3 4 7: Isgalamido killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@"  3:40 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  3:41 Kill: 2 3 6: Dono da Bola killed Isgalamido by MOD_ROCKET");
            linhas.Add(@"  3:44 Item: 2 ammo_rockets");
            linhas.Add(@"  3:49 ClientUserinfoChanged: 5 n\Assasinu Credi\t\0\model\sarge\hmodel\sarge\g_redteam\\g_blueteam\\c1\4\c2\5\hc\100\w\0\l\0\tt\0\tl\0");
            linhas.Add(@"  3:49 ClientBegin: 5");
            linhas.Add(@"  3:51 Kill: 1022 2 19: <world> killed Dono da Bola by MOD_FALLING");
            linhas.Add(@"  3:54 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  3:56 Item: 4 item_armor_shard");
            linhas.Add(@"  3:56 Item: 4 item_armor_combat");
            linhas.Add(@"  3:59 Kill: 4 5 6: Zeh killed Assasinu Credi by MOD_ROCKET");
            linhas.Add(@"  4:00 Kill: 4 2 6: Zeh killed Dono da Bola by MOD_ROCKET");
            linhas.Add(@"  4:02 Item: 4 item_health_large");
            linhas.Add(@"  4:03 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  4:07 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  4:08 Kill: 4 3 7: Zeh killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@"  4:11 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  4:11 Kill: 1022 5 19: <world> killed Assasinu Credi by MOD_FALLING");
            linhas.Add(@"  4:15 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  4:15 Item: 4 item_armor_shard");
            linhas.Add(@"  4:16 Kill: 1022 3 22: <world> killed Isgalamido by MOD_TRIGGER_HURT");
            linhas.Add(@"  4:17 Item: 5 item_armor_body");
            linhas.Add(@"  4:19 Item: 5 ammo_rockets");
            linhas.Add(@"  4:20 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  4:20 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  4:21 Kill: 4 2 7: Zeh killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  4:22 Kill: 3 4 7: Isgalamido killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@"  4:23 Item: 3 item_armor_shard");
            linhas.Add(@"  4:35 Item: 3 item_quad");
            linhas.Add(@"  4:37 Kill: 3 5 3: Isgalamido killed Assasinu Credi by MOD_MACHINEGUN");
            linhas.Add(@"  4:40 Kill: 3 4 3: Isgalamido killed Zeh by MOD_MACHINEGUN");
            linhas.Add(@"  4:41 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  4:44 Item: 4 ammo_rockets");
            linhas.Add(@"  4:45 Item: 2 item_armor_shard");
            linhas.Add(@"  4:48 Kill: 2 3 6: Dono da Bola killed Isgalamido by MOD_ROCKET");
            linhas.Add(@"  4:48 Item: 2 item_quad");
            linhas.Add(@"  4:50 Item: 5 item_armor_shard");
            linhas.Add(@"  4:51 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  4:54 Kill: 2 2 7: Dono da Bola killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  4:56 Item: 4 item_quad");
            linhas.Add(@"  4:56 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  4:58 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  4:58 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  5:00 Kill: 4 3 7: Zeh killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@"  5:00 Item: 5 item_armor_body");
            linhas.Add(@"  5:02 Item: 5 ammo_rockets");
            linhas.Add(@"  5:03 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  5:03 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  5:04 Item: 4 item_armor_combat");
            linhas.Add(@"  5:08 Item: 4 item_health_large");
            linhas.Add(@"  5:11 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  5:11 Kill: 4 5 6: Zeh killed Assasinu Credi by MOD_ROCKET");
            linhas.Add(@"  5:11 Kill: 4 3 7: Zeh killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@"  5:11 Kill: 5 4 7: Assasinu Credi killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@"  5:14 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  5:22 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  5:24 Kill: 1022 3 19: <world> killed Isgalamido by MOD_FALLING");
            linhas.Add(@"  5:25 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  5:26 Kill: 4 2 7: Zeh killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  5:27 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  5:29 Kill: 4 5 7: Zeh killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@"  5:31 Item: 4 item_armor_shard");
            linhas.Add(@"  5:34 Item: 5 item_armor_body");
            linhas.Add(@"  5:34 Kill: 3 2 7: Isgalamido killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  5:36 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  5:38 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  5:38 Kill: 1022 3 19: <world> killed Isgalamido by MOD_FALLING");
            linhas.Add(@"  5:38 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  5:41 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  5:41 Item: 4 item_health_large");
            linhas.Add(@"  5:41 Item: 2 item_armor_combat");
            linhas.Add(@"  5:43 Kill: 5 4 7: Assasinu Credi killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@"  5:44 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  5:46 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  5:47 Item: 4 ammo_rockets");
            linhas.Add(@"  5:50 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  5:52 Kill: 5 2 7: Assasinu Credi killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  5:56 Kill: 5 3 7: Assasinu Credi killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@"  5:56 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  6:05 Item: 3 weapon_railgun");
            linhas.Add(@"  6:06 Kill: 2 3 6: Dono da Bola killed Isgalamido by MOD_ROCKET");
            linhas.Add(@"  6:07 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  6:08 Kill: 1022 2 22: <world> killed Dono da Bola by MOD_TRIGGER_HURT");
            linhas.Add(@"  6:08 Kill: 4 5 7: Zeh killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@"  6:11 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  6:11 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  6:21 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  6:24 Kill: 1022 2 22: <world> killed Dono da Bola by MOD_TRIGGER_HURT");
            linhas.Add(@"  6:28 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  6:28 Kill: 4 5 7: Zeh killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@"  6:28 Kill: 5 4 7: Assasinu Credi killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@"  6:28 Item: 2 ammo_rockets");
            linhas.Add(@"  6:32 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  6:32 Item: 4 weapon_shotgun");
            linhas.Add(@"  6:34 Kill: 2 3 7: Dono da Bola killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@"  6:34 Item: 5 item_armor_body");
            linhas.Add(@"  6:34 Item: 2 item_quad");
            linhas.Add(@"  6:35 Item: 2 item_armor_shard");
            linhas.Add(@"  6:35 Kill: 2 4 6: Dono da Bola killed Zeh by MOD_ROCKET");
            linhas.Add(@"  6:35 Kill: 2 2 7: Dono da Bola killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  6:38 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  6:44 Item: 4 weapon_railgun");
            linhas.Add(@"  6:46 Kill: 5 3 7: Assasinu Credi killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@"  6:47 Item: 5 weapon_railgun");
            linhas.Add(@"  6:50 Kill: 1022 2 22: <world> killed Dono da Bola by MOD_TRIGGER_HURT");
            linhas.Add(@"  6:51 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  7:02 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  7:02 Kill: 1022 3 22: <world> killed Isgalamido by MOD_TRIGGER_HURT");
            linhas.Add(@"  7:02 Item: 5 weapon_shotgun");
            linhas.Add(@"  7:03 Item: 5 ammo_shells");
            linhas.Add(@"  7:03 Kill: 4 2 6: Zeh killed Dono da Bola by MOD_ROCKET");
            linhas.Add(@"  7:08 Item: 4 item_health_large");
            linhas.Add(@"  7:09 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  7:11 Item: 4 item_armor_combat");
            linhas.Add(@"  7:12 Kill: 2 5 7: Dono da Bola killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@"  7:12 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  7:12 Kill: 4 2 7: Zeh killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  7:13 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  7:14 Item: 4 item_armor_shard");
            linhas.Add(@"  7:18 Item: 5 ammo_rockets");
            linhas.Add(@"  7:21 Item: 3 ammo_bullets");
            linhas.Add(@"  7:21 Kill: 2 2 7: Dono da Bola killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  7:23 Kill: 4 3 7: Zeh killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@"  7:23 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  7:25 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  7:29 Item: 3 ammo_rockets");
            linhas.Add(@"  7:29 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  7:29 Kill: 2 5 7: Dono da Bola killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@"  7:30 Kill: 1022 2 19: <world> killed Dono da Bola by MOD_FALLING");
            linhas.Add(@"  7:30 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  7:36 Item: 5 item_armor_body");
            linhas.Add(@"  7:37 Kill: 1022 4 19: <world> killed Zeh by MOD_FALLING");
            linhas.Add(@"  7:37 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  7:44 Kill: 5 2 6: Assasinu Credi killed Dono da Bola by MOD_ROCKET");
            linhas.Add(@"  7:46 Item: 4 item_quad");
            linhas.Add(@"  7:47 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  7:50 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  7:53 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  7:53 Kill: 5 4 7: Assasinu Credi killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@"  7:53 Kill: 3 5 6: Isgalamido killed Assasinu Credi by MOD_ROCKET");
            linhas.Add(@"  7:54 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  7:59 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  8:00 Item: 5 ammo_rockets");
            linhas.Add(@"  8:04 Kill: 2 5 7: Dono da Bola killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@"  8:05 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  8:07 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  8:08 Kill: 2 5 7: Dono da Bola killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@"  8:10 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  8:11 Kill: 4 3 7: Zeh killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@"  8:11 Kill: 2 2 7: Dono da Bola killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  8:12 Item: 4 item_armor_shard");
            linhas.Add(@"  8:22 Item: 3 weapon_railgun");
            linhas.Add(@"  8:24 Kill: 3 4 10: Isgalamido killed Zeh by MOD_RAILGUN");
            linhas.Add(@"  8:25 Item: 3 weapon_railgun");
            linhas.Add(@"  8:28 Item: 5 item_health_large");
            linhas.Add(@"  8:28 Item: 4 weapon_shotgun");
            linhas.Add(@"  8:30 Item: 2 item_health_large");
            linhas.Add(@"  8:31 Kill: 3 5 10: Isgalamido killed Assasinu Credi by MOD_RAILGUN");
            linhas.Add(@"  8:35 Kill: 4 3 1: Zeh killed Isgalamido by MOD_SHOTGUN");
            linhas.Add(@"  8:37 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  8:39 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  8:39 Kill: 2 4 7: Dono da Bola killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@"  8:40 Item: 2 weapon_shotgun");
            linhas.Add(@"  8:45 Item: 3 item_armor_shard");
            linhas.Add(@"  8:45 Item: 3 item_armor_shard");
            linhas.Add(@"  8:45 Item: 3 item_armor_combat");
            linhas.Add(@"  8:46 Kill: 4 5 7: Zeh killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@"  8:47 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  8:49 Kill: 3 2 7: Isgalamido killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  8:49 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  8:56 Item: 3 ammo_rockets");
            linhas.Add(@"  9:01 Kill: 4 5 6: Zeh killed Assasinu Credi by MOD_ROCKET");
            linhas.Add(@"  9:04 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  9:05 Kill: 4 5 7: Zeh killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@"  9:09 Item: 4 ammo_rockets");
            linhas.Add(@"  9:09 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  9:09 Item: 3 item_armor_shard");
            linhas.Add(@"  9:09 Item: 3 item_armor_shard");
            linhas.Add(@"  9:10 Kill: 3 4 7: Isgalamido killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@"  9:10 Kill: 1022 2 19: <world> killed Dono da Bola by MOD_FALLING");
            linhas.Add(@"  9:11 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  9:19 Item: 4 weapon_shotgun");
            linhas.Add(@"  9:20 Kill: 3 2 3: Isgalamido killed Dono da Bola by MOD_MACHINEGUN");
            linhas.Add(@"  9:20 Kill: 5 4 7: Assasinu Credi killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@"  9:21 Item: 5 weapon_shotgun");
            linhas.Add(@"  9:22 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  9:23 Item: 5 item_armor_shard");
            linhas.Add(@"  9:24 Item: 5 item_armor_combat");
            linhas.Add(@"  9:24 Item: 3 weapon_shotgun");
            linhas.Add(@"  9:25 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  9:25 Kill: 3 2 1: Isgalamido killed Dono da Bola by MOD_SHOTGUN");
            linhas.Add(@"  9:25 Kill: 5 3 7: Assasinu Credi killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@"  9:25 Kill: 2 5 6: Dono da Bola killed Assasinu Credi by MOD_ROCKET");
            linhas.Add(@"  9:27 Item: 4 weapon_rocketlauncher");
            linhas.Add(@"  9:27 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  9:29 Item: 4 item_armor_shard");
            linhas.Add(@"  9:31 Item: 5 item_armor_body");
            linhas.Add(@"  9:32 Item: 2 weapon_rocketlauncher");
            linhas.Add(@"  9:36 Kill: 2 4 6: Dono da Bola killed Zeh by MOD_ROCKET");
            linhas.Add(@"  9:36 Item: 2 item_armor_shard");
            linhas.Add(@"  9:44 Item: 4 ammo_shells");
            linhas.Add(@"  9:46 Item: 3 weapon_rocketlauncher");
            linhas.Add(@"  9:47 Kill: 5 2 7: Assasinu Credi killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@"  9:50 Item: 5 weapon_rocketlauncher");
            linhas.Add(@"  9:51 Item: 4 weapon_shotgun");
            linhas.Add(@"  9:52 Kill: 5 4 7: Assasinu Credi killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@"  9:53 Item: 5 weapon_shotgun");
            linhas.Add(@"  9:53 Kill: 3 5 7: Isgalamido killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@"  9:53 Item: 2 item_armor_shard");
            linhas.Add(@"  9:56 Item: 3 item_armor_shard");
            linhas.Add(@"  9:56 Item: 3 item_armor_combat");
            linhas.Add(@"  9:58 Kill: 3 2 6: Isgalamido killed Dono da Bola by MOD_ROCKET");
            linhas.Add(@"  9:58 Item: 3 weapon_rocketlauncher");
            linhas.Add(@" 10:07 Item: 5 weapon_rocketlauncher");
            linhas.Add(@" 10:09 Kill: 5 3 7: Assasinu Credi killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@" 10:12 Item: 4 item_quad");
            linhas.Add(@" 10:17 Item: 5 item_armor_shard");
            linhas.Add(@" 10:19 Kill: 3 4 7: Isgalamido killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@" 10:19 Item: 5 weapon_shotgun");
            linhas.Add(@" 10:20 Item: 3 item_quad");
            linhas.Add(@" 10:21 Item: 5 item_armor_shard");
            linhas.Add(@" 10:22 Item: 4 weapon_rocketlauncher");
            linhas.Add(@" 10:24 Kill: 3 5 7: Isgalamido killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@" 10:26 Item: 3 weapon_rocketlauncher");
            linhas.Add(@" 10:27 Item: 4 item_armor_shard");
            linhas.Add(@" 10:28 Item: 4 item_armor_combat");
            linhas.Add(@" 10:29 Kill: 3 4 3: Isgalamido killed Zeh by MOD_MACHINEGUN");
            linhas.Add(@" 10:30 Item: 5 weapon_rocketlauncher");
            linhas.Add(@" 10:35 Item: 4 weapon_rocketlauncher");
            linhas.Add(@" 10:37 Item: 3 item_health_mega");
            linhas.Add(@" 10:41 Kill: 1022 5 22: <world> killed Assasinu Credi by MOD_TRIGGER_HURT");
            linhas.Add(@" 10:42 Item: 3 item_armor_shard");
            linhas.Add(@" 10:42 Kill: 2 4 10: Dono da Bola killed Zeh by MOD_RAILGUN");
            linhas.Add(@" 10:42 Item: 3 item_armor_shard");
            linhas.Add(@" 10:43 Item: 5 ammo_rockets");
            linhas.Add(@" 10:51 Item: 5 item_armor_shard");
            linhas.Add(@" 10:55 Item: 4 item_health_large");
            linhas.Add(@" 10:57 Kill: 3 4 7: Isgalamido killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@" 11:00 Item: 4 weapon_rocketlauncher");
            linhas.Add(@" 11:00 Kill: 5 2 7: Assasinu Credi killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@" 11:00 Item: 5 item_quad");
            linhas.Add(@" 11:04 Item: 3 weapon_rocketlauncher");
            linhas.Add(@" 11:07 Item: 2 item_armor_combat");
            linhas.Add(@" 11:16 Kill: 1022 3 19: <world> killed Isgalamido by MOD_FALLING");
            linhas.Add(@" 11:17 Kill: 2 5 7: Dono da Bola killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@" 11:18 Item: 2 item_armor_body");
            linhas.Add(@" 11:25 Item: 3 weapon_rocketlauncher");
            linhas.Add(@" 11:26 Item: 2 weapon_rocketlauncher");
            linhas.Add(@" 11:26 Kill: 5 4 7: Assasinu Credi killed Zeh by MOD_ROCKET_SPLASH");
            linhas.Add(@" 11:30 Item: 4 weapon_shotgun");
            linhas.Add(@" 11:30 Kill: 5 5 7: Assasinu Credi killed Assasinu Credi by MOD_ROCKET_SPLASH");
            linhas.Add(@" 11:30 Kill: 3 2 7: Isgalamido killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@" 11:31 Item: 3 weapon_rocketlauncher");
            linhas.Add(@" 11:34 Item: 2 weapon_rocketlauncher");
            linhas.Add(@" 11:37 Item: 5 ammo_rockets");
            linhas.Add(@" 11:37 Kill: 1022 4 22: <world> killed Zeh by MOD_TRIGGER_HURT");
            linhas.Add(@" 11:38 Item: 3 item_armor_combat");
            linhas.Add(@" 11:40 Kill: 1022 5 19: <world> killed Assasinu Credi by MOD_FALLING");
            linhas.Add(@" 11:40 Item: 4 weapon_rocketlauncher");
            linhas.Add(@" 11:43 Item: 5 weapon_rocketlauncher");
            linhas.Add(@" 11:44 Item: 3 item_armor_shard");
            linhas.Add(@" 11:44 Item: 3 item_armor_shard");
            linhas.Add(@" 11:44 Item: 3 item_armor_shard");
            linhas.Add(@" 11:47 Kill: 4 2 7: Zeh killed Dono da Bola by MOD_ROCKET_SPLASH");
            linhas.Add(@" 11:47 Kill: 3 5 6: Isgalamido killed Assasinu Credi by MOD_ROCKET");
            linhas.Add(@" 11:50 Item: 5 ammo_rockets");
            linhas.Add(@" 11:57 Item: 2 item_armor_shard");
            linhas.Add(@" 11:57 Kill: 4 3 7: Zeh killed Isgalamido by MOD_ROCKET_SPLASH");
            linhas.Add(@" 11:57 Exit: Fraglimit hit.");
            linhas.Add(@" 11:57 score: 20  ping: 4  client: 4 Zeh");
            linhas.Add(@" 11:57 score: 19  ping: 3  client: 3 Isgalamido");
            linhas.Add(@" 11:57 score: 11  ping: 0  client: 5 Assasinu Credi");
            linhas.Add(@" 11:57 score: 5  ping: 9  client: 2 Dono da Bola");
            linhas.Add(@" 12:13 ShutdownGame:");
            linhas.Add(@" 12:13 ------------------------------------------------------------");
            #endregion

            //action
            List<Kill> kills = _logQuakeService.ConverterArquivoEmListaDeKill(linhas);

            //assert
            Assert.IsTrue(kills.Count == 109);

            Assert.IsTrue(kills[0].Id == 0);
            Assert.IsTrue(kills[0].IdGame == 1);
            Assert.IsTrue(kills[0].PlayerKiller == "Isgalamido");
            Assert.IsTrue(kills[0].PlayerKilled == "Mocinha");

            Assert.IsTrue(kills[1].Id == 0);
            Assert.IsTrue(kills[1].IdGame == 1);
            Assert.IsTrue(kills[1].PlayerKiller == "<world>");
            Assert.IsTrue(kills[1].PlayerKilled == "Zeh");

            Assert.IsTrue(kills[2].Id == 0);
            Assert.IsTrue(kills[2].IdGame == 1);
            Assert.IsTrue(kills[2].PlayerKiller == "<world>");
            Assert.IsTrue(kills[2].PlayerKilled == "Zeh");

            Assert.IsTrue(kills[3].Id == 0);
            Assert.IsTrue(kills[3].IdGame == 1);
            Assert.IsTrue(kills[3].PlayerKiller == "<world>");
            Assert.IsTrue(kills[3].PlayerKilled == "Dono da Bola");

            Assert.IsTrue(kills[4].Id == 0);
            Assert.IsTrue(kills[4].IdGame == 2);
            Assert.IsTrue(kills[4].PlayerKiller == "<world>");
            Assert.IsTrue(kills[4].PlayerKilled == "Isgalamido");

            Assert.IsTrue(kills[50].Id == 0);
            Assert.IsTrue(kills[50].IdGame == 2);
            Assert.IsTrue(kills[50].PlayerKiller == "Dono da Bola");
            Assert.IsTrue(kills[50].PlayerKilled == "Isgalamido");

            Assert.IsTrue(kills[108].Id == 0);
            Assert.IsTrue(kills[108].IdGame == 2);
            Assert.IsTrue(kills[108].PlayerKiller == "Zeh");
            Assert.IsTrue(kills[108].PlayerKilled == "Isgalamido");

        }

        [TestMethod]
        public void AdicionarEmBDListaDeKill()
        {
            //arrange
            List<Kill> Kills = new List<Kill>();
            Kills.Add(new Kill { IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Isgalamido" });
            Kills.Add(new Kill { IdGame = 1, PlayerKiller = "Zeh", PlayerKilled = "Isgalamido" });
            Kills.Add(new Kill { IdGame = 1, PlayerKiller = "<world>", PlayerKilled = "Zeh" });
            Kills.Add(new Kill { IdGame = 1, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });

            Kills.Add(new Kill { IdGame = 2, PlayerKiller = "Isgalamido", PlayerKilled = "Zeh" });
            Kills.Add(new Kill { IdGame = 2, PlayerKiller = "Isgalamido", PlayerKilled = "Dono da Bola" });
            Kills.Add(new Kill { IdGame = 2, PlayerKiller = "<world>", PlayerKilled = "Zeh" });

            //action
            int retornoServico = _logQuakeService.AdicionarEmBDListaDeKill(Kills);

            Dictionary<string, Game> game1 = _logQuakeService.GetById(1);
            Dictionary<string, Game> game2 = _logQuakeService.GetById(2);

            int QuantidadeRegistroKillRepository = _killRepository.Count();

            //assert
            Assert.IsTrue(retornoServico == 7);

            Assert.IsTrue(game1.Values.First().TotalKills == 4);
            Assert.IsTrue(game1.Values.First().Kills["Zeh"] == 1);
            Assert.IsTrue(game1.Values.First().Kills["Isgalamido"] == -1);
            Assert.IsTrue(game1.Values.First().Kills["Dono da Bola"] == -1);

            Assert.IsTrue(game2.Values.First().TotalKills == 3);
            Assert.IsTrue(game2.Values.First().Kills["Zeh"] == -2);
            Assert.IsTrue(game2.Values.First().Kills["Isgalamido"] == 2);
            Assert.IsTrue(game2.Values.First().Kills["Dono da Bola"] == -1);

            Assert.IsTrue(QuantidadeRegistroKillRepository == 7);
        }
        #endregion
    }
}
