﻿using GovernanceProjectService.Data;
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
    //[Authorize]
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
        public async Task<IActionResult> Get()
        {
            var results = await _rhaFile.GetAll();
            var files = results.ToList();
            List<byte[]> myArrays = new List<byte[]>();
            files.ForEach(async file =>
            {
                var path = file.FilePath;
                var fileName = file.FileName;
                var fileType = file.FileType;
                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                byte[] arr = memory.ToArray();
                myArrays.Add(arr);
            });
            //files.ForEach(file =>
            //{
            //    var fileId = file.Id;
            //});
            // dari fe dibinding pake id di-endpointnya
            return Ok(new { rha_count = files.Count(), data = files});
        }

        //// GET api/<RHAFilesController>/5
        //[HttpGet("Download/{id}")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    var results = await _rhaFile.GetById(id.ToString());
        //    if (results == null)
        //        return BadRequest(new { status = "Error", message = "There is no such a file" });

        //    var path = results.FilePath;
        //    var fileName = results.FileName;
        //    var fileType = results.FileType;
        //    //byte[] arr = File.ReadAllBytes(fileName);
        //    //var files = Directory.GetFiles(path);
        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(path, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    byte[] arr = memory.ToArray();
        //    memory.Position = 0;
        //    //return File(memory, fileType, fileName);
        //    return Ok(new { data = results, file = arr });
        //    //return File.ReadAllBytes(fileName);
        //}

        [HttpGet("GetOnlyDetails/{id}")]
        public async Task<IActionResult> GetOnlyDetails(int id)
        {
            var results = await _rhaFile.GetById(id.ToString());
            if (results == null)
                return BadRequest(new { status = "Error", message = "There is no such a file" });

            //var path = results.FilePath;
            //var fileName = results.FileName;
            //var fileType = results.FileType;
            ////byte[] arr = File.ReadAllBytes(fileName);
            ////var files = Directory.GetFiles(path);
            //var memory = new MemoryStream();
            //using (var stream = new FileStream(path, FileMode.Open))
            //{
            //    await stream.CopyToAsync(memory);
            //}
            //byte[] arr = memory.ToArray();
            //memory.Position = 0;
            //return File(memory, fileType, fileName);
            return Ok(new { data = results });
            //return File.ReadAllBytes(fileName);
        }

        [HttpGet("GetOnlyFile/{id}")]
        public async Task<IActionResult> GetOnlyFile(int id)
        {
            var results = await _rhaFile.GetById(id.ToString());
            if (results == null)
                return BadRequest(new { status = "Error", message = "There is no such a file" });

            var path = results.FilePath;
            var fileName = results.FileName;
            var fileType = results.FileType;
            //byte[] arr = File.ReadAllBytes(fileName);
            //var files = Directory.GetFiles(path);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            byte[] arr = memory.ToArray();
            memory.Position = 0;
            return File(memory, fileType, fileName);
            //return Ok(new { data = results });
            //return File.ReadAllBytes(fileName);
        }

        [HttpGet("GetByNPP/{npp}")]
        public async Task<IActionResult> GetByNPP(string npp)
        {
            var results = await _rhaFile.GetByNPP(npp);
            var count = results.ToList();
            if (results == null)
                return BadRequest(new { status = "Error", message = "There is no such a file" });
            return Ok(new { count = count.Count, data = results });
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
