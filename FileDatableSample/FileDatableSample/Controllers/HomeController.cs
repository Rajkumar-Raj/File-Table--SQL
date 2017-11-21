using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FileDatableSample;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace FileDatableSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FileTable()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FileTable(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        //string path = "\\\\Desktop-0mdu7e8\\sqlexpress2014\\FileTableDB\\FileTableTb_Dir\\" + file.FileName;
                        //file.SaveAs(path);
                         FileDatableSample.FileTable.InsertFile(file);
                    }
                }
            }

            return RedirectToAction("FileTable");
        }


        public FileResult Download(string streamid)
        {
            FileTableData f = FileDatableSample.FileTable.GetFileData(streamid);

            return File(f.bytes, System.Net.Mime.MediaTypeNames.Application.Octet, f.fileName);
        }

        public HttpResponseMessage Downloadfile(string streamid)
        {
            // var fileDescription = _fileRepository.GetFileDescription(id);
            string contentType = MimeMapping.GetMimeMapping("6-2-2017 12-48-00 PM.png");
            string path = "D:\\FileTable\\FS\\693738de-f095-411b-8814-49a1cbb318dd\\87061aba-eebb-46ad-aa6e-5691c30c139b\\00000027-00000170-0006";// ServerUploadFolder + "\\" + fileDescription.FileName;
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return result;
        }

    }
}