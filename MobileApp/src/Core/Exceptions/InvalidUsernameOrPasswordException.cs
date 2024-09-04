using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class InvalidUsernameOrPasswordException : Exception
    {
        public InvalidUsernameOrPasswordException(string message ) :base(message)
        {
            
        }
    }


}
