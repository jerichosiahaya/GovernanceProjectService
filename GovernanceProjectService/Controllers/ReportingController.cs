using GovernanceProjectService.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GovernanceProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        private IRhafile _rhaFile;
        public ReportingController(IRhafile rhaFile)
        {
            _rhaFile = rhaFile;
        }

        [HttpGet("CountRHA")]
        public async Task<IActionResult> CountRHA()
        {
            var results = await _rhaFile.CountRha();
            var resultsCount = results.ToList();
            if (resultsCount == null)
                return BadRequest();
            return Ok(new { rha_count = resultsCount.Count(), log_time = DateTime.Now });
        }

        //// GET: api/<ReportingController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<ReportingController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<ReportingController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<ReportingController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ReportingController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
