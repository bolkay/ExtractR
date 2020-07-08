using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.Models;

namespace ExtractR.Droid.Helpers
{
    public class RVItemTouchCallback : ItemTouchHelper.SimpleCallback
    {
        private readonly MainActivity mainActivity;

        public RVItemTouchCallback(int dragDir, int swipeDir,
            RecyclerView recyclerView, Android.Support.V4.App.Fragment fragment, List<ImageFileNameModel> imageFileNameModels,
            MainActivity mainActivity)
            : base(dragDir, swipeDir)
        {
            RecyclerView = recyclerView;
            Fragment = fragment;
            ImageFileNameModels = imageFileNameModels;
            this.mainActivity = mainActivity;
        }


        public RecyclerView RecyclerView { get; }
        public Android.Support.V4.App.Fragment Fragment { get; }
        public List<ImageFileNameModel> ImageFileNameModels { get; }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            throw new NotImplementedException();
        }
        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            bool finallyDeleted = true;

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
            }

            if (itemDeleted != null)
            {
                //The item was actually deleted. Ask for restoration.

                Snackbar.Make(Fragment.View, $"You have removed {itemDeleted.FileName}", Snackbar.LengthLong)
                    .SetAction("Mistake", (x) =>
                    {
                        //Restore the item.
                        ImageFileNameModels.Insert(position, itemDeleted);
                        ItemCountHelper.UpdateExportItemsCount(mainActivity, ImageFileNameModels);
                        RecyclerView.GetAdapter().NotifyItemInserted(position);

                        finallyDeleted = false;
                    })
                    .SetActionTextColor(Android.Graphics.Color.ParseColor("#ffffff").ToArgb())
                    .AddCallback(new FileDeletionCallback(finallyDeleted, itemDeleted.FullPath))
                    .JavaCast<Snackbar>()
                    .Show();
            }
        }
    }
}