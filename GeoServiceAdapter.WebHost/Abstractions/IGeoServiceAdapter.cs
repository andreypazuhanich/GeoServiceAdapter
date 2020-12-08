using GeoServiceAdapter.WebHost.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GeoServiceAdapter.WebHost
{
    public interface IGeoServiceAdapter<T> where T : GeoServiceRequest
    {
        Task<string> GetSerializedPoints(T request);
    }
}