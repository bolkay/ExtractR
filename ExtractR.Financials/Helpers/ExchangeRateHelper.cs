using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using System;

namespace ExtractR.Financials.Helpers
{
    public class ExchangeRateHelper
    {
        public static HttpClient httpClient = new HttpClient();

        public static async Task<float> GetNairaTodayAgainstDollar()
        {

            string json = await httpClient.GetStringAsync("https://api.exchangerate.host/latest?base=USD&symbols=NGN");

            var result = JsonConvert.DeserializeObject<dynamic>(json);

            return result.rates.NGN;
        }

        /// <summary>
        /// Sample structure of the object returned.
        /// </summary>

        public class Rootobject
        {
            public bool success { get; set; }
            public string _base { get; set; }
            public string date { get; set; }
            public Rates rates { get; set; }
        }

        public class Rates
        {
            public float NGN { get; set; }
        }

    }
}
