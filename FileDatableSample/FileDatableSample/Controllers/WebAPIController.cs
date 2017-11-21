using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace FileDatableSample.Controllers
{
    [RoutePrefix("api/WebAPI")]
    public class WebAPIController : ApiController
    {
        [Route("download/{streamid}")]
        [HttpGet]
        public HttpResponseMessage Downloadfile(string streamid)
        {
            var data = FileTable.GetFileData(streamid);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new MemoryStream(data.bytes);// new FileStream(data.physicalfilePath, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(data.fileName));
            return result;
        }
    }
}
