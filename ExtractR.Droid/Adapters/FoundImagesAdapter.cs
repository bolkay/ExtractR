using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using ExtractR.Droid.Models;
using ExtractR.Droid.Helpers;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Bitmap;
using System.Threading.Tasks;
using Bumptech.Glide.Load.Engine;
using Android.Support.V4.App;
using System.Threading;
using System.IO;

namespace ExtractR.Droid.Adapters
{
    class FoundImagesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<FoundImagesAdapterClickEventArgs> ItemClick;
        public event EventHandler<FoundImagesAdapterClickEventArgs> ItemLongClick;
        List<ImageFileNameModel> items;

        public FoundImagesAdapter(List<ImageFileNameModel> data, Android.Support.V4.App.Fragment fragment)
        {
            items = data;
            Fragment = fragment;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;

            var id = Resource.Layout.imageview_template;

            itemView = LayoutInflater.From(parent.Context).
                 Inflate(id, parent, false);

            var vh = new FoundImagesAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as FoundImagesAdapterViewHolder;

            holder.FileNameTextView.Text = items[position].FileName.GetFileNameWithoutExtension();
            holder.FileSizeTextView.Text = items[position].FileSize;

            Glide.With(Fragment)
                .Load(Android.Net.Uri.FromFile(new Java.IO.File(item.FullPath)))
                .Thumbnail(0.9f)
                .SetDiskCacheStrategy(DiskCacheStrategy.All)
                .Placeholder(Resource.Drawable.abc_spinner_mtrl_am_alpha)
                .Into(holder.ImageView);

        }

        public override int ItemCount => items.Count;

        public Fragment Fragment { get; }

        void OnClick(FoundImagesAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(FoundImagesAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class FoundImagesAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView FileSizeTextView { get; set; }
        public TextView FileNameTextView { get; set; }
        public ImageView ImageView { get; set; }

        public FoundImagesAdapterViewHolder(View itemView, Action<FoundImagesAdapterClickEventArgs> clickListener,
                            Action<FoundImagesAdapterClickEventArgs> longClickListener) : base(itemView)
        {

            FileNameTextView = itemView.FindViewById<TextView>(Resource.Id.imageNameTextView);
            ImageView = itemView.FindViewById<ImageView>(Resource.Id.imgView);
            FileSizeTextView = itemView.FindViewById<TextView>(Resource.Id.fileSizeTextView);

            itemView.Click += (sender, e) => clickListener(new FoundImagesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new FoundImagesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class FoundImagesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}