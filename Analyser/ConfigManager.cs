using System.Configuration;
using System.IO;

namespace JiebaNet.Analyser
{
    public class ConfigManager
    {
        public static string Root
        {
            get { return System.Web.HttpContext.Current.Server.MapPath("/App_Data"); }
        }

        public static string IdfFile
        {
            get
            {
                return ConfigurationManager.AppSettings["IdfFile"] ??
                    Path.Combine(Root, @"Resources\idf.txt");
            }
        }

        public static string StopWordsFile
        {
            get
            {
                return ConfigurationManager.AppSettings["StopWordsFile"] ??
                    Path.Combine(Root, @"Resources\stopwords.txt");
            }
        }
    }
}