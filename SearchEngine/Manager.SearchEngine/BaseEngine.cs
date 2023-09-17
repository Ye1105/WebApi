using JiebaNet.Segmenter;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Manager.Core.Page;
using Manager.Extensions;
using Manager.SearchEngine.Analyzers;
using System.Configuration;

namespace Manager.SearchEngine
{
    #region description

    /*
     * StringField:创建索引 但不分词
     * TextField:创建索引 也会分词
     * StoreField:一定会被存储，不创建索引，多用于创建各种数据类型的字段
     *
     * 多列
        MultiFieldQueryParser parse = new MultiFieldQueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, new string[] { "id","name" }, new JieBaAnalyzerExtend(TokenizerMode.Search));
        Query query= parse.Parse("***");
        booleanQuery.Add(query, Occur.MUST);

        TermQuery可以用“field:key”方式，例如“content:lucene”。

        BooleanQuery中‘与’用‘+’，‘或’用‘| ’，例如“content:java contenterl”。

        WildcardQuery仍然用‘?’和‘*’，例如“content:use*”。

        PhraseQuery用‘~’，例如“content:"中日"~5”。

        PrefixQuery用‘*’，例如“中*”。

        FuzzyQuery用‘~’，例如“content: wuzza ~”。

        RangeQuery用‘[]’或‘{}’，前者表示闭区间，后者表示开区间，例如“time:[20060101 TO 20060130]”，注意TO区分大小写。

        QueryParser parser = new QueryParser("content", newStandardAnalyzer());

        Query query = parser.parse("+(title:lucene content:lucene)+time:[20060101 TO 20060130]";
     *
     */

    #endregion description

    public abstract class BaseEngine
    {
        protected readonly string luceneDir;
        protected readonly string jiebaTextDir;

        public BaseEngine()
        {
            luceneDir = ConfigurationManager.AppSettings["LuceneDir"]?.ToString() ?? throw new ArgumentNullException("LuceneDir is null");
            jiebaTextDir = ConfigurationManager.AppSettings["JiebaTextDir"]?.ToString() ?? throw new ArgumentNullException("JiebaTextDir is null"); ;
        }

        /// <summary>
        /// 新增 Document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="funcDoc"> 返回新增的 Document </param>
        protected void Add<T>(T t, Func<T, Document> funcDoc) where T : class
        {
            try
            {
                //FSDirectory.Open 中封装了用来对索引目录进行的加锁方案，使得同一时间只能有一个IndexWriter对象对索引目录进行操作
                var directory = FSDirectory.Open(new DirectoryInfo(luceneDir));
                var VERSION = Lucene.Net.Util.LuceneVersion.LUCENE_48;
                var analyzer = new JieBaAnalyzer(TokenizerMode.Search, jiebaTextDir);
                var indexWriterConfig = new IndexWriterConfig(VERSION, analyzer);
                //设置打开方式：OpenMode.APPEND 会在索引库的基础上追加新的索引；OpenMode.CREATE 会清空原来的数据，再提交索引
                //indexWriterConfig.SetOpenMode(OpenMode.APPEND);
                //indexWriter创建索引写入器
                using var indexWriter = new IndexWriter(directory, indexWriterConfig);
                var document = funcDoc(t);
                //插入一条数据，即使已存在也插入
                indexWriter.AddDocument(document);
                //当修改、删除或插入数据的时候，如果短时间内没有关闭IndexWriter，可以使用commit来提交当前的更新，这样的话indexReader马上可以察觉到索引被更新。
                indexWriter.Commit();
            }
            catch (Exception ex)
            {
                //LockObtainFailedException 同时打开多个IndexWriter 就会报告异常
                Serilog.Log.Error("SearchEngine Add 异常【{0}】,参数值【{1}】", ex.ToString(), t.SerObj());
            }
        }

        /// <summary>
        /// 更新 Document 底层是先匹配 term 的所有 document 先删除，然后再创建新的。一般吧 term 限定为 id 比较合适
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="termFunc">the termFunc return to identify the document(s) to be deleted</param>
        /// <param name="funcDoc">返回需要修改的 Document</param>

        protected void Update<T>(T t, Func<Term> termFunc, Func<T, Document> funcDoc) where T : class
        {
            try
            {
                //FSDirectory.Open 中封装了用来对索引目录进行的加锁方案，使得同一时间只能有一个IndexWriter对象对索引目录进行操作
                var directory = FSDirectory.Open(new DirectoryInfo(luceneDir));
                var VERSION = Lucene.Net.Util.LuceneVersion.LUCENE_48;
                var analyzer = new JieBaAnalyzer(TokenizerMode.Search, jiebaTextDir);
                var indexWriterConfig = new IndexWriterConfig(VERSION, analyzer);
                //indexWriter创建索引写入器
                using var indexWriter = new IndexWriter(directory, indexWriterConfig);

                var document = funcDoc(t);

                var term = termFunc();

                indexWriter.UpdateDocument(term, document);
                indexWriter.Commit();
            }
            catch (Exception ex)
            {
                //LockObtainFailedException 同时打开多个IndexWriter 就会报告异常
                Serilog.Log.Error("SearchEngine Update 异常【{0}】,参数值【{1}】", ex.ToString(), t.SerObj());
            }
        }

        /// <summary>
        /// 删除 Document
        /// </summary>
        /// <param name="queryFunc"></param>
        protected void Delete(Func<Query> queryFunc)
        {
            try
            {
                //FSDirectory.Open 中封装了用来对索引目录进行的加锁方案，使得同一时间只能有一个IndexWriter对象对索引目录进行操作
                var directory = FSDirectory.Open(new DirectoryInfo(luceneDir));
                var VERSION = Lucene.Net.Util.LuceneVersion.LUCENE_48;
                var analyzer = new JieBaAnalyzer(TokenizerMode.Search, jiebaTextDir);
                var indexWriterConfig = new IndexWriterConfig(VERSION, analyzer);

                //indexWriter创建索引写入器
                using var indexWriter = new IndexWriter(directory, indexWriterConfig);

                //query
                var query = queryFunc();

                indexWriter.DeleteDocuments(query);
                indexWriter.Commit();
            }
            catch (Exception ex)
            {
                //LockObtainFailedException 同时打开多个IndexWriter 就会报告异常
                Serilog.Log.Error("SearchEngine Delete 异常【{0}】,参数值【{1}】", ex.ToString(), queryFunc());
            }
        }

        /// <summary>
        /// 删除所有所有 document
        /// </summary>
        protected void DeleteAll()
        {
            try
            {
                //FSDirectory.Open 中封装了用来对索引目录进行的加锁方案，使得同一时间只能有一个IndexWriter对象对索引目录进行操作
                var directory = FSDirectory.Open(new DirectoryInfo(luceneDir));
                var VERSION = Lucene.Net.Util.LuceneVersion.LUCENE_48;
                var analyzer = new JieBaAnalyzer(TokenizerMode.Search, jiebaTextDir);
                var indexWriterConfig = new IndexWriterConfig(VERSION, analyzer);
                //indexWriter创建索引写入器
                using var indexWriter = new IndexWriter(directory, indexWriterConfig);
                indexWriter.DeleteAll();
                indexWriter.Commit();
            }
            catch (Exception ex)
            {
                //LockObtainFailedException 同时打开多个IndexWriter 就会报告异常
                Serilog.Log.Error("SearchEngine Delete 异常【{0}】", ex.ToString());
            }
        }

        /// <summary>
        /// Search Order By Score
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="offSet">偏移量</param>
        /// <param name="queryFunc">lucene queryFunc 查询条件</param>
        /// <param name="filterFunc">lucene filterFunc 查询过滤器</param>
        /// <param name="n"> return only the top n results 默认:100</param>
        /// <returns></returns>
        protected PagedList<Document>? Search(Func<Query> queryFunc, Func<Filter>? filterFunc = null, int n = 100, int pageIndex = 1, int pageSize = 10, int offSet = 0)
        {
            try
            {
                var directory = FSDirectory.Open(new DirectoryInfo(luceneDir));
                var reader = DirectoryReader.Open(directory);
                var searcher = new IndexSearcher(reader);

                var query = queryFunc();

                //filter
                var filter = filterFunc is null ? null : filterFunc();

                //存放文档的收集器实例
                var collector = TopScoreDocCollector.Create(n, true);

                //默认搜索结果以Document.Score作为排序依据，该数值越大排名越靠前
                searcher.Search(query, filter, collector);

                //获得匹配的文档对象
                var docs = collector.GetTopDocs(0, collector.TotalHits).ScoreDocs;

                var sortDocs = docs?.Skip((pageIndex - 1) * pageSize + offSet).Take(pageSize);

                var res = new List<Document>();

                if (docs is not null && sortDocs is not null && sortDocs.Any())
                {
                    foreach (var d in sortDocs)
                    {
                        // a hit document's number
                        var docId = d.Doc;
                        var document = searcher.Doc(docId);
                        res.Add(document);
                    }

                    return PagedList<Document>.Create(res, docs.Length, pageIndex, pageSize, offSet);
                }

                return null;
            }
            catch (Exception ex)
            {
                //LockObtainFailedException 同时打开多个IndexWriter 就会报告异常
                Serilog.Log.Error("SearchEngine Search 异常【{0}】", ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="offSet">偏移量</param>
        /// <param name="queryFunc"> The query to search for</param>
        /// <param name="sortFunc">The Lucene.Net.Search.Sort object</param>
        /// <param name="n"> Return only the top n results 默认:100</param>
        /// <returns></returns>
        protected PagedList<Document>? Search(Func<Query> queryFunc, Func<Sort>? sortFunc = null, int n = 100, int pageIndex = 1, int pageSize = 10, int offSet = 0)
        {
            try
            {
                var directory = FSDirectory.Open(new DirectoryInfo(luceneDir));
                var reader = DirectoryReader.Open(directory);
                var searcher = new IndexSearcher(reader);

                //query
                var query = queryFunc();

                //排序
                var sort = sortFunc is null ? null : sortFunc();

                //默认搜索结果以Document.Score作为排序依据，该数值越大排名越靠前
                var docs = sortFunc is null ? searcher.Search(query, n).ScoreDocs : searcher.Search(query, n, sort).ScoreDocs;

                var sortDocs = docs?.Skip((pageIndex - 1) * pageSize + offSet).Take(pageSize);

                var res = new List<Document>();

                if (docs is not null && sortDocs is not null && sortDocs.Any())
                {
                    foreach (var d in sortDocs)
                    {
                        // a hit document's number
                        var docId = d.Doc;
                        var document = searcher.Doc(docId);
                        res.Add(document);
                    }

                    return PagedList<Document>.Create(res, docs.Length, pageIndex, pageSize, offSet);
                }
                return null;
            }
            catch (Exception ex)
            {
                //LockObtainFailedException 同时打开多个IndexWriter 就会报告异常
                Serilog.Log.Error("SearchEngine Search 异常【{0}】", ex.ToString());
                return null;
            }
        }
    }
}