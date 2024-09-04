using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class SystemUnavailableException : Exception
    {
        public SystemUnavailableException(string message) : base(message)
        {

        }
    }
    
}
