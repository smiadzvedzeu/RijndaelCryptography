using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RijndaelCryptography.Framework.Source
{
    public class StringCipherCrypt
    {
        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private readonly string _initVector;

        // This constant is used to determine the keysize of the encryption algorithm.
        private readonly int _keySize;

        public StringCipherCrypt(string initVector, int keySize)
        {
            _initVector = initVector;
            _keySize = keySize;
        }

        public string Encrypt(string plainText, string passPhrase)
        {
            try
            {
                byte[] initVectorBytes = Encoding.UTF8.GetBytes(_initVector);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
                byte[] keyBytes = password.GetBytes(_keySize / 8);
                RijndaelManaged symmetricKey = CreateSymmetricKey();
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                return HttpUtility.UrlEncode(Convert.ToBase64String(cipherTextBytes));
            }
            catch (Exception e)
            {
                //TODO We should use logger here, return null for now
                return null;
            }
        }

        private RijndaelManaged CreateSymmetricKey()
        {
            RijndaelManaged symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};
            return symmetricKey;
        }

        public string Decrypt(string cipherText, string passPhrase)
        {
            try
            {
                string decodedCipherText = cipherText ?? String.Empty;
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(_initVector);
                byte[] cipherTextBytes = Convert.FromBase64String(decodedCipherText);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
                byte[] keyBytes = password.GetBytes(_keySize / 8);
                RijndaelManaged symmetricKey = CreateSymmetricKey();
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
            catch (Exception e)
            {
                //TODO We should use logger here, return null for now
                return null;
            }
        }
    }
}
