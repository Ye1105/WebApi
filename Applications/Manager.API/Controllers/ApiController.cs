using Manager.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Manager.Core
{
    public class ApiController : ControllerBase
    {
        protected Dictionary<string, string>? UserClaims
        {
            get
            {
                var dicClaims = new Dictionary<string, string>();
                var claims = HttpContext.User?.Claims.AsEnumerable();
                if (claims is not null && claims.Any())
                {
                    foreach (var item in claims?.AsEnumerable() ?? new List<Claim>())
                    {
                        var k = item.Type;
                        var v = item.Value;
                        dicClaims[k] = v;
                    }
                }
                return dicClaims;
            }
        }

        /// <summary>
        /// context 上下文中的用户id
        /// </summary>
        protected Guid UId
        {
            get
            {
                var claims = HttpContext.User?.Claims.AsEnumerable();
                if (claims is not null && claims.Any())
                {
                    foreach (var item in claims?.AsEnumerable() ?? new List<Claim>())
                    {
                        if (item.Type == "uId")
                        {
                            return Guid.Parse(item.Value);
                        }
                    }
                    return Guid.Empty;
                }
                else
                    return Guid.Empty;
            }
        }

        public static ApiResultModel Res(ApiResultStatus status, string msg = "", string uimsg = "", object? data = null)
        {
            var res = new ApiResultModel()
            {
                Status = status,
                Msg = msg,
                Uimsg = uimsg,
                Data = data
            };
            return res;
        }

        public static ApiResultModel Success(string msg, string uimsg, object? data = null)
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.OK,
                Msg = msg,
                Uimsg = uimsg,
                Data = data
            };

            return res;
        }

        public static ApiResultModel Success(string detail, object? data = null)
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.OK,
                Msg = detail,
                Uimsg = detail,
                Data = data
            };

            return res;
        }

        public static ApiResultModel Success(object? data = null)
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.OK,
                Msg = "操作成功",
                Uimsg = "操作成功",
                Data = data
            };

            return res;
        }

        public static ApiResultModel Fail(object? errors = null, string? msg = "", string? uimsg = "")
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.BAD_REQUEST,
                Msg = msg,
                Uimsg = uimsg,
                Errors = errors
            };

            return res;
        }

        public static ApiResultModel Fail(object? errors = null, string? detail = "")
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.BAD_REQUEST,
                Msg = detail,
                Uimsg = detail,
                Errors = errors
            };

            return res;
        }

        public static ApiResultModel Fail(string? msg = "", string? uimsg = "")
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.BAD_REQUEST,
                Msg = msg,
                Uimsg = uimsg,
            };

            return res;
        }

        public static ApiResultModel Fail(string? detail = "")
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.BAD_REQUEST,
                Msg = detail,
                Uimsg = detail,
            };
            return res;
        }

        public static ApiResultModel Fail()
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.FAILED_DEPENDENCY,
                Msg = "操作失败",
                Uimsg = "操作失败",
            };
            return res;
        }

        public static ApiResultModel UnAuthorized(string? detail = "")
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.FORBIDDEN,
                Msg = detail,
                Uimsg = detail,
            };
            return res;
        }

        public static ApiResultModel UnAuthorized(object? errors = null, string? msg = "", string? uimsg = "")
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.FORBIDDEN,
                Msg = msg,
                Uimsg = uimsg,
                Errors = errors
            };

            return res;
        }
    }

    public class ApiResultModel
    {
        [JsonProperty("status")]
        public ApiResultStatus Status { get; set; }

        [JsonProperty("errors")]
        public object? Errors { get; set; }

        [JsonProperty("data")]
        public object? Data { get; set; }

        [JsonProperty("msg")]
        public string? Msg { get; set; } = "";

        [JsonProperty("uimsg")]
        public string? Uimsg { get; set; } = "";
    }
}