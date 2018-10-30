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
        #region Atributos
        private readonly IKillRepository _killRepository;
        #endregion

        #region Construtor
        public LogQuakeService(IKillRepository killRepository)
        {
            _killRepository = killRepository;
        }
        #endregion


        /// <summary>
        /// Método responsável por ler o arquivo de log do jogo Quake 3 Arena e criar uma lista de string, contendo as linhas do log.
        /// </summary>
        /// <param name="filename">arquivo a ser lido</param>
        /// <returns>
        /// Retorna uma lista de string, contendo as linhas do arquivo de log lido.
        /// </returns>
        public List<string> LerArquivoDeLog(string fileName)
        {
            List<string> linhas;

            if (File.Exists(fileName))
                linhas = File.ReadAllLines(fileName).ToList();
            else
                linhas = new List<string>();

            return linhas;
        }

        /// <summary>
        /// Método responsável por converter as linhas do arquivo de log contidas em uma lista de strings, em uma lista de Kills.
        /// </summary>
        /// <param name="linhas">lista de string, contendo as linhas do arquivo de log</param>
        /// <returns>
        /// Retorna uma lista de Kills.
        /// </returns>
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
                        string killer = linha.Substring(pos1 + 2, posKilled - (pos1 + 2)).Trim();
                        string killed = linha.Substring(posKilled + 8, linha.Substring(posKilled + 8).IndexOf(" by "));

                        kill = new Kill();

                        kill.IdGame = IdGame;
                        kill.PlayerKiller = killer;
                        kill.PlayerKilled = killed;

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

        /// <summary>
        /// Adicionar no Banco de Dados uma lista Kills.
        /// </summary>
        /// <param name="Kills">lista de kills</param>
        /// <returns>
        /// Retorna a quantidade de registros inseridos no Banco de Dados.
        /// </returns>
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

        private void Validate(Kill obj, AbstractValidator<Kill> validator)
        {
            if (obj == null)
                throw new Exception("Registros não detectados!");

            validator.ValidateAndThrow(obj);
        }

        /// <summary>
        /// Busca no Banco de Dados os dados de um determinado Jogo.
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
        /// Busca no Banco de Dados os dados de todos os jogos, respeitando a paginação informada.
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
        /// <param name="listaKill">lista Kill de entrada</param>
        /// <param name="games">retorna uma lista preenchida com os Jogos encontrados de acordo com a listaKill</param>
        /// <param name="ContadorGame">variável de controle para indicar o número do Jogo "game_x" dentro da lista games</param>
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

                game = new Game();

                //criando uma lista unificada com todos os Players 
                var listKillers = listaKillFiltrada.Select(x => x.PlayerKiller).ToList();
                var listKilleds = listaKillFiltrada.Select(x => x.PlayerKilled).ToList();
                var listKills = listKillers.Union(listKilleds).ToList();

                //adiciona os players ao retorno
                game.RegistraPlayers(listKills);

                foreach (Kill item in listaKillFiltrada)
                {
                    game.RegistraMorte(item.PlayerKiller, item.PlayerKilled);
                }

                games.Add("game_" + (ContadorGame), game);
                ContadorGame++;

                //remove os jogos da lista original até zerar a lista
                listaKill.RemoveAll(x => x.IdGame == idgame);
            } while (listaKill.Count() > 0);
        }
    }
}
