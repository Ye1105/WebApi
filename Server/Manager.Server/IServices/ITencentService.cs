using Manager.Core.Models.Logs;
using Manager.Core.Tencent;

namespace Manager.Server.IServices
{
    public interface ITencentService
    {
        /// <summary>
        /// 获取最新发送的腾讯Sms
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="sms"></param>
        /// <returns></returns>
        LogTencentSMS? FirstOrDefaultAsync(string phone, string sms);

        /// <summary>
        /// 是否超过当日的腾讯Sms数量上限
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="upperCount"></param>
        /// <returns></returns>
        bool ExceedUpSmsDayLimitCount(string phone, int upperCount = 10);

        /// <summary>
        /// 是否存在此时间之后发送的sms
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="afterTime"></param>
        /// <returns></returns>
        bool FirstOrDefaultAsync(string phone, DateTime afterTime);

        /// <summary>
        /// 是否存在此时间之后发送的sms
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="sms"></param>
        /// <param name="afterTime"></param>
        /// <param name="beforeTime"></param>
        /// <returns></returns>
        bool ExsitAsync(string phone, string sms, DateTime afterTime, DateTime? beforeTime);

        /// <summary>
        /// 发送腾讯Sms
        /// </summary>
        /// <param name="tencentSendSmsConfig"></param>
        /// <returns></returns>
        Task<(bool, object?)> AddAsync(TencentSendSmsConfig tencentSendSmsConfig);
    }
}