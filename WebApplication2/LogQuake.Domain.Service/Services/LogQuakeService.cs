using LogQuake.Domain.Entities;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.CrossCuting;
using LogQuake.Infra.Data.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogQuake.Service.Services
{
    public class LogQuakeService : ILogQuakeService
    {
        //private KillRepository repository = new KillRepository();
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IKillRepository _killRepository;
        //private readonly IUploadRepository _uploadRepository;
        //private bool _disposed = false;

        //public LogQuakeService(IUnitOfWork unitOfWork, IJogoRepository jogoRepository, IUploadRepository uploadRepository)
        //public LogQuakeService()
        //{
        //}

        public LogQuakeService(IKillRepository killRepository)
        {
            //_unitOfWork = unitOfWork;
            _killRepository = killRepository;
            //_uploadRepository = uploadRepository;
        }

        public List<Game> CarregarLog(string fileName)
        {
            List<string> linhas;
            //string fileName = @"c:\LogQuake\games.txt";

            try
            {
                linhas = File.ReadAllLines(fileName).ToList();
                List<Game> games = new List<Game>();

                Game game = null;
                foreach (string linha in linhas)
                {
                    if (linha.Contains("InitGame"))
                    {
                        game = new Game();
                        game.Player = new List<Player>();
                        game.Kills = new Kills();
                        game.Id = games.Count() + 1;
                        games.Add(game);
                    }

                    //verificando se houve algum assassinato, caso encontre deve adicionar ao Game
                    int posKilled = linha.IndexOf(" killed ");

                    if (posKilled > 0)
                    {
                        int pos1 = linha.Substring(0, posKilled).LastIndexOf(": ");
                        string Assassino = linha.Substring(pos1 + 2, posKilled - (pos1 + 2)).Trim();
                        string Assassinado = linha.Substring(posKilled + 8, linha.Substring(posKilled + 8).IndexOf(" by "));

                        //Incrementa o contador de mortos no jogo independente se é um Player ou '<world>'
                        game.TotalKills++;

                        bool AssassinoEstaNoJogo = game.Player.Any(x => x.PlayerName == Assassino);
                        bool AssassinadoEstaNoJogo = game.Player.Any(x => x.PlayerName == Assassinado);

                        if (!AssassinoEstaNoJogo && Assassino != "<world>")
                        {
                            //adicionar Assasino na lista de Players
                            game.Player.Add(new Player() { PlayerName = Assassino });
                        }
                        if (!AssassinadoEstaNoJogo)
                        {
                            //adicionar Assasinado na lista de Players
                            game.Player.Add(new Player() { PlayerName = Assassinado });
                        }

                        if (Assassino == "<world>")
                        {
                            //Assasinado deve perder -1 kill
                            if (game.Kills.values.ContainsKey(Assassinado))
                            {
                                game.Kills.values[Assassinado] -= 1;
                            }
                            else
                            {
                                game.Kills.values.Add(Assassinado, -1);
                            }
                        }
                        else
                        {
                            //Assasino deve ganhar +1 kill
                            if (game.Kills.values.ContainsKey(Assassino))
                            {
                                game.Kills.values[Assassino] += 1;
                            }
                            else
                            {
                                game.Kills.values.Add(Assassino, 1);
                            }
                        }

                    }
                }

                return games;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar arquivo de log " + fileName, ex);
            }
        }


        public List<string> LerArquivoDeLog(string fileName)
        {
            List<string> linhas;


            if (File.Exists(fileName))
                linhas = File.ReadAllLines(fileName).ToList();
            else
                linhas = new List<string>();

            return linhas;
        }

        public List<Kill> CarregarLogParaDB(List<string> linhas)
        {
            _killRepository.RemoveAll();

            try
            {
                List<Kill> kills = new List<Kill>();
                Kill kill;
                int IdGame = 0;

                foreach (string linha in linhas)
                {
                    if (linha.Contains("InitGame"))
                    {
                        IdGame++;
                    }

                    //verificando se houve algum assassinato, caso encontre deve adicionar ao Game
                    int posKilled = linha.IndexOf(" killed ");

                    if (posKilled > 0)
                    {
                        int pos1 = linha.Substring(0, posKilled).LastIndexOf(": ");
                        string Assassino = linha.Substring(pos1 + 2, posKilled - (pos1 + 2)).Trim();
                        string Assassinado = linha.Substring(posKilled + 8, linha.Substring(posKilled + 8).IndexOf(" by "));

                        kill = new Kill();

                        kill.IdGame = IdGame;
                        kill.PlayerKilled = Assassinado;
                        kill.PlayerKiller = Assassino;

                        kills.Add(kill);
                    }
                }

                return kills;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao carregar arquivo de log ", ex);
            }
        }

        public Dictionary<string, _Game> GetById(int Id)
        {
            List<Kill> listaKill = _killRepository.GetByIdList(Id).ToList();

            Dictionary<string, _Game> games = new Dictionary<string, _Game>();

            if (listaKill.Count == 0)
                return games;

            ProcessaListaKill(listaKill, games, Id);

            return games;
        }

        public Dictionary<string, _Game> GetAll(PageRequestBase pageRequest)
        {
            List<Kill> listaKill = _killRepository.GetAll(pageRequest).ToList();

            Dictionary<string, _Game> games = new Dictionary<string, _Game>();

            if (listaKill.Count == 0)
                return games;

            ProcessaListaKill(listaKill, games, ((pageRequest.PageNumber - 1) * pageRequest.PageSize) + 1);

            return games;
        }

        private static void ProcessaListaKill(List<Kill> listaKill, Dictionary<string, _Game> games, int ContadorGame)
        {
            _Game game;
            int idgame = 0;
            List<Kill> listaKillFiltrada;
            do
            {

                try
                {
                    idgame = listaKill[0].IdGame;
                    listaKillFiltrada = listaKill.Where(x => x.IdGame == idgame).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                game = new _Game
                {
                    TotalKills = listaKillFiltrada.Count()
                };

                var listKillers = listaKillFiltrada.Select(x => x.PlayerKiller).ToList();
                var listKilleds = listaKillFiltrada.Select(x => x.PlayerKilled).ToList();
                var listKills = listKillers.Union(listKilleds).ToList();
                listKills.Remove("<world>");
                listKills.Remove(null);
                game.Players = listKills.ToArray();

                foreach (Kill item in listaKillFiltrada)
                {
                    string Assassino = item.PlayerKiller;
                    string Assassinado = item.PlayerKilled;

                    //Assasino deve ganhar +1 kill
                    if (!string.IsNullOrEmpty(Assassino) && Assassino != "<world>")
                    {
                        if (game.Kills.ContainsKey(Assassino))
                        {
                            if (game.Kills[Assassino] + 1 == 0)
                                game.Kills.Remove(Assassino);
                            else
                                game.Kills[Assassino] += 1;
                        }
                        else
                        {
                            game.Kills.Add(Assassino, 1);
                        }
                    }
                    if (!string.IsNullOrEmpty(Assassinado))
                    {
                        if (!string.IsNullOrEmpty(Assassinado) && game.Kills.ContainsKey(Assassinado))
                        {
                            if (game.Kills[Assassinado] - 1 == 0)
                                game.Kills.Remove(Assassinado);
                            else
                                game.Kills[Assassinado] -= 1;
                        }
                        else
                        {
                            game.Kills.Add(Assassinado, -1);
                        }
                    }
                }
                games.Add("game_" + (ContadorGame), game);
                ContadorGame++;

                //remove os jogos da lista até zerar a lista
                listaKill.RemoveAll(x => x.IdGame == idgame);
            } while (listaKill.Count() > 0);
        }

    }
}
