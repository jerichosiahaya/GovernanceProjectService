using GovernanceProjectService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GovernanceProjectService.Controllers
{
    [Authorize]
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
            var countAll = await _rhaFile.CountRha();
            var countDone = await _rhaFile.CountRhaDone();
            var countPending = await _rhaFile.CountRhaPending();
            if (countAll == null && countDone == null && countPending == null)
                return BadRequest();
            return Ok(new { rha_count_all = countAll.Count(), rha_count_pending = countPending.Count(), rha_count_done = countDone.Count(), log_time = DateTime.Now });
        }

        [HttpGet("RHADone")]
        public async Task<IActionResult> GetRHADone()
        {
            //var countAll = await _rhaFile.CountRha();
            var countDone = await _rhaFile.CountRhaDone();
            //var countPending = await _rhaFile.CountRhaPending();
            if (countDone == null)
                return BadRequest();
            return Ok(new { data = countDone, log_time = DateTime.Now });
        }

        [HttpGet("RHAPending")]
        public async Task<IActionResult> GetRHAPending()
        {
            //var countAll = await _rhaFile.CountRha();
            //var countDone = await _rhaFile.CountRhaDone();
            var countPending = await _rhaFile.CountRhaPending();
            if (countPending == null)
                return BadRequest();
            return Ok(new { data = countPending, log_time = DateTime.Now });
        }

    }
}
