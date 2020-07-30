using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using ExtractR.Droid.Helpers;
using ExtractR.Financials;
using ExtractR.Financials.Core;
using ExtractR.Financials.Implementations;
using ExtractR.Financials.Models;
using ExtractR.Financials.Results;
using Paystack.Xamarin.Payment.Helper;
using Paystack.Xamarin.Payment.Helper.Config;
using Paystack.Xamarin.Payment.Helper.Helpers;
using PaystackObject = ExtractR.Financials.Implementations.Paystack;
namespace ExtractR.Droid.ERFragments
{
    public class DonationFragment : Android.Support.V4.App.Fragment
    {

        Button donationButton;
        EditText amountText;
        EditText donorEmailAddress;

        PaystackObject paystack;

        ISharedPreferences sharedPreferences;
        public DonationFragment()
        {

        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this.Context);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.donation_fragment, container, false);

            donationButton = view.FindViewById<Button>(Resource.Id.donateButton);
            amountText = view.FindViewById<EditText>(Resource.Id.donateEditText);
            donorEmailAddress = view.FindViewById<EditText>(Resource.Id.donorEmailEditText);

            donationButton.Click += DonationButton_Click;

            return view;
        }

        private async void DonationButton_Click(object sender, EventArgs e)
        {
            PopUpHelper popUpHelper = new PopUpHelper();
            PopupWindow popupWindow = popUpHelper.ShowLoadingPopUp(Context, View);

            string key = Context.GetString(Resource.String.paystack_secret);

            //Validate input and key before processing.

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(amountText.Text))
            {
                paystack = new PaystackObject(key);

                AuthorisationDetails authorisationDetails = new PaystackAuthorisationDetails
                {
                    Amount = amountText.Text,
                    Channels = new string[] { "bank", "card" },
                    Email = donorEmailAddress.Text
                };

                ProviderAuthorisationResult authResult = await paystack.AuthoriseAsync(authorisationDetails);
                try
                {
                    string path = System.IO.Path.Combine(PathHelper.GetOrCreateAuthDetailsPath(), "auth.exr");
                    //Important if you need to verify later.
                    await paystack.StoreValuesAsync(authResult, path);

                    Intent intent = new Intent(Context, typeof(TestActivity));

                    intent.PutExtra(PaystackOptions.PAYSTACK_TRANSFER_KEY, authResult.AuthEndpoint);

                    popupWindow.Dismiss();

                    StartActivityForResult(intent, 2500);
                }
                catch
                {
                    return;
                }
            }
        }

        public override async void OnActivityResult(int requestCode, int resultCode, Intent data)
        {

            if (requestCode == 2500)
            {
                if (paystack != null)
                {
                    //Verify the transaction.
                    string storedReference = paystack.GetReference();

                    var result = await paystack.VerifyTransactionAsync(storedReference);

                    if (result.status)
                    {
                        //Transaction was successful.
                        //Save user donation.
                        //var editor = sharedPreferences.Edit();
                        //editor.PutBoolean(ExtractRAdManager.USER_DONATION_KEY, true);
                        //editor.Apply();
                        ////User has donated.
                        ///
                        ExtractRAdManager.SetUserHasDonated(this.Context);
                    }

                    if (null != result)
                        Toast.MakeText(Context, $"{result.message} {result.data.amount}", ToastLength.Long).Show();

                    else
                        Toast.MakeText(Context, "Error occurred", ToastLength.Short).Show();
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}