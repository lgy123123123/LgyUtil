using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using Quartz.Util;

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

        #region RSA加密解密Xml
        /// <summary>
        /// 获取RSA公钥和私钥，xml格式
        /// </summary>
        /// <returns>公钥，私钥</returns>
        public static (string, string) GetRSAKey_Xml()
        {
            (string publicKey, string privateKey) = ("", "");
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 2048;
                privateKey = rsa.ToXmlString(true);
                publicKey = rsa.ToXmlString(false);
            }
            return (publicKey, privateKey);
        }
        /// <summary>
        /// RSA公钥加密,xml格式
        /// </summary>
        /// <param name="data">需要加密的字符串</param>
        /// <param name="xmlPublicKey">xml公钥</param>
        /// <returns>返回RSA加密后的密文</returns>
        public static string RSAPublicEncrypt_Xml(string data, string xmlPublicKey)
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
        /// RSA私钥解密,xml格式
        /// </summary>
        /// <param name="data">需要解密的长字符串</param>
        /// <param name="xmlPrivateKey">xml私钥</param>
        /// <returns>返回RSA分段解密的明文</returns>
        public static string RSAPrivateDecrypt_Xml(string data, string xmlPrivateKey)
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

        #region RSA加密解密Pem
        /// <summary>
        /// 获取RSA公钥和私钥，pem格式
        /// </summary>
        /// <param name="isFormat">是否包含pem前缀后缀格式，默认不包含</param>
        /// <returns></returns>
        public static (string, string) GetRSAKey_Pem(bool isFormat = false)
        {
            (string publicKey, string privateKey) = ("", "");
            RsaKeyPairGenerator gen = new RsaKeyPairGenerator();
            gen.Init(new KeyGenerationParameters(new Org.BouncyCastle.Security.SecureRandom(), 2048));
            var keyPair = gen.GenerateKeyPair();

            #region 私钥生成
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetEncoded();
            privateKey = Convert.ToBase64String(serializedPrivateBytes);
            #endregion

            #region 公钥生成
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            publicKey = Convert.ToBase64String(serializedPublicBytes);
            #endregion

            if (isFormat)
            {
                publicKey = RsaFormatPem(publicKey, true);
                privateKey = RsaFormatPem(privateKey, false);
            }

            return (publicKey, privateKey);
        }
        /// <summary>
        /// 格式化公钥/私钥
        /// </summary>
        /// <param name="key">生成的公钥/私钥</param>
        /// <param name="isPublic">true:公钥 false:私钥</param>
        /// <returns>PEM格式的公钥/私钥</returns>
        private static string RsaFormatPem(string key, bool isPublic)
        {
            if (key.StartsWith("-----"))
                return key;
            else if (key.Contains(Environment.NewLine))
                return RsaFormatPemAddPerfixSuffix(key, isPublic);
            else
            {
                string result = string.Empty;
                int length = key.Length / 64;
                for (int i = 0; i < length; i++)
                {
                    int start = i * 64;
                    result = result + key.Substring(start, 64) + Environment.NewLine;
                }
                result = result + key.Substring(length * 64);
                result = RsaFormatPemAddPerfixSuffix(key, isPublic);
                return result;
            }
        }
        /// <summary>
        /// 加上pem格式的标准头和尾
        /// </summary>
        /// <param name="key">秘钥</param>
        /// <param name="isPublic">是否是公钥</param>
        /// <returns></returns>
        private static string RsaFormatPemAddPerfixSuffix(string key, bool isPublic)
        {
            string typeName = isPublic ? "PUBLIC" : "PRIVATE";
            key = key.Insert(0, $"-----BEGIN {typeName} KEY-----{Environment.NewLine}");
            key += $"{Environment.NewLine}-----END {typeName} KEY-----";
            return key;
        }
        /// <summary>
        /// rsa公钥加密，pem格式
        /// </summary>
        /// <param name="data">需要加密的字符串</param>
        /// <param name="pemPublicKey">pem公钥</param>
        /// <returns></returns>
        /// <exception cref="LgyUtilException"></exception>
        public static string RSAPublicEncrypt_Pem(string data, string pemPublicKey)
        {
            pemPublicKey = RsaFormatPem(pemPublicKey, true);//必须用标准pem格式进行加密解密
            StringReader sr = new StringReader(pemPublicKey);
            AsymmetricKeyParameter publickey = new PemReader(sr).ReadObject() as AsymmetricKeyParameter;

            if (publickey == null)
                throw new LgyUtilException("PEM格式公钥不能为空");

            try
            {
                var engine = new Pkcs1Encoding(new RsaEngine());
                engine.Init(true, publickey);

                byte[] bytes = Encoding.UTF8.GetBytes(data);
                bytes = engine.ProcessBlock(bytes, 0, bytes.Length);

                return Convert.ToBase64String(bytes);
            }
            catch
            {
                throw new LgyUtilException("加密失败");
            }
        }
        /// <summary>
        /// rsa私钥解密，pem格式
        /// </summary>
        /// <param name="data">需要解密的字符串</param>
        /// <param name="pemPrivateKey">pem私钥</param>
        /// <returns></returns>
        public static string RSAPrivateDecrypt_Pem(string data, string pemPrivateKey)
        {
            pemPrivateKey = RsaFormatPem(pemPrivateKey, false);//必须用标准pem格式进行加密解密
            byte[] bytes = Convert.FromBase64String(data);
            StringReader sr = new StringReader(pemPrivateKey);
            AsymmetricKeyParameter prikey = new PemReader(sr).ReadObject() as AsymmetricKeyParameter;

            if (prikey == null)
                throw new LgyUtilException("私钥读取失败");

            try
            {
                var engine = new Pkcs1Encoding(new RsaEngine());
                engine.Init(false, prikey);
                bytes = engine.ProcessBlock(bytes, 0, bytes.Length);

                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                throw new LgyUtilException("解密失败");
            }
        }
        #endregion

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
