using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneSearch.Data.Model;
using Version = Lucene.Net.Util.Version;

namespace LuceneSearch.Library
{
    public static class GoLucene
    {
        public static string _luceneDir { get; set; }
            //LuceneDir ?? Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, @"App_Data\lucene_index");

        private static FSDirectory _directoryTemp;

        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null)
                {
                    if (!System.IO.Directory.Exists(_luceneDir))
                    {
                        System.IO.Directory.CreateDirectory(_luceneDir);
                    }
                    _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                }
                if (IndexWriter.IsLocked(_directoryTemp))
                {
                    IndexWriter.Unlock(_directoryTemp);
                }
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath))
                {
                    File.Delete(lockFilePath);
                }
                return _directoryTemp;
            }
        }

        public static Analyzer GetAnalyzer()
        {
            return new JiebaAnalyzer();
        }

        public static JiebaSegmenter GetSegmenter()
        {
            return new JiebaSegmenter();
        }

        #region 分词

        /// <summary>
        ///添加词典
        /// </summary>
        /// <param name="key"></param>
        public static void AddWords(string key)
        {
            GetSegmenter().AddWord(key);//添加输入词
            IEnumerable<string> keys = GetSegmenter().CutForSearch(key);
            foreach (var item in keys)
            {
                GetSegmenter().AddWord(item);
            }
        }
        #endregion

        #region Add & Update Index

        private static void AddToLuceneIndex(SampleData data, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term("Id", data.Id.ToString()));
            writer.DeleteDocuments(searchQuery);

            var doc = new Lucene.Net.Documents.Document();
            doc.Add(new Field("Id", data.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name", data.Name, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Description", data.Description, Field.Store.YES, Field.Index.ANALYZED));

            writer.AddDocument(doc);
        }

        public static void UpdateLuceneIndex(IEnumerable<SampleData> data)
        {
            var analyzer = GetAnalyzer();
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var sd in data)
                {
                    AddToLuceneIndex(sd, writer);
                }
                analyzer.Close();
            }
        }

        public static void UpdateLuceneIndex(SampleData data)
        {
            UpdateLuceneIndex(new[] { data });
        }
        #endregion

        #region Clear Index

        public static void ClearLuceneIndexRecord(int recordId)
        {
            var analyzer = GetAnalyzer();
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var searchQuery = new TermQuery(new Term("Id", recordId.ToString()));
                writer.DeleteDocuments(searchQuery);

                analyzer.Close();
            }
        }

        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = GetAnalyzer();
                using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    writer.DeleteAll();
                    analyzer.Close();
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Optimize Index

        public static void OptimizeLuceneIndex()
        {
            var analyzer = GetAnalyzer();
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
            }
        }
        #endregion

        #region Mappers

        private static SampleData MapDataToModel(Document doc)
        {
            return new SampleData()
            {
                Id = int.Parse(doc.Get("Id")),
                Name = doc.Get("Name"),
                Description = doc.Get("Description"),
            };
        }

        private static IEnumerable<SampleData> MapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(MapDataToModel).ToList();
        }

        private static IEnumerable<SampleData> MapLuceneToDataList(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => MapDataToModel(searcher.Doc(hit.Doc))).ToList();
        }
        #endregion

        #region Search

        private static string GetKeyWordsSplitBySpace(string keywords, JiebaTokenizer tokenizer)
        {
            var result = new StringBuilder();
            var words = tokenizer.Tokenize(keywords);
            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word.Word))
                {
                    continue;
                }
                result.AppendFormat("{0} ", word.Word);
            }
            return result.ToString().Trim();
        }

        private static Query ParseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException pe)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim() + "*"));
            }
            return query;
        }

        private static IEnumerable<SampleData> SearchQuery(string searchQuery, string searchField = "")
        {
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", "")))
            {
                return new List<SampleData>();
            }

            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hitsLimit = 1000;
                var analyzer = GetAnalyzer();
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Version.LUCENE_30, searchField, analyzer);
                    var query = ParseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hitsLimit).ScoreDocs;
                    var results = MapLuceneToDataList(hits, searcher);

                    analyzer.Dispose();
                    return results;
                }
                else
                {
                    var parser = new MultiFieldQueryParser(Version.LUCENE_30, new[] { "Id", "Name", "Description" }, analyzer);
                    var query = ParseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, null, hitsLimit, Sort.RELEVANCE).ScoreDocs;
                    var results = MapLuceneToDataList(hits, searcher);

                    analyzer.Close();
                    return results;
                }
            }
        }

        public static IEnumerable<SampleData> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<SampleData>();
            }

            var kwords = input;
            kwords = GetKeyWordsSplitBySpace(kwords, new JiebaTokenizer(new JiebaSegmenter(), kwords));
            var terms = kwords.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);
            return SearchQuery(input, fieldName);
        }

        public static IEnumerable<SampleData> SearchDefault(string input, string fieldName = "")
        {
            return string.IsNullOrEmpty(input) ? new List<SampleData>() : SearchQuery(input, fieldName);
        }
        #endregion

        public static IEnumerable<SampleData> GetAllData()
        {
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any())
            {
                return new List<SampleData>();
            }

            var searcher = new IndexSearcher(_directory, false);
            var reader = IndexReader.Open(_directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();

            while (term.Next())
            {
                docs.Add(searcher.Doc(term.Doc));
            }

            reader.Dispose();
            searcher.Dispose();
            return MapLuceneToDataList(docs);
        }

        public static IEnumerable<SampleData> GetData(int id)
        {
            return null;
        }
    }
}
