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

//import android.os.AsyncTask;
//import android.os.Bundle;
//import android.support.v4.app.FragmentActivity;
//import android.widget.ArrayAdapter;
//import android.widget.ListView;

//import com.handmark.pulltorefresh.extras.listfragment.PullToRefreshListFragment;
//import com.handmark.pulltorefresh.library.PullToRefreshBase;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.OnRefreshListener;
//import com.handmark.pulltorefresh.library.PullToRefreshListView;

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

using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using Java.Lang;
using Com.Handmark.PullToRefresh.Library;
using Com.Handmark.PullToRefresh.Extras.Listfragment;
using Android.Support.V4.App;

namespace PullToRefresh.Net.Example
{

    [Activity(Label = "PullToRefreshListFragmentActivity")]
    public sealed class PullToRefreshListFragmentActivity : FragmentActivity, OnRefreshListener<ListView>
    {

        private IList<string> mListItems;
        private ArrayAdapter<string> mAdapter;

        private PullToRefreshListFragment mPullRefreshListFragment;
        private PullToRefreshListView mPullRefreshListView;

        /** Called when the activity is first created. */
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_ptr_list_fragment);

            mPullRefreshListFragment = (PullToRefreshListFragment)SupportFragmentManager.FindFragmentById(
                    Resource.Id.frag_ptr_list);

            // Get PullToRefreshListView from Fragment
            mPullRefreshListView = mPullRefreshListFragment.getPullToRefreshListView();

            // Set a listener to be invoked when the list should be refreshed.
            mPullRefreshListView.setOnRefreshListener(this);

            // You can also just use mPullRefreshListFragment.getListView()
            ListView actualListView = mPullRefreshListView.getRefreshableView();

            mListItems = mStrings.ToList();
            //mListItems.addAll(Arrays.asList(mStrings));
            mAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mListItems);

            // You can also just use setListAdapter(mAdapter) or
            // mPullRefreshListView.setAdapter(mAdapter)
            //actualListView.SetAdapter(mAdapter);
            actualListView.Adapter = mAdapter;
            mPullRefreshListFragment.SetListShown(true);

        }

        //@Override
        public void onRefresh(PullToRefreshBase<ListView> refreshView)
        {


            // Do work to refresh the list here.
            new GetDataTask(this).Execute();
        }

        private class GetDataTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, string[]>
        {


            PullToRefreshListFragmentActivity inst;
            public GetDataTask(PullToRefreshListFragmentActivity instance)
            {
                inst = instance;
            }

            protected override string[] RunInBackground(params Java.Lang.Void[] @params)
            {
                try
                {
                    Thread.Sleep(4000);
                }
                catch (InterruptedException e)
                {
                }
                return mStrings;
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {
                inst.mListItems.Add("Added after refresh..." + DateTime.Now.ToString());
                inst.mAdapter.NotifyDataSetChanged();

                // Call onRefreshComplete when the list has been refreshed.
                inst.mPullRefreshListView.onRefreshComplete();

                base.OnPostExecute(result);
            }

        }

        private static readonly string[] mStrings = { "Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam", "Abondance", "Ackawi",
			"Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale", "Aisy Cendre",
			"Allgauer Emmentaler", "Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam", "Abondance", "Ackawi",
			"Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale", "Aisy Cendre",
			"Allgauer Emmentaler" };
    }
}