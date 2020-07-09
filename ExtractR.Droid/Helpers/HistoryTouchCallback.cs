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
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.Models;

namespace ExtractR.Droid.Helpers
{
    public class HistoryTouchCallback : ItemTouchHelper.SimpleCallback
    {
        private readonly RecyclerView recyclerView;
        private readonly List<HistoryViewModel> historyViewModels;
        private readonly Android.Support.V4.App.Fragment fragment;

        public HistoryTouchCallback(int dragDirs, int swipeDirs, RecyclerView recyclerView,
            List<HistoryViewModel> historyViewModels, Android.Support.V4.App.Fragment fragment) : base(dragDirs, swipeDirs)
        {
            this.recyclerView = recyclerView;
            this.historyViewModels = historyViewModels;
            this.fragment = fragment;
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            throw new NotImplementedException();
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
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
            }

            if (itemDeleted != null)
            {
                //The item was actually deleted. Ask for restoration.

                var snackBar = Snackbar.Make(fragment.View, $"You have removed entry {position} : " +
                    $"{itemDeleted.FileName.GetFileNameWithoutExtension()}", Snackbar.LengthLong)
                    .SetAction("UNDO", (x) =>
                    {
                        //Restore the item.
                        historyViewModels.Insert(position, itemDeleted);
                        recyclerView.GetAdapter().NotifyItemInserted(position);

                        PermissionHelper.ShouldDelete = false;
                    })
                    .SetActionTextColor(Android.Graphics.Color.ParseColor("#BB86FC").ToArgb());

                snackBar.AddCallback(new FileDeletionCallback(itemDeleted.FileName));
                
                snackBar.View.SetBackgroundColor(Android.Graphics.Color.ParseColor("#121212"));
                snackBar.Show();

            }
        }
    }
}