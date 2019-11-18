using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TIApi.Models
{
    [Serializable]
    public class JsonDevice
    {
        public string DeviceName { get; set; }

        public string LastWriteTime { get; set; }

        public string Id { get; set; }
    }
}
