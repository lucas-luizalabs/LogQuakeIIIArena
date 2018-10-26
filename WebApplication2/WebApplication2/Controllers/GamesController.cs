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

        private readonly ILogQuakeService _logQuakeService;
        private readonly IServiceBase<Kill> _serviceBase;

        /// <summary>
        /// Constrututor da classe
        /// </summary>
        /// <param name="logQuakeService">objeto de Serviço do Jogo</param>
        /// <param name="serviceBase">objeto de Serviço Base</param>
        public GamesController(ILogQuakeService logQuakeService, IServiceBase<Kill> serviceBase)
        {
            _logQuakeService = logQuakeService;
            _serviceBase = serviceBase;
        }

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get([FromQuery]PageRequestBase pageRequest)

        {
            List<_Game> jogos;

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

            List<object> retorno = new List<object>();
            Dictionary<string, _Game> SomeKey = null;
            SomeKey = new Dictionary<string, _Game>();
            for (int i = 0; i < 2; i++)
            {
                //SomeKey = new Dictionary<string, _Game>();
                _Game game = new _Game();
                game.Players = new string[] { "one", "two", "three" };
                //game.Kills = new Kills();
                game.TotalKills = 10;
                game.Kills.Add("one", 2 * i);
                game.Kills.Add("two", 1 * i);


                SomeKey["game_" + i] = game;
                retorno.Add(SomeKey);

            }

            return new ObjectResult(SomeKey);

            return new ObjectResult(jogos);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
            string fileName = @"c:\LogQuake\games.log";
            //LogQuakeService log = new LogQuakeService();
            //ServiceBase<Kill> serviceKill = new ServiceBase<Kill>();
            //ServiceBase<Player> servicePlayer = new ServiceBase<Player>();
            List<Game> Games;
            List<Kill> Kills;
            //Kill kill;


            Games = _logQuakeService.CarregarLog(fileName);
            Kills = _logQuakeService.CarregarLogParaDB(fileName);

            foreach (Kill item in Kills)
            {
                _serviceBase.Add<KillValidator>(item);
            }


            //service.Add<PlayerValidator>(player);

        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
