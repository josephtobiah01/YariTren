using Core;
using Core.Security;

namespace SharedLogic
{
    public static class Encryptor
    {
        public static IEncryptor Get(string key)
        {
           return new AESEncryptionProvider(key, Consts.AppName);
        }
    }
}
