using Manager.Core.RequestModels;

namespace Manager.JwtAuthorizePolicy.IServices
{
    public interface IAuthenticateService
    {
        /// <summary>
        /// API 后端 生成 AccessToken 和 RefreshToken 接口
        /// </summary>
        /// <param name="request"></param>
        /// <param name="assessToken"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        bool IsAuthenticated(AuthenticateRequest request, out string assessToken, out string refreshToken);
    }
}