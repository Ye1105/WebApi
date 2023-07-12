using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;

namespace Manager.SearchEngine.Tokenizers
{
    /// <summary>
    /// 分词器
    /// </summary>
    public class JieBaTokenizer : Tokenizer
    {
        private readonly List<JiebaNet.Segmenter.Token> _WordList = new();

        private string? _InputText;

        private ICharTermAttribute? termAtt;

        private IOffsetAttribute? offsetAtt;

        private IPositionIncrementAttribute? posIncrAtt;

        private ITypeAttribute? typeAtt;

        private JiebaSegmenter segmenter;

        private IEnumerator<JiebaNet.Segmenter.Token>? iter;

        private int start;

        private TokenizerMode mode;

        /// <summary>
        /// 分词器
        /// </summary>
        /// <param name="input">文本</param>
        /// <param name="model">TokenizerMode:0 default 1 search</param>
        /// <param name="userDictFile">附加用户自定义字典txt的文件路径</param>
        public JieBaTokenizer(TextReader input, TokenizerMode Mode, string userDictFile = "")
            : base(AttributeFactory.DEFAULT_ATTRIBUTE_FACTORY, input)
        {
            segmenter = new JiebaSegmenter();
            if (!string.IsNullOrWhiteSpace(userDictFile))
                segmenter.LoadUserDict(userDictFile);
            mode = Mode;
            Init();
        }

        private void Init()
        {
            termAtt = AddAttribute<ICharTermAttribute>();
            offsetAtt = AddAttribute<IOffsetAttribute>();
            posIncrAtt = AddAttribute<IPositionIncrementAttribute>();
            typeAtt = AddAttribute<ITypeAttribute>();
        }

        private static string ReadToEnd(TextReader input)
        {
            return input.ReadToEnd();
        }

        public override sealed bool IncrementToken()
        {
            ClearAttributes();

            var token = Next();
            if (token != null)
            {
                var s = token.ToString();
                termAtt?.SetEmpty().Append(s);
                offsetAtt?.SetOffset(CorrectOffset(token.StartOffset), CorrectOffset(token.EndOffset));
                if (typeAtt is not null)
                    typeAtt.Type = token.Type;
                return true;
            }

            End();
            Dispose();
            return false;
        }

        public Lucene.Net.Analysis.Token? Next()
        {
            int num = 0;
            if (iter is not null && iter.MoveNext())
            {
                JiebaNet.Segmenter.Token current = iter.Current;
                Lucene.Net.Analysis.Token result = new(current.Word, current.StartIndex, current.EndIndex);
                start += num;
                return result;
            }
            return null;
        }

        public override void Reset()
        {
            base.Reset();
            _InputText = ReadToEnd(m_input);
            var enumerable = segmenter.Tokenize(_InputText, mode);
            _WordList.Clear();
            foreach (JiebaNet.Segmenter.Token item in enumerable)
            {
                _WordList.Add(item);
            }

            start = 0;
            iter = _WordList.GetEnumerator();
        }
    }
}