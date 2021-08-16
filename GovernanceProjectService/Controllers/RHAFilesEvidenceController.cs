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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GovernanceProjectService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RHAFilesEvidenceController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        private IRhafilesEvidence _rhaFileEvidence;
        public RHAFilesEvidenceController(IRhafilesEvidence rhaFileEvidence, IHostingEnvironment hostingEnvironment)
        {
            _rhaFileEvidence = rhaFileEvidence;
            _hostingEnvironment = hostingEnvironment;
        }

        List<string> allowedFileExtensions = new List<string>() { "jpg", "png", "doc", "docx", "xls", "xlsx", "pdf", "csv", "txt", "zip", "rar" };
        
        // GET: api/<RHAFilesEvidenceController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _rhaFileEvidence.GetAll();
            var files = results.ToList();
            if (files.Count == 0)
                return Ok(new { status = "null", message = "Empty data" });
            
            return Ok(new { data = files });
        }

        //// GET api/<RHAFilesEvidenceController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        [HttpGet("GetOnlyFile/{id}")]
        public async Task<IActionResult> GetOnlyFile(int id)
        {
            var results = await _rhaFileEvidence.GetById(id.ToString());
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

        [HttpGet("{rhaId}")]
        public async Task<IActionResult> GetByRhaID(string rhaId)
        {
            var results = await _rhaFileEvidence.GetByRhaID(rhaId);
            var files = results.ToList();
            if (files.Count == 0)
                return Ok(new { status = "null", message = "Empty data" });

            return Ok(new { data = files });
        }

        [HttpGet("GetBundleFiles/{rhaId}")]
        public async Task<IActionResult> GetBundleFiles(string rhaId)
        {
            List<byte[]> filesPath = new List<byte[]>();
            var results = await _rhaFileEvidence.GetByRhaID(rhaId);
            var files = results.ToList();
            if (files.Count == 0)
                return Ok(new { status = "null", message = "Empty data" });

            files.ForEach(file =>
            {
                var fPath = file.FilePath;
                byte[] bytes = Encoding.ASCII.GetBytes(fPath);
                filesPath.Add(bytes);
            });

            return DownloadMultipleFiles(filesPath);
        }

        private FileResult DownloadMultipleFiles(List<byte[]> byteArrayList)
        {
            var zipName = $"archive-EvidenceFiles-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            using (MemoryStream ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var file in byteArrayList)
                    {
                        string fPath = Encoding.ASCII.GetString(file);
                        var entry = archive.CreateEntry(fPath, CompressionLevel.Fastest);
                        using (var zipStream = entry.Open())
                        {
                            zipStream.Write(file, 0, file.Length);
                        }
                    }
                }
                return File(ms.ToArray(), "application/zip", zipName);
            }
        }

        // POST api/<RHAFilesEvidenceController>
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload([Required] IFormFile formFile, [FromForm] RhafilesEvidence rhafileEvidence)
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
                rhafileEvidence.FileName = formFile.FileName;
                rhafileEvidence.FilePath = filePath;
                rhafileEvidence.FileType = formFile.ContentType;
                rhafileEvidence.FileSize = formFile.Length;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                    await _rhaFileEvidence.Insert(rhafileEvidence);
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

        // PUT api/<RHAFilesEvidenceController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<RHAFilesEvidenceController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
