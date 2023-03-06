using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace CORE_API.General
{
    public static class Logger
    {
        public static void TraceService(string content, string filename)
        {
            try
            {
                //set up a filestream
                //string basefilepath = HttpContext.Current.Server.MapPath("~/Logs/");
                //string loglocation = ConfigurationManager.AppSettings["LogFileLocation"];

                string basefilepath = ConfigurationManager.AppSettings["LogFileLocation"];

                string logfilename = Path.Combine(basefilepath, filename + "_" + DateTime.Now.ToString("ddMMyyyy") + ".txt");

                FileStream fs = new FileStream(@logfilename, FileMode.OpenOrCreate, FileAccess.Write);

                //set up a streamwriter for adding text
                StreamWriter sw = new StreamWriter(fs);

                //find the end of the underlying filestream
                sw.BaseStream.Seek(0, SeekOrigin.End);

                //add the text
                sw.WriteLine(DateTime.Now + " : " + content);
                //add the text to the underlying filestream

                sw.Flush();
                //close the writer
                sw.Close();
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
           
        }
    }
}