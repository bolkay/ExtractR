using ExtractR.Financials.Models;
using ExtractR.Financials.Results;
using System.Threading.Tasks;

namespace ExtractR.Financials.Core
{
    public abstract class PaymentProviderBase
    {
        public abstract Task<bool> ProcessPaymentAsync(PaymentDetails paymentDetails);
        public abstract Task StoreValuesAsync(ProviderAuthorisationResult providerAuthorisationResult, string storagePath = null);
        public abstract Task<ProviderAuthorisationResult> AuthoriseAsync(AuthorisationDetails paystackAuthorisationDetails);
    }
}
