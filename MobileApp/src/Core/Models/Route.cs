using System;
namespace Core.Models
{
    public class Route
    {
        public string Code { get; set; }
        public string Title { get; set; }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} - {1}", Code, Title);
            }
        }
    }
}
