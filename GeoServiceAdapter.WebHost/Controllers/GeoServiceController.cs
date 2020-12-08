using System;
using GeoServiceAdapter.WebHost.Abstractions;
using GeoServiceAdapter.WebHost.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace GeoServiceAdapter.WebHost.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeoServiceController : ControllerBase
    {
        private readonly IGeoServiceAdapter<OpenStreetMapRequest> adapter;

        public GeoServiceController(IGeoServiceAdapter<OpenStreetMapRequest> adapter)
        {
            this.adapter = adapter;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string location, string fileName, int frequencyPoints)
        {
            if (frequencyPoints < 1)
                return BadRequest($"{nameof(frequencyPoints)} is lesser than 1");
            if (string.IsNullOrEmpty(location))
                return BadRequest($"{nameof(location)} должно быть заполнено");

            try
            {
                await adapter.GetPoligon(
                    new OpenStreetMapRequest()
                    {
                        Location = location,
                        FileName = fileName,
                        FrequencyPoints = frequencyPoints
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
    }
}
