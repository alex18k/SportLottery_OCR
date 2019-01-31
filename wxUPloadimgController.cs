using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using wxAPI2.Models;


namespace wxAPI2.Controllers
{
    public class wxUPloadimgController : ApiController
    {
        private OCREntities db = new OCREntities();
        public IHttpActionResult Post()
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            string openid = HttpContext.Current.Request.Form[0];
            string imgpath = HttpContext.Current.Server.MapPath("~/imgcoll/");
            foreach (string key in files.AllKeys)
            {
                HttpPostedFile file = files[key];
                if (string.IsNullOrEmpty(file.FileName) == false)
                    file.SaveAs(imgpath + file.FileName);
                //写入数据库
                wximginfo info = new wximginfo{
                    openid = openid,
                    file_name = imgpath + file.FileName,
                    upload_date = System.DateTime.Now,
                    ocr = "N"
                };
                db.wximginfo.Add(info);
            }
            db.SaveChanges();
            return Ok("Upload Success!");
        }
    }
}
