using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Net;

namespace Core.Models
{
    public class UserCredential
    {
        public string Username { get; set; }
        public SecureString Password { get; set; }
        public string DigestValue { get; set; }
        public string AccessToken { get; set; }
    }
}
