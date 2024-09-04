using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Core.Security
{
    public class AESEncryptionProvider : IEncryptor
    {
        private string _iv;
        private string _key;
        private int KeyLength = 16;
        private int BlockSize = 128;
        private int _iterations = 1000;
        private readonly byte[] _codeKey = { 11, 15, 44, 22, 32, 12, 68, 33, 111, 11, 10, 22, 12, 87, 27, 21, 11, 15, 44, 22, 32, 12, 68, 33, 111, 11, 10, 22, 12, 87, 27, 21 };
        private readonly byte[] _codeIv = { 10, 85, 15, 22, 91, 22, 68, 77, 12, 96, 103, 11, 116, 21, 51, 97, 10, 85, 15, 22, 91, 22, 68, 77, 12, 96, 103, 11, 116, 21, 51, 97 };
        private Aes _encryptor;

        public AESEncryptionProvider(string key, string iv)
        {
            InitialiseEncryptor(key, iv);
        }
        public AESEncryptionProvider(string key, string iv, int keyLength = 16, int blockSize = 128)
        {
            KeyLength = keyLength;
            BlockSize = blockSize;
            InitialiseEncryptor(key, iv);
        }

        private void InitialiseEncryptor(string key, string iv)
        {
            _key = key;
            _iv = iv;
            _encryptor = Aes.Create();
            _encryptor.BlockSize = BlockSize;
            _encryptor.Mode = CipherMode.CBC;
            _encryptor.Padding = PaddingMode.ISO10126;
            _encryptor.Key = new Rfc2898DeriveBytes(_key, _codeKey, _iterations, HashAlgorithmName.SHA1).GetBytes(KeyLength);
            _encryptor.IV = new Rfc2898DeriveBytes(_iv, _codeIv, _iterations, HashAlgorithmName.SHA1).GetBytes(KeyLength);
        }

        public string Encrypt(string data)
        {
            // We wont encrypt null values so just return the data
            if (string.IsNullOrEmpty(data))
                return data;

            byte[] encryptedData;
            using (var encryptor = _encryptor.CreateEncryptor())
            {
                // Convert the data to a byte array
                byte[] plainBytes = Encoding.Unicode.GetBytes(data);
                encryptedData = encryptor.TransformFinalBlock(plainBytes,
                                                 0, plainBytes.Length);
                return Convert.ToBase64String(encryptedData);
            }
        }

        public string Decrypt(string data)
        {
            //// We wont decrypt null values so just return the data
            if (string.IsNullOrEmpty(data))
                return data;
            
            // Convert base 64 encrypted string into byte array
            byte[] messageToDecrypt = Convert.FromBase64String(data);
            
            string decryptedText;
            using (var decryptor = _encryptor.CreateDecryptor())
            {
                byte[] decryptedBytes = decryptor.TransformFinalBlock(
                                                            messageToDecrypt,
                                                            0, messageToDecrypt.Length);
                decryptedText = Encoding.Unicode.GetString(decryptedBytes);
            }
            return decryptedText;
        }

        public string Encrypt(SecureString data)
        {
            return Encrypt(data?.ToInsecureString());
        }

        public string Decrypt(SecureString data)
        {
            return Decrypt(data?.ToInsecureString());
        }

        public void Dispose()
        {
            _encryptor?.Dispose();
        }
    }
}
