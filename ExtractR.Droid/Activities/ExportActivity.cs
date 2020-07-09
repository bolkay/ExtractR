using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.ERFragments;

namespace ExtractR.Droid.Activities
{
    [Activity(Theme = "@style/AppTheme")]
    public class ExportActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.export_activity);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.exportToolbar);

            SetSupportActionBar(toolbar);

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


        }
    }
}