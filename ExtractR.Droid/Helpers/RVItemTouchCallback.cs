using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.Models;

namespace ExtractR.Droid.Helpers
{
    public class RVItemTouchCallback : AndroidX.RecyclerView.Widget.ItemTouchHelper.SimpleCallback
    {
        private readonly MainActivity mainActivity;

        public RVItemTouchCallback(int dragDir, int swipeDir,
            AndroidX.RecyclerView.Widget.RecyclerView recyclerView, Android.Support.V4.App.Fragment fragment, List<ImageFileNameModel> imageFileNameModels,
            MainActivity mainActivity)
            : base(dragDir, swipeDir)
        {
            RecyclerView = recyclerView;
            Fragment = fragment;
            ImageFileNameModels = imageFileNameModels;
            this.mainActivity = mainActivity;
        }


        public AndroidX.RecyclerView.Widget.RecyclerView RecyclerView { get; }
        public Android.Support.V4.App.Fragment Fragment { get; }
        public List<ImageFileNameModel> ImageFileNameModels { get; }

        public override bool OnMove(AndroidX.RecyclerView.Widget.RecyclerView recyclerView,
           AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder viewHolder, AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder target)
        {
            throw new NotImplementedException();
        }
        public override void OnSwiped(AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder viewHolder, int direction)
        {
            //Store the position in case of restoration.
            int position = viewHolder.LayoutPosition;

            //Store the potentially deleted item.
            ImageFileNameModel itemDeleted = null;

            if (direction == ItemTouchHelper.Right || direction == ItemTouchHelper.Left)
            {
                //Remove the item in the model.
                itemDeleted = ImageFileNameModels[position];

                ImageFileNameModels.RemoveAt(position);
                ItemCountHelper.UpdateExportItemsCount(mainActivity, ImageFileNameModels);
                RecyclerView.GetAdapter().NotifyItemRemoved(position);

                PermissionHelper.ShouldDelete = true;
            }

            if (itemDeleted != null)
            {
                //The item was actually deleted. Ask for restoration.

                string accentColor = "#" + Fragment.Context.GetColor(Resource.Color.colorAccent).ToString("X");
                string primaryColor = "#" + Fragment.Context.GetColor(Resource.Color.colorPrimary).ToString("X");

                var snackBar = Snackbar.Make(Fragment.View, $"Item Removed", Snackbar.LengthLong)
                    .SetAction("Restore", (x) =>
                    {
                        //Restore the item.
                        ImageFileNameModels.Insert(position, itemDeleted);
                        ItemCountHelper.UpdateExportItemsCount(mainActivity, ImageFileNameModels);
                        RecyclerView.GetAdapter().NotifyItemInserted(position);

                        mainActivity.SupportActionBar.Subtitle = $"Current count - {ImageFileNameModels.Count}";

                        PermissionHelper.ShouldDelete = false;
                    })
                    .SetActionTextColor(Android.Graphics.Color.ParseColor(accentColor));

                snackBar.AddCallback(new FileDeletionCallback(itemDeleted.FileName, mainActivity, ImageFileNameModels));

                snackBar.View.SetBackgroundColor(Android.Graphics.Color.ParseColor(primaryColor));

                //Get the exisiting text.

                TextView textView = snackBar.View.FindViewById<TextView>(Resource.Id.snackbar_text);

                if (AppCompatDelegate.DefaultNightMode == AppCompatDelegate.ModeNightNo)
                    textView.SetTextColor(Android.Graphics.Color.Black);
                else
                    textView.SetTextColor(Android.Graphics.Color.White);

                snackBar.Show();

            }
        }
    }
}