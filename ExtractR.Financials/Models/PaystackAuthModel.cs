using ExtractR.Financials.Core;

namespace ExtractR.Financials.Models
{
    public class PaystackAuthModel : BaseAuthModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }
    public class Data
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }

}
