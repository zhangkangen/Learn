using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace LuceneSearch.Library
{
    public class JiebaTokenizer : Tokenizer
    {
        private JiebaSegmenter segmenter;
        private ITermAttribute termAttribute;
        private IOffsetAttribute offsetAttribute;
        private ITypeAttribute typeAttribute;

        private List<JiebaNet.Segmenter.Token> tokens;
        private int position = -1;

        public JiebaTokenizer(JiebaSegmenter seg, TextReader input) : this(seg, input.ReadToEnd()) { }

        public JiebaTokenizer(JiebaSegmenter seg, string input)
        {
            segmenter = seg;
            termAttribute = AddAttribute<ITermAttribute>();
            offsetAttribute = AddAttribute<IOffsetAttribute>();
            typeAttribute = AddAttribute<ITypeAttribute>();

            var text = input;
            tokens = segmenter.Tokenize(text, TokenizerMode.Search).ToList();
        }

        public override bool IncrementToken()
        {
            ClearAttributes();
            position++;
            if (position<tokens.Count)
            {
                var token = tokens[position];
                termAttribute.SetTermBuffer(token.Word);
                offsetAttribute.SetOffset(token.StartIndex,token.EndIndex);
                typeAttribute.Type = "Jieba";
                return true;
            }
            End();
            return false;
        }

        public IEnumerable<JiebaNet.Segmenter.Token> Tokenize(string text, TokenizerMode mode = TokenizerMode.Search)
        {
            return segmenter.Tokenize(text, mode);
        } 
    }
}
