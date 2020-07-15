using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using iText.Layout.Element;
using ExtractR.Droid.Models;
using System.Collections.Generic;
using ExtractR.Droid.Activities;
using Android.Support.V4.Content;
using Bumptech.Glide;
using ExtractR.Droid.Helpers;

namespace ExtractR.Droid.Adapters
{
    class HistoryAdapter : AndroidX.RecyclerView.Widget.RecyclerView.Adapter
    {
        public event EventHandler<HistoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<HistoryAdapterClickEventArgs> ItemLongClick;
        List<HistoryViewModel> imageViewModels;
        private readonly MainActivity mainActivity;

        public HistoryAdapter(List<HistoryViewModel> data, MainActivity mainActivity)
        {
            imageViewModels = data;
            this.mainActivity = mainActivity;
        }

        // Create new views (invoked by the layout manager)
        public override AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View itemView = null;
            var id = Resource.Layout.history_items_template;

            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new HistoryAdapterViewHolder(itemView, OnClick, OnLongClick);

            return vh;
        }

        public override void OnBindViewHolder(AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = imageViewModels[position];

            var holder = viewHolder as HistoryAdapterViewHolder;
            holder.FileNameTextView.Text = item.FileName.GetFileNameWithoutExtension();
            holder.SizeTextView.Text = item.FileSizeAndDayModified;

            var drawable = item.FileName.EndsWith("zip") ? ContextCompat.GetDrawable(mainActivity, Resource.Drawable.zip_drawable)
                : ContextCompat.GetDrawable(mainActivity, Resource.Drawable.pdf_drawable);

            Glide.With(mainActivity)
                .Load(drawable)
                .Thumbnail(0.5f)
                .Into(holder.FileIcon);
        }

        public override int ItemCount => imageViewModels.Count;

        void OnClick(HistoryAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(HistoryAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class HistoryAdapterViewHolder : AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder
    {
        public TextView SizeTextView { get; set; }
        public TextView FileNameTextView { get; set; }
        public ImageView FileIcon { get; set; }

        public HistoryAdapterViewHolder(View itemView, Action<HistoryAdapterClickEventArgs> clickListener,
                            Action<HistoryAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            SizeTextView = itemView.FindViewById<TextView>(Resource.Id.savedFileSizeTextView);
            FileNameTextView = itemView.FindViewById<TextView>(Resource.Id.savedFileNameTextView);
            FileIcon = itemView.FindViewById<ImageView>(Resource.Id.savedFileIconImageView);

            itemView.Click += (sender, e) => clickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class HistoryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}