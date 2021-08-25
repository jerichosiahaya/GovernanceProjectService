using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GovernanceProjectService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelReaderController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        public ExcelReaderController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Get([Required] IFormFile formFile)
        {
            var subDirectory = "UploadedFiles";
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory);
            var filePath = Path.Combine(target, formFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var conf = new ExcelDataSetConfiguration
                    {
                        UseColumnDataType = true,
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    var result = reader.AsDataSet(conf);
                    //result.Tables
                    if (result != null)
                    {
                        //var json = Newtonsoft.Json.JsonConvert.SerializeObject(result.Tables[0], Newtonsoft.Json.Formatting.Indented);
                        var obj = result.Tables[0];
                        var sheetName = obj.TableName;
                        var objCount = obj.Rows.Count;
                        //var obj = System.Text.Json.JsonSerializer.Serialize(result.Tables);
                        //JObject jsonResult = JObject.Parse(json);
                        return Ok(new { status = true, count = objCount, sheet_name = sheetName, data = obj });
                    }
                    else
                    {
                        return NotFound();
                    }
                    // The result of each spreadsheet is in result.Tables
                }
            }
        }

    }
}
