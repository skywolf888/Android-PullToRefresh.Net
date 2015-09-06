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
//package com.handmark.pulltorefresh.library;

//import android.annotation.TargetApi;
//import android.content.Context;
//import android.os.Build.VERSION;
//import android.os.Build.VERSION_CODES;
//import android.util.AttributeSet;
//import android.view.View;
//import android.widget.ScrollView;

using Android.Content;
using Android.Widget;
using PTROrientation = Com.Handmark.PullToRefresh.Library.PtrOrientation;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using Android.Util;
using Android.Views;
using System;
using Android.Annotation;


namespace Com.Handmark.PullToRefresh.Library
{

    public class PullToRefreshScrollView : PullToRefreshBase<ScrollView>
    {

        public PullToRefreshScrollView(Context context)
            : base(context)
        {
        }

        public PullToRefreshScrollView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {

        }

        public PullToRefreshScrollView(Context context, Mode mode)
            : base(context, mode)
        {

        }

        public PullToRefreshScrollView(Context context, Mode mode, AnimationStyle style)
            : base(context, mode, style)
        {

        }

        //@Override
        public override PTROrientation getPullToRefreshScrollDirection()
        {
            return PTROrientation.VERTICAL;
        }

        //@Override
        protected override ScrollView createRefreshableView(Context context, IAttributeSet attrs)
        {
            ScrollView scrollView;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Gingerbread)
            {
                scrollView = new InternalScrollViewSDK9(context, attrs, this);
            }
            else
            {
                scrollView = new ScrollView(context, attrs);
            }

            scrollView.Id = Resource.Id.scrollview;
            return scrollView;
        }

        //@Override
        protected override bool isReadyForPullStart()
        {
            return mRefreshableView.ScrollY == 0;
        }

        //@Override
        protected override bool isReadyForPullEnd()
        {
            View scrollViewChild = mRefreshableView.GetChildAt(0);
            if (null != scrollViewChild)
            {
                return mRefreshableView.ScrollY >= (scrollViewChild.Height - Height);
            }
            return false;
        }

        [TargetApi(Value=9)]
        class InternalScrollViewSDK9 : ScrollView
        {
            PullToRefreshScrollView inst;
            public InternalScrollViewSDK9(Context context, IAttributeSet attrs, PullToRefreshScrollView instance)
                : base(context, attrs)
            {
                //super(context, attrs);
                inst = instance;
            }

            //@Override
            protected override bool OverScrollBy(int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX,
                    int scrollRangeY, int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
            {

                bool returnValue = base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX,
                        scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);

                // Does all of the hard work...
                OverscrollHelper.overScrollBy(inst, deltaX, scrollX, deltaY, scrollY,
                        getScrollRange(), isTouchEvent);

                return returnValue;
            }

            /**
             * Taken from the AOSP ScrollView source
             */
            private int getScrollRange()
            {
                int scrollRange = 0;
                if (ChildCount > 0)
                {
                    View child = GetChildAt(0);
                    scrollRange = Math.Max(0, child.Height - (Height - PaddingBottom - PaddingTop));
                }
                return scrollRange;
            }
        }
    }

}