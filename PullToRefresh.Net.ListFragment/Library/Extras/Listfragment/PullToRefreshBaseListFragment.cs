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
//package com.handmark.pulltorefresh.extras.listfragment;

//import android.os.Bundle;
//import android.support.v4.app.ListFragment;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.AbsListView;
//import android.widget.ListView;

//import com.handmark.pulltorefresh.library.PullToRefreshBase;

using Android.Widget;

using Android.Views;
using Android.OS;
using Android.Support.V4.App;
using Com.Handmark.PullToRefresh.Library;

namespace Com.Handmark.PullToRefresh.Extras.Listfragment
{
    public abstract class PullToRefreshBaseListFragment<U,T> : ListFragment  where T : PullToRefreshAdapterViewBase<U> where U:AbsListView
    {

        private T mPullToRefreshListView;

        //@Override
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View layout = base.OnCreateView(inflater, container, savedInstanceState);

            ListView lv = (ListView)layout.FindViewById(Android.Resource.Id.List);
            ViewGroup parent = (ViewGroup)lv.Parent;
             
            // Remove ListView and add PullToRefreshListView in its place
            int lvIndex = parent.IndexOfChild(lv);
            parent.RemoveViewAt(lvIndex);
            mPullToRefreshListView = onCreatePullToRefreshListView(inflater, savedInstanceState);
            parent.AddView(mPullToRefreshListView, lvIndex, lv.LayoutParameters);

            return layout;
        }

        /**
         * @return The {@link PullToRefreshBase} attached to this ListFragment.
         */
        public T getPullToRefreshListView()
        {
            return mPullToRefreshListView;
        }

        /**
         * Returns the {@link PullToRefreshBase} which will replace the ListView
         * created from ListFragment. You should override this method if you wish to
         * customise the {@link PullToRefreshBase} from the default.
         * 
         * @param inflater - LayoutInflater which can be used to inflate from XML.
         * @param savedInstanceState - Bundle passed through from
         *            {@link ListFragment#onCreateView(LayoutInflater, ViewGroup, Bundle)
         *            onCreateView(...)}
         * @return The {@link PullToRefreshBase} which will replace the ListView.
         */
        protected abstract T onCreatePullToRefreshListView(LayoutInflater inflater, Bundle savedInstanceState);

    }
}