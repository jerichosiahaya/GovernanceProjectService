﻿using GovernanceProjectService.Data;
using GovernanceProjectService.Models;
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
    [Route("api/[controller]")]
    [ApiController]
    public class RHAFilesController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        private IRhafile _rhaFile;
        public RHAFilesController(IRhafile rhaFile, IHostingEnvironment hostingEnvironment)
        {
            _rhaFile = rhaFile;
            _hostingEnvironment = hostingEnvironment;
        }
        List<string> allowedFileExtensions = new List<string>() { "jpg", "png", "doc", "docx", "xls", "xlsx", "pdf", "csv", "txt", "zip", "rar" };

        //// GET: api/<RHAFilesController>
        [HttpGet]
        public async Task<IEnumerable<Rhafile>> Get()
        {
            //return new string[] { "value1", "value2" };
            return await _rhaFile.GetAll();
        }

        //// GET api/<RHAFilesController>/5
        [HttpGet("Download/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var results = await _rhaFile.GetById(id.ToString());
            if (results == null)
                return BadRequest(new { status = "Error", message = "There is no such a file" });

            var path = results.FilePath;
            var fileName = results.FileName;
            var fileType = results.FileType;
            //var files = Directory.GetFiles(path);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, fileType, fileName);

            //return Ok();

        }

        // POST api/<RHAFilesController>
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile formFile, [FromForm] Rhafile rhafile)
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
                rhafile.FileName = formFile.FileName;
                rhafile.FilePath = filePath;
                rhafile.FileType = formFile.ContentType;
                rhafile.FileSize = formFile.Length;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                    await _rhaFile.Insert(rhafile);
                }
                //await _rhaFile.Insert(rhafile);
                //return Ok("Success");
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

        //[HttpPost]
        //public async Task<IActionResult> Post([FromForm] Rhafile rhafile)
        //{
        //    try
        //    {
        //        rhafile.FilePath = "DRIVE D";
        //        await _rhaFile.Insert(rhafile);
        //        return Ok("Success");
        //     }
        //    catch (DbUpdateException dbEx)
        //    {
        //        throw new Exception(dbEx.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //// PUT api/<RHAFilesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<RHAFilesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}