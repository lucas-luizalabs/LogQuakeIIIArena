using FluentValidation;
using LogQuake.Domain.Entities;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.CrossCuting;
using LogQuake.Infra.Data.Repositories;
using LogQuake.Service.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogQuake.Service.Services
{
    public class LogQuakeService :  ILogQuakeService
    {
        private readonly IKillRepository _killRepository;

        public LogQuakeService(IKillRepository killRepository)
        {
            _killRepository = killRepository;
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

        public List<Kill> ConverterArquivoEmListaDeKill(List<string> linhas)
        {
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

        public int AdicionarEmBDListaDeKill(List<Kill> Kills)
        {
            _killRepository.RemoveAll();

            foreach (Kill item in Kills)
            {
                Validate(item, Activator.CreateInstance<KillValidator>());
                _killRepository.Add(item);
            }
            _killRepository.SaveChanges();

            return Kills.Count;
        }

        public Kill Add<V>(Kill obj) where V : AbstractValidator<Kill>
        {
            Validate(obj, Activator.CreateInstance<V>());

            _killRepository.Add(obj);
            return obj;
        }

        private void Validate(Kill obj, AbstractValidator<Kill> validator)
        {
            if (obj == null)
                throw new Exception("Registros não detectados!");

            validator.ValidateAndThrow(obj);
        }

        /// <summary>
        /// Busca no Banco de Dados todos os dados de um determinado Jogo
        /// </summary>
        /// <param name="Id">Identificador do Jogo</param>
        public Dictionary<string, Game> GetById(int Id)
        {
            List<Kill> listaKill = _killRepository.GetByIdList(Id).ToList();

            Dictionary<string, Game> games = new Dictionary<string, Game>();

            if (listaKill.Count == 0)
                return games;

            ConverteListKillParaGame(listaKill, games, Id);

            return games;
        }

        /// <summary>
        /// Busca no Banco de Dados todos os Kill, respeitando a paginação
        /// </summary>
        /// <param name="pageRequest">parâmetros de paginação para buscar no Banco de Dados</param>
        public Dictionary<string, Game> GetAll(PageRequestBase pageRequest)
        {
            List<Kill> listaKill = _killRepository.GetAll(pageRequest).ToList();

            Dictionary<string, Game> games = new Dictionary<string, Game>();

            if (listaKill.Count == 0)
                return games;

            ConverteListKillParaGame(listaKill, games, ((pageRequest.PageNumber - 1) * pageRequest.PageSize) + 1);

            return games;
        }

        /// <summary>
        /// Converter lista de Kill que foi obtida do Banco de Dados e converte para o objeto de retorno da API (Game)
        /// </summary>
        /// <param name="listaKill">lista de entrada</param>
        /// <param name="games">retorno preenchido com os Jogos encontrados no Banco de Dados</param>
        /// <param name="ContadorGame">variável de controle para indicar o número do Jogo "game_x"</param>
        private static void ConverteListKillParaGame(List<Kill> listaKill, Dictionary<string, Game> games, int ContadorGame)
        {
            Game game;
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

                game = new Game
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
