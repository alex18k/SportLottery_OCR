using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using wxAPI2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using wxAPI2.Com;


namespace wxAPI2.Controllers
{
    public class ScanDbController : ApiController
    {
        private OCREntities db = new OCREntities();
        public string getinfo()
        {
            //获得需要OCR的记录=>rs
            var rs = from p in db.wximginfo
                     where p.ocr == "N"
                     select new wximginfoDTO
                     {
                         file_name = p.file_name,
                         id = p.id,
                         openid = p.openid
                     };
            if ( rs.Count() == 0) 
                return "Null";
            foreach (var item in rs)
            {
                //循环调用OCR,逐个文件进行识别， 并保存到数据库
                var imgfile = item.file_name;
                JcOcr jcorc = new JcOcr();
                JArray OCRrs = jcorc.AccurateBasicDemo(imgfile);
                JArray OCRrs2 = jcorc.ReceiptDemo(imgfile);
                //匹配字段
                jcMatch jcm = new jcMatch();
                //string rs1 = jcorc.jcmatch(OCRrs,OCRrs2, item.openid);
                string rs1 = jcm.Match(OCRrs, OCRrs2, item.openid);
                //识别成功后，标识OCR="Y"
                if (rs1 == "ok")
                {
                    wximginfo rc = db.wximginfo.Find(item.id); //
                    rc.ocr = "Y";
                };
            }
            db.SaveChanges();
            return "OCR Succeed! and the data had saved !";
        }
    }
}
