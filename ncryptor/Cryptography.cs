using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ncryptor
{
    internal class Cryptography
    {

        private const string HEADER = "ncryptr";

        internal static bool IsEncrypted(FileStream fs)
        {
            var b = new byte[HEADER.Length];
            fs.Read(b, 0, b.Length);
            fs.Seek(0, SeekOrigin.Begin);

            return Encoding.ASCII.GetString(b) == HEADER;
        }

        internal static long GetLength(FileStream fs)
        {
            var b = new byte[8];
            fs.Seek(HEADER.Length, SeekOrigin.Begin);
            fs.Read(b, 0, b.Length);
            fs.Seek(0, SeekOrigin.Begin);

            return BitConverter.ToInt64(b, 0);
        }

        internal static void DecryptFile(string sourceFilename, string destinationFilename, string password, RSACryptoServiceProvider rsa)
        {
            using (var aes = new AesManaged())
            {
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                using (var source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    //read header
                    source.Seek(HEADER.Length, SeekOrigin.Begin);

                    //read unencrypted file length
                    var lengthBytes = new byte[8];
                    source.Read(lengthBytes, 0, lengthBytes.Length);

                    //read salt
                    var salt = new byte[32];
                    source.Read(salt, 0, salt.Length);

                    //read iterations
                    var iterationBytes = new byte[4];
                    source.Read(iterationBytes, 0, iterationBytes.Length);
                    var iterations = BitConverter.ToInt32(iterationBytes, 0);

                    using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
                    {
                        aes.Key = deriveBytes.GetBytes(aes.KeySize / 8);
                        aes.IV = deriveBytes.GetBytes(aes.BlockSize / 8);
                        aes.Mode = CipherMode.CBC;

                        //byte[] keyEncrypted = null;
                        //if (rsa != null)
                        //{
                        //    var keyFormatter = new RSAPKCS1KeyExchangeFormatter(rsa);
                        //    keyEncrypted = keyFormatter.CreateKeyExchange(aes.Key).Take(aes.Key.Length).ToArray();
                        //}
                        //else
                        //{
                        //    keyEncrypted = aes.Key;
                        //}

                        var transform = aes.CreateDecryptor(aes.Key, aes.IV);

                        using (var destination = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                        {
                            using (var cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
                            {
                                try
                                {
                                    source.CopyTo(cryptoStream);
                                }
                                catch (CryptographicException exception)
                                {
                                    if (exception.Message == "Padding is invalid and cannot be removed.")
                                        throw new ApplicationException("Universal Microsoft Cryptographic Exception (Not to be believed!)", exception);
                                    else
                                        throw;
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static CryptoStream DecryptFile(FileStream source, string password, RSACryptoServiceProvider rsa)
        {
            using (var aes = new AesManaged())
            {
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                //read header
                source.Seek(HEADER.Length, SeekOrigin.Begin);

                //read unencrypted file length
                var lengthBytes = new byte[8];
                source.Read(lengthBytes, 0, lengthBytes.Length);

                //read salt
                var salt = new byte[32];
                source.Read(salt, 0, salt.Length);

                //read iterations
                var iterationBytes = new byte[4];
                source.Read(iterationBytes, 0, iterationBytes.Length);
                var iterations = BitConverter.ToInt32(iterationBytes, 0);

                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
                {
                    aes.Key = deriveBytes.GetBytes(aes.KeySize / 8);
                    aes.IV = deriveBytes.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;

                    //byte[] keyEncrypted = null;
                    //if (rsa != null)
                    //{
                    //    var keyFormatter = new RSAPKCS1KeyExchangeFormatter(rsa);
                    //    keyEncrypted = keyFormatter.CreateKeyExchange(aes.Key).Take(aes.Key.Length).ToArray();
                    //}
                    //else
                    //{
                    //    keyEncrypted = aes.Key;
                    //}

                    var transform = aes.CreateDecryptor(aes.Key, aes.IV);

                    return new CryptoStream(source, transform, CryptoStreamMode.Read);
                }
            }
        }

        internal static void EncryptFile(string sourceFilename, string destinationFilename, string password, RSACryptoServiceProvider rsa)
        {
            var fileInfo = new FileInfo(sourceFilename);

            using (var aes = new AesManaged())
            {
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                using (var deriveBytes = new Rfc2898DeriveBytes(password, 32, 10000))
                {
                    var salt = deriveBytes.Salt;
                    aes.Key = deriveBytes.GetBytes(aes.KeySize / 8);
                    aes.IV = deriveBytes.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;

                    //byte[] keyEncrypted = null;
                    //if (rsa != null)
                    //{
                    //    var keyFormatter = new RSAPKCS1KeyExchangeFormatter(rsa);
                    //    keyEncrypted = keyFormatter.CreateKeyExchange(aes.Key).Take(aes.Key.Length).ToArray();
                    //}
                    //else
                    //{
                    //    keyEncrypted = aes.Key;
                    //}

                    var transform = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (var destination = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        using (var cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
                        {
                            using (var source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                //write header
                                var h = ASCIIEncoding.ASCII.GetBytes(HEADER);
                                destination.Write(h, 0, h.Length);

                                //write unencrypted file length
                                var l = BitConverter.GetBytes(fileInfo.Length);
                                destination.Write(l, 0, l.Length);

                                //write salt
                                destination.Write(salt, 0, salt.Length);

                                //write iterations
                                var i = BitConverter.GetBytes(10000);
                                destination.Write(i, 0, i.Length);

                                //write encrypted data
                                source.CopyTo(cryptoStream);
                            }
                        }
                    }
                }
            }
        }

        internal static void RSA(string destinationFilename)
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    File.WriteAllText(destinationFilename + ".key", Convert.ToBase64String(rsa.ExportCspBlob(true)));
                    File.WriteAllText(destinationFilename + ".pub", Convert.ToBase64String(rsa.ExportCspBlob(false)));
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}
