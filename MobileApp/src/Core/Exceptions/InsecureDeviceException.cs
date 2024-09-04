using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Exceptions
{
    public class InsecureDeviceException : Exception
    {
        public InsecureDeviceException(string message) : base(message)
        {

        }
    }
}
