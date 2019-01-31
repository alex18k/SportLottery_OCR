using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using wxAPI2.Models;

namespace wxAPI2.Com
{
    public class jcMatch
    {
        private OCREntities db = new OCREntities();
        public string Match(JArray jcArray, JArray jcArray2, string openid)
        {
            string pattern;
            var jctxt = "";
            dynamic jcc = jcArray;
            dynamic jcc2 = jcArray2;
            var jctxt2 = "";
            try
            {
                foreach (var item in jcc)
                {
                    jctxt = jctxt + item["words"].ToString() + "##";
                }
                foreach (var item in jcc2)
                {
                    jctxt2 = jctxt2 + item["words"].ToString() + "##";
                }
            }
            catch (Exception e)
            {
                return "OcrError caught : " + e;
            }

            //倍数
            pattern = @"\d{1,3}(?=倍)";
            string jc_bs = Regex.Match(jctxt2, pattern).Value;
            //过关方式
             pattern = @"(?<=\b过关方式)\d(×|x)\d";
            string jc_ggfs = Regex.Match(jctxt2, pattern).Value;
            if (String.IsNullOrEmpty(jc_ggfs))
            {
                pattern = @"\b[0-9\.,]*(?=关)";
                jc_ggfs = Regex.Match(jctxt2, pattern).Value;
            };

            //匹配票号
            pattern = @"\b\d{6}-\d{6}-\d{6}-[0-9 A-Za-z]+";
            string jc_nbr = Regex.Match(jctxt, pattern).Value;
            //查找是否有相同票号，如果有，则退出
            if (db.jcm.Where(c => c.jc_nbr == jc_nbr).Count() > 0)
                return "ok";
            //串关类型
            pattern = @"(?<=\b竞彩足球)\w+\b";
            string jc_cglx = Regex.Match(jctxt, pattern).Value;
            //总金额
            pattern = @"(?<=合计)\d+";
            string jc_amount = Regex.Match(jctxt, pattern).Value;
             //日期
            pattern = @"\b\d{2}/\d{2}/\d{2}\b";
            string jc_date = Regex.Match(jctxt, pattern).Value;
            //固定奖金
            pattern = @"(?<=固定奖金:)[0-9\,\.]*";
            string jc_maxamount = Regex.Match(jctxt, pattern).Value;
            jcm jcmstr = new jcm
            {
                openid = openid,
                jc_nbr = jc_nbr,
                jc_cglx = jc_cglx,
                jc_ggfs = jc_ggfs,
                jc_bs = jc_bs,
                jc_amount = jc_amount,
                jc_date = jc_date,
                jc_maxamount = jc_maxamount
            };
            db.jcm.Add(jcmstr);
            db.SaveChanges();

            //获得明细数据：场次+投注选项数组
            pattern = @"(?<=第\d场)\w+\d{3}";
            MatchCollection values = Regex.Matches(jctxt, pattern);
            int line = 1;
            foreach (Match item in values)
            {
                //场次
                var jcd_round = item.Value;
                //过关方式
                pattern = @"(?<=" + item.Value + @")\w{2,5}";
                var jcd_option = Regex.Match(jctxt, pattern).Value;
                //场次行
                var patternline = @"(?<=" + item.Value + @").*?元##";
                var varline = Regex.Match(jctxt, patternline).ToString() ;
                //胜平负金额，赔率
                pattern = @"(?<=[胜,平,负,胜@,平@,负@,胜0,平0,负0])\d\.\d{3}";
                var jcd_spf_amt = Regex.Match(varline, pattern).Value ;
                //胜，平，负
                pattern = "(?<=##)[胜,平,负]";
                var jcd_spf = Regex.Match(varline, pattern).Value;
                //让球数
                pattern = @"(?<=[让球,让球:]主).\w+";
                var jcd_rq  = Regex.Match(varline, pattern).Value;
                jcd_det jcd = new jcd_det
                {
                    jcd_nbr = jc_nbr,
                    jcd_line = line,
                    jcd_round = jcd_round,
                    jcd_option = jcd_option,
                    jcd_spf = jcd_spf,
                    jcd_spf_amt = jcd_spf_amt,
                    jcd_rq = jcd_rq
                      
                };
                db.jcd_det.Add(jcd);
                line++;
            }
            db.SaveChanges();
            return "ok";
        }
    }
}