namespace Manager.Extensions
{
    public static class CommonHelper
    {
        /// <summary>
        /// object => string
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string Str(this object o)
        {
            return Convert.ToString(o);
        }

        /// <summary>
        /// convert object to bool
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool ToBool(this object o)
        {
            return Convert.ToBoolean(o);
        }

        /// <summary>
        /// object => int
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static int Int(this object o)
        {
            if (int.TryParse(o.Str(), out int res))
            {
                return res;
            }
            return res;
        }

        /// <summary>
        /// object => sbyte
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static sbyte Sbyte(this object o)
        {
            if (sbyte.TryParse(o.Str(), out sbyte res))
            {
                return res;
            }
            return res;
        }

        /// <summary>
        /// 追加文本
        /// </summary>
        /// <param name="context">内容</param>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static async Task AppendTextAsync(this string context, string fileDirectory, string text)
        {
            /*
             * 1.判定Directory是否存在
             */
            CreateDirectory(fileDirectory);
            var path = Path.Combine(fileDirectory, text);
            using StreamWriter file = new(path, append: true);
            await file.WriteLineAsync(context);
        }

        /// <summary>
        /// 创建路径 Directory
        /// </summary>
        /// <param name="str"></param>
        public static void CreateDirectory(string str)
        {
            if (!new DirectoryInfo(str).Exists)
            {
                Directory.CreateDirectory(str);
            }
        }

        /// <summary>
        /// 校验 Dictionary 是否包含键名且键值不为null
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool DicValidate(this Dictionary<string, object> dic, string key)
        {
            return dic.ContainsKey(key) && dic[key] is not null;
        }

        /// <summary>
        /// 校验 Dictionary 是否包含键名且键值不为null
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool DicValidate(this Dictionary<string, object> dic, string key, out object value)
        {
            value = null;
            var res = dic.ContainsKey(key) && dic[key] is not null;
            if (res) value = dic[key];
            return res;
        }
    }
}