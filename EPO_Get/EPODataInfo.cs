using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace EPO_Get
{
    public class EPODataInfo
    {
        public string Title = string.Empty;
        public string PubNo = string.Empty;
        public string PubDate = string.Empty;
        public string Inventor = string.Empty;
        public string Applicant = string.Empty;
        public string AppNo = string.Empty;
        public string AppDate = string.Empty;
        public string PriorityNumber = string.Empty;
        public string IPC = string.Empty;
        public string Claims = string.Empty;
        public string Description = string.Empty;
        public string ABS = string.Empty;
        public static Regex htmlTager = new Regex("</?[^>]*>");

        public EPODataInfo(string pubno)
        {
            this.PubNo = pubno;
        }
        public bool GetData()
        {
            getBiblio();
            getClaims();
            getDescription();
            return true;
        }
        public bool getBiblio()
        {

            //string BiblioUrl = string.Format("http://worldwide.espacenet.com/publicationDetails/biblio?FT=D&CC={0}&NR={1}", PubNo.Substring(0, 2), PubNo.Substring(2, PubNo.Length - 2));
            string BiblioUrl = string.Format("http://worldwide.espacenet.com/data/publicationDetails/biblio?FT=D&CC={0}&NR={1}", PubNo.Substring(0, 2), PubNo.Substring(2, PubNo.Length - 2));
            string strResult = GetHtml(BiblioUrl);
            strResult = strResult.Replace("&amp;", "&").Replace("&nbsp;", "").Replace("<wbr/>", "").Trim();
            if (string.IsNullOrEmpty(strResult) || strResult.IndexOf("Please enter the digits that can be read in the image below:") > 0)
            {
                throw new Exception("对不起！服务器繁忙！！请稍候重试！！！");

            }

            Regex regTitle = new Regex("<h3\\s*(lang=\"en\")?>(\n|\r|\t|\\s)*(?<title>[^<]*)(\n|\r|\t|\\s)*</h3>");

            Match mhTitle = regTitle.Match(strResult);
            if (mhTitle.Success)
            {
                Title = mhTitle.Groups["title"].Value.Trim();
            }

            Regex regInverts = new Regex("Inventor\\(s\\)\\:(\n|\t|\r|\\s)*</th>(\n|\t|\r|\\s)*<td\\sclass=\"printTableText\"\\sid=\"inventorsRow\">(?<Inventors>(.|\n|\t|\r|\\s)*?)</td>");

            Match mhInverts = regInverts.Match(strResult);
            if (mhInverts.Success)
            {
                Inventor = mhInverts.Groups["Inventors"].Value.Trim();
                Inventor = Regex.Replace(Inventor, "<[^>]*>", "");
                Inventor = Regex.Replace(Inventor, "(\\t|\\n|\\r|\\+)+", "");
                //Inventor = mhInverts.Groups["Inventors"].Value.Trim().Replace("\n", "").Replace("<a href=\"#\" onclick=\"$('#secondaryInventors').toggleClass('hidden'); return false\">+</a>", ";").Replace("<span id=\"secondaryInventors\" class=\"hidden\">", "").Replace("</span>", "").Replace("\t", "").Replace("<span id=\"inventors\">","");
                Inventor = Regex.Replace(Inventor, "\\s{2,}", "");

            }

            Regex regApplicants = new Regex("Applicant\\(s\\)\\:(\n|\t|\r|\\s)*</th>(\n|\t|\r|\\s)*<td\\sclass=\"printTableText\"\\sid=\"applicantsRow\">(?<Applicants>(.|\n|\t|\r|\\s)*?)</td>");

            Match mhApplicants = regApplicants.Match(strResult);
            if (mhApplicants.Success)
            {
                Applicant = mhApplicants.Groups["Applicants"].Value.Trim();
                Applicant = Regex.Replace(Applicant, "<[^>]*>", "");
                Applicant = Regex.Replace(Applicant, "(\\t|\\n|\\r|\\+)+", "");
                Applicant = Regex.Replace(Applicant, "\\s{2,}", "");
            }

            Regex regapno = new Regex("Application\\snumber\\:(\n|\t|\r|\\s)*</th>(\n|\t|\r|\\s)*<td\\sclass=\"printTableText\">(\n|\t|\r|\\s)*(?<appno>.*)\\s(?<appdate>\\d{8})(\n|\t|\r|\\s)*?(</td>|<a)");

            Match mhapno = regapno.Match(strResult);
            if (mhapno.Success)
            {
                AppNo = mhapno.Groups["appno"].Value.Trim();
                AppDate = mhapno.Groups["appdate"].Value.Trim();
            }

            Regex regpubno = new Regex("Bibliographic\\sdata\\:(\\n|\\r|\\s)*(?<pubno>.{3,20}(\r|\n|\\s|\t)*\\((\\w|\\d){0,4}\\)?)(\r|\n|\\s|\t)*―\\s(?<pubdate>.*)");

            Match mhpubno = regpubno.Match(strResult);
            if (mhpubno.Success)
            {
                string tmpPubNo = mhpubno.Groups["pubno"].Value.Trim();
                tmpPubNo = Regex.Replace(tmpPubNo, "(\\s{2,}|\n|\\(|\\))", "");
                PubDate = mhpubno.Groups["pubdate"].Value.Trim();
            }

            Regex regipc = new Regex("international\\:</th>(\n|\t|\r|\\s)*<td\\sclass=\"printTableText\">(\n|\t|\r|\\s)*\\<i\\>\\<b\\>(?<ipc>(.|\\s)*?)</td>"); //[^<]*

            Match mhipc = regipc.Match(strResult);
            if (mhipc.Success)
            {
                IPC = mhipc.Groups["ipc"].Value.Trim();
                IPC = Regex.Replace(IPC, "<[^>]*>", "");
                IPC = Regex.Replace(IPC, "(\\t|\\n|\\r|\\+)+", "");
                IPC = Regex.Replace(IPC, "\\s{2,}", "");
            }
            //abs
            //
            Regex regabs = new Regex("printAbstract\">(?<abs>(.|\\s|\n|\t)*?)</p>");

            Match mhabs = regabs.Match(strResult);
            if (mhabs.Success)
            {
                ABS = mhabs.Groups["abs"].Value.Trim();
            }

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
                    PriorityNumber += prinum + " " + pridate + ";";
                }


            }
            return true;
        }

        private bool getClaims()
        {
            // string ClaimUrl = string.Format("http://worldwide.espacenet.com/publicationDetails/claims?CC={0}&NR={1}&FT=D", PubNo.Substring(0, 2), PubNo.Substring(2, PubNo.Length - 2));
            string ClaimUrl = string.Format("http://worldwide.espacenet.com/data/publicationDetails/claims?CC={0}&NR={1}&FT=D", PubNo.Substring(0, 2), PubNo.Substring(2, PubNo.Length - 2));
            string strResult = GetHtml(ClaimUrl);
            strResult = strResult.Replace("&amp;", "&").Replace("&nbsp;", "").Trim();
            if (string.IsNullOrEmpty(strResult) || strResult.IndexOf("Please enter the digits that can be read in the image below:") > 0)
            {
                throw new EPOException("对不起！服务器繁忙！！请稍候重试！！！");

            }
            Regex reg2 = new Regex("<div id=\"claims\" class=\"application article clearfix printTableText\">(\n|\\s|\t)*(?<xy>(.|\n)*)?(\n|\\s|\t|\r\n)</div>");
            //Regex reg = new Regex("<div\\sid=\"description\"\\sclass=\"application\\s{1,3}article\\s{1,3}clearfix\">(\t|\n|\\s)*<p>(?<xy>(.|\n)*)</p></div>");
            //Regex reg2 = new Regex("<p lang=\"(en|de|fr)\">(\n|\\s|\t)*(?<xy>(.|\n)*)(<br/>|<br>){1,2}(\n|\\s|\t|\r\n)</p>");

            Match mh2 = reg2.Match(strResult);
            if (mh2.Success)
            {
                Claims = mh2.Groups["xy"].Value.Replace("<br/>", Environment.NewLine);
                Claims = htmlTager.Replace(Claims, "");
            }
            else
            {
                Claims = "NO Claims";

            }
            return true;
        }

        private bool getDescription()
        {
            //string DesUrl = string.Format("http://worldwide.espacenet.com/publicationDetails/description?CC={0}&NR={1}&FT=D", PubNo.Substring(0, 2), PubNo.Substring(2, PubNo.Length - 2));
            string DesUrl = string.Format("http://worldwide.espacenet.com/data/publicationDetails/description?CC={0}&NR={1}&FT=D", PubNo.Substring(0, 2), PubNo.Substring(2, PubNo.Length - 2));

            string strResult = GetHtml(DesUrl);
            strResult = strResult.Replace("&amp;", "&").Replace("&nbsp;", "").Trim();
            if (string.IsNullOrEmpty(strResult) || strResult.IndexOf("Please enter the digits that can be read in the image below:") > 0)
            {
                throw new EPOException("对不起！服务器繁忙！！请稍候重试！！！");
            }
            Regex reg = new Regex("<div\\sid=\"description\"\\sclass=\"application\\s{1,3}article\\s{1,3}clearfix\">(\t|\n|\\s)*<p>(?<xy>(.|\n)*)</p></div>");
            //<p lang="en" class="printTableText">
            Regex reg2 = new Regex("<p lang=\"(en|de|fr)\" class=\"printTableText\">(?<xy>(.|\n)*)(<br/>|<br>){1,2}(\n|\\s|\t|\r\n)*</p>");
            Match mh = reg2.Match(strResult);
            if (mh.Success)
            {
                Description = mh.Groups["xy"].Value.Replace("<br/>", Environment.NewLine);
                Description = htmlTager.Replace(Description, "");
            }
            else
            {
                Description = "NO Description";
            }

            return true;
        }
        public override string ToString()
        {
            return string.Format("标题：{0}{12}公开号：{1}{12}公开日：{2}{12}发明人：{3}{12}申请人：{4}{12}申请号：{5}{12}申请日：{6}{12}优先权：{7}{12}IPC：{8}{12}摘要：{12}{11}{12}权利要求：{12}{9}{12}说明书：{12}{10}{12}", Title, PubNo, PubDate, Inventor, Applicant, AppNo, AppDate, PriorityNumber, IPC, Claims, Description, ABS, Environment.NewLine);
        }


        public string GetHtml(string url)
        {
            //string PorxyAddress = System.Configuration.ConfigurationManager.AppSettings["ProxyAddress"];
            //int ProxyPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ProxyPort"]);
            ////Console.WriteLine("解析得到权利要求说明书的URL");
            //WebProxy wp = new WebProxy(PorxyAddress, ProxyPort);
            //wp.Credentials = CredentialCache.DefaultCredentials;
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.GetEncoding("utf-8");
            string strResult = wc.DownloadString(url);



            //HttpWebRequest hwq = (HttpWebRequest)HttpWebRequest.Create(url);
            ////hwq.Proxy = wp;
            //hwq.Timeout = 1000 * 60;
            //hwq.Method = "GET";


            ////hwq.ContentType = "application/x-www-form-urlencoded";
            //hwq.AllowAutoRedirect = false;
            ////CookieContainer cookieCon = new CookieContainer();
            ////hwq.CookieContainer = cookieCon;
            ////hwq.ContentLength = 0;
            ////hwq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)";
            ////hwq.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, */*";
            ////hwq.KeepAlive = true;
            //WebResponse wrp = hwq.GetResponse();
            //Stream streamReceive = wrp.GetResponseStream();
            //Encoding encoding = Encoding.GetEncoding("utf-8");
            //StreamReader streamReader = new StreamReader(streamReceive, encoding);
            //string strResult = streamReader.ReadToEnd();
            if (string.IsNullOrEmpty(strResult))
            {
                return "reject";
            }
            strResult = strResult.Replace("&amp;", "&").Replace("&nbsp;", "").Replace("&lt;", "<").Replace("&gt;", ">");
            if (strResult.IndexOf("Please enter the digits that can be read in the image below:") > 0)
            {
                return "reject";
            }
            return strResult;
        }
    }
    [global::System.Serializable]
    public class EPOException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public EPOException() { }
        public EPOException(string message) : base(message) { }
        public EPOException(string message, Exception inner) : base(message, inner) { }
        protected EPOException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    public class ixy : IWebProxyScript
    {
        #region IWebProxyScript 成员

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool Load(Uri scriptLocation, string script, Type helperType)
        {
            throw new NotImplementedException();
        }

        public string Run(string url, string host)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
