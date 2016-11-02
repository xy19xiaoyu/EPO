using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace EPO_Get
{
    public class HttpGet
    {
        private string ShenQingHao;
        public string ShenQingHao2;
        private string ShenQingRi;
        private string DesUrl;
        private string ClaimUrl;
        private string BasePath;
        private string SavePath;
        private string regUrl;
        private string ABSUrl;

        public HttpGet(string shenqinghao)
        {
            this.ShenQingHao = shenqinghao;
        }
        public string Search(out Dictionary<string, string> pris)
        {
            string url;
            //Console.WriteLine("开始发送检索式:" +ShenQingHao);
            //ShenQingHao = "FR2970795";
            //http://worldwide.espacenet.com/searchResults?compact=false&PN={0}&ST=advanced&locale=en_EP&DB=EPODOC
            HttpWebRequest hwq = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://worldwide.espacenet.com/searchResults?compact=false&PN={0}&ST=advanced&locale=en_EP&DB=EPODOC", ShenQingHao));
            //HttpWebRequest hwq = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://worldwide.espacenet.com/searchResults?NUM={0}&DB=EPODOC&locale=en_EP&ST=number&compact=false",ShenQingHao));
            hwq.Timeout = 1000 * 60;
            hwq.Method = "GET";
            hwq.ContentType = "application/x-www-form-urlencoded";
            hwq.AllowAutoRedirect = false;
            CookieContainer cookieCon = new CookieContainer();
            hwq.CookieContainer = cookieCon;
            hwq.ContentLength = 0;
            hwq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            hwq.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, */*";
            hwq.KeepAlive = true;
            WebResponse wrp = hwq.GetResponse();
            Stream streamReceive = wrp.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(streamReceive, encoding);

            string strResult = streamReader.ReadToEnd();
            strResult = strResult.Replace("&amp;", "&").Replace("&nbsp;", "");
            if (string.IsNullOrEmpty(strResult.Trim()))
            {
                pris = null;
                return "reject";
            }
            if (strResult.IndexOf("Please enter the digits that can be read in the image below:") > 0)
            {
                pris = null;
                return "reject";
            }
            //Regex reg = new Regex("1.(\\s|\\n|\\t)*<a id=\"publicationId1\" lang=\"en\" href=\"(?<url>.*)\">");
            //<a id="publicationId1" lang="en" href="(?<url>.*)">
            Regex reg = new Regex("<a id=\"publicationId1\" (lang=\"en\"|lang=\"de\"|lang=\"fr\")? href=\"(?<url>.*)\">");
            Match mh = reg.Match(strResult);
            if (mh.Success)
            {
                url = "http://worldwide.espacenet.com" + mh.Groups["url"].Value.Replace("&amp;", "&").Replace("&nbsp;", "");
                string s;
                pris = GetPriority(url, out s); ;
                return s;
            }
            else
            {
                pris = null;
                return "NoSearched";
            }


            //
        }
        public string Search()
        {
            string url;
            //Console.WriteLine("开始发送检索式:" +ShenQingHao);
            //ShenQingHao = "FR2970795";
            //http://worldwide.espacenet.com/searchResults?compact=false&PN={0}&ST=advanced&locale=en_EP&DB=EPODOC
            HttpWebRequest hwq = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://worldwide.espacenet.com/searchResults?compact=false&PN={0}&ST=advanced&locale=en_EP&DB=EPODOC", ShenQingHao));
            //HttpWebRequest hwq = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://worldwide.espacenet.com/searchResults?NUM={0}&DB=EPODOC&locale=en_EP&ST=number&compact=false",ShenQingHao));
            hwq.Timeout = 1000 * 60;
            hwq.Method = "GET";
            hwq.ContentType = "application/x-www-form-urlencoded";
            hwq.AllowAutoRedirect = false;
            CookieContainer cookieCon = new CookieContainer();
            hwq.CookieContainer = cookieCon;
            hwq.ContentLength = 0;
            hwq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            hwq.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, */*";
            hwq.KeepAlive = true;
            WebResponse wrp = hwq.GetResponse();
            Stream streamReceive = wrp.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(streamReceive, encoding);

            string strResult = streamReader.ReadToEnd();
            strResult = strResult.Replace("&amp;", "&").Replace("&nbsp;", "");
            if (strResult.IndexOf("Please enter the digits that can be read in the image below:") > 0)
            {
                return "reject";
            }
            //Regex reg = new Regex("1.(\\s|\\n|\\t)*<a id=\"publicationId1\" lang=\"en\" href=\"(?<url>.*)\">");
            //<a id="publicationId1" lang="en" href="(?<url>.*)">
            Regex reg = new Regex("<a id=\"publicationId1\" (lang=\"en\"|lang=\"de\"|lang=\"fr\")? href=\"(?<url>.*)\">");
            Match mh = reg.Match(strResult);
            if (mh.Success)
            {
                url = "http://worldwide.espacenet.com" + mh.Groups["url"].Value.Replace("&amp;", "&").Replace("&nbsp;", "");
                System.Threading.Thread.Sleep(100);
                GetUrl(url);
                return "";
            }
            else
            {              
                return "NoSearched";
            }


            //
        }
        public Dictionary<string, string> GetPriority(string url, out string OperatorReuslt)
        {
            Dictionary<string, string> dicPri = new Dictionary<string, string>();
            OperatorReuslt = string.Empty;
            //Console.WriteLine("解析得到权利要求说明书的URL");
            HttpWebRequest hwq = (HttpWebRequest)HttpWebRequest.Create(url);
            hwq.Timeout = 1000 * 60;
            hwq.Method = "GET";
            hwq.ContentType = "application/x-www-form-urlencoded";
            hwq.AllowAutoRedirect = false;
            CookieContainer cookieCon = new CookieContainer();
            hwq.CookieContainer = cookieCon;
            hwq.ContentLength = 0;
            hwq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            hwq.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, */*";
            hwq.KeepAlive = true;
            WebResponse wrp = hwq.GetResponse();
            Stream streamReceive = wrp.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(streamReceive, encoding);
            string strResult = streamReader.ReadToEnd();
            if (string.IsNullOrEmpty(strResult))
            {
                OperatorReuslt = "reject";
                return null;
            }
            strResult = strResult.Replace("&amp;", "&").Replace("&nbsp;", "");
            if (strResult.IndexOf("Please enter the digits that can be read in the image below:") > 0)
            {
                OperatorReuslt = "reject";
                return null;
            }
            Regex regTitle = new Regex("<h3 lang=\"en\">(?<title>(.|\n\r\t\\s)*),/h3>");

            Regex regPri = new Regex(@"(?<prinum>[^\'\>]{3,20})\s{1,3}(?<pridate>\d{8})");

            Regex reg = new Regex("Priority\\snumber\\(s\\)\\:(\\n|\\s)*\\</th\\>(\\n|\\s)*\\<td\\sclass=\"printTableText\"\\>(\\n|\\s)*(?<xy>(\\n|.)*)\\</td\\>(\\n|\\s)*\\</tr\\>(\\n|\\s)*(\\<tr\\>|\\</tbody\\>)");

            string pris = reg.Match(strResult).Groups["xy"].Value;

            string[] arypris = pris.Split(';');
            foreach (string s in arypris)
            {
                string tmp = s.Replace("\n", "").Trim();
                Match ms = regPri.Match(s);
                string prinum = "";
                string pridate = "";
                if (ms.Success)
                {
                    prinum = ms.Groups["prinum"].Value;
                    pridate = ms.Groups["pridate"].Value;
                    if (!dicPri.ContainsKey(prinum))
                    {
                        dicPri.Add(prinum, pridate);
                    }
                }


            }

            return dicPri;
        }

        public string GetUrl(string url)
        {
            //Console.WriteLine("解析得到权利要求说明书的URL");
            HttpWebRequest hwq = (HttpWebRequest)HttpWebRequest.Create(url);
            hwq.Timeout = 1000 * 60;
            hwq.Method = "GET";
            hwq.ContentType = "application/x-www-form-urlencoded";
            hwq.AllowAutoRedirect = false;
            CookieContainer cookieCon = new CookieContainer();
            hwq.CookieContainer = cookieCon;
            hwq.ContentLength = 0;
            hwq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            hwq.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, */*";
            hwq.KeepAlive = true;
            WebResponse wrp = hwq.GetResponse();
            Stream streamReceive = wrp.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(streamReceive, encoding);
            string strResult = streamReader.ReadToEnd();
            strResult = strResult.Replace("&amp;", "&").Replace("&nbsp;", "");
            if (strResult.IndexOf("Please enter the digits that can be read in the image below:") > 0)
            {
                return "reject";
            }
            Regex regd = new Regex("href=\"(?<url>.*)\">(\\s|\n|\t)*Description");
            Regex regc = new Regex("href=\"(?<url>.*)\">(\\s|\n|\t)*Claims");
            Match mh = regc.Match(strResult);
            //Regex regOrig = new Regex("href=\"(?<url>.*)\">(\\s|\n|\t)*Original document");
            Regex reg3 = new Regex("<h1 class=\"noBottomMargin\">(\n|\\s|\r\n)*Bibliographic data: (?<xy1>[a-zA-Z0-9]{7,30})(\n|\\s|\r\n)*\\((?<xy>[a-zA-Z0-9]{0,4})\\)");
            Match mh3 = reg3.Match(strResult);

            if (mh3.Success)
            {
                ShenQingHao2 = mh3.Groups["xy1"].Value.Replace("<br/>", Environment.NewLine) + mh3.Groups["xy"].Value.Replace("<br/>", Environment.NewLine);

            }
            string ABS = string.Empty;
            Regex regABS = new Regex("<p\\s{1,3}(lang=\"en\"|lang=\"de\"|lang=\"fr\")? class=\"printAbstract\">(\n|\\s|\t)*(?<xy>(.|\n)*\\.)(<br/>|<br>){0,2}(\n|\\s|\t|\r\n)*</p>");
            Match mhabs = regABS.Match(strResult);
            if (mhabs.Success)
            {
                ABS = mhabs.Groups["xy"].Value.Replace("<br/>", Environment.NewLine);
            }
            else
            {
                ABS = "NoABS";
            }


            if (mh.Success)
            {
                DesUrl = "http://worldwide.espacenet.com" + regd.Match(strResult).Groups["url"].Value.Replace("&amp;", "&").Replace("&nbsp;", "");
                ClaimUrl = "http://worldwide.espacenet.com" + regc.Match(strResult).Groups["url"].Value.Replace("&amp;", "&").Replace("&nbsp;", "");
                // regUrl = "http://worldwide.espacenet.com" + regOrig.Match(strResult).Groups["url"].Value.Replace("&amp;", "&").Replace("&nbsp;", "");
                return ABS;
            }
            else
            {
                return "NoUrl";
            }
        }

        public string GetClaims()
        {

            string Claims = string.Empty;

            // Console.WriteLine("开始获取权利要求");
            HttpWebRequest hwq = (HttpWebRequest)HttpWebRequest.Create(ClaimUrl);
            hwq.Timeout = 1000 * 60;
            hwq.Method = "GET";
            hwq.ContentType = "application/x-www-form-urlencoded";
            hwq.AllowAutoRedirect = false;
            CookieContainer cookieCon = new CookieContainer();
            hwq.CookieContainer = cookieCon;
            hwq.ContentLength = 0;
            hwq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            hwq.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, */*";
            hwq.KeepAlive = true;
            WebResponse wrp = hwq.GetResponse();
            Stream streamReceive = wrp.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(streamReceive, encoding);
            string strResult = streamReader.ReadToEnd();
            strResult = strResult.Replace("&amp;", "&").Replace("&nbsp;", "");
            if (strResult.IndexOf("Please enter the digits that can be read in the image below:") > 0)
            {
                return "reject";
            }
            // Regex reg = new Regex("<p lang=\"(en|de|fr)\">(\n|\\s|\t|\r\n)*(?<xy>(.|\n)*)(<br/>|<br>){1,2}(\n|\\s|\t|\r\n)*2\\.");
            Regex reg2 = new Regex("<p lang=\"(en|de|fr)\">(\n|\\s|\t)*(?<xy>(.|\n)*)(<br/>|<br>){1,2}(\n|\\s|\t|\r\n)</p>");


            // Match mh = reg.Match(strResult);
            Match mh2 = reg2.Match(strResult);



            //if (mh.Success)
            //{
            //    Claims = mh.Groups["xy"].Value.Replace("<br/>", Environment.NewLine);
            //}
            //else
            //{
            if (mh2.Success)
            {
                Claims = mh2.Groups["xy"].Value.Replace("<br/>", Environment.NewLine);
            }
            else
            {
                return "NoClaims";
            }
            //}
            return Claims;

        }

        public string GetDes()
        {
            string Des;
            //Console.WriteLine("开始获取说明书");
            HttpWebRequest hwq = (HttpWebRequest)HttpWebRequest.Create(DesUrl);
            hwq.Timeout = 1000 * 60;
            hwq.Method = "GET";
            hwq.ContentType = "application/x-www-form-urlencoded";
            hwq.AllowAutoRedirect = false;
            CookieContainer cookieCon = new CookieContainer();
            hwq.CookieContainer = cookieCon;
            hwq.ContentLength = 0;
            hwq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            hwq.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, */*";
            hwq.KeepAlive = true;
            WebResponse wrp = hwq.GetResponse();
            Stream streamReceive = wrp.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(streamReceive, encoding);
            string strResult = streamReader.ReadToEnd();
            strResult = strResult.Replace("&amp;", "&").Replace("&nbsp;", "");
            if (strResult.IndexOf("Please enter the digits that can be read in the image below:") > 0)
            {
                return "reject";
            }
            Regex reg = new Regex("<div\\sid=\"description\"\\sclass=\"application\\s{1,3}article\\s{1,3}clearfix\">(\t|\n|\\s)*<p>(?<xy>(.|\n)*)</p></div>");
            //<p lang="en" class="printTableText">
            Regex reg2 = new Regex("<p lang=\"(en|de|fr)\" class=\"printTableText\">(?<xy>(.|\n)*)(<br/>|<br>){1,2}(\n|\\s|\t|\r\n)*</p>");
            Match mh = reg2.Match(strResult);
            if (mh.Success)
            {
                Des = mh.Groups["xy"].Value.Replace("<br/>", Environment.NewLine);
            }
            else
            {
                Des = "NoDes";
            }

            return Des;
        }


    }
}
