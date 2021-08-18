using GovernanceProjectService.Data;
using GovernanceProjectService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GovernanceProjectService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InputTLFilesController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        private IInputTlfile _tlFile;
        public InputTLFilesController(IInputTlfile tlFile, IHostingEnvironment hostingEnvironment)
        {
            _tlFile = tlFile;
            _hostingEnvironment = hostingEnvironment;
        }
        List<string> allowedFileExtensions = new List<string>() { "jpg", "png", "doc", "docx", "xls", "xlsx", "pdf", "csv", "txt", "zip", "rar" };
        // GET: api/<InputTLFilesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var results = await _tlFile.GetAll();
                if (results == null)
                    return BadRequest(new { status = "Null", messaga = "File is empty" });
                var files = results.ToList();
                return Ok(new { data = files });
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(dbEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // GET api/<InputTLFilesController>/5
        [HttpGet("GetOnlyDetails/{id}")]
        public async Task<IActionResult> GetOnlyDetails(int id)
        {
            var results = await _tlFile.GetById(id.ToString());
            if (results == null)
                return BadRequest(new { status = "Error", message = "There is no such a file" });
            return Ok(new { data = results });
        }

        [HttpGet("GetOnlyFile/{id}")]
        public async Task<IActionResult> GetOnlyFile(int id)
        {
            var results = await _tlFile.GetById(id.ToString());
            if (results == null)
                return BadRequest(new { status = "Error", message = "There is no such a file" });
            var path = results.FilePath;
            var fileName = results.FileName;
            var fileType = results.FileType;
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            byte[] arr = memory.ToArray();
            memory.Position = 0;
            return File(memory, fileType, fileName);
        }

        // PUT api/<InputTLFilesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] string notes)
        {
            try
            {
                await _tlFile.UpdateNotes(id.ToString(), notes);
                return Ok(new { status = "Success", message = "Notes updated!" });
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(dbEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // POST api/<InputTLFilesController>
        // POST api/<RHAFilesController>
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile formFile, [FromForm] InputTlfile inputTL)
        {
            var subDirectory = "UploadedFiles";
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory);
            Directory.CreateDirectory(target);
            try
            {
                if (formFile.Length <= 0)
                {
                    return BadRequest(new { status = "Error", message = "File is empty" });
                }
                else if (formFile.Length > 2000000)
                {
                    return BadRequest(new { status = "Error", message = "Maximum file upload exceeded" });
                }

                string s = formFile.FileName;
                int i = s.LastIndexOf('.');
                string rhs = i < 0 ? "" : s.Substring(i + 1);
                if (!allowedFileExtensions.Any(a => a.Equals(rhs)))
                {
                    return BadRequest(new { status = "Error", message = $"File with extension {rhs} is not allowed", logtime = DateTime.Now });
                }
                var filePath = Path.Combine(target, formFile.FileName);
                inputTL.FileName = formFile.FileName;
                inputTL.FilePath = filePath;
                inputTL.FileType = formFile.ContentType;
                inputTL.FileSize = formFile.Length;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                    await _tlFile.Insert(inputTL);
                }
                return Ok(new { status = "Success", message = "File successfully uploaded", file_size = formFile.Length, file_path = filePath, logtime = DateTime.Now });
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(dbEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //// DELETE api/<InputTLFilesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
