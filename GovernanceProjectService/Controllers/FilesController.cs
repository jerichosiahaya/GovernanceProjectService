using GovernanceProjectService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GovernanceProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost(nameof(Upload))]
        public IActionResult Upload([Required] List<IFormFile> formFiles)
        {
            try
            {
                _fileService.UploadFile(formFiles);

                return Ok(new { formFiles.Count, Size = _fileService.SizeConverter(formFiles.Sum(f => f.Length)) });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet(nameof(Download))]
        public IActionResult Download()
        {

            try
            {
                var subDirectory = "UploadedFiles";
                var (fileType, archiveData, archiveName) = _fileService.DownloadFiles(subDirectory);
                return File(archiveData, fileType, archiveName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
