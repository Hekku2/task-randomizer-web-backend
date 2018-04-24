using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiTests.Tools
{
    public static class HttpResponseExtensions
    {
        public static async Task<T> ToObject<T>(this HttpResponseMessage response)
        {
            var datastring = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(datastring);
        }
    }
}
