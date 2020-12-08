using GeoServiceAdapter.WebHost.Abstractions;
using GeoServiceAdapter.WebHost.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeoServiceAdapter.WebHost
{
    public class OpenStreetMapServiceAdapter : IGeoServiceAdapter<OpenStreetMapRequest>
    {
        private readonly HttpClient HttpClient;

        public OpenStreetMapServiceAdapter(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task GetPoligon(OpenStreetMapRequest request)
        {
            var response = await SendAsync(request.Location);
            
            if(!response.IsSuccessStatusCode)
                throw new Exception("Failed Load Data from API");
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            var deserializeContent = JsonConvert.DeserializeObject<JArray>(responseContent);
            List<List<JToken>> resultPoints = new List<List<JToken>>();
            foreach(var item in deserializeContent)
            {
                var geojson = item["geojson"].Value<JToken>();
                var type = geojson["type"].Value<JToken>();
                var coordinates = geojson["coordinates"].Value<JArray>();
                if (type.Value<string>() == "Point")
                {
                    resultPoints.Add( new List<JToken> { coordinates });
                    continue;
                }

                var points = SelectPointsRecursive(coordinates, request.FrequencyPoints);
                if(points.Any())
                    resultPoints.Add(points);
            }
            
            var serializedObjects = JsonConvert.SerializeObject(resultPoints, Formatting.Indented);
            if(!string.IsNullOrEmpty(request.FileName))
                File.WriteAllText(request.FileName,serializedObjects);
            else
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,request.Location),serializedObjects);
        }


        public List<JToken> SelectPointsRecursive(JToken token, int frequencyPoint)
        {
            List<JToken> points = new List<JToken>();
            foreach (var item in token)
            {
                if(item.Children().FirstOrDefault()?.Type != JTokenType.Float)
                    points.AddRange(SelectPointsRecursive(item,frequencyPoint));
                else
                {
                    var id = 1;
                    var items = item.Parent.Where(s => { return id++ % frequencyPoint == 0; });
                   
                        points.Add(item.Parent.First);
                        if (items.Any())
                            points.AddRange(items);
                        
                        if(!points.Contains(item.Parent.Last))
                            points.Add(item.Parent.Last);
                    
                    return points;
                }
            }
            return points;
        }

        public async Task<HttpResponseMessage> SendAsync(string location)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/search?q={location}&format=json&polygon_geojson=1");
            //API return 403 without User-Agent
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Mobile/7B405");
            return await HttpClient.SendAsync(request);
        }
    }
}