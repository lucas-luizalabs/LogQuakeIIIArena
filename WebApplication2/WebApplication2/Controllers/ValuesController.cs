using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LogQuake.API.ViewModels;
using LogQuake.Domain.Entities;
using LogQuake.Infra.Data.Repositories;
using LogQuake.Service.Services;
using LogQuake.Service.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly PlayerRepository _playerRepository = new PlayerRepository();
        private ServiceBase<Player> service = new ServiceBase<Player>();

        // GET api/values
        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        /// <description>
        /// Teste xpto.
        /// </description>
        /// <operationId>
        /// Teste.
        /// </operationId>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    //var playerViewmodel = Mapper.Map<IEnumerable<Player>, IEnumerable<PlayerViewModel>>(_playerRepository.GetAll());
        //    return new string[] { "value1", "value2" };
        //}

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return new ObjectResult(1);// service.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            //var playerViewmodel = Mapper.Map<IEnumerable<Player>, IEnumerable<PlayerViewModel>>(_playerRepository.GetAll());
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Player player = new Player();
            player.PlayerName = "teste";
            //player.Sobrenome = "xxx";

            //Player retorno = service.Post<UserValidator>(player);

            try
            {
                string fileName = @"c:\LogQuake\games.log";
                LogQuakeService log = new LogQuakeService();
                ServiceBase<Kill> serviceKill = new ServiceBase<Kill>();
                ServiceBase<Player> servicePlayer = new ServiceBase<Player>();
                List<Game> Games;
                List<Kill> Kills;
                Kill kill;

                Games = log.CarregarLog(fileName);
                Kills = log.CarregarLogParaDB(fileName);

                foreach (Kill item in Kills)
                {
                    serviceKill.Add<KillValidator>(item);
                }


                service.Add<PlayerValidator>(player);
                return new ObjectResult(player.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            //return "value";
        }

        /// <summary>
        /// Cria um novo item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/values
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Retorno um item criado</response>
        /// <response code="400">Quando o retorno for nulo</response>
        #region snippet_CreateActionAttributes
        [ProducesResponseType(201)]     // Created
        [ProducesResponseType(400)]     // BadRequest
        #endregion snippet_CreateActionAttributes
        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
