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

//import android.app.Activity;
//import android.os.AsyncTask;
//import android.os.Bundle;
//import android.widget.ScrollView;

//import com.handmark.pulltorefresh.library.PullToRefreshBase;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.OnRefreshListener;
//import com.handmark.pulltorefresh.library.PullToRefreshScrollView;
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
    [Activity(Label = "PullToRefreshScrollViewActivity")]
    public sealed class PullToRefreshScrollViewActivity : Activity, OnRefreshListener<ScrollView>
    {

        PullToRefreshScrollView mPullRefreshScrollView;
        ScrollView mScrollView;

        /** Called when the activity is first created. */
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_ptr_scrollview);

            mPullRefreshScrollView = (PullToRefreshScrollView)FindViewById(Resource.Id.pull_refresh_scrollview);
            mPullRefreshScrollView.setOnRefreshListener(this);
            //mPullRefreshScrollView.setOnRefreshListener(new OnRefreshListener<ScrollView>() {

            //    @Override
            //    public void onRefresh(PullToRefreshBase<ScrollView> refreshView) {
            //        new GetDataTask().execute();
            //    }
            //});

            mScrollView = mPullRefreshScrollView.getRefreshableView();

        }

        private class GetDataTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, string[]>
        {
            private PullToRefreshScrollViewActivity inst;

            ////@Override
            //protected string[] doInBackground(Void... params) {
            //    // Simulates a background job.
            //    try {
            //        Thread.sleep(4000);
            //    } catch (InterruptedException e) {
            //    }
            //    return null;
            //}

            public GetDataTask(PullToRefreshScrollViewActivity instance)
            {
                inst = instance;
            }


            protected override string[] RunInBackground(params Java.Lang.Void[] @params)
            {
                //Simulates a background job.
                try
                {
                    Thread.Sleep(4000);
                }
                catch (InterruptedException e)
                {
                }
                return null;
            }


            //@Override
            protected override void OnPostExecute(Java.Lang.Object result)
            {
                // Do some stuff here

                // Call onRefreshComplete when the list has been refreshed.
                inst.mPullRefreshScrollView.onRefreshComplete();

                base.OnPostExecute(result);
            }
        }


        public void onRefresh(PullToRefreshBase<ScrollView> refreshView)
        {
            new GetDataTask(this).Execute();
        }


    }
}