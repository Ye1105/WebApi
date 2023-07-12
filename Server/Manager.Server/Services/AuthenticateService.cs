using Manager.Core.AuthorizationModels;
using Manager.Core.RequestModels;
using Manager.Core.Settings;
using Manager.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Manager.Server.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IOptions<AppSettings> appSettings;

        public AuthenticateService(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings;
        }

        public bool IsAuthenticated(AuthenticateRequest request, out string assessToken, out string refreshToken)
        {
            assessToken = string.Empty;
            refreshToken = string.Empty;

            try
            {
                var claims = new[]
                {
                    new Claim(Policys.Id,request.Id.Str()),
                    new Claim(Policys.UId,request.UId.Str()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.JwtBearer.SecurityKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var date = DateTime.Now;

                //生成accessToken
                var jwtAssessToken = new JwtSecurityToken(
                    appSettings.Value.JwtBearer.Issuer,
                    appSettings.Value.JwtBearer.Audience,
                    claims,
                    expires: date.AddMinutes(appSettings.Value.JwtBearer.AccessExpiration),
                    //expires: DateTime.Now.AddSeconds(10),
                    signingCredentials: credentials);
                assessToken = new JwtSecurityTokenHandler().WriteToken(jwtAssessToken);

                var keyRefresh = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.JwtBearer.RefreshSecurityKey));
                var credentialsRefresh = new SigningCredentials(keyRefresh, SecurityAlgorithms.HmacSha256);
                //生成refreshToken
                var jwtRefreshToken = new JwtSecurityToken(
                    appSettings.Value.JwtBearer.Issuer,
                    appSettings.Value.JwtBearer.Audience,
                    claims,
                    //expires: DateTime.Now.AddSeconds(60),
                    expires: date.AddMinutes(appSettings.Value.JwtBearer.RefreshExpiration),
                    signingCredentials: credentialsRefresh);
                refreshToken = new JwtSecurityTokenHandler().WriteToken(jwtRefreshToken);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("IsAuthenticated error : {0}", ex);
                return false;
            }
        }
    }
}