using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using JiebaNet.Segmenter;
using Lucene.Net.Analysis;

namespace LuceneSearch.Library
{
    public class JiebaAnalyzer : Analyzer
    {
        protected static readonly ISet<string> DefaultStopWords = StopAnalyzer.ENGLISH_STOP_WORDS_SET;
        private static ISet<string> StopWords;

        static JiebaAnalyzer()
        {
            var stopWordsFile = Path.GetFullPath(JiebaNet.Analyser.ConfigManager.StopWordsFile);
            if (File.Exists(stopWordsFile))
            {
                var lines = File.ReadAllLines(stopWordsFile);
                StopWords = new HashSet<string>();
                foreach (var line in lines)
                {
                    StopWords.Add(line.Trim());
                }
            }
            else
            {
                StopWords = DefaultStopWords;
            }
        }

        public override TokenStream TokenStream(string fieldName, System.IO.TextReader reader)
        {
            var seg = new JiebaSegmenter();
            TokenStream result = new JiebaTokenizer(seg, reader);

            result = new LowerCaseFilter(result);
            result = new StopFilter(true, result, StopWords);
            return result;
        }
    }
}
