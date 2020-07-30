using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.ERFragments;
using ExtractR.Droid.Helpers;

namespace ExtractR.Droid.Activities
{
    [Activity(Theme = "@style/AppTheme")]
    public class ExportActivity : AppCompatActivity
    {
        AdView adView;
        Android.Support.V7.Widget.Toolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.export_activity);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.exportToolbar);
            adView = FindViewById<AdView>(Resource.Id.adView);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDefaultDisplayHomeAsUpEnabled(true);

            double count = 0;
            var getExtra = Intent.Extras.Get("itemCount");

            if (getExtra != null)
            {
                count = (double)getExtra;
            }

            SupportActionBar.Title = $"Export {count} Items";

            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.exportPlaceholder, new NewExportFragment(this))
                .Commit();


            //Present simple banner ads.
            AdRequest adRequest = new AdRequest.Builder().Build();

            //Load ad if the user has not donated.
            if (!ExtractRAdManager.UserHasDonated(this))
                adView.LoadAd(adRequest);

        }
        public override bool OnSupportNavigateUp()
        {
            MainActivity.interstitialAd?.ShowLoadedAd();

            OnBackPressed();

            return base.OnSupportNavigateUp();
        }
        public override bool OnNavigateUp()
        {
            return base.OnNavigateUp();
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}