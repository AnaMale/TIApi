// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace TIAPi
{
  public class UploaderController : Controller
  {
    private IHostingEnvironment hostingEnvironment;

    public UploaderController(IHostingEnvironment hostingEnvironment)
    {
      this.hostingEnvironment = hostingEnvironment;
    }

    [HttpPost]
    public async Task<IActionResult> Index(IList<IFormFile> files)
    {

      var totalBytes = files.Sum(f => f.Length);

      foreach (var source in files)
      {
        var filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.ToString().Trim('"');

        filename = this.EnsureCorrectFilename(filename);

        var buffer = new byte[16 * 1024];

        using (var output = System.IO.File.Create(this.GetPathAndFilename(filename)))
        {
          using (var input = source.OpenReadStream())
          {
            long totalReadBytes = 0;
            int readBytes;

            while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
            {
              await output.WriteAsync(buffer, 0, readBytes);
              totalReadBytes += readBytes;
              await Task.Delay(10);
            }
          }
        }
      }

      return this.Content("success");
    }

    private string EnsureCorrectFilename(string filename)
    {
      if (filename.Contains("\\"))
        filename = filename.Substring(filename.LastIndexOf("\\") + 1);

      return filename;
    }

    private string GetPathAndFilename(string filename)
    {
      var path = this.hostingEnvironment.WebRootPath + "\\uploads\\";

      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

      return path + filename;
    }
  }
}