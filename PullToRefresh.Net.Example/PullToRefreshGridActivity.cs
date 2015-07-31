/*******************************************************************************
 * Copyright 2011, 2012 Chris Banes.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/
//package com.handmark.pulltorefresh.samples;

//import java.util.Arrays;
//import java.util.LinkedList;

//import android.app.Activity;
//import android.os.AsyncTask;
//import android.os.Bundle;
//import android.view.Gravity;
//import android.view.Menu;
//import android.view.MenuItem;
//import android.widget.ArrayAdapter;
//import android.widget.GridView;
//import android.widget.TextView;
//import android.widget.Toast;

//import com.handmark.pulltorefresh.library.PullToRefreshBase;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.Mode;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.OnRefreshListener2;
//import com.handmark.pulltorefresh.library.PullToRefreshGridView;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Handmark.PullToRefresh.Library;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;

namespace PullToRefresh.Net.Example
{
    [Activity(Label = "PullToRefreshGridActivity")]
    public sealed class PullToRefreshGridActivity : Activity, OnRefreshListener2<GridView>
    {

        public const int MENU_SET_MODE = 0;

        private IList<string> mListItems;
        private PullToRefreshGridView mPullRefreshGridView;
        private GridView mGridView;
        private ArrayAdapter<string> mAdapter;

        /** Called when the activity is first created. */
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_ptr_grid);

            mPullRefreshGridView = (PullToRefreshGridView)FindViewById(Resource.Id.pull_refresh_grid);
            mGridView = mPullRefreshGridView.getRefreshableView();

            mPullRefreshGridView.setOnRefreshListener(this);

            // Set a listener to be invoked when the list should be refreshed.
            //mPullRefreshGridView.setOnRefreshListener(new OnRefreshListener2<GridView>() {

            //    @Override
            //    public void onPullDownToRefresh(PullToRefreshBase<GridView> refreshView) {
            //        Toast.makeText(PullToRefreshGridActivity.this, "Pull Down!", Toast.LENGTH_SHORT).show();
            //        new GetDataTask().execute();
            //    }

            //    @Override
            //    public void onPullUpToRefresh(PullToRefreshBase<GridView> refreshView) {
            //        Toast.makeText(PullToRefreshGridActivity.this, "Pull Up!", Toast.LENGTH_SHORT).show();
            //        new GetDataTask().execute();
            //    }

            //});

            mListItems = mStrings.ToList() ;//new List<string>();
            
            TextView tv = new TextView(this);
            tv.Gravity = GravityFlags.Center;
            tv.Text = "Empty View, Pull Down/Up to Add Items";
            mPullRefreshGridView.setEmptyView(tv);
            
            mAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mListItems);
            mGridView.Adapter=mAdapter;
        }

        private class GetDataTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, string[]> {
            private PullToRefreshGridActivity inst;

            public GetDataTask(PullToRefreshGridActivity instance)
            {
                inst = instance;
            }

            //@Override
            //protected string[] doInBackground(Void... params) {
            //    // Simulates a background job.
            //    try {
            //        Thread.sleep(2000);
            //    } catch (InterruptedException e) {
            //    }
            //    return mStrings;
            //}

            protected override string[] RunInBackground(params Java.Lang.Void[] @params)
            {
                // Simulates a background job.
                try
                {
                    Thread.Sleep(2000);
                }
                catch (Java.Lang.InterruptedException e)
                {
                }
                return mStrings;
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {
                inst.mListItems.Add("Added after refresh..."+System.DateTime.Now.ToString());
                //inst.mAdapter.Add("Added after refresh...");
                string[] data = mStrings;//(string[])result.instance;

                foreach (var item in data )
                {
                    inst.mListItems.Add(item);
                }
                //inst.mAdapter.NotifyDataSetChanged();
                 
                inst.mAdapter = new ArrayAdapter<string>(inst, Android.Resource.Layout.SimpleListItem1, inst.mListItems);
                inst.mGridView.Adapter = inst.mAdapter;

                // Call onRefreshComplete when the list has been refreshed.
                inst.mPullRefreshGridView.onRefreshComplete();

                base.OnPostExecute(result);
            }

             
        }

        //@Override
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(0, MENU_SET_MODE, 0,
                    mPullRefreshGridView.getMode() == Mode.BOTH ? "Change to MODE_PULL_DOWN"
                            : "Change to MODE_PULL_BOTH");
            return base.OnCreateOptionsMenu(menu);
        }

        //@Override
        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            IMenuItem setModeItem = menu.FindItem(MENU_SET_MODE);
            setModeItem.SetTitle(mPullRefreshGridView.getMode() == Mode.BOTH ? "Change to MODE_PULL_FROM_START"
                    : "Change to MODE_PULL_BOTH");

            return base.OnPrepareOptionsMenu(menu);
        }

        //@Override
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case MENU_SET_MODE:
                    mPullRefreshGridView
                            .setMode(mPullRefreshGridView.getMode() == Mode.BOTH ? Mode.PULL_FROM_START
                                    : Mode.BOTH);
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        static string[] mStrings = { "Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam", "Abondance", "Ackawi",
			"Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale", "Aisy Cendre",
			"Allgauer Emmentaler" };

        public void onPullDownToRefresh(PullToRefreshBase<GridView> refreshView)
        {

            Toast.MakeText(this, "Pull Down!", ToastLength.Short).Show();
            new GetDataTask(this).Execute();
        }

        public void onPullUpToRefresh(PullToRefreshBase<GridView> refreshView)
        {
            Toast.MakeText(this, "Pull Up!", ToastLength.Short).Show();
                new GetDataTask(this).Execute();
        }

         
    }
}