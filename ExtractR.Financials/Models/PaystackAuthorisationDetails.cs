using ExtractR.Financials.Core;

namespace ExtractR.Financials.Models
{
    public class PaystackAuthorisationDetails : AuthorisationDetails
    {
        public string[] Channels { get; set; }
        public string Reference { get; set; }
    }
}
