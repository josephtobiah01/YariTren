using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTramsMobileMauiBlazor.Services
{
    public static class ServiceLocator
    {
        public static IServiceProvider? Current { get; set; }

        public static T GetService<T>() where T : class
        {
            return Current?.GetService<T>() ?? throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
        }
    }
}
