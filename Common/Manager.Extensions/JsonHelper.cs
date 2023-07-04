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
    }
}