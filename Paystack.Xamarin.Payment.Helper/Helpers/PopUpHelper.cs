using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Paystack.Xamarin.Payment.Helper.Helpers
{
    public class PopUpHelper
    {
        public PopupWindow ShowLoadingPopUp(Context context, View view)
        {
            LayoutInflater layoutInflater = context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;

            View loadingView = layoutInflater.Inflate(Resource.Layout.loading_dialog_popup, null);

            PopupWindow popupWindow = new PopupWindow(loadingView, LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);

            popupWindow.ShowAtLocation(view, GravityFlags.Center, 0, 0);

            return popupWindow;
        }
    }
}