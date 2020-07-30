using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Views;
using Android.Widget;

namespace ExtractR.Droid.Helpers
{
    public static class ExtractRAdManager
    {
        public const string USER_DONATION_KEY = "exr_2020_donated_true";

        public class CustomAdListener : AdListener
        {
            Timer timer;
            private readonly InterstitialAd interstitialAd;

            public AppCompatActivity AppCompatActivity { get; }

            public CustomAdListener(InterstitialAd interstitialAd, AppCompatActivity appCompatActivity)
            {
                this.interstitialAd = interstitialAd;
                AppCompatActivity = appCompatActivity;
            }
            public override void OnAdLoaded()
            {
                //Show ads every 60 seconds.
                timer = new Timer(ShowAd, null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(60));

                base.OnAdLoaded();
            }
            private void ShowAd(object state)
            {
                AppCompatActivity.RunOnUiThread(() =>
                {
                    Console.WriteLine("I am loaded: " + interstitialAd.IsLoaded);
                    if (!interstitialAd.IsLoaded)
                        interstitialAd.LoadAd(new AdRequest.Builder().Build());

                    interstitialAd.Show();
                });
            }
        }
        public static InterstitialAd LoadAdInBackground(Context context)
        {
            InterstitialAd interstitialAd = new InterstitialAd(context);
            interstitialAd.AdUnitId = context.GetString(Resource.String.int_ad); //TESTING UNIT. Should be changed for production.
            interstitialAd.SetImmersiveMode(true);
            interstitialAd.AdListener = new CustomAdListener(interstitialAd, context as AppCompatActivity);
            interstitialAd.LoadAd(new AdRequest.Builder().Build());

            return interstitialAd;
        }

        public static void ShowLoadedAd(this InterstitialAd interstitialAd)
        {
            if (interstitialAd.IsLoaded)
                interstitialAd.Show();
        }
        public static bool UserHasDonated(Context context)
        {
            return PreferenceManager.GetDefaultSharedPreferences(context).GetBoolean(USER_DONATION_KEY, false);
        }
        public static void SetUserHasDonated(Context context)
        {
            var editor = PreferenceManager.GetDefaultSharedPreferences(context).Edit();

            editor.PutBoolean(USER_DONATION_KEY, true);

            editor.Apply();

            //User has donated.
        }
    }
}