using Android.App;
using Android.Content;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.Activities;
using ExtractR.Droid.ERFragments;
using ExtractR.Droid.Helpers;
using Java.Lang;
using System.Linq;

namespace ExtractR.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", HardwareAccelerated = true, AlwaysRetainTaskState = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {

        public Android.Support.V7.Widget.Toolbar toolbar;
        public TextView itemCountView;
        NewTaskFragment newTaskFragment;
        HistoryFragment historyFragment;
        //Store reference to menu.

        public IMenu menu;
        BottomNavigationView navigation;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            newTaskFragment = new NewTaskFragment(this);
            historyFragment = new HistoryFragment(this);


            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.mainToolBar);

            SetSupportActionBar(toolbar);


            navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            ChangeFragment(newTaskFragment);

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {
                case Resource.Id.navigation_dashboard:
                    menu.FindItem(Resource.Id.action_refresh).SetVisible(true);
                    menu.FindItem(Resource.Id.action_save).SetVisible(true);
                    ChangeFragment(newTaskFragment);
                    return true;

                case Resource.Id.navigation_history:
                    menu.FindItem(Resource.Id.action_refresh).SetVisible(false);
                    menu.FindItem(Resource.Id.action_save).SetVisible(false);
                    ChangeFragment(historyFragment);
                    return true;
            }
            return false;
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.export_menu, menu);
            itemCountView = menu.FindItem(Resource.Id.action_save).ActionView.FindViewById<TextView>(Resource.Id.export_badge);

            itemCountView.Click += ItemCountView_Click;

            this.menu = menu;

            return base.OnCreateOptionsMenu(menu);
        }


        private void ChangeFragment(Android.Support.V4.App.Fragment fragment)
        {
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.framePlaceholder, fragment)
                .Commit();
        }
        private void ItemCountView_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ExportActivity));
            intent.PutExtra("itemCount", (double)newTaskFragment?.ImageFileNameModels.Count);
            StartActivity(intent);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var fragment = SupportFragmentManager.Fragments.OfType<NewTaskFragment>().FirstOrDefault();

            switch (item.ItemId)
            {
                case Resource.Id.action_save:
                    break;
                
                case Resource.Id.action_refresh:
                    if (fragment._recyclerView.GetAdapter().ItemCount < 1 && !fragment.CouldBeRefreshed)
                    {
                        var snackbar = Snackbar.Make(fragment.View, "No work in progress.", Snackbar.LengthShort);
                        snackbar.View.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF6500"));
                        snackbar.Show();
                    }

                    else
                        RefreshTaskFragment(fragment);
                    return true;

                case Resource.Id.action_help:
                    StartActivity(typeof(HelpActivity));
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        public void RefreshTaskFragment(NewTaskFragment fragment)
        {
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle("Reset Work Space")
                .SetMessage("Would you like to start all over again?")
                .SetPositiveButton("Yes",
                (s, e) =>
                {
                    ResetView(fragment);
                })
                .SetNegativeButton("Quit", (s, e) => { ResetView(fragment); QuitApplication(); })
                .Show();

        }
        public void QuitApplication()
        {
            MoveTaskToBack(true);
        }
        public void ResetView(NewTaskFragment fragment)
        {
            PathHelper.DeleteAllTempFiles(); //Delete all temp files in case.

            itemCountView.Text = "0";
            toolbar.Subtitle = "Try again. I want to help.";
            if (fragment.fileChooserCard.Visibility == ViewStates.Gone
            || fragment.fileChooserCard.Visibility == ViewStates.Invisible)
            {
                fragment.fileChooserCard.Visibility = ViewStates.Visible;
                fragment.processingGroup.Visibility = ViewStates.Gone;
            }
            fragment.ImageFileNameModels.Clear();
            fragment._recyclerView.GetAdapter().NotifyDataSetChanged();
        }

        public override void OnBackPressed()
        {
            //When the user pressed the backbutton, we want to check if the currently selected bottom nav item is the dashboard.
            if (navigation.SelectedItemId == Resource.Id.navigation_dashboard)
            {
                //QUIT THE APPLICATION....
                base.OnBackPressed();
            }

            else
                navigation.SelectedItemId = Resource.Id.navigation_dashboard;
        }
    }
}

