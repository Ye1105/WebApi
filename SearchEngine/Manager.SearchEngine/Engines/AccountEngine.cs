using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.JieBa;
using Lucene.Net.Documents;
using Lucene.Net.Documents.Extensions;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Manager.Core.Enums;
using Manager.Core.Models.Accounts;
using Manager.Core.Page;
using Manager.Core.RequestModels;
using Serilog;
using System.Text;
using static Lucene.Net.Documents.Field;

namespace Manager.SearchEngine.Engines
{
    public class AccountEngine : BaseEngine
    {
        private static AccountEngine? instance;

        private AccountEngine() : base()
        {

        }
        /// <summary>
        /// 单例模式
        /// </summary>
        /// <returns></returns>
        public static AccountEngine Instance()
        {
            instance ??= new AccountEngine();
            return instance;
        }

        /// <summary>
        /// 添加 account document
        /// </summary>
        /// <param name="t"></param>
        public void Add(Account t)
        {
            base.Add(t, (t) =>
            {
                var doc = new Document();
                doc.AddStringField("UId", t.UId.ToString(), Store.YES);
                doc.AddTextField("Name", t.Name, Store.YES);
                doc.AddTextField("Mail", t.Mail, Store.YES);
                doc.AddStringField("Created", t.Created?.ToString("yyyyMMddHHmmss"), Store.NO);
                doc.AddStringField("Status", t.Status.ToString(), Store.NO);
                doc.AddStringField("DocType", DocType.ACCOUNT.ToString(), Store.NO);
                return doc;
            });
        }

        public void Update(Account t)
        {
            base.Update(t,
            () => new Term("UId", t.UId.ToString()),
            (t) =>
            {
                var doc = new Document();
                doc.AddStringField("UId", t.UId.ToString(), Store.YES);
                doc.AddTextField("Name", t.Name, Store.YES);
                doc.AddTextField("Mail", t.Mail, Store.YES);
                doc.AddStringField("Created", t.Created?.ToString("yyyyMMddHHmmss"), Store.NO);
                doc.AddStringField("Status", t.Status.ToString(), Store.NO);
                doc.AddStringField("DocType", DocType.ACCOUNT.ToString(), Store.NO);
                return doc;
            });
        }

        public PagedList<Document>? Search(GetSearchAccountEngineRequest req, Func<Sort>? sortFunc = null)
        {
            try
            {
                return Search(
                           queryFunc: () =>
                           {
                               var VERSION = Lucene.Net.Util.LuceneVersion.LUCENE_48;

                               Analyzer analyzer = new Analyzers.JieBaAnalyzer(TokenizerMode.Search, base.jiebaConfigCustomDictDir);

                               QueryParser parser = new(VERSION, "", analyzer);

                               var stringBuilder = new StringBuilder();

                               if (string.IsNullOrWhiteSpace(req.Name))
                               {
                                   stringBuilder.Append($"Name:{req.Name}");
                               }


                               Query query = parser.Parse(stringBuilder.ToString());//+(title:lucene content:lucene)+time:[20060101 TO 20060130]

                               return query;
                           },
                            filterFunc: null,
                            n: 100,
                            pageIndex: req.PageIndex,
                            pageSize: req.PageSize,
                            offSet: req.OffSet
                        );
            }
            catch (Exception ex)
            {
                Log.Error("AccountEngine Search {0}", ex.ToString());
                return null;
            }
        }
    }
}
