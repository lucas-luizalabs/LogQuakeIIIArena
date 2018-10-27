using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LogQuake.API.ViewModels;
using LogQuake.Domain.Entities;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.CrossCuting;
using LogQuake.Infra.Data.Repositories;
using LogQuake.Service.Services;
using LogQuake.Service.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LogQuake.API.Controllers
{
    /// <summary>
    /// API controladora do jogo
    /// </summary>
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
        private readonly IKillRepository _killRepository;// = new PlayerRepository();
        private readonly ILogQuakeService _logQuakeService;
        private readonly IServiceBase<Kill> _serviceBase;

        /// <summary>
        /// Constrututor da classe
        /// </summary>
        /// <param name="killRepository">repositório do log</param>
        /// <param name="logQuakeService">objeto de Serviço do Jogo</param>
        /// <param name="serviceBase">objeto de Serviço Base</param>
        //public GamesController(IKillRepository killRepository, ILogQuakeService logQuakeService, IServiceBase<Kill> serviceBase)
        //{
        //    _logQuakeService = logQuakeService;
        //    _serviceBase = serviceBase;
        //    _killRepository = killRepository;
        //}

        public GamesController(ILogQuakeService logQuakeService)
        {
            _logQuakeService = logQuakeService;
        }


        /// <summary>
        /// Consultar log de todos os jogos, respeitando paginação
        /// </summary>
        /// <param name="pageRequest">controle de paginação</param>
        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get([FromQuery]PageRequestBase pageRequest)

        {
            Dictionary<string, _Game> jogos;

            if (pageRequest == null)
                pageRequest = new PageRequestBase { PageNumber = 1, PageSize = 5 };

            try
            {
                jogos = _logQuakeService.GetAll(pageRequest);
                return new ObjectResult(jogos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Consultar log de todos os jogos po IdGame
        /// </summary>
        /// <param name="IdGame">Código de identificação do jogo</param>
        // GET api/<controller>/5
        [HttpGet("{IdGame}")]
        public IActionResult Get(int IdGame)
        {
            Dictionary<string, _Game> jogo;

            try
            {
                jogo = _logQuakeService.GetById(IdGame);
                return new ObjectResult(jogo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Método para gravar o log em Banco de Dados
        /// </summary>
        /// <param name="arquivo">nome do arquivo a ser processado</param>
        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string arquivo)
        {
            string fileName = @"c:\LogQuake\games.log";
            List<Kill> Kills;

            List<string> linhas = _logQuakeService.LerArquivoDeLog(fileName);

            if (linhas.Count > 0)
            {
                Kills = _logQuakeService.CarregarLogParaDB(linhas);

                foreach (Kill item in Kills)
                {
                    _serviceBase.Add<KillValidator>(item);
                }
            }
        }

        //// PUT api/<controller>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
