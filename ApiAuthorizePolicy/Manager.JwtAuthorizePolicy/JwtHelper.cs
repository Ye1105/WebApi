using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Serilog;
using System.Text;

namespace Manager.JwtAuthorizePolicy
{
    /// <summary>
    /// Jwt的加密和解密
    /// 注：加密和加密用的是用一个密钥
    /// 依赖程序集：【JWT】
    /// </summary>

    public class JwtHelper
    {
        //https://blog.csdn.net/weixin_43950528/article/details/113846865

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="base64UrlStr"></param>
        /// <returns></returns>

        public static string Base64UrlDecode(string base64UrlStr)
        {
            base64UrlStr = base64UrlStr.Replace('-', '+').Replace('_', '/');
            switch (base64UrlStr.Length % 4)
            {
                case 2:
                    base64UrlStr += "==";
                    break;

                case 3:
                    base64UrlStr += "=";
                    break;
            }
            var bytes = Convert.FromBase64String(base64UrlStr);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// JWT解密算法
        /// </summary>
        /// <param name="token">需要解密的token串</param>
        /// <param name="secret">密钥</param>
        /// <returns></returns>
        [Obsolete]
        public static string JWTJieM(string token, string secret)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                var algorithm = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                var json = decoder.Decode(token, secret, true);
                //校验通过，返回解密后的字符串
                return json;
            }
            catch (TokenNotYetValidException ex)
            {
                Console.WriteLine("Token is not valid yet {0}", ex.ToString());
                //表示无效
                return "notvalid";
            }
            catch (TokenExpiredException ex)
            {
                Log.Error("JWTJieM TokenExpiredException {0}", ex.ToString());
                //表示过期
                return "expired";
            }
            catch (SignatureVerificationException ex)
            {
                Log.Error("JWTJieM SignatureVerificationException {0}", ex.ToString());
                //表示验证不通过
                return "invalid";
            }
            catch (Exception ex)
            {
                Log.Error("JWTJieM Exception {0}", ex.ToString());
                return "error";
            }
        }
    }
}