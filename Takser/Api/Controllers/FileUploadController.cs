using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Takser.Api.Controllers
{
    public class FileUploadController : Controller
    {
        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long totalSize = files.Sum(file => file.Length);

            List<string> filePaths = new List<string>();
            foreach(IFormFile formFile in files)
            {
                if(formFile.Length > 0)
                {
                    string filePath = Path.GetTempFileName();
                    filePaths.Add(filePath);

                    using (Stream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return Ok(new { count = files.Count, totalSize, filePaths });
        }
    }
}