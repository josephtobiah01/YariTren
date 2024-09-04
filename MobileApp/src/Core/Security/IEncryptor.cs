using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Core.Security
{
    public interface IEncryptor : IDisposable
    {
        string Encrypt(string data);

        string Decrypt(string data);
        string Encrypt(SecureString data);

        string Decrypt(SecureString data);
    }
}
