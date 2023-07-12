using Manager.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Manager.API.Utility.Schemas
{
    public class JsonSchemas
    {
        private static JsonSchemas? _instance;

        /// <summary>
        /// JsonSchema 配置路径
        /// </summary>
        private static string schemaPath = "";

        /// <summary>
        /// JsonSchema 配置信息
        /// </summary>
        private JObject? _schemas;

        public JsonSchemas()
        {
            schemaPath = AppDomain.CurrentDomain.BaseDirectory + @"jsonschemas.json";
        }

        /// <summary>
        /// 单例  Async
        /// </summary>
        /// <returns></returns>
        private static async Task Instance()
        {
            _instance ??= new JsonSchemas()
            {
                _schemas = await JsonHelper.ReadJsonFileToJObjectAsync(schemaPath)
            };
        }

        /// <summary>
        /// 单例  Sync
        /// </summary>
        private static void InstanceSync()
        {
            _instance ??= new JsonSchemas()
            {
                _schemas = JsonHelper.ReadJsonFileToJObjectSync(schemaPath)
            };
        }

        /// <summary>
        /// 获取规则
        /// </summary>
        /// <param name="webHostEnvironment">Web托管环境</param>
        /// <param name="key">Schema规则 wwwroot/schema/jsonSchemas.json中的key</param>
        /// <returns></returns>
        public static async Task<string> GetSchema(string key)
        {
            await Instance();
            return JsonConvert.SerializeObject(_instance!._schemas![key]);
        }

        /// <summary>
        /// 获取规则
        /// </summary>
        /// <param name="webHostEnvironment">Web托管环境</param>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string GetSchemaSync(string key)
        {
            InstanceSync();
            return JsonConvert.SerializeObject(_instance!._schemas![key]);
        }

        /// <summary>
        /// 初始化/重载配置
        /// </summary>
        public static void Init()
        {
            if (_instance != null)
            {
                _instance = null;
            }
        }
    }
}