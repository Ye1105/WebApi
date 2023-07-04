using COSSTS;
using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/coses")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class CosesController : ControllerBase
    {
        private readonly IOptions<AppSettings> appSettings;

        public CosesController(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings;
        }

        /// <summary>
        /// 获取腾讯COS临时权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("{number}/auths")]
        public IActionResult GetTencentAuthorization(TencentCosEnum number)
        {
            string[] allowActions = new string[] {  // 允许的操作范围，这里以上传操作为例
                    "name/cos:PutObject",
                    "name/cos:PutObject",
                    "name/cos:GetObject",
                    "name/cos:DeleteObject",
                    "name/cos:InitiateMultipartUpload",
                    "name/cos:ListMultipartUploads",
                    "name/cos:ListParts",
                    "name/cos:UploadPart",
                    "name/cos:CompleteMultipartUpload"
                };

            string allowPrefix = "*";

            #region Descrption

            // 这里改成允许的路径前缀，可以根据自己网站的用户登录态判断允许上传的具体路径，例子： a.jpg 或者 a/* 或者 * (使用通配符*存在重大安全风险, 请谨慎评估使用)
            // 也可以通过 allowPrefixes 指定路径前缀的集合
            // values.Add("allowPrefixes", new string[] {
            //     "path/to/dir1/*",
            //     "path/to/dir2/*",
            // });

            #endregion Descrption

            Dictionary<string, object> values = new();
            Dictionary<string, object> credential = new();

            switch (number)
            {
                case TencentCosEnum.Video:
                    var tencentCos = appSettings.Value.TencentCos;
                    values.Add("bucket", tencentCos.Bucket);
                    values.Add("region", tencentCos.Region);
                    values.Add("allowPrefix", allowPrefix);
                    values.Add("allowActions", allowActions);
                    values.Add("durationSeconds", 1800);
                    values.Add("secretId", tencentCos.SecretId);
                    values.Add("secretKey", tencentCos.SecretKey);
                    credential = STSClient.genCredential(values);

                    break;

                case TencentCosEnum.Picture:

                    var TencentCosTwo = appSettings.Value.TencentCosTwo;
                    values.Add("bucket", TencentCosTwo.Bucket);
                    values.Add("region", TencentCosTwo.Region);
                    values.Add("allowPrefix", allowPrefix);
                    values.Add("allowActions", allowActions);
                    values.Add("durationSeconds", 1800);
                    values.Add("secretId", TencentCosTwo.SecretId);
                    values.Add("secretKey", TencentCosTwo.SecretKey);
                    credential = STSClient.genCredential(values);

                    break;

                default:
                    break;
            }

            return Ok(ApiResult.Success(credential));
        }
    }
}