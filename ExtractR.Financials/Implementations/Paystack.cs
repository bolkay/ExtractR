using ExtractR.Financials.Core;
using ExtractR.Financials.Helpers;
using ExtractR.Financials.Models;
using ExtractR.Financials.Results;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExtractR.Financials.Implementations
{
    /// <summary>
    /// Process payment using paystack as the provider.
    /// </summary>
    public class Paystack : PaymentProviderBase
    {
        private const string BaseEndpoint = "https://api.paystack.co";
        private const string AuthorizationEndpoint = @"/transaction/initialize";
        private const string VerificationEndpoint = @"/transaction/verify";
        private string StorageDirectory { get; set; }
        private ProviderAuthorisationResult ProviderAuthorisationResult { get; set; }
        private HttpClient httpClient;
        public Paystack(string secretToken)
        {
            httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri(BaseEndpoint);

            httpClient.DefaultRequestHeaders.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", secretToken);

        }
        public override Task<bool> ProcessPaymentAsync(PaymentDetails paymentDetails)
        {
            throw new NotImplementedException();
        }
        public override async Task<ProviderAuthorisationResult> AuthoriseAsync(AuthorisationDetails authorisationDetails)
        {
            try
            {
                //Convert before sending to paystack.
                float amount = (await ExchangeRateHelper.GetNairaTodayAgainstDollar());

                int nonStringAmount = int.Parse(authorisationDetails.Amount);
                authorisationDetails.Amount = (amount * nonStringAmount).ToString();
                authorisationDetails.Reference = Guid.NewGuid().ToString();

                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(authorisationDetails),
                    Encoding.UTF8, "application/json");

                var getAuthEndpoint = await httpClient.PostAsync(AuthorizationEndpoint, httpContent);

                Console.WriteLine(getAuthEndpoint.Content.ReadAsStringAsync().Result);

                if (getAuthEndpoint.IsSuccessStatusCode)
                {
                    var json = await getAuthEndpoint.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(json))
                    {
                        var payStackAuthModel = JsonConvert.DeserializeObject<PaystackAuthModel>(json);

                        if (null != payStackAuthModel)
                        {
                            return new ProviderAuthorisationResult
                            {
                                AccessCode = payStackAuthModel.data.access_code,
                                AuthEndpoint = payStackAuthModel.data.authorization_url,
                                Status = getAuthEndpoint.StatusCode.ToString(),
                                Reference = payStackAuthModel.data.reference
                            };
                        }
                    }
                }

            }
            catch
            {
                return null;
            }
            return null;
        }
        public override Task StoreValuesAsync(ProviderAuthorisationResult providerAuthResult, string storagePath = null)
        {
            //Store the authorisation details for later use.
            if (providerAuthResult == null)
                throw new ArgumentNullException(nameof(providerAuthResult));

            ProviderAuthorisationResult = providerAuthResult;

            if (!string.IsNullOrEmpty(storagePath))
                //Up to the caller to ensure appropriate permissions exist.
                File.WriteAllText(storagePath, Convert.ToBase64String(Encoding.UTF8.
                    GetBytes(JsonConvert.SerializeObject(providerAuthResult))));

            return Task.CompletedTask;
        }

        public async Task<TransactionVerificationResult> VerifyTransactionAsync(string reference)
        {
            TransactionVerificationResult transactionVerificationResult = null;

            var tryVerify = await httpClient.GetAsync(VerificationEndpoint + $"/{reference}");

            if (tryVerify.IsSuccessStatusCode)
            {
                try
                {
                    string json = await tryVerify.Content.ReadAsStringAsync();
                    transactionVerificationResult = JsonConvert.DeserializeObject<TransactionVerificationResult>
                        (json);

                    return transactionVerificationResult;
                }
                catch
                {
                    return null;
                }

            }
            return null;
        }
        public string GetStorageDirectory()
        {
            return StorageDirectory;
        }
        public ProviderAuthorisationResult GetProviderAuthorisationResult() => ProviderAuthorisationResult;
        public string GetReference() => ProviderAuthorisationResult.Reference;

    }
}
