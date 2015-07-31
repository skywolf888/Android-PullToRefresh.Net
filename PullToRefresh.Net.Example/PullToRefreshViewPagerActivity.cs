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
//import android.support.v4.view.PagerAdapter;
//import android.support.v4.view.ViewPager;
//import android.view.View;
//import android.view.ViewGroup;
//import android.view.ViewGroup.LayoutParams;
//import android.widget.ImageView;

//import com.handmark.pulltorefresh.extras.viewpager.PullToRefreshViewPager;
//import com.handmark.pulltorefresh.library.PullToRefreshBase;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.OnRefreshListener;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
//using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Handmark.PullToRefresh.Library;
//using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using Android.Support.V4.App;
using Android.Support.V4;
using Android.Support.V4.View;
using Com.Handmark.PullToRefresh.Extras.Viewpager;
using System.Threading;

namespace PullToRefresh.Net.Example
{
     
	[Activity(Label = "PullToRefreshViewPagerActivity")]		 
	public sealed class PullToRefreshViewPagerActivity : Activity, OnRefreshListener<ViewPager>
	{
        
		private PullToRefreshViewPager mPullToRefreshViewPager;

		//@Override
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_ptr_viewpager);

			mPullToRefreshViewPager = (PullToRefreshViewPager)FindViewById(Resource.Id.pull_refresh_viewpager);
			mPullToRefreshViewPager.setOnRefreshListener(this);

			ViewPager vp = mPullToRefreshViewPager.getRefreshableView();
			vp.Adapter =new SamplePagerAdapter();
		}

		//@Override
		public void onRefresh(PullToRefreshBase<ViewPager> refreshView)
		{
			new GetDataTask(this).Execute();
		}

		class SamplePagerAdapter : PagerAdapter
		{

            private static int[] sDrawables = { Resource.Drawable.wallpaper, Resource.Drawable.wallpaper, Resource.Drawable.wallpaper,
				Resource.Drawable.wallpaper, Resource.Drawable.wallpaper, Resource.Drawable.wallpaper };

			//@Override

            public override int Count
            {
                get { return sDrawables.Length; }
            }

            public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
            {
                ImageView imageView = new ImageView(container.Context);
                imageView.SetImageResource(sDrawables[position]);

                // Now just add ImageView to ViewPager and return it
                container.AddView(imageView, Android.Views.ViewGroup.LayoutParams.MatchParent, Android.Views.ViewGroup.LayoutParams.MatchParent);

                return imageView;
            }            
			//@Override

            public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
            {
                container.RemoveView((View) @object);
            }


            public override bool IsViewFromObject(View view, Java.Lang.Object @object)
            {
                return view ==  @object;
            }
			
		}

		private class GetDataTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, Java.Lang.Void>
		{


            PullToRefreshViewPagerActivity inst;

            public GetDataTask(PullToRefreshViewPagerActivity instance)
            {
                inst = instance;
            }


            protected override Java.Lang.Void RunInBackground(params Java.Lang.Void[] @params)
            {
                try
                {
                    Thread.Sleep(4000);
                }
                catch (Java.Lang.InterruptedException e)
                {
                }
                return null;
            }
			//@Override
			//protected Void doInBackground(Void... params) {
			//    // Simulates a background job.
			//    try {
			//        Thread.sleep(4000);
			//    } catch (InterruptedException e) {
			//    }
			//    return null;
			//}

            protected override void OnPostExecute(Java.Lang.Object result)
            {
                inst.mPullToRefreshViewPager.onRefreshComplete();
                base.OnPostExecute(result);
            }

			 
		}

	}
}