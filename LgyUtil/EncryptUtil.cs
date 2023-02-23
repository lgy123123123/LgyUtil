using Quartz.Util;
using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace LgyUtil
{
    /// <summary>
    /// DES算法加密
    /// </summary>
    public sealed class EncryptUtil
    {
        #region DES加密（对称加密）
        /// <summary>
        /// DES默认密钥向量(8位)
        /// </summary>
        static byte[] Keys = { 0x22, 0x34, 0x56, 0x32, 0x90, 0xAF, 0xCD, 0xBE };
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="data">待加密的字符串</param>
        /// <param name="key">加密密钥(8位)</param>
        /// <param name="vector">向量字符串，可不填写(8位)</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string data, string key, string vector = "")
        {
            if (key.Length != 8 || (vector.IsNotNullOrEmpty() && vector.Length != 8))
                throw new LgyUtilException("参数key和vector的长度，必须都为8");
            byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] rgbIV = vector.IsNullOrEmpty() ? Keys : Encoding.UTF8.GetBytes(vector);
            byte[] inputByteArray = Encoding.UTF8.GetBytes(data);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }


        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="data">待解密的字符串</param>
        /// <param name="key">解密密钥(8位)</param>
        /// <param name="vector">向量字符串(8位)</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string data, string key, string vector = "")
        {
            if (key.Length != 8 || (vector.IsNotNullOrEmpty() && vector.Length != 8))
                throw new LgyUtilException("参数key和vector的长度，必须都为8");
            if (data == "")
                return "";
            byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] rgbIV = vector.IsNullOrEmpty() ? Keys : Encoding.UTF8.GetBytes(vector);
            byte[] inputByteArray = Convert.FromBase64String(data);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        #endregion

        #region AES加密（对称加密）
        /// <summary>
        /// AES默认密钥向量(16位)
        /// </summary>
        static byte[] Keys2 = { 0x22, 0x34, 0x56, 0x32, 0x90, 0xAF, 0xCD, 0xBE, 0x22, 0x34, 0x56, 0x32, 0x90, 0xAF, 0xCD, 0xBE };
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="data">被加密的明文</param>
        /// <param name="key">密钥(16位)</param>
        /// <param name="vector">向量(16位)</param>
        /// <returns>密文</returns>
        public static string AESEncrypt(string data, string key, string vector = "")
        {
            if (key.Length != 16 || (vector.IsNotNullOrEmpty() && vector.Length != 16))
                throw new LgyUtilException("参数key和vector的长度，必须都为16");
            Byte[] plainBytes = Encoding.UTF8.GetBytes(data);

            byte[] bKey = Encoding.UTF8.GetBytes(key);
            byte[] bVector = vector.IsNullOrEmpty() ? Keys2 : Encoding.UTF8.GetBytes(vector);

            Byte[] Cryptograph = null; // 加密后的密文

            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流
                using (MemoryStream Memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Encryptor = new CryptoStream(Memory,
                    Aes.CreateEncryptor(bKey, bVector),
                    CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流
                        Encryptor.Write(plainBytes, 0, plainBytes.Length);
                        Encryptor.FlushFinalBlock();

                        Cryptograph = Memory.ToArray();
                    }
                }
            }
            catch
            {
                Cryptograph = null;
            }
            return Convert.ToBase64String(Cryptograph).Replace("/", "_").Replace("+", "-");
        }


        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="data">被解密的密文</param>
        /// <param name="key">密钥(16位)</param>
        /// <param name="vector">向量(16位)</param>
        /// <returns>明文</returns>
        public static string AESDecrypt(string data, string key, string vector = "")
        {
            if (data.IsNullOrEmpty())
                return "";
            if (key.Length != 16 || (vector.IsNotNullOrEmpty() && vector.Length != 16))
                throw new LgyUtilException("参数key和vector的长度，必须都为16");
            data = data.Replace("_", "/").Replace("-", "+");
            Byte[] encryptedBytes = Convert.FromBase64String(data);

            byte[] bKey = Encoding.UTF8.GetBytes(key);
            byte[] bVector = vector.IsNullOrEmpty() ? Keys2 : Encoding.UTF8.GetBytes(vector);
            Byte[] original = null; // 解密后的明文

            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流，存储密文
                using (MemoryStream Memory = new MemoryStream(encryptedBytes))
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Decryptor = new CryptoStream(Memory,
                    Aes.CreateDecryptor(bKey, bVector),
                    CryptoStreamMode.Read))
                    {
                        // 明文存储区
                        using (MemoryStream originalMemory = new MemoryStream())
                        {
                            Byte[] Buffer = new Byte[1024];
                            Int32 readBytes = 0;
                            while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                originalMemory.Write(Buffer, 0, readBytes);
                            }

                            original = originalMemory.ToArray();
                        }
                    }
                }
            }
            catch
            {
                original = null;
            }
            return Encoding.UTF8.GetString(original);
        }
        #endregion

        #region RSA加密（非对称加密）
        /// <summary>
        /// 获取RSA公钥和私钥
        /// </summary>
        /// <returns>公钥，私钥</returns>
        public static (string, string) GetRSAKey()
        {
            (string publicKey, string privateKey) = ("", "");
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                privateKey = rsa.ToXmlString(true);
                publicKey = rsa.ToXmlString(false);
            }
            return (publicKey, privateKey);
        }
        /// <summary>
        /// RSA公钥加密
        /// </summary>
        /// <param name="data">需要加密的字符串</param>
        /// <param name="xmlPublicKey">公钥</param>
        /// <returns>返回RSA加密后的密文</returns>
        public static String RSAPublicEncrypt(string data, string xmlPublicKey)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPublicKey);
            Byte[] bytes = Encoding.Default.GetBytes(data);
            int MaxBlockSize = provider.KeySize / 8 - 11;  //加密块最大长度限制

            if (bytes.Length <= MaxBlockSize)
                return Convert.ToBase64String(provider.Encrypt(bytes, false));

            using (MemoryStream PlaiStream = new MemoryStream(bytes))
            using (MemoryStream CrypStream = new MemoryStream())
            {
                Byte[] Buffer = new Byte[MaxBlockSize];
                int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);

                while (BlockSize > 0)
                {
                    Byte[] ToEncrypt = new Byte[BlockSize];
                    Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                    Byte[] Cryptograph = provider.Encrypt(ToEncrypt, false);
                    CrypStream.Write(Cryptograph, 0, Cryptograph.Length);

                    BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                }

                return Convert.ToBase64String(CrypStream.ToArray(), Base64FormattingOptions.None);
            }

        }
        /// <summary>
        /// RSA私钥解密
        /// </summary>
        /// <param name="data">需要解密的长字符串</param>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <returns>返回RSA分段解密的明文</returns>
        public static String RSAPrivateDecrypt(string data, string xmlPrivateKey)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPrivateKey);
            Byte[] bytes = Convert.FromBase64String(data);
            int MaxBlockSize = provider.KeySize / 8;  //解密块最大长度限制

            if (bytes.Length <= MaxBlockSize)
                return Encoding.Default.GetString(provider.Decrypt(bytes, false));

            using (MemoryStream CrypStream = new MemoryStream(bytes))
            using (MemoryStream PlaiStream = new MemoryStream())
            {
                Byte[] Buffer = new Byte[MaxBlockSize];
                int BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);

                while (BlockSize > 0)
                {
                    Byte[] ToDecrypt = new Byte[BlockSize];
                    Array.Copy(Buffer, 0, ToDecrypt, 0, BlockSize);

                    Byte[] Plaintext = provider.Decrypt(ToDecrypt, false);
                    PlaiStream.Write(Plaintext, 0, Plaintext.Length);

                    BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                }

                return Encoding.Default.GetString(PlaiStream.ToArray());
            }
        }
        #endregion

        /// <summary>
        /// 获取md5
        /// </summary>
        /// <param name="str">明文串</param>
        /// <param name="encoding">不填写是Utf8</param>
        /// <returns></returns>
        public static string GetMd5(string str, Encoding encoding = null)
        {
            using (MD5 mi = MD5.Create())
            {
                if (encoding is null)
                    encoding = Encoding.UTF8;
                byte[] buffer = encoding.GetBytes(str);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
