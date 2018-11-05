using FluentValidation;
using LogQuake.CrossCutting;
using LogQuake.Domain.Dto;
using LogQuake.Domain.Entities;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.CrossCuting;
using LogQuake.Service.Validators;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LogQuake.Service.Services
{
    public class LogQuakeService :  ILogQuakeService
    {
        #region Atributos
        private readonly IKillRepository _killRepository;
        private readonly ILogger _logger;
        #endregion

        #region Construtor
        public LogQuakeService(IKillRepository killRepository, ILogger<LogQuakeService> logger)
        {
            _killRepository = killRepository;
            _logger = logger;
        }
        #endregion


        /// <summary>
        /// Método responsável por ler o arquivo de log do jogo Quake 3 Arena e criar uma lista de string, contendo as linhas do log.
        /// </summary>
        /// <param name="filename">arquivo a ser lido</param>
        /// <returns>
        /// Retorna uma lista de string, contendo as linhas do arquivo de log lido.
        /// </returns>
        public List<string> ReadLogFile(string fileName)
        {
            List<string> linhas;

            if (File.Exists(fileName))
                linhas = File.ReadAllLines(fileName).ToList();
            else
                linhas = new List<string>();

            return linhas;
        }

        /// <summary>
        /// Método responsável receber o Upload do arquivo de log.
        /// </summary>
        /// <param name="folder">local de destino do arquivo de log</param>
        /// <param name="filename">arquivo a ser lido</param>
        /// <param name="stream">arquivo propriamente dito no formato Stream</param>
        /// <returns>
        /// Retorna um objeto contendo nome do arquivo, quantidade de registro inseridos, se ouve sucesso Sim ou Não.
        /// </returns>
        public DtoUploadResponse UploadFile(string folder, string fileName, Stream file)
        {
            DtoUploadResponse response = new DtoUploadResponse();
            string path = folder;
            int RegistrosInseridos = 0;

            //caso não exista o destino, deve-se criar as pastas
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path += "\\" + fileName;
            }
            catch (Exception ex)
            {
                response.AddNotification(Notifications.ErroInesperado, string.Format("Falha ao criar o diretório {0}", path), ex);
                return response;
            }

            //criando o arquivo de log em seu destino
            try
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                response.AddNotification(Notifications.ErroInesperado, string.Format("Falha ao copiar o arquivo {0} para o diretório {1}", fileName, path), ex);
                return response;
            }

            //Processando o arquivo de log:
            //1 - Efetua a leitura do arquivo de log
            //2 - Converte arquivo de log em lista de objetos Kill
            //3 - Inserindo no Banco de Dados a lista de Kill
            //4 - Retorna o objeto para o método que solicitou o Upload
            try
            {
                List<Kill> Kills;
                List<string> linhas = ReadLogFile(path);

                if (linhas.Count > 0)
                {
                    Kills = ConvertLogFileInListKill(linhas);

                    RegistrosInseridos = AddKillListInDB(Kills);
                }

                response.FileName = fileName;
                response.Length = file.Length;
                response.RegistrosInseridos = RegistrosInseridos;

                return response;
            }
            catch (Exception ex)
            {
                response.AddNotification(Notifications.ErroInesperado, "Falha ao processar o arquivo " + fileName, ex);
                return response;
            }
        }

        /// <summary>
        /// Método responsável por converter as linhas do arquivo de log contidas em uma lista de strings, em uma lista de Kills.
        /// </summary>
        /// <param name="linhas">lista de string, contendo as linhas do arquivo de log</param>
        /// <returns>
        /// Retorna uma lista de Kills.
        /// </returns>
        public List<Kill> ConvertLogFileInListKill(List<string> linhas)
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
        public int AddKillListInDB(List<Kill> Kills)
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
        public DtoGameResponse GetById(int Id)
        {
            Dictionary<string, Game> games = new Dictionary<string, Game>();
            DtoGameResponse response = new DtoGameResponse();
            List<Kill> listaKill;

            try
            {
                _logger.LogError(LoggingEvents.Information, "Buscando o registro da partida {0}", Id);
                listaKill = _killRepository.GetByIdList(Id).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.Critial, "Falha ao acessar o banco de dados das partidas.");
                response.AddNotification(Notifications.ErroInesperado, "Falha ao acessar o banco de dados das partidas.", ex);
                return response;
            }

            if (listaKill.Count == 0)
            {
                _logger.LogError(LoggingEvents.Error, "Getting item {ID}", Id);
                response.AddNotification(Notifications.ItemNaoEncontrado, string.Format("Nenhum item encontrado ao buscar Game com o Id {0}", Id), MethodBase.GetCurrentMethod().ToString());
                return response;
            }

            try
            {
                ConvertKillListToGame(listaKill, games, Id);
            }
            catch (Exception ex)
            {
                response.AddNotification(Notifications.ErroInesperado, "Falha ao converter lista de jogos.", ex);
                return response;
            }

            response.Game = games;

            return response;
        }

        /// <summary>
        /// Busca no Banco de Dados os dados de todos os jogos, respeitando a paginação informada.
        /// </summary>
        /// <param name="pageRequest">parâmetros de paginação para buscar no Banco de Dados</param>
        public DtoGameResponse GetAll(PagingRequest pageRequest)
        {
            List<Kill> listaKill = _killRepository.GetAll(pageRequest).ToList();
            Dictionary<string, Game> games = new Dictionary<string, Game>();
            DtoGameResponse response = new DtoGameResponse();

            if (listaKill.Count == 0)
            {
                response.AddNotification(Notifications.ItemNaoEncontrado, string.Format("Nenhum item encontrado ao buscar Game para a página {0} com tamanho {1}", pageRequest.PageNumber, pageRequest.PageSize), MethodBase.GetCurrentMethod().ToString());
                return response;
            }

            try
            {
                ConvertKillListToGame(listaKill, games, ((pageRequest.PageNumber - 1) * pageRequest.PageSize) + 1);
            }
            catch (Exception ex)
            {
                response.AddNotification(Notifications.ErroInesperado, "Falha ao converter lista de jogos.", ex);
            }
            response.Game = games;

            return response;
        }

        /// <summary>
        /// Converter lista de Kill que foi obtida do Banco de Dados e converte para o objeto de retorno da API (Game)
        /// </summary>
        /// <param name="listaKill">lista Kill de entrada</param>
        /// <param name="games">retorna uma lista preenchida com os Jogos encontrados de acordo com a listaKill</param>
        /// <param name="ContadorGame">variável de controle para indicar o número do Jogo "game_x" dentro da lista games</param>
        private static void ConvertKillListToGame(List<Kill> listaKill, Dictionary<string, Game> games, int ContadorGame)
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
                game.RegisterPlayers(listKills);

                foreach (Kill item in listaKillFiltrada)
                {
                    game.RegisterDeath(item.PlayerKiller, item.PlayerKilled);
                }

                games.Add("game_" + (ContadorGame), game);
                ContadorGame++;

                //remove os jogos da lista original até zerar a lista
                listaKill.RemoveAll(x => x.IdGame == idgame);
            } while (listaKill.Count() > 0);
        }
    }
}
