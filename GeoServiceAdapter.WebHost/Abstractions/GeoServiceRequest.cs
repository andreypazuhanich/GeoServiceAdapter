using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoServiceAdapter.WebHost.Abstractions
{
    public abstract class GeoServiceRequest
    {
        public string Location { get; set; }
        
        public int FrequencyPoints { get; set; }
        
        public string FileName { get; set; }
    }
}
