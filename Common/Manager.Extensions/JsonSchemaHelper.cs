using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace Manager.Extensions
{
    public class JsonSchemaHelper
    {
        private static JSchemaGenerator instance;

        private JsonSchemaHelper()
        {
        }

        /// <summary>
        ///  Json Schema 校验
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="errorMessages"></param>
        /// <returns></returns>
        public static bool Validator<T>(T t, out IList<ValidationError> errorMessages)
        {
            JSchema schema = Instance().Generate(typeof(T));
            return JObject.Parse(t.SerObj()).IsValid(schema, out errorMessages);
        }

        /// <summary>
        ///  Json Schema 校验
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="errorMessages"></param>
        /// <returns></returns>
        public static bool Validator<T>(T t, out IList<string> errorMessages)
        {
            JSchema schema = Instance().Generate(typeof(T));
            return JObject.Parse(t.SerObj()).IsValid(schema, out errorMessages);
        }

        /// <summary>
        ///  Json Schema 校验
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool Validator<T>(T t)
        {
            JSchema schema = Instance().Generate(typeof(T));
            return JObject.Parse(t.SerObj()).IsValid(schema);
        }

        public static JSchemaGenerator Instance()
        {
            instance ??= new JSchemaGenerator();
            return instance;
        }
    }
}