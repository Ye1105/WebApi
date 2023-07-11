using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Manager.Extensions
{
    public static class JsonHelper
    {
        /// <summary>
        /// object => string
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public static string SerObj(this object? obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string SerJArray(JArray obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// string => object
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static object DesObj(this string str)
        {
            return JsonConvert.DeserializeObject(str);
        }

        /// <summary>
        /// string => T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T? DesObj<T>(this string str) where T : class
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// string => dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDic<TKey, TValue>(this string str)
        {
            var dic = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(str);
            return dic;
        }

        /// <summary>
        /// 读取 Json 文件，返回 string
        /// </summary>
        /// <param name="filePath">json文件路径</param>
        /// <returns></returns>
        public static async Task<string> ReadJsonFile(string filePath)
        {
            byte[] result;
            using (FileStream SourceStream = File.Open(filePath, FileMode.Open))
            {
                result = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
            }
            return System.Text.Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// 读取 Json 文件，返回 JObject?
        /// </summary>
        /// <param name="filePath">json文件路径</param>
        /// <returns></returns>
        public static async Task<JObject?> ReadJsonFileToJObjectAsync(string filePath)
        {
            byte[] result;
            using (FileStream SourceStream = File.Open(filePath, FileMode.Open))
            {
                result = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
            }
            if (result.Length > 0)
            {
                return (JObject)JsonConvert.DeserializeObject(System.Text.Encoding.UTF8.GetString(result))!;
            }
            else
                return null;
        }

        /// <summary>
        /// 读取 Json 文件，返回 JObject?
        /// </summary>
        /// <param name="filePath">json文件路径</param>
        /// <returns></returns>
        public static JObject? ReadJsonFileToJObjectSync(string filePath)
        {
            byte[] result;
            using (FileStream SourceStream = File.Open(filePath, FileMode.Open))
            {
                result = new byte[SourceStream.Length];
                SourceStream.Read(result, 0, (int)SourceStream.Length);
            }
            if (result.Length > 0)
            {
                return (JObject)JsonConvert.DeserializeObject(System.Text.Encoding.UTF8.GetString(result))!;
            }
            else
                return null;
        }
    }
}