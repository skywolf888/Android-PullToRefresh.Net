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
//package com.handmark.pulltorefresh.extras.viewpager;

//import android.content.Context;
//import android.support.v4.view.PagerAdapter;
//import android.support.v4.view.ViewPager;
//import android.util.AttributeSet;

//import com.handmark.pulltorefresh.library.PullToRefreshBase;


using Android.OS;
using Android.Views;
using Android.Support.V4.View;
using Android.Content;
using Android.Util;
using PTROrientation = Com.Handmark.PullToRefresh.Library.PtrOrientation;
using Com.Handmark.PullToRefresh.Library;


namespace Com.Handmark.PullToRefresh.Extras.Viewpager
{
    public class PullToRefreshViewPager : PullToRefreshBase<ViewPager>
    {

        public PullToRefreshViewPager(Context context)
            : base(context)
        {
            //super(context);
        }

        public PullToRefreshViewPager(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            //super(context, attrs);
        }

        //@Override
        public override PTROrientation getPullToRefreshScrollDirection()
        {
            return PTROrientation.HORIZONTAL;
        }

        //@Override
        protected override ViewPager createRefreshableView(Context context, IAttributeSet attrs)
        {
            ViewPager viewPager = new ViewPager(context, attrs);
            viewPager.Id = Resource.Id.viewpager;
            return viewPager;
        }

        //@Override
        protected override bool isReadyForPullStart()
        {
            ViewPager refreshableView = getRefreshableView();

            PagerAdapter adapter = refreshableView.Adapter;
            if (null != adapter)
            {
                return refreshableView.CurrentItem == 0;
            }

            return false;
        }

        //@Override
        protected override bool isReadyForPullEnd()
        {
            ViewPager refreshableView = getRefreshableView();

            PagerAdapter adapter = refreshableView.Adapter;
            if (null != adapter)
            {
                return refreshableView.CurrentItem == adapter.Count - 1;
            }

            return false;
        }
    }
}