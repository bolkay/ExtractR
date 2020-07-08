using Android.App;
using Android.Icu.Util;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.ERFragments;
using Java.Lang;
using System.Linq;

namespace ExtractR.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", HardwareAccelerated = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        public Android.Support.V7.Widget.Toolbar toolbar;
        public TextView itemCountView;
        NewTaskFragment newTaskFragment;
        NewExportFragment newExportFragment;

        //Store reference to menu.
        public IMenu menu;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            newTaskFragment = new NewTaskFragment(this);
            newExportFragment = new NewExportFragment(this);
            SetupFragments();

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.mainToolBar);

            SetSupportActionBar(toolbar);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

        }

        private void SetupFragments()
        {
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.framePlaceholder, newTaskFragment)
                .Commit();
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
                case Resource.Id.navigation_home:
                    return true;
                case Resource.Id.navigation_dashboard:
                    return true;
                case Resource.Id.navigation_notifications:
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

        private void ItemCountView_Click(object sender, System.EventArgs e)
        {
            if (SupportFragmentManager.FindFragmentByTag(nameof(newTaskFragment)) == null)
            {
                SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.framePlaceholder, newExportFragment, nameof(newExportFragment))
                   .AddToBackStack(nameof(newTaskFragment))
                    .Commit();
            }

            //We cannot refresh now.
            menu.FindItem(Resource.Id.action_refresh).SetVisible(false);
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
                        Snackbar.Make(fragment.View, "No work in progress.", Snackbar.LengthShort).Show();
                    else
                        RefreshFragment(fragment);
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        public void RefreshFragment(NewTaskFragment fragment)
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
    }
}

