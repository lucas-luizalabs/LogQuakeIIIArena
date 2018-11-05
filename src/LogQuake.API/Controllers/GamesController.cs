using System;
using System.Collections.Generic;
using System.IO;
using LogQuake.CrossCutting;
using LogQuake.Domain.Dto;
using LogQuake.Domain.Entities;
using LogQuake.Infra.CrossCuting;
using LogQuake.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LogQuake.API.Controllers
{
    /// <summary>
    /// API controladora do jogo
    /// </summary>
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
        #region Atributos
        private readonly ILogQuakeService _logQuakeService;
        private readonly ILogger _logger;
        #endregion

        #region Construtor
        /// <summary>
        /// Constrututor da classe controladora de Games
        /// </summary>
        /// <param name="logQuakeService">objeto de Serviço do Jogo</param>
        /// /// <param name="logger">objeto para controle de log</param>
        public GamesController(ILogQuakeService logQuakeService, ILogger<GamesController> logger)
        {
            _logQuakeService = logQuakeService;
            _logger = logger;
        }
        #endregion

        #region Métodos
        // GET: api/<controller>
        /// <summary>
        /// Consultar log de todos os jogos, respeitando paginação
        /// </summary>
        /// <param name="pageRequest">controle de paginação</param>
        /// <example>teste</example>
        /// <remarks>
        /// Busca partidas registradas em Banco de Dados de forma paginada, para isso deve informar os parâmetros de paginação.
        /// </remarks>        
        /// <response code="200">Lista de partidas encontradas.</response>
        /// <response code="404">Nenhuma partida encontrada.</response>
        [HttpGet]
        [ProducesResponseType(typeof(Game), 200)]
        [ProducesResponseType(typeof(List<Notification>), 400)]
        [ProducesResponseType(typeof(List<Notification>), 404)]
        public IActionResult Get([FromQuery]DtoGameRequest pageRequest)

        {
            DtoGameResponse response = new DtoGameResponse();

            if (pageRequest == null)
                pageRequest = new DtoGameRequest { PageNumber = 1, PageSize = 5 };

            try
            {
                response = _logQuakeService.GetAll(pageRequest);
                if (response.Game == null || response.Game.Count == 0)
                {
                    _logger.LogWarning(LoggingEvents.Information, "Nada encontrado para a página {0} tamanho {1}", pageRequest.PageNumber, pageRequest.PageSize);
                    return NotFound(response);
                }
                else
                {
                    _logger.LogInformation(LoggingEvents.Information, "Busca realizada com sucesso para a página {0} com tamanho {1}", pageRequest.PageNumber, pageRequest.PageSize);
                    return Ok(response.Game);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(LoggingEvents.Critial, ex, "Falha ao realizar a busca pela página {0} com tamnho {1}", pageRequest.PageNumber, pageRequest.PageSize);
                response.AddNotification(Notifications.ErroInesperado, ex);
                return BadRequest(response);
            }
        }

        // GET api/<controller>/5
        /// <summary>
        /// Consultar log dos jogos por IdGame
        /// </summary>
        /// <param name="idGame">Código de identificação do jogo</param>
        /// <remarks>
        /// Busca partida registrada em Banco de Dados por IdGame.
        /// </remarks>        
        /// <response code="200">Partida encontrada <paramref name="idGame"/>.</response>
        /// <response code="404">Nenhuma partida encontrada.</response>
        [HttpGet("{idGame}")]
        [ProducesResponseType(typeof(Game), 200)]
        [ProducesResponseType(typeof(List<Notification>), 400)]
        [ProducesResponseType(typeof(List<Notification>), 404)]
        public IActionResult Get(int idGame)
        {
            DtoGameResponse response = new DtoGameResponse();

            try
            {
                response = _logQuakeService.GetById(idGame);
                if (response.Game == null || response.Game.Count == 0)
                {
                    _logger.LogWarning(LoggingEvents.Warning, "Não encontrado o item {ID}", idGame);
                    return NotFound(response);
                }
                else
                {
                    _logger.LogInformation(LoggingEvents.Information, "Busca realizada com sucesso para o item {ID}", idGame);
                    return Ok(response.Game);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical(LoggingEvents.Critial, ex, "Getting item {ID}", idGame);
                response.AddNotification(Notifications.ErroInesperado, ex);
                return BadRequest(response);
            }
        }

        // POST api/<controller>
        /// <summary>
        /// Método para efetuar o Upload do arquivo de Log do jogo Quake III Arena.
        /// </summary>
        /// <remarks>
        /// Após a API receber o arquivo de log ele será processado e amazenado em Banco de Dados para posterior consulta de cada partida do jogos.
        /// </remarks>        
        /// <param name="file">arquivo a ser processado</param>
        /// <response code="200">Upload realizado com sucesso.</response>
        /// <response code="400">Bad Request.</response>
        [HttpPost("Upload")]
        [ProducesResponseType(typeof(DtoUploadResponse), 200)]
        [ProducesResponseType(typeof(List<Notification>), 400)]
        public IActionResult Upload(IFormFile file)
        {
            string path = "";
            DtoUploadResponse response = new DtoUploadResponse();

            if (file == null || file.Length == 0)
            {
                _logger.LogError(LoggingEvents.Critial, "Arquivo não selecionado.");
                response.AddNotification(Notifications.ErroInesperado, "Arquivo não selecionado.");
                return BadRequest(response);
            }

            try
            {
                path = Directory.GetCurrentDirectory() + "\\wwwroot\\Log";
                response = _logQuakeService.UploadFile(path, file.FileName, file.OpenReadStream());

                if (response.Notifications != null)
                {
                    _logger.LogWarning(LoggingEvents.Information, "Falha ao realizar o Upload do arquivo {0}", file.FileName);
                    return BadRequest(response);
                }
                else
                {
                    _logger.LogInformation(LoggingEvents.Information, "Upload realizado com sucesso do arquivo {0}", file.FileName);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(LoggingEvents.Critial, ex, "Upload do log {0}", file.FileName);
                response.AddNotification(Notifications.ErroInesperado, ex);
                return BadRequest(response);
            }
        }
        #endregion
    }
}
