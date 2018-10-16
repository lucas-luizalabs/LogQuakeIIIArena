using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
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
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
