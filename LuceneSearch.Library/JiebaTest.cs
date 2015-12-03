using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiebaNet.Segmenter;

namespace LuceneSearch.Library
{
    public class JiebaTest
    {
        public string JiebaSegment(String str)
        {
            var segmenter = new JiebaSegmenter();

            var segments = segmenter.Cut(str, false);
            return segments.Join("/");
        }
    }
}
