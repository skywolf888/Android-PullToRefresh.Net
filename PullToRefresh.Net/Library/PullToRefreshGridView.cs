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
//import android.widget.GridView;

//import com.handmark.pulltorefresh.library.Inner.InternalGridView;
//import com.handmark.pulltorefresh.library.Inner.InternalGridViewSDK9;


using Android.Content;
using Android.Widget;
using PtrOrientation = Com.Handmark.PullToRefresh.Library.PtrOrientation;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using Android.Util;
using Android.Views;
using Com.Handmark.PullToRefresh.Library.Internal;

namespace Com.Handmark.PullToRefresh.Library
{

    public class PullToRefreshGridView : PullToRefreshAdapterViewBase<GridView>
    {

        public PullToRefreshGridView(Context context)
            : base(context)
        {
            //super(context);
        }

        public PullToRefreshGridView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            //super(context, attrs);
        }

        public PullToRefreshGridView(Context context, Mode mode)
            : base(context, mode)
        {
            //super(context, mode);
        }

        public PullToRefreshGridView(Context context, Mode mode, AnimationStyle style)
            : base(context, mode, style)
        {
            //super(context, mode, style);
        }

        public override PtrOrientation getPullToRefreshScrollDirection()
        {
            return PtrOrientation.VERTICAL;
        }

        //@Override
        protected override GridView createRefreshableView(Context context, IAttributeSet attrs)
        {
            GridView gv;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Gingerbread)
            {
                gv = new InternalGridViewSDK9(context, attrs, this);
            }
            else
            {
                gv = new InternalGridView(context, attrs, this);
            }

            // Use Generated ID (from res/values/ids.xml)
            gv.Id = Resource.Id.gridview;
            return gv;
        }

        public class InternalGridView : GridView, IEmptyViewMethodAccessor
        {

            protected PullToRefreshGridView inst;
            public InternalGridView(Context context, IAttributeSet attrs, PullToRefreshGridView instance)
                : base(context, attrs)
            {
                //super(context, attrs);
                inst = instance;
            }

            //@Override
            public void setEmptyView(View emptyView)
            {
                inst.setEmptyView(emptyView);
            }

            //@Override
            public void setEmptyViewInternal(View emptyView)
            {
                base.EmptyView = emptyView;
            }
        }

        //@TargetApi(9)
        public class InternalGridViewSDK9 : InternalGridView
        {

            public InternalGridViewSDK9(Context context, IAttributeSet attrs, PullToRefreshGridView instance)
                : base(context, attrs, instance)
            {

            }

            //@Override
            protected override bool OverScrollBy(int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX,
                    int scrollRangeY, int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
            {


                bool returnValue = base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX,
                        scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);

                //Does all of the hard work...
                OverscrollHelper.overScrollBy(inst, deltaX, scrollX, deltaY, scrollY, isTouchEvent);

                return returnValue;
            }
        }
    }

}