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
        public string JiebaSegment()
        {
            var segmenter = new JiebaSegmenter();
            
            var segments = segmenter.Cut("我来到北京清华大学", true);
            return segments.Join("/");
        }
    }
}
