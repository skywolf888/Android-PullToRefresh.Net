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
//import android.widget.ExpandableListView;

//import com.handmark.pulltorefresh.library.internal.EmptyViewMethodAccessor;


using Android.Content;
using Android.Widget;
using PTROrientation = Com.Handmark.PullToRefresh.Library.PtrOrientation;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using Android.Util;
using Android.Views;
using Com.Handmark.PullToRefresh.Library.Internal;
using Android.Annotation;

namespace Com.Handmark.PullToRefresh.Library
{
    public class PullToRefreshExpandableListView : PullToRefreshAdapterViewBase<ExpandableListView>
    {

        public PullToRefreshExpandableListView(Context context)
            : base(context)
        {
            //super(context);
        }

        public PullToRefreshExpandableListView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            //super(context, attrs);
        }

        public PullToRefreshExpandableListView(Context context, Mode mode)
            : base(context, mode)
        {
            //super(context, mode);
        }

        public PullToRefreshExpandableListView(Context context, Mode mode, AnimationStyle style)
            : base(context, mode, style)
        {
            //super(context, mode, style);
        }

        //@Override
        public override PTROrientation getPullToRefreshScrollDirection()
        {
            return PTROrientation.VERTICAL;
        }

        //@Override
        protected override ExpandableListView createRefreshableView(Context context, IAttributeSet attrs)
        {
            ExpandableListView lv;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Gingerbread)
            {
                lv = new InternalExpandableListViewSDK9(context, attrs, this);
            }
            else
            {
                lv = new InternalExpandableListView(context, attrs, this);
            }

            // Set it to this so it can be used in ListActivity/ListFragment
            lv.Id = Android.Resource.Id.List;
            return lv;
        }

        public class InternalExpandableListView : ExpandableListView, IEmptyViewMethodAccessor
        {

            protected PullToRefreshExpandableListView inst;
            public InternalExpandableListView(Context context, IAttributeSet attrs, PullToRefreshExpandableListView instance)
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

        [TargetApi(Value=9)]
        public class InternalExpandableListViewSDK9 : InternalExpandableListView
        {

            public InternalExpandableListViewSDK9(Context context, IAttributeSet attrs, PullToRefreshExpandableListView instance)
                : base(context, attrs, instance)
            {
                //super(context, attrs,instance);
            }

            //@Override
            protected override bool OverScrollBy(int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX,
                    int scrollRangeY, int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
            {

                bool returnValue = base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX,
                        scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);

                // Does all of the hard work...
                //OverscrollHelper.overScrollBy(inst, deltaX, scrollX, deltaY, scrollY,isTouchEvent);

                OverscrollHelper.overScrollBy(inst, deltaX, scrollX, deltaY, scrollY, isTouchEvent);
                return returnValue;
            }
        }
    }

}