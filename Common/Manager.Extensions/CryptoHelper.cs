using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography;
using System.Text;

namespace Manager.Extensions
{
    public class CryptoHelper
    {
        private static readonly string _AES_SIV = "ASoib345n1f767gI";
        public static readonly string AesKey = "(&*@lq#94Yjc1105";

        //默认AES密钥向量
        private static readonly byte[] _AES_IV = { 0x12, 0x3c, 0xcc, 0x78, 0x90, 0x99, 0xCD, 0x1F, 0x12, 0x34, 0x56, 0xcb, 0x46, 0x10, 0xf1, 0xEF };

        #region AES加密和解密

        /// <summary>
        /// Aes加密
        /// </summary>
        /// <param name="plain"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static byte[] AesEncrypt(byte[] plain, string strKey)
        {
            try
            {
                PaddedBufferedBlockCipher aes = new(new CfbBlockCipher(new AesEngine(), 128), new Pkcs7Padding());
                var ivAndKey = new ParametersWithIV(new KeyParameter(Encoding.UTF8.GetBytes(strKey)), Encoding.UTF8.GetBytes(_AES_SIV));
                aes.Init(true, ivAndKey);
                return CipherData(aes, plain);
            }
            catch
            {
                return null;
            }
        }

        public static string AesEncrypt(string cipher, string strkey)
        {
            try
            {
                return Convert.ToBase64String(AesEncrypt(Encoding.UTF8.GetBytes(cipher), strkey));
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Aes解密
        /// </summary>
        /// <param name="cipher"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static byte[] AesDecrypt(byte[] cipher, string strKey)
        {
            try
            {
                PaddedBufferedBlockCipher aes = new(new CfbBlockCipher(new AesEngine(), 128), new Pkcs7Padding());
                var ivAndKey = new ParametersWithIV(new KeyParameter(Encoding.UTF8.GetBytes(strKey)), Encoding.UTF8.GetBytes(_AES_SIV));
                aes.Init(false, ivAndKey);
                return CipherData(aes, cipher);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string AesDecrypt(string cipher, string strkey)
        {
            try
            {
                return Encoding.UTF8.GetString(AesDecrypt(Convert.FromBase64String(cipher), strkey));
            }
            catch
            {
                return string.Empty;
            }
        }

        private static byte[] CipherData(PaddedBufferedBlockCipher cipher, byte[] data)
        {
            int minSize = cipher.GetOutputSize(data.Length);
            byte[] outBuf = new byte[minSize];
            int length1 = cipher.ProcessBytes(data, 0, data.Length, outBuf, 0);
            int length2 = cipher.DoFinal(outBuf, length1);
            int actualLength = length1 + length2;
            byte[] result = new byte[actualLength];
            Array.Copy(outBuf, 0, result, 0, result.Length);
            return result;
        }

        #endregion AES加密和解密

        #region RSA加密和解密

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="Data">原文</param>
        /// <param name="PublicKeyString">公钥</param>
        /// <param name="KeyType">密钥类型XML/PEM</param>
        /// <returns></returns>
        public static string RSAEncrypt(string Data, string PublicKeyString, string KeyType)
        {
            byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(Data);
            RSACryptoServiceProvider rsa = new();
            switch (KeyType)
            {
                case "XML":
                    rsa.FromXmlString(PublicKeyString);
                    break;

                case "PEM":
                    rsa = RSA_PEM.FromPEM(PublicKeyString);
                    break;

                default:
                    throw new Exception("不支持的密钥类型");
            }
            //加密块最大长度限制，如果加密数据的长度超过 秘钥长度/8-11，会引发长度不正确的异常，所以进行数据的分块加密
            int MaxBlockSize = rsa.KeySize / 8 - 11;
            //正常长度
            if (data.Length <= MaxBlockSize)
            {
                byte[] hashvalueEcy = rsa.Encrypt(data, false); //加密
                return System.Convert.ToBase64String(hashvalueEcy);
            }
            //长度超过正常值
            else
            {
                using MemoryStream PlaiStream = new(data);
                using MemoryStream CrypStream = new();
                Byte[] Buffer = new Byte[MaxBlockSize];
                int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                while (BlockSize > 0)
                {
                    Byte[] ToEncrypt = new Byte[BlockSize];
                    Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                    Byte[] Cryptograph = rsa.Encrypt(ToEncrypt, false);
                    CrypStream.Write(Cryptograph, 0, Cryptograph.Length);
                    BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                }
                return System.Convert.ToBase64String(CrypStream.ToArray(), Base64FormattingOptions.None);
            }
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="Data">密文</param>
        /// <param name="PrivateKeyString">私钥</param>
        /// <param name="KeyType">密钥类型XML/PEM</param>
        /// <returns></returns>
        public static string RSADecrypt(string Data, string PrivateKeyString, string KeyType)
        {
            RSACryptoServiceProvider rsa = new();
            switch (KeyType)
            {
                case "XML":
                    rsa.FromXmlString(PrivateKeyString);
                    break;

                case "PEM":
                    rsa = RSA_PEM.FromPEM(PrivateKeyString);
                    break;

                default:
                    throw new Exception("不支持的密钥类型");
            }
            int MaxBlockSize = rsa.KeySize / 8;    //解密块最大长度限制
            //正常解密
            if (Data.Length <= MaxBlockSize)
            {
                byte[] hashvalueDcy = rsa.Decrypt(System.Convert.FromBase64String(Data), false);//解密
                return Encoding.GetEncoding("UTF-8").GetString(hashvalueDcy);
            }
            //分段解密
            else
            {
                using MemoryStream CrypStream = new(System.Convert.FromBase64String(Data));
                using MemoryStream PlaiStream = new();
                Byte[] Buffer = new Byte[MaxBlockSize];
                int BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);

                while (BlockSize > 0)
                {
                    Byte[] ToDecrypt = new Byte[BlockSize];
                    Array.Copy(Buffer, 0, ToDecrypt, 0, BlockSize);

                    Byte[] Plaintext = rsa.Decrypt(ToDecrypt, false);
                    PlaiStream.Write(Plaintext, 0, Plaintext.Length);
                    BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                }
                string output = Encoding.GetEncoding("UTF-8").GetString(PlaiStream.ToArray());
                return output;
            }
        }

        /// <summary>
        /// 取得私钥和公钥 XML 格式,返回数组第一个是私钥,第二个是公钥.
        /// </summary>
        /// <param name="size">密钥长度,默认1024,可以为2048</param>
        /// <returns></returns>
        public static string[] CreateXmlKey(int size = 1024)
        {
            //密钥格式要生成pkcs#1格式的  而不是pkcs#8格式的
            RSACryptoServiceProvider sp = new(size);
            string privateKey = sp.ToXmlString(true);//private key
            string publicKey = sp.ToXmlString(false);//public  key
            return new string[] { privateKey, publicKey };
        }

        /// <summary>
        /// 取得私钥和公钥 CspBlob 格式,返回数组第一个是私钥,第二个是公钥.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string[] CreateCspBlobKey(int size = 1024)
        {
            //密钥格式要生成pkcs#1格式的  而不是pkcs#8格式的
            RSACryptoServiceProvider sp = new(size);
            string privateKey = System.Convert.ToBase64String(sp.ExportCspBlob(true));//private key
            string publicKey = System.Convert.ToBase64String(sp.ExportCspBlob(false));//public  key

            return new string[] { privateKey, publicKey };
        }

        /// <summary>
        /// 导出PEM PKCS#1格式密钥对，返回数组第一个是私钥,第二个是公钥.
        /// </summary>
        public static string[] CreateKey_PEM_PKCS1(int size = 1024)
        {
            RSACryptoServiceProvider rsa = new(size);
            string privateKey = RSA_PEM.ToPEM(rsa, false, false);
            string publicKey = RSA_PEM.ToPEM(rsa, true, false);
            return new string[] { privateKey, publicKey };
        }

        /// <summary>
        /// 导出PEM PKCS#8格式密钥对，返回数组第一个是私钥,第二个是公钥.
        /// </summary>
        public static string[] CreateKey_PEM_PKCS8(int size = 1024, bool convertToPublic = false)
        {
            RSACryptoServiceProvider rsa = new(size);
            string privateKey = RSA_PEM.ToPEM(rsa, false, true);
            string publicKey = RSA_PEM.ToPEM(rsa, true, true);
            return new string[] { privateKey, publicKey };
        }

        #endregion RSA加密和解密
    }
}