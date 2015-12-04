using System.Configuration;
using System.IO;

namespace JiebaNet.Segmenter
{
    public class ConfigManager
    {
        public static string Root
        {
            get { return System.Web.HttpContext.Current.Server.MapPath("/App_Data"); }
        }

        public static string MainDictFile
        {
            get
            {
                return ConfigurationManager.AppSettings["MainDictFile"] ??
                    Path.Combine(Root, @"Resources\dict.txt");
            }
        }

        public static string ProbTransFile
        {
            get
            {
                var path = ConfigurationManager.AppSettings["ProbTransFile"] ?? Path.Combine(Root, @"Resources\prob_trans.json");
                return path;

            }
        }

        public static string ProbEmitFile
        {
            get
            {
                return ConfigurationManager.AppSettings["ProbEmitFile"] ??
                    Path.Combine(Root, @"Resources\prob_emit.json");
            }
        }

        public static string PosProbStartFile
        {
            get
            {
                return ConfigurationManager.AppSettings["PosProbStartFile"] ??
                       Path.Combine(Root, @"Resources\pos_prob_start.json");
            }
        }

        public static string PosProbTransFile
        {
            get
            {
                return ConfigurationManager.AppSettings["PosProbTransFile"] ??
                    Path.Combine(Root, @"Resources\pos_prob_trans.json");
            }
        }

        public static string PosProbEmitFile
        {
            get
            {
                return ConfigurationManager.AppSettings["PosProbEmitFile"] ??
                    Path.Combine(Root, @"Resources\pos_prob_emit.json");
            }
        }

        public static string CharStateTabFile
        {
            get
            {
                return ConfigurationManager.AppSettings["CharStateTabFile"]
                    ?? Path.Combine(Root, @"Resources\char_state_tab.json");
            }
        }
    }
}