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

        public List<Kill> CarregarLogParaDB(string fileName)
        {
            List<string> linhas;
            //string fileName = @"c:\LogQuake\games.txt";

            try
            {
                linhas = File.ReadAllLines(fileName).ToList();
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
                throw new Exception("Erro ao carregar arquivo de log " + fileName, ex);
            }
        }

        public List<_Game> GetAll(PageRequestBase pageRequest)
        {
            List<Kill> lista = _killRepository.GetAll(pageRequest).ToList();

            List<_Game> games = new List<_Game>();

            if (lista.Count == 0)
                return games;

            _Game game;
            int idgame = 0;
            List<Kill> lista2;
            do
            {

                try {
                    idgame = lista[0].IdGame;
                    lista2 = lista.Where(x => x.IdGame == idgame).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                game = new _Game
                {
                    TotalKills = lista2.Count()
                };

                var listKillers = lista2.Select(x => x.PlayerKiller).ToList();
                var listKilleds = lista2.Select(x => x.PlayerKilled).ToList();
                var listKills = listKillers.Union(listKilleds).ToList();
                listKills.Remove("<world>");
                listKills.Remove(null);
                game.Players = listKills.ToArray();

                foreach (var item in lista2)
                {
                    string Assassino = item.PlayerKiller;
                    string Assassinado = item.PlayerKilled;
                    if (Assassino == "<world>")
                    {
                        //Assasinado deve perder -1 kill
                        if (!string.IsNullOrEmpty(Assassinado))
                        {
                            if (!string.IsNullOrEmpty(Assassinado) && game.Kills.ContainsKey(Assassinado))
                            {
                                game.Kills[Assassinado] -= 1;
                            }
                            else
                            {
                                game.Kills.Add(Assassinado, -1);
                            }
                        }
                    }
                    else
                    {
                        //Assasino deve ganhar +1 kill
                        if (!string.IsNullOrEmpty(Assassino))
                        {
                            if (game.Kills.ContainsKey(Assassino))
                            {
                                game.Kills[Assassino] += 1;
                            }
                            else
                            {
                                game.Kills.Add(Assassino, 1);
                            }
                        }
                    }
                }
                games.Add(game);

                //remove os jogos da lista até zerar a lista
                lista.RemoveAll(x => x.IdGame == idgame);
            } while (lista.Count() > 0);


            return games;
        }

    }
}
