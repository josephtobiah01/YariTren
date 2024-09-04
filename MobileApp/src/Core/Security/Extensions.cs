using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Core.Security
{
    [DebuggerStepThrough]
    public static class Extensions
    {
        /// <summary>
        /// Converts a secure string into an insecure string
        /// </summary>
        /// <param name="secureString">The SecureString object to convert</param>
        /// <returns>Insecure open text string</returns>
        public static string ToInsecureString(this SecureString secureString)
        {
            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Converts an insecure string to a Secure string
        /// </summary>
        /// <param name="insecureString">The insecure string text to convert</param>
        /// <returns>A SecureString object</returns>
        public static SecureString ToSecureString(this string insecureString)
        {
            var secureString = new SecureString();
            insecureString.ToCharArray().ToList().ForEach(c => secureString.AppendChar(c));
            return secureString;
        }
    }
}
