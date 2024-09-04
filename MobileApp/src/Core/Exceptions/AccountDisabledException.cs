using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class AccountDisabledException : Exception
    {
        public AccountDisabledException(string message) : base(message)
        {
        }
    }
}
