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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LogQuake.API.Controllers
{
    [Route("api/[controller]")]
    public class GamesController : Controller
    {

        //private readonly ILogQuakeService _GameService;
        private readonly LogQuakeService _GameService = new LogQuakeService();

        //public GamesController(ILogQuakeService GameService)
        //{
        //    _GameService = GameService;
        //}

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get([FromQuery]PageRequestBase pageRequest)

        {
            IEnumerable<Kill> jogos;

            if (pageRequest == null)
                pageRequest = new PageRequestBase { PageNumber = 1, PageSize = 5 };

            try
            {
                jogos = _GameService.GetAll(pageRequest);// (new BuscarTodosJogosCommand(pageRequest.pageSize, pageRequest.pageNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

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
