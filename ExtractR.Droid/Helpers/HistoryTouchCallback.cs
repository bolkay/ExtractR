using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using ExtractR.Droid.ERFragments;
using ExtractR.Droid.Models;

namespace ExtractR.Droid.Helpers
{
    public class HistoryTouchCallback : AndroidX.RecyclerView.Widget.ItemTouchHelper.SimpleCallback
    {
        private readonly AndroidX.RecyclerView.Widget.RecyclerView recyclerView;
        private readonly List<HistoryViewModel> historyViewModels;
        private readonly Android.Support.V4.App.Fragment fragment;

        public HistoryTouchCallback(int dragDirs, int swipeDirs, AndroidX.RecyclerView.Widget.RecyclerView recyclerView,
            List<HistoryViewModel> historyViewModels, Android.Support.V4.App.Fragment fragment) : base(dragDirs, swipeDirs)
        {
            this.recyclerView = recyclerView;
            this.historyViewModels = historyViewModels;
            this.fragment = fragment;
        }

        public override bool OnMove(AndroidX.RecyclerView.Widget.RecyclerView recyclerView,
           AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder viewHolder, AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder target)
        {
            throw new NotImplementedException();
        }

        public override void OnSwiped(AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder viewHolder, int direction)
        {
            //Store the position swiped.
            int position = viewHolder.LayoutPosition;

            //Store the deleted item in case of restoration.
            HistoryViewModel itemDeleted = null;

            if (direction == ItemTouchHelper.Left || direction == ItemTouchHelper.Right)
            {
                //The item was swiped.
                //Remove from list.
                itemDeleted = historyViewModels[position];
                historyViewModels.RemoveAt(position);
                recyclerView.GetAdapter().NotifyItemRemoved(position);

                //Should delete
                PermissionHelper.ShouldDelete = true;
            }

            if (itemDeleted != null)
            {
                //The item was actually deleted. Ask for restoration.

                string accentColor = "#" + fragment.Context.GetColor(Resource.Color.colorAccent).ToString("X");
                string primaryColor = "#" + fragment.Context.GetColor(Resource.Color.colorPrimary).ToString("X");

                var snackBar = Snackbar.Make(fragment.View, $"Saved Item Deleted", Snackbar.LengthLong)
                    .SetAction("Restore", (x) =>
                    {
                        //Restore the item.
                        historyViewModels.Insert(position, itemDeleted);
                        recyclerView.GetAdapter().NotifyItemInserted(position);

                        //Dont delete.
                        PermissionHelper.ShouldDelete = false;
                    })
                    .SetActionTextColor(Android.Graphics.Color.ParseColor(accentColor));


                snackBar.AddCallback(new FileDeletionCallback(itemDeleted.FileName, fragment as HistoryFragment, historyViewModels));

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