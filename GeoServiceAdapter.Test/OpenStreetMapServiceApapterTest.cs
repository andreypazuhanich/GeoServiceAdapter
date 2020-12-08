using System;
using System.IO;
using System.Net;
using System.Net.Http;
using GeoServiceAdapter.WebHost;
using GeoServiceAdapter.WebHost.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GeoServiceAdapter.Test
{
    public class OpenStreetMapServiceApapterTest
    {
        private readonly OpenStreetMapServiceAdapter serviceAdapter;

        public OpenStreetMapServiceApapterTest()
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://nominatim.openstreetmap.org"),
                Timeout = TimeSpan.FromMinutes(1)
            };
            serviceAdapter = new OpenStreetMapServiceAdapter(httpClient);
        }
        
        
        [Fact]
        public void ApiResponseTest()
        {
            var response =  serviceAdapter.SendAsync("губкин").Result;

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public void GetPoligonTest()
        {
            OpenStreetMapRequest request = new OpenStreetMapRequest
            {
                Location = "Черкасское поречное",
                FileName = "Test",
                FrequencyPoints = 4
            };

            
            var serializedPoints = serviceAdapter.GetSerializedPoints(request).Result;

            string testResult = null;
            if(File.Exists(@".\Files\test.txt"))
            {
                testResult = File.ReadAllText(@".\Files\test.txt");
            }
            
            
            var result = string.Equals(testResult.Replace(@"\s+",
                string.Empty).Trim(), serializedPoints.Replace(@"\s+", string.Empty).Trim());
            
            Assert.True(result);

        }
        
    }
}