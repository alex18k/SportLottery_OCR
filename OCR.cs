using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Baidu.Aip;


namespace wxAPI2.Com
{
    public class OCRdemo

    {
        public JObject AccurateBasicDemo(string img)
        {
            // 调用通用文字识别（高精度版），可能会抛出网络等异常，请使用try/catch捕获
            var image = File.ReadAllBytes(img);
            var ApiKey = "ApiKey";
            var SecretKey = "SecretKey";
            var client = new Baidu.Aip.Ocr.Ocr(ApiKey, SecretKey);
            var result = client.AccurateBasic(image);
            return result;
        }
        public JObject CustomDemo(string img)
        {
            // 自定义模板识别

            var image = File.ReadAllBytes(img);
            var ApiKey = "ApiKey";
            var SecretKey = "SecretKey";
            //var templateSign = "b3153a0a3890abcf78ed3338a3f1a4eb";
            var templateSign = "86b6e4bbf2ad15045d4d2be6d2f263fe";
            var client = new Baidu.Aip.Ocr.Ocr(ApiKey, SecretKey);
            var result = client.Custom(image, templateSign);
            return result;
 
        }
    }
}
