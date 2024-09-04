using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class TokenRetrievalException : Exception
    {
        public TokenRetrievalException(string message) : base(message)
        {

        }
    }
    
}
