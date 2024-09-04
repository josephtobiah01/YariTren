using System;
using SQLite;
namespace Core.Models
{
    public class DeviceInfo
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string GuidText { get; set; }
        public string DeviceTokenText { get; set; }
    }
}
