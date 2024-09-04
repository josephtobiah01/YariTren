using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTramsMobileMauiBlazor.Messages
{
    public class PdfFileMessage : ValueChangedMessage<string>
    {
        public PdfFileMessage(string filePath) : base(filePath)
        {

        }
    }
}
