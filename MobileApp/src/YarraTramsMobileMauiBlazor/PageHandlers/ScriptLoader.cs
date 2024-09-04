using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTramsMobileMauiBlazor.PageHandlers
{
    public class ScriptLoader
    {
        public static string LoadLoginScript()
        {
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("YarraTramsMobile.PageHandlers.LoginScript.js"))
            {
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        public static string LoadChangePasswordScript()
        {
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("YarraTramsMobile.PageHandlers.ChangePasswordScript.js"))
            {
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        public static string LoadEAPLoginScript()
        {
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("YarraTramsMobile.PageHandlers.EAPLoginScript.js"))
            {
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

    }
}
