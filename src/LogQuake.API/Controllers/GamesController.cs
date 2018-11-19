using System;
using System.Collections.Generic;
using System.IO;
using LogQuake.CrossCutting;
using LogQuake.CrossCutting.Cache;
using LogQuake.Domain.Dto;
using LogQuake.Domain.Entities;
using LogQuake.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LogQuake.API.Controllers
{
    /// <summary>
    /// API controladora do jogo.
    /// </summary>
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
        #region Atributos
        private readonly ILogQuakeService _logQuakeService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        #endregion

        #region Construtor
        /// <summary>
        /// Constrututor da classe controladora de Games
        /// </summary>
        /// <param name="logQuakeService">objeto de Serviço do Jogo</param>
        /// <param name="logger">objeto para controle de log</param>
        /// <param name="cache">representa um banco local in-memory para controle de cache de dados</param>
        /// <param name="configuration">carrega arquivo appsettings.json</param>
        public GamesController(ILogQuakeService logQuakeService, ILogger<GamesController> logger, IMemoryCache cache, IConfiguration configuration)
        {
            _logQuakeService = logQuakeService;
            _logger = logger;
            _cache = cache;
            _configuration = configuration;
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
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Nenhuma partida encontrada.</response>
        [HttpGet("{idGame}")]
        [ProducesResponseType(typeof(Game), 200)]
        [ProducesResponseType(typeof(List<Notification>), 400)]
        [ProducesResponseType(typeof(List<Notification>), 401)]
        [ProducesResponseType(typeof(List<Notification>), 404)]
        [Authorize]
        public IActionResult Get(int idGame)
        {
            ////conferindo se o scope é LogQuake
            //bool userHasRightScope = User.HasClaim("scope", "LogQuake");
            //if (userHasRightScope == false)
            //{
            //    throw new Exception("Invalid scope");
            //}
            ////obter as Claims associadas
            //var identity = (ClaimsIdentity)User.Identity;
            //IEnumerable<Claim> claims = identity.Claims;

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

        // GET api/<controller>/CacheController/5
        /// <summary>
        /// Consultar log dos jogos por IdGame, utilizando CacheMemory na Controller
        /// </summary>
        /// <param name="idGame">Código de identificação do jogo</param>
        /// <remarks>
        /// Busca partida registrada em Banco de Dados por IdGame.
        /// </remarks>        
        /// <response code="200">Partida encontrada <paramref name="idGame"/>.</response>
        /// <response code="404">Nenhuma partida encontrada.</response>
        [HttpGet("CacheController/{idGame}")]
        [ProducesResponseType(typeof(Game), 200)]
        [ProducesResponseType(typeof(List<Notification>), 400)]
        [ProducesResponseType(typeof(List<Notification>), 404)]
        public IActionResult GetWithCacheController(int idGame)
        {
            DtoGameResponse response = new DtoGameResponse();

            try
            {
                var key = $"GamesController.GetWithCacheController{idGame.ToString()}";

                if (!_cache.TryGetValue(key, out response))
                {
                    lock (Cache.lockCache) // ensure concurrent request won't access DB at the same time
                    {
                        if (!_cache.TryGetValue(key, out response)) // double-check
                        {
                            response = _logQuakeService.GetById(idGame);
                            var minutes = _configuration.GetValue<int>("Cache:GamesController:SlidingExpiration");
                            var size = _configuration.GetValue<int>("Cache:GamesController:Size");
                            _cache.Set(key, response,
                                new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(minutes), Size = size });
                        }
                    }
                }

                if (response.Game == null || response.Game.Count == 0)
                {
                    _logger.LogWarning(LoggingEvents.Warning, "Não encontrado o item {ID}, para Cache de Controller", idGame);
                    return NotFound(response);
                }
                else
                {
                    _logger.LogInformation(LoggingEvents.Information, "Busca realizada com sucesso para o item {ID}, para Cache de Controller", idGame);
                    return Ok(response.Game);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical(LoggingEvents.Critial, ex, "GetWithCacheController item {ID}", idGame);
                response.AddNotification(Notifications.ErroInesperado, ex);
                return BadRequest(response);
            }
        }

        // GET api/<controller>/CacheService/5
        /// <summary>
        /// Consultar log dos jogos por IdGame, utilizando CacheMemory na Services
        /// </summary>
        /// <param name="idGame">Código de identificação do jogo</param>
        /// <remarks>
        /// Busca partida registrada em Banco de Dados por IdGame.
        /// </remarks>        
        /// <response code="200">Partida encontrada <paramref name="idGame"/>.</response>
        /// <response code="404">Nenhuma partida encontrada.</response>
        [HttpGet("CacheService/{idGame}")]
        [ProducesResponseType(typeof(Game), 200)]
        [ProducesResponseType(typeof(List<Notification>), 400)]
        [ProducesResponseType(typeof(List<Notification>), 404)]
        public IActionResult GetWithCacheService(int idGame)
        {
            DtoGameResponse response = new DtoGameResponse();

            try
            {
                response = _logQuakeService.GetCacheById(idGame);
                if (response.Game == null || response.Game.Count == 0)
                {
                    _logger.LogWarning(LoggingEvents.Warning, "Não encontrado o item {ID}, para Cache de Serviço", idGame);
                    return NotFound(response);
                }
                else
                {
                    _logger.LogInformation(LoggingEvents.Information, "Busca realizada com sucesso para o item {ID}, para Cache de Serviço", idGame);
                    return Ok(response.Game);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical(LoggingEvents.Critial, ex, "GetWithCacheService item {ID}", idGame);
                response.AddNotification(Notifications.ErroInesperado, ex);
                return BadRequest(response);
            }
        }

        // GET api/<controller>/CacheRepository/5
        /// <summary>
        /// Consultar log dos jogos por IdGame, utilizando CacheMemory na camada de Repositório
        /// </summary>
        /// <param name="idGame">Código de identificação do jogo</param>
        /// <remarks>
        /// Busca partida registrada em Banco de Dados por IdGame.
        /// </remarks>        
        /// <response code="200">Partida encontrada <paramref name="idGame"/>.</response>
        /// <response code="404">Nenhuma partida encontrada.</response>
        [HttpGet("CacheRepository/{idGame}")]
        [ProducesResponseType(typeof(Game), 200)]
        [ProducesResponseType(typeof(List<Notification>), 400)]
        [ProducesResponseType(typeof(List<Notification>), 404)]
        public IActionResult GetWithCacheRepository(int idGame)
        {
            DtoGameResponse response = new DtoGameResponse();

            try
            {
                response = _logQuakeService.GetCacheRepositoryById(idGame);
                if (response.Game == null || response.Game.Count == 0)
                {
                    _logger.LogWarning(LoggingEvents.Warning, "Não encontrado o item {ID}, para Cache de Repositório", idGame);
                    return NotFound(response);
                }
                else
                {
                    _logger.LogInformation(LoggingEvents.Information, "Busca realizada com sucesso para o item {ID}, para Cache de Repositório", idGame);
                    return Ok(response.Game);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical(LoggingEvents.Critial, ex, "GetWithCacheRepository item {ID}", idGame);
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
        [ProducesResponseType(typeof(List<Notification>), 401)]
        [ProducesResponseType(typeof(List<Notification>), 404)]
        [Authorize("Admin")]
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
