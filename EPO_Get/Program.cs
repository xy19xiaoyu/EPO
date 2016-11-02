using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Linq;


namespace EPO_Get
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("pubnolist.txt"))
            {
                using (StreamWriter log = new StreamWriter("pubnolist.log") { AutoFlush = true })
                {
                    int k = 0;
                    while (!sr.EndOfStream)
                    {
                        k = k + 1;
                        string pubno = sr.ReadLine().Trim();
                        try
                        {
                            EPODataInfo epo = new EPODataInfo(pubno);
                            epo.GetData();
                            string path = System.IO.Directory.GetCurrentDirectory() + "\\output\\" + epo.PubNo + "\\";
                            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                            using (StreamWriter sw = new StreamWriter(path + epo.PubNo + ".txt", false, Encoding.UTF8))
                            {
                                sw.WriteLine(epo.ToString());
                                Console.WriteLine(epo.PubNo + "\t ok" + "----" + k);
                            }
                        }
                        catch (Exception ex)
                        {
                            //log.WriteLine(pubno + Environment.NewLine + ex.ToString() + Environment.NewLine);
                            log.WriteLine(pubno);
                            Console.WriteLine(pubno + "\t -----错误" + "----" + k);
                        }

                        System.Threading.Thread.Sleep(1000 * 60 * 3);
                        //System.Threading.Thread.SpinWait(1000 * 60 * 50);                        
                    }
                }
            }
        }
    }
}
