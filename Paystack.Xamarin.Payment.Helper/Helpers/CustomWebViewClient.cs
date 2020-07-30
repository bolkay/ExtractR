using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Http;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace Paystack.Xamarin.Payment.Helper.Helpers
{
    public class CustomWebViewClient : WebViewClient
    {
        public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
        {
            handler.Proceed();
            //base.OnReceivedSslError(view, handler, error);
        }
        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            return base.ShouldOverrideUrlLoading(view, request);
        }
    }
}