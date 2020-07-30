using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Sql;
using Paystack.Xamarin.Payment.Helper.Config;
using Paystack.Xamarin.Payment.Helper.Helpers;

namespace Paystack.Xamarin.Payment.Helper
{
    [Activity]
    public class TestActivity : AppCompatActivity
    {
        TextView textView;
        WebView webView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.test_layout);

            textView = FindViewById<TextView>(Resource.Id.poweredByTextView);
            webView = FindViewById<WebView>(Resource.Id.paymentWV);
            webView.SetWebViewClient(new CustomWebViewClient());

            webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;

            textView.Text = $"Proudly powered by DotNetFlow - {DateTime.Now.Year}";

            string url = Intent.GetStringExtra(PaystackOptions.PAYSTACK_TRANSFER_KEY);

            if (!string.IsNullOrEmpty(url))
                webView.LoadUrl(url);






























        }
    }
}