using Manager.Core.Enums;
using Newtonsoft.Json;

namespace Manager.Core
{
    public class ApiResult
    {
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
                Status = ApiResultStatus.Success,
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
                Status = ApiResultStatus.Success,
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
                Status = ApiResultStatus.Success,
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
                Status = ApiResultStatus.Fail,
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
                Status = ApiResultStatus.Fail,
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
                Status = ApiResultStatus.Fail,
                Msg = msg,
                Uimsg = uimsg,
            };

            return res;
        }

        public static ApiResultModel Fail(string? detail = "")
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.Fail,
                Msg = detail,
                Uimsg = detail,
            };
            return res;
        }

        public static ApiResultModel Fail()
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.Fail,
                Msg = "操作失败",
                Uimsg = "操作失败",
            };
            return res;
        }

        public static ApiResultModel UnAuthorized(string? detail = "")
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.UnAuthorized,
                Msg = detail,
                Uimsg = detail,
            };
            return res;
        }

        public static ApiResultModel UnAuthorized(object? errors = null, string? msg = "", string? uimsg = "")
        {
            var res = new ApiResultModel()
            {
                Status = ApiResultStatus.UnAuthorized,
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