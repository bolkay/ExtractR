using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Util;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.Adapters;
using ExtractR.Droid.Helpers;
using ExtractR.Droid.Models;

namespace ExtractR.Droid.ERFragments
{
    public class HistoryFragment : Android.Support.V4.App.Fragment
    {
        public HistoryFragment(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        List<HistoryViewModel> historyViewModels;
        RecyclerView recyclerView;
        LinearLayoutManager layoutManager;
        CardView nohistory;
        private readonly MainActivity mainActivity;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var files = PathHelper.GetAllExportedFiles();
            
            if (files != null)
            {
                historyViewModels = files.Select(x => new HistoryViewModel { FileName = x }).ToList();
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.history_fragment, container, false);
            nohistory = view.FindViewById<CardView>(Resource.Id.noHistoryGroup);

            if (historyViewModels.Any())
                nohistory.Visibility = ViewStates.Gone;

            HistoryAdapter historyAdapter = new HistoryAdapter(historyViewModels, mainActivity);
            layoutManager = new LinearLayoutManager(this.Context);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.historyRecyclerView);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.NestedScrollingEnabled = false;
            recyclerView.HasFixedSize = true;

            var decoration = new DividerItemDecoration(this.Context, layoutManager.Orientation);
            var inset = Resources.GetDimensionPixelSize(Resource.Dimension.divider_margin_vertical);
            InsetDrawable insetDrawable = new InsetDrawable(ContextCompat.GetDrawable(this.Context,Resource.Drawable.abc_list_divider_material)
                , 0, inset, 0, inset);

            decoration.SetDrawable(insetDrawable);
            ItemTouchHelper itemTouchHelper = new ItemTouchHelper(new HistoryTouchCallback(
                0, ItemTouchHelper.Right | ItemTouchHelper.Left, recyclerView, historyViewModels, this));

            itemTouchHelper.AttachToRecyclerView(recyclerView);

            recyclerView.SetAdapter(historyAdapter);
            
            return view;
        }
    }
}