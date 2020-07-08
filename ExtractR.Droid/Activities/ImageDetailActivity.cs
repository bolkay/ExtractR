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
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using ExtractR.Droid.Helpers;
using Sephiroth.ImageZoom;

namespace ExtractR.Droid.Activities
{
    [Activity(Label = "ImageActivity", Theme = "@style/AppTheme", HardwareAccelerated = true)]
    public class ImageDetailActivity : AppCompatActivity
    {
        ImageViewTouch imageView;
        Android.Support.V7.Widget.Toolbar toolbar;
        Android.Graphics.Bitmap original;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.image_detail_activity);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.imageToolBar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            imageView = FindViewById<ImageViewTouch>(Resource.Id.myImageDetailView);

            var retrievedFilePath = (string)Intent.Extras.Get("filePath");
            try
            {
                Glide.With(this)
                    .Load(Android.Net.Uri.FromFile(new Java.IO.File(retrievedFilePath)))
                    .SetDiskCacheStrategy(DiskCacheStrategy.All)
                    .Into(imageView);

                original = await Android.Graphics.BitmapFactory.DecodeFileAsync(retrievedFilePath);

                SupportActionBar.Title = $"{retrievedFilePath.GetFileNameWithoutExtension()}";
                SupportActionBar.Subtitle = $"{original.Width} x {original.Height} | {PathHelper.GetFileLength(retrievedFilePath)} Kb";
            }
            catch
            {
                imageView.ContentDescription = "An error occured.";
            }
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();

            return base.OnSupportNavigateUp();
        }
        public override bool OnNavigateUp()
        {
            original?.Dispose();
            
            return base.OnNavigateUp();
        }
    }
}