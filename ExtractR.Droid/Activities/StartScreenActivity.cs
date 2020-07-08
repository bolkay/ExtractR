using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View.Animation;
using Android.Support.V7.App;
using Android.Text.Format;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;
namespace ExtractR.Droid.Activities
{
    [Activity(Label = "StartScreenActivity", Theme = "@style/Theme.MaterialComponents.Light.NoActionBar", MainLauncher = true, NoHistory = true)]
    public class StartScreenActivity : AppCompatActivity
    {
        private TextView extractRText;
        Timer _timer;
        private const double TimeToWait = 3000; //In Milliseconds.
        private static int PermissionsRequestCode = 1;
        private static string[] PermissionsNeeded = new string[]
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_start);

            extractRText = FindViewById<TextView>(Resource.Id.startScreenAppName);

            //Extend to the status and navigation bar.
            Window.SetFlags(WindowManagerFlags.LayoutNoLimits | WindowManagerFlags.TranslucentStatus | WindowManagerFlags.TranslucentNavigation,
                WindowManagerFlags.TranslucentStatus | WindowManagerFlags.TranslucentStatus | WindowManagerFlags.LayoutNoLimits);

            _timer = new Timer(3000);

            _timer.Start();

            _timer.Elapsed += _timer_Elapsed;

        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Lets check for permission.
            if (CheckSelfPermission(PermissionsNeeded.First()) != Permission.Granted)
            {
                _timer.Stop();

                RunOnUiThread(() =>
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle(Resource.String.dialog_title)
                        .SetIcon(GetDrawable(Resource.Drawable.ic_home_black_24dp))
                        .SetMessage(Resource.String.dialog_message)
                        .SetPositiveButton(Resource.String.dialog_postive, PositiveButtonSelected)
                        .SetNegativeButton(Resource.String.dialog_negative, NegativeButtonSelected)
                        .Show();

                });

            }
            else
                GetOnToMainActivity();
            //Whatever happens.... dispose the timer.

            _timer.Dispose();
        }

        private void PositiveButtonSelected(object sender, DialogClickEventArgs dialogClickEventArgs)
        {
            RequestPermissions(PermissionsNeeded, PermissionsRequestCode);
        }
        /// <summary>
        /// Fires if the user selects No.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dialogClickEventArgs"></param>
        private void NegativeButtonSelected(object sender, DialogClickEventArgs dialogClickEventArgs)
        {
            PromptForPermission();
        }

        private void GetOnToMainActivity()
        {
            StartActivity(typeof(MainActivity));
        }

        private void PromptForPermission()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetTitle(Resource.String.dialog_quit_title)
                .SetIcon(GetDrawable(Resource.Drawable.ic_home_black_24dp))
                .SetPositiveButton(Resource.String.dialog_quit_positive, PositiveButtonSelected)
                .SetNegativeButton(Resource.String.dialog_quit, (s, e) =>
                {
                    MoveTaskToBack(true);
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    Java.Lang.JavaSystem.Exit(1);
                }).Show();

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {

            if (requestCode == PermissionsRequestCode)
            {
                var somePermissionIsMissing = grantResults.Any(x => x == Permission.Denied);

                if (somePermissionIsMissing)
                    PromptForPermission();
                else
                    GetOnToMainActivity();
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }
    }
}