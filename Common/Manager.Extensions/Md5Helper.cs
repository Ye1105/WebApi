using Org.BouncyCastle.Crypto.Digests;
using System.Security.Cryptography;
using System.Text;

namespace Manager.Extensions
{
    public class Md5Helper
    {
        /// <summary>
        /// 生成标准MD5字符
        /// </summary>
        /// <param name="Str">需要加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public static string MD5(string Str)
        {
            var data = Encoding.UTF8.GetBytes(Str);
            return Md5Encrypt(data);
        }

        public static string Md5Encrypt(byte[] data)
        {
            try
            {
                MD5Digest digest = new();
                digest.BlockUpdate(data, 0, data.Length);
                byte[] md5Buf = new byte[digest.GetDigestSize()];
                digest.DoFinal(md5Buf, 0);
                return BitConverter.ToString(md5Buf).Replace("-", "").ToLower();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #region "MD5加密"

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <param name="code">加密位数16/32</param>
        /// <returns></returns>

        public static string Encrypt(string str, int code)
        {
            string strEncrypt = string.Empty;
            if (code == 16)
            {
                strEncrypt = Hash(str).Substring(8, 16);
            }

            if (code == 32)
            {
                strEncrypt = Hash(str);
            }
            return strEncrypt;
        }

        /// <summary>
        /// 32位MD5加密（小写）
        /// </summary>
        /// <param name="input">输入字段</param>
        /// <returns></returns>

        public static string Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        #endregion "MD5加密"

        #region 使用MD5做签名

        /// <summary>
        /// 签名字符串
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>签名结果</returns>

        public static string Sign(string prestr, string key, string _input_charset = "utf-8")
        {
            StringBuilder sb = new(32);

            prestr += key;

            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }

        #endregion 使用MD5做签名
    }
}