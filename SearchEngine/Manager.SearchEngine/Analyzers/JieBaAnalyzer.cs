using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Util;
using Manager.SearchEngine.Tokenizers;

namespace Manager.SearchEngine.Analyzers
{
    /// <summary>
    /// 分析器
    /// </summary>
    public class JieBaAnalyzer : Analyzer
    {
        private readonly TokenizerMode model;
        private readonly string userDictFile;

        /// <summary>
        /// Jieba分析器
        /// </summary>
        /// <param name="model">TokenizerMode:0 default 1 search</param>
        /// <param name="userDictFile">用户字典文件路径</param>
        public JieBaAnalyzer(TokenizerMode model,string userDictFile)
        {
            this.model = model;
            this.userDictFile = userDictFile;
        }

        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            var jieBaTokenizer = new JieBaTokenizer(reader, model, userDictFile);
            TokenStream tokenStream = new LowerCaseFilter(LuceneVersion.LUCENE_48, jieBaTokenizer);
            tokenStream.AddAttribute<ICharTermAttribute>();
            tokenStream.AddAttribute<IOffsetAttribute>();
            return new TokenStreamComponents(jieBaTokenizer, tokenStream);
        }
    }
}
