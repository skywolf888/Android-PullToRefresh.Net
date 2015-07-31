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

//import java.util.ArrayList;
//import java.util.HashMap;
//import java.util.List;
//import java.util.Map;

//import android.app.ExpandableListActivity;
//import android.os.AsyncTask;
//import android.os.Bundle;
//import android.widget.ExpandableListView;
//import android.widget.SimpleExpandableListAdapter;

//import com.handmark.pulltorefresh.library.PullToRefreshBase;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.OnRefreshListener;
//import com.handmark.pulltorefresh.library.PullToRefreshExpandableListView;

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Handmark.PullToRefresh.Library;
using Java.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;

namespace PullToRefresh.Net.Example
{
    [Activity(Label = "PullToRefreshExpandableListActivity")]
    public sealed class PullToRefreshExpandableListActivity : ExpandableListActivity, OnRefreshListener<ExpandableListView>
    {
        private const string KEY = "key";

        //public IList<IDictionary<string, object>> groupData = new List<Dictionary<string, object>>() as IList<IDictionary<string, object>>;
        private IList<IDictionary<string, object>> groupData = new List<IDictionary<string,object>>();

        private IList<IList<IDictionary<string, object>>> childData = new List<IList<IDictionary<string, object>>>();

        private PullToRefreshExpandableListView mPullRefreshListView;
        private SimpleExpandableListAdapter mAdapter;

        /** Called when the activity is first created. */
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_ptr_expandable_list);

            mPullRefreshListView = (PullToRefreshExpandableListView)FindViewById(Resource.Id.pull_refresh_expandable_list);
            mPullRefreshListView.setOnRefreshListener(this);
            // Set a listener to be invoked when the list should be refreshed.
            //mPullRefreshListView.setOnRefreshListener(new OnRefreshListener<ExpandableListView>() {
            //    @Override
            //    public void onRefresh(PullToRefreshBase<ExpandableListView> refreshView) {
            //        // Do work to refresh the list here.
            //        new GetDataTask().execute();
            //    }
            //});

            foreach (string group in mGroupStrings)
            {

                IDictionary<string, object> groupMap1 =new JavaDictionary<string,object>();//  new Dictionary<string, object>();
                                
                groupData.Add(groupMap1);
                groupMap1.Add(KEY, group);

                IList<IDictionary<string, object>> childList = new JavaList<IDictionary<string, object>>();
                foreach (string child in mChildStrings)
                {
                    IDictionary<string, object> childMap = new JavaDictionary<string, object>();
                    childList.Add(childMap);
                    childMap.Add(KEY, child);
                }
                childData.Add(childList);
            }
             
            mAdapter = new SimpleExpandableListAdapter(this, groupData, Android.Resource.Layout.SimpleExpandableListItem1,
                    new string[] { KEY }, new int[] { Android.Resource.Id.Text1 }, childData,
                    Android.Resource.Layout.SimpleExpandableListItem2, new string[] { KEY }, new int[] { Android.Resource.Id.Text1 });
            SetListAdapter(mAdapter);
            
        }

        private class GetDataTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, string[]>
        {
            private PullToRefreshExpandableListActivity inst;

            //@Override
            //protected String[] doInBackground(Void... params) {
            //    // Simulates a background job.
            //    try {
            //        Thread.sleep(2000);
            //    } catch (Java.Lang.InterruptedException e) {
            //    }
            //    return mChildStrings;
            //}

            public GetDataTask(PullToRefreshExpandableListActivity instance)
            {
                inst = instance;
            }

            protected override string[] RunInBackground(params Java.Lang.Void[] @params)
            {
                try
                {
                    Thread.Sleep(2000);
                }
                catch (Java.Lang.InterruptedException e)
                {
                }
                return mChildStrings;
            }

            //@Override
            protected override void OnPostExecute(Java.Lang.Object result)
            {

                IDictionary<string, object> newMap = new JavaDictionary<string, object>();
                newMap.Add(KEY, "Added after refresh...");

                inst.groupData.Add(newMap);

                IList<IDictionary<string, object>> childList = new List<IDictionary<string, object>>();
                foreach (string child in mChildStrings)
                {
                    IDictionary<string, object> childMap = new JavaDictionary<string, object>();
                    childMap.Add(KEY, child);
                    childList.Add(childMap);
                }
                inst.childData.Add(childList);

                inst.mAdapter = new SimpleExpandableListAdapter(inst, inst.groupData, Android.Resource.Layout.SimpleExpandableListItem1,
                    new string[] { KEY }, new int[] { Android.Resource.Id.Text1 }, inst.childData,
                    Android.Resource.Layout.SimpleExpandableListItem2, new string[] { KEY }, new int[] { Android.Resource.Id.Text1 });
                inst.SetListAdapter(inst.mAdapter);

                // Call onRefreshComplete when the list has been refreshed.
                inst.mPullRefreshListView.onRefreshComplete();

                base.OnPostExecute(result);
            }
        }

        static string[] mChildStrings = { "Child One", "Child Two", "Child Three", "Child Four", "Child Five", "Child Six" };

        static string[] mGroupStrings = { "Group One", "Group Two", "Group Three" };

        public void onRefresh(PullToRefreshBase<ExpandableListView> refreshView)
        {
            new GetDataTask(this).Execute();
        }
    }

}