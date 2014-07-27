﻿using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V4.App;
using Android.Graphics.Drawables;
using FortySevenDeg.SwipeListView;
using Android.Views.Animations;
using System.Threading.Tasks;

namespace SwipeListViewSample
{
	[Activity(Label = "SwipeListView", MainLauncher = true)]
	public class MainActivity : FragmentActivity
	{
		SwipeListView _swipeListView;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			_swipeListView = FindViewById<SwipeListView>(Resource.Id.example_lv_list);

			var adapter = new SpeakersAdapter(this, Speakers.GetSpeakerData());

			_swipeListView.SwipeListViewListener = SetupListener(adapter);
			_swipeListView.Adapter = adapter;	
		}

		private ISwipeListViewListener SetupListener(SpeakersAdapter adapter)
		{
			var listener = new SwipeListViewListener();
			listener.OnDismiss = (reverseSortedPosition) =>
			{
				foreach (var i in reverseSortedPosition)
				{
					adapter.RemoveView(i);
				}
			};
			listener.OnClickFrontView = (position) => Toast.MakeText(this, string.Format("Open item {0}", position), ToastLength.Short).Show();
			listener.OnClickBackView = _swipeListView.Close;

			return listener;
		}
	}


	public class SpeakersAdapter: BaseAdapter<Speaker>
	{
		private readonly List<Speaker> data;
		private readonly Activity context;

		public SpeakersAdapter(Activity activity, IEnumerable<Speaker> speakers)
		{
			data = speakers.OrderBy(s => s.Name).ToList();
			context = activity;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override Speaker this [int index] {
			get { return data[index]; }
		}

		public override int Count {
			get { return data.Count; }
		}

		public void RemoveView(int position)
		{
			data.RemoveAt(position);
			NotifyDataSetChanged();
		}
			
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var view = convertView;
			if (view == null) {
				// inflate the custom AXML layout
				view = context.LayoutInflater.Inflate(Resource.Layout.package_row, null);
			}

			((SwipeListView)parent).Recycle(view, position);

			var speaker = data[position];

			var ivImage = view.FindViewById<ImageView>(Resource.Id.example_row_iv_image);
			var tvTitle = view.FindViewById<TextView>(Resource.Id.example_row_tv_title);
			var tvDescription = view.FindViewById<TextView>(Resource.Id.example_row_tv_description);

			var headshot = GetHeadShot(speaker.HeadshotUrl);
			ivImage.SetImageDrawable(headshot);
			tvTitle.Text = speaker.Name;
			tvDescription.Text = speaker.Company;

			view.Click += (sender, e) => 
			{
				((SwipeListView)parent).OnClickBackView(position);
			};

			return view;
		}

		private Drawable GetHeadShot(string url)
		{
			Drawable headshotDrawable = null;
			try {
				headshotDrawable = Drawable.CreateFromStream(context.Assets.Open(url), null);
			} catch (Exception ex) {
				Android.Util.Log.Debug(GetType().FullName, "Error getting headshot for " + url + ", " + ex.ToString());
				headshotDrawable = null;
			}
			return headshotDrawable;
		}
	}
}


