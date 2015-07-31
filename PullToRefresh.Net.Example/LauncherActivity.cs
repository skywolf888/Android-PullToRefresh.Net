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

//import android.app.ListActivity;
//import android.content.Intent;
//import android.os.Bundle;
//import android.view.View;
//import android.widget.ArrayAdapter;
//import android.widget.ListView;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content;

namespace PullToRefresh.Net.Example
{
    [Activity(Label = "PullToRefresh.Net.Example", MainLauncher = true, Icon = "@drawable/icon")]
    public class LauncherActivity : ListActivity
    {

        public static string[] options = { "ListView", "ExpandableListView", "GridView", "WebView", "ScrollView",
			"Horizontal ScrollView", "ViewPager", "ListView Fragment", "WebView Advanced", "ListView in ViewPager" };

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, options);			
        }



        //@Override
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
			
            Intent intent;

            switch (position)
            {
                default:
                case 0:
                    intent = new Intent(this, typeof(PullToRefreshListActivity));
                    break;
                case 1:
                    intent = new Intent(this, typeof(PullToRefreshExpandableListActivity));
                    break;
                case 2:
                    intent = new Intent(this, typeof(PullToRefreshGridActivity));
                    break;
                case 3:
                    intent = new Intent(this, typeof(PullToRefreshWebViewActivity));
                    break;
                case 4:
                    intent = new Intent(this, typeof(PullToRefreshScrollViewActivity));
                    break;
                case 5:
                    intent = new Intent(this, typeof(PullToRefreshHorizontalScrollViewActivity));
                    break;
                case 6:                    
                    intent = new Intent(this, typeof(PullToRefreshViewPagerActivity));
                    break;
                case 7:                     
                    intent = new Intent(this, typeof(PullToRefreshListFragmentActivity));
                    break;
                //case 8:
                //    intent = new Intent(this, PullToRefreshWebView2Activity.class);
                //    break;
                case 9:
                    intent = new Intent(this, typeof(PullToRefreshListInViewPagerActivity));
                    break;
            }

            StartActivity(intent);
        }

    }
}