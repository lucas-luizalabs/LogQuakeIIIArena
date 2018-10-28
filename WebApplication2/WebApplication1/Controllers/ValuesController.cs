using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

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

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        //public async Task<IActionResult> Upload([FromForm] IFormFile file)
        //public async Task<IActionResult> PostProfilePicture(ICollection<IFormFile> file)
        {
            var stream = file.OpenReadStream();
            var name = file.FileName;

            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/log",
                        file.FileName);

            using (var stream2 = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream2);
            }

            return Ok(new { length = file.Length, name = file.Name });
        }
    }
}
