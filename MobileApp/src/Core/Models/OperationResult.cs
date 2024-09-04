using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class OperationResult
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; }
        public object Object { get; set; }
    }
}
