using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Mode=Com.Handmark.PullToRefresh.Library.PtrMode;
using Java.Lang;
using Com.Handmark.PullToRefresh.Library;
using Com.Handmark.PullToRefresh.Library.Extras;


namespace PullToRefresh.Net.Example
{
	[Activity(Label = "PullToRefreshListActivity")]
	public class PullToRefreshListActivity : ListActivity, OnRefreshListener<ListView>, OnLastItemVisibleListener
	{


		const int MENU_MANUAL_REFRESH = 0;
		const int MENU_DISABLE_SCROLL = 1;
		const int MENU_SET_MODE = 2;
		const int MENU_DEMO = 3;

		private IList<string> mListItems;
		private PullToRefreshListView mPullRefreshListView;
		private ArrayAdapter<string> mAdapter;

		/** Called when the activity is first created. */
		//@Override
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_ptr_list);

			mPullRefreshListView = (PullToRefreshListView)FindViewById(Resource.Id.pull_refresh_list);

			mPullRefreshListView.setOnRefreshListener(this);
			//mPullRefreshListView.setOnLastItemVisibleListener(this);

			mPullRefreshListView.setOnLastItemVisibleListener(this);
			// Set a listener to be invoked when the list should be refreshed.
			//mPullRefreshListView.setOnRefreshListener(new OnRefreshListener<ListView>() {
			//    //@Override
			//    public void onRefresh(PullToRefreshBase<ListView> refreshView) {
			//        String label = DateUtils.formatDateTime(getApplicationContext(), System.currentTimeMillis(),
			//                DateUtils.FORMAT_SHOW_TIME | DateUtils.FORMAT_SHOW_DATE | DateUtils.FORMAT_ABBREV_ALL);

			//        // Update the LastUpdatedLabel
			//        refreshView.getLoadingLayoutProxy().setLastUpdatedLabel(label);

			//        // Do work to refresh the list here.
			//        new GetDataTask().execute();
			//    }
			//});

			// Add an end-of-list listener
			//mPullRefreshListView.setOnLastItemVisibleListener(new OnLastItemVisibleListener() {

			//    @Override
			//    public void onLastItemVisible() {
			//        Toast.makeText(PullToRefreshListActivity.this, "End of List!", Toast.LENGTH_SHORT).show();
			//    }
			//});

			 

			ListView actualListView = mPullRefreshListView.getRefreshableView();

			// Need to use the Actual ListView when registering for Context Menu
			RegisterForContextMenu(actualListView);

			mListItems = mStrings.ToList();// <string>();


			//mListItems = mStrings.ToList(); 
			//foreach (var item in mStrings)
			//{
			//    mListItems.AddFirst(item);
			//}

			mAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mListItems);

			/**
			 * Add Sound Event Listener
			 */
			//SoundPullEventListener<ListView> soundListener = new SoundPullEventListener<ListView>(this);
			//soundListener.addSoundEvent(State.PULL_TO_REFRESH, R.raw.pull_event);
			//soundListener.addSoundEvent(State.RESET, R.raw.reset_sound);
			//soundListener.addSoundEvent(State.REFRESHING, R.raw.refreshing_sound);
			//mPullRefreshListView.setOnPullEventListener(soundListener);

			var soundListener = new SoundPullEventListener<ListView>(this);
			soundListener.addSoundEvent(State.PULL_TO_REFRESH, Resource.Raw.pull_event);
			soundListener.addSoundEvent(State.RESET, Resource.Raw.reset_sound);
			soundListener.addSoundEvent(State.REFRESHING, Resource.Raw.refreshing_sound);
			mPullRefreshListView.setOnPullEventListener(soundListener);
			// You can also just use setListAdapter(mAdapter) or
			// mPullRefreshListView.setAdapter(mAdapter)
			actualListView.Adapter = mAdapter;
		}
		private class GetDataTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, string[]>
		{
			private readonly PullToRefreshListActivity inst;

			public GetDataTask(PullToRefreshListActivity instance)
			{
				inst = instance;
			}

			protected override string[] RunInBackground(params Java.Lang.Void[] @params)
			{
				try
				{
					Thread.Sleep(4000);
				}
				catch (InterruptedException)
				{
				}

				return PullToRefreshListActivity.mStrings;
			}

			protected override void OnPostExecute(Java.Lang.Object result)
			{
				inst.mAdapter.Insert("added after refresh:" + DateTime.Now.ToString("t"), 0);
				inst.mAdapter.NotifyDataSetChanged();

				inst.mPullRefreshListView.onRefreshComplete();

				base.OnPostExecute(result);
			}
		}

		//@Override
		public override bool OnCreateOptionsMenu(IMenu menu)
		{

			menu.Add(0, MENU_MANUAL_REFRESH, 0, "Manual Refresh");
			menu.Add(0, MENU_DISABLE_SCROLL, 1,
					mPullRefreshListView.isScrollingWhileRefreshingEnabled() ? "Disable Scrolling while Refreshing"
							: "Enable Scrolling while Refreshing");
			menu.Add(0, MENU_SET_MODE, 0, mPullRefreshListView.getMode() == Mode.BOTH ? "Change to MODE_PULL_DOWN"
					: "Change to MODE_PULL_BOTH");
			menu.Add(0, MENU_DEMO, 0, "Demo");

			return base.OnCreateOptionsMenu(menu);
		}

		//@Override
		public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			AdapterView.AdapterContextMenuInfo info = (AdapterView.AdapterContextMenuInfo)menuInfo;

			menu.SetHeaderTitle("Item: " + this.ListView.GetItemAtPosition(info.Position));
			menu.Add("Item 1");
			menu.Add("Item 2");
			menu.Add("Item 3");
			menu.Add("Item 4");

			base.OnCreateContextMenu(menu, v, menuInfo);
		}

		//@Override
		public bool onPrepareOptionsMenu(IMenu menu)
		{
			IMenuItem disableItem = menu.FindItem(MENU_DISABLE_SCROLL);
			disableItem
					.SetTitle(mPullRefreshListView.isScrollingWhileRefreshingEnabled() ? "Disable Scrolling while Refreshing"
							: "Enable Scrolling while Refreshing");

			IMenuItem setModeItem = menu.FindItem(MENU_SET_MODE);
			setModeItem.SetTitle(mPullRefreshListView.getMode() == Mode.BOTH ? "Change to MODE_FROM_START"
					: "Change to MODE_PULL_BOTH");

			return base.OnPrepareOptionsMenu(menu);
		}

		//@Override
		public override bool OnOptionsItemSelected(IMenuItem item)
		{

			switch (item.ItemId)
			{
				case PullToRefreshListActivity.MENU_MANUAL_REFRESH:
					new GetDataTask(this).Execute();
					mPullRefreshListView.setRefreshing(false);
					break;
				case MENU_DISABLE_SCROLL:
					mPullRefreshListView.setScrollingWhileRefreshingEnabled(!mPullRefreshListView
							.isScrollingWhileRefreshingEnabled());
					break;
				case MENU_SET_MODE:
					mPullRefreshListView.setMode(mPullRefreshListView.getMode() == Mode.BOTH ? Mode.PULL_FROM_START
							: Mode.BOTH);
					break;
				case MENU_DEMO:
					mPullRefreshListView.demo();
					break;
			}

			return base.OnOptionsItemSelected(item);
		}

		public static string[] mStrings = { "Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam", "Abondance", "Ackawi",
			"Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale", "Aisy Cendre",
			"Allgauer Emmentaler", "Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam", "Abondance", "Ackawi",
			"Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale", "Aisy Cendre",
			"Allgauer Emmentaler" };

		public void onRefresh(PullToRefreshBase<ListView> refreshView)
		{
			//string label = DateUtils.formatDateTime(getApplicationContext(), System.currentTimeMillis(),
			//                DateUtils.FORMAT_SHOW_TIME | DateUtils.FORMAT_SHOW_DATE | DateUtils.FORMAT_ABBREV_ALL);

			string label = DateTime.Now.ToString();

			// Update the LastUpdatedLabel
			refreshView.getLoadingLayoutProxy().setLastUpdatedLabel(label);

			// Do work to refresh the list here.
			new GetDataTask(this).Execute();
		}

		//public void onLastItemVisible()
		//{
		//    Toast.MakeText(this, "End of List!", ToastLength.Short).Show();
		//}

		void OnLastItemVisibleListener.onLastItemVisible()
		{
			//throw new NotImplementedException();

			Toast.MakeText(this, "End of List!", ToastLength.Short).Show();
		}
	}



}