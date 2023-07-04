using Manager.Core.Models.Logs;
using Manager.Core.Tencent;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using MySqlConnector;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;

namespace Manager.Server.Services
{
    public class TencentService : ITencentService
    {
        private readonly IBase baseService;

        public TencentService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public bool ExceedUpSmsDayLimitCount(string phone, int upperCount = 10)
        {
            var date = Convert.ToDateTime(DateTime.Now.ToString("D").ToString());

            MySqlParameter[] mySqlParameter =
            {
                new MySqlParameter("@code","Ok"),
                new MySqlParameter("@phone",$"+86{phone}"),
                new MySqlParameter("@created",date)
            };

            var res = baseService.ExecuteQuery<LogTencentSMS>(
                "Select * from log_tencent_sms where JSON_CONTAINS(SendStatusSet,JSON_OBJECT('Code',@code)) and JSON_CONTAINS(SendStatusSet, JSON_OBJECT('PhoneNumber',@phone)) and  Created>=@created",
                false,
                mySqlParameter
            ).ToList();

            return res != null && res.Count() > 0 && (res.Count >= upperCount);
        }

        public LogTencentSMS? GetTencentSms(string phone, string sms)
        {
            MySqlParameter[] mySqlParameter =
            {
                new MySqlParameter("@code","Ok"),
                new MySqlParameter("@sms",sms),
                new MySqlParameter("@phone",$"+86{phone}"),
            };

            var res = baseService.ExecuteQuery<LogTencentSMS>(
                "Select * from log_tencent_sms where JSON_CONTAINS(SendStatusSet,JSON_OBJECT('Code',@code)) and JSON_CONTAINS(SendStatusSet, JSON_OBJECT('PhoneNumber',@phone)) and  Sms = @sms order by Created desc limit 1",
                false,
                mySqlParameter
            );

            return res.FirstOrDefault();
        }

        public bool GetTencentSms(string phone, DateTime afterTime)
        {
            MySqlParameter[] mySqlParameter =
            {
                new MySqlParameter("@code","Ok"),
                new MySqlParameter("@phone",$"+86{phone}"),
                new MySqlParameter("@created",afterTime)
            };

            var res = baseService.ExecuteQuery<LogTencentSMS>(
                "Select * from log_tencent_sms where JSON_CONTAINS(SendStatusSet,JSON_OBJECT('Code',@code)) and JSON_CONTAINS(SendStatusSet, JSON_OBJECT('PhoneNumber',@phone)) and  Created>=@created limit 1",
                false,
                mySqlParameter
            );

            return res != null && res.Count() > 0;
        }

        public bool GetTencentSms(string phone, string sms, DateTime afterTime, DateTime? beforeTime)
        {
            MySqlParameter[] mySqlParameter =
            {
                new MySqlParameter("@code","Ok"),
                new MySqlParameter("@phone",$"+86{phone}"),
                new MySqlParameter("@sms",sms),
                new MySqlParameter("@afterTime",afterTime),
                new MySqlParameter("@beforeTime",beforeTime==null?DateTime.Now:beforeTime)
            };

            var res = baseService.ExecuteQuery<LogTencentSMS>(
                "Select * from log_tencent_sms where JSON_CONTAINS(SendStatusSet,JSON_OBJECT('Code',@code)) and JSON_CONTAINS(SendStatusSet, JSON_OBJECT('PhoneNumber',@phone)) and  Sms = @sms and  Created>=@afterTime and Created<@beforeTime limit 1",
                false,
                mySqlParameter
            );

            return res != null && res.Count() > 0;
        }

        public async Task<(bool, object?)> SendSMS(TencentSendSmsConfig tencentSendSmsConfig)
        {
            var credential = new Credential
            {
                SecretId = tencentSendSmsConfig.SecretId,
                SecretKey = tencentSendSmsConfig.SecretKey
            };

            ClientProfile clientProfile = new();
            HttpProfile httpProfile = new()
            {
                Endpoint = ("sms.tencentcloudapi.com")
            };
            clientProfile.HttpProfile = httpProfile;

            var client = new SmsClient(credential, "ap-nanjing", clientProfile);
            var req = new SendSmsRequest
            {
                PhoneNumberSet = tencentSendSmsConfig.PhoneNumberSet,
                SmsSdkAppId = tencentSendSmsConfig.SmsSdkAppId,
                SignName = tencentSendSmsConfig.SignName,
                TemplateId = tencentSendSmsConfig.TemplateId,
                TemplateParamSet = tencentSendSmsConfig.TemplateParamSet
            };
            SendSmsResponse res = await client.SendSms(req);
            if (res != null)
            {
                var logTencentSMS = new LogTencentSMS()
                {
                    Sms = tencentSendSmsConfig.TemplateParamSet[0],
                    RequestId = Guid.Parse(res.RequestId),
                    SendStatusSet = res.SendStatusSet.SerObj(),
                    Created = DateTime.Now
                };
                await baseService.AddAsync(logTencentSMS);
                var resp = (dynamic)res.SendStatusSet;
                if (resp?[0].Code.ToLower() == "ok")
                {
                    return (true, "");
                }
                else
                    return (false, resp?[0]);
            }
            else
                return (false, "返回为空");
        }
    }
}