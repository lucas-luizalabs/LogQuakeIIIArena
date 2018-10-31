using System;
using System.Collections.Generic;
using System.IO;
using LogQuake.Domain.Entities;
using LogQuake.Infra.CrossCuting;
using LogQuake.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        #endregion

        #region Construtor
        /// <summary>
        /// Constrututor da classe controladora de Games
        /// </summary>
        /// <param name="logQuakeService">objeto de Serviço do Jogo</param>
        public GamesController(ILogQuakeService logQuakeService)
        {
            _logQuakeService = logQuakeService;
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
        [Produces("application/json", Type = typeof(Game))]
        public IActionResult Get([FromQuery]PageRequestBase pageRequest)

        {
            Dictionary<string, Game> games;

            if (pageRequest == null)
                pageRequest = new PageRequestBase { PageNumber = 1, PageSize = 5 };

            try
            {
                games = _logQuakeService.GetAll(pageRequest);
                if (games.Count == 0)
                    return NotFound(games);
                else
                    return Ok(games);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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
        [Produces("application/json", Type = typeof(Game))]
        public IActionResult Get(int idGame)
        {
            Dictionary<string, Game> game;

            try
            {
                game = _logQuakeService.GetById(idGame);
                if (game.Count == 0)
                    return NotFound(game);
                else
                    return Ok(game);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<controller>
        /// <summary>
        /// Método para efetuar o Upload do arquivo de Log do jogo Quake III Arena
        /// </summary>
        /// <remarks>
        /// Selecione um arquivo texto.
        /// </remarks>        
        /// <param name="file">arquivo a ser processado</param>
        /// <response code="200">Upload realizado com sucesso.</response>
        /// <response code="400">Bad Request.</response>
        [HttpPost("Upload")]
        public IActionResult Upload(IFormFile file)
        {
            string path;
            int RegistrosInseridos = 0;

            if (file == null || file.Length == 0)
                return BadRequest("Arquivo não selecionado.");

            try
            {
                var stream = file.OpenReadStream();
                var name = file.FileName;

                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/log", file.FileName);

                using (var stream2 = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream2);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Falha ao ler o arquivo " + file.FileName + Environment.NewLine + ex.InnerException);
            }

            try
            {
                List<Kill> Kills;
                List<string> linhas = _logQuakeService.ReadLogFile(path);

                if (linhas.Count > 0)
                {
                    Kills = _logQuakeService.ConvertLogFileInListKill(linhas);

                    RegistrosInseridos = _logQuakeService.AddKillListInDB(Kills);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Falha ao processar o arquivo " + file.FileName + Environment.NewLine + ex.InnerException);
            }

            return Ok(new { length = file.Length, name = file.Name, Registros = RegistrosInseridos });
        }
        #endregion
    }
}
