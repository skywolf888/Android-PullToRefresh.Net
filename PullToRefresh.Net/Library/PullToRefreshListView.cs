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
//import android.content.res.TypedArray;
//import android.graphics.Canvas;
//import android.os.Build.VERSION;
//import android.os.Build.VERSION_CODES;
//import android.util.AttributeSet;
//import android.view.Gravity;
//import android.view.MotionEvent;
//import android.view.View;
//import android.widget.FrameLayout;
//import android.widget.ListAdapter;
//import android.widget.ListView;

//import com.handmark.pulltorefresh.library.internal.EmptyViewMethodAccessor;
//import com.handmark.pulltorefresh.library.internal.LoadingLayout;

using System;

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using PTROrientation = Com.Handmark.PullToRefresh.Library.PtrOrientation;
using Com.Handmark.PullToRefresh.Library.Internal;
using Android.Annotation;


namespace Com.Handmark.PullToRefresh.Library
{

    public class PullToRefreshListView : PullToRefreshAdapterViewBase<ListView>
    {

        private LoadingLayout mHeaderLoadingView;
        private LoadingLayout mFooterLoadingView;

        private FrameLayout mLvFooterLoadingFrame;

        private bool mListViewExtrasEnabled;


        public PullToRefreshListView(Context context)
            : base(context)
        {
            //super(context);
        }

        public PullToRefreshListView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            //super(context, attrs);
        }

        public PullToRefreshListView(Context context, Mode mode)
            : base(context, mode)
        {
            //super(context, mode);
        }

        public PullToRefreshListView(Context context, Mode mode, AnimationStyle style)
            : base(context, mode, style)
        {
            //super(context, mode, style);
        }

        //@Override
        public override PtrOrientation getPullToRefreshScrollDirection()
        {
            return PTROrientation.VERTICAL;
        }

        //@Override
        protected override void onRefreshing(bool doScroll)
        {
            /**
             * If we're not showing the Refreshing view, or the list is empty, the
             * the header/footer views won't show so we use the normal method.
             */
            IListAdapter adapter = mRefreshableView.Adapter;
            if (!mListViewExtrasEnabled || !getShowViewWhileRefreshing() || null == adapter || adapter.IsEmpty)
            {
                base.onRefreshing(doScroll);
                return;
            }

            base.onRefreshing(false);

            LoadingLayout origLoadingView, listViewLoadingView, oppositeListViewLoadingView;
            int selection, scrollToY;

            switch (getCurrentMode())
            {
                case Mode.MANUAL_REFRESH_ONLY:
                case Mode.PULL_FROM_END:
                    origLoadingView = getFooterLayout();
                    listViewLoadingView = mFooterLoadingView;
                    oppositeListViewLoadingView = mHeaderLoadingView;
                    selection = mRefreshableView.Count - 1;

                    scrollToY = ScrollY - getFooterSize();
                    break;
                case Mode.PULL_FROM_START:
                default:
                    origLoadingView = getHeaderLayout();
                    listViewLoadingView = mHeaderLoadingView;
                    oppositeListViewLoadingView = mFooterLoadingView;
                    selection = 0;
                    scrollToY = ScrollY + getHeaderSize();
                    break;
            }

            // Hide our original Loading View
            origLoadingView.reset();
            origLoadingView.hideAllViews();

            // Make sure the opposite end is hidden too
            oppositeListViewLoadingView.Visibility = ViewStates.Gone;

            // Show the ListView Loading View and set it to refresh.
            listViewLoadingView.Visibility = ViewStates.Visible;
            listViewLoadingView.refreshing();

            if (doScroll)
            {
                // We need to disable the automatic visibility changes for now
                disableLoadingLayoutVisibilityChanges();

                // We scroll slightly so that the ListView's header/footer is at the
                // same Y position as our normal header/footer
                setHeaderScroll(scrollToY);

                // Make sure the ListView is scrolled to show the loading
                // header/footer
                mRefreshableView.SetSelection(selection);


                // Smooth scroll as normal
                smoothScrollTo(0);
            }
        }

        //@Override
        protected override void onReset()
        {
            /**
             * If the extras are not enabled, just call up to super and return.
             */
            if (!mListViewExtrasEnabled)
            {
                base.onReset();
                return;
            }

            LoadingLayout originalLoadingLayout, listViewLoadingLayout;
            int scrollToHeight, selection;
            bool scrollLvToEdge;

            switch (getCurrentMode())
            {
                case Mode.MANUAL_REFRESH_ONLY:
                case Mode.PULL_FROM_END:
                    originalLoadingLayout = getFooterLayout();
                    listViewLoadingLayout = mFooterLoadingView;
                    selection = mRefreshableView.Count - 1;
                    scrollToHeight = getFooterSize();
                    scrollLvToEdge = Math.Abs(mRefreshableView.LastVisiblePosition - selection) <= 1;
                    break;
                case Mode.PULL_FROM_START:
                default:
                    originalLoadingLayout = getHeaderLayout();
                    listViewLoadingLayout = mHeaderLoadingView;
                    scrollToHeight = -getHeaderSize();
                    selection = 0;
                    scrollLvToEdge = Math.Abs(mRefreshableView.FirstVisiblePosition - selection) <= 1;
                    break;
            }

            // If the ListView header loading layout is showing, then we need to
            // flip so that the original one is showing instead
            if (listViewLoadingLayout.Visibility == ViewStates.Visible)
            {

                // Set our Original View to Visible
                originalLoadingLayout.showInvisibleViews();

                // Hide the ListView Header/Footer
                listViewLoadingLayout.Visibility = ViewStates.Gone;

                /**
                 * Scroll so the View is at the same Y as the ListView
                 * header/footer, but only scroll if: we've pulled to refresh, it's
                 * positioned correctly
                 */
                if (scrollLvToEdge && getState() != State.MANUAL_REFRESHING)
                {
                    mRefreshableView.SetSelection(selection);
                    setHeaderScroll(scrollToHeight);
                }
            }

            // Finally, call up to super
            base.onReset();
        }

        //@Override
        protected override LoadingLayoutProxy createLoadingLayoutProxy(bool includeStart, bool includeEnd)
        {
            LoadingLayoutProxy proxy = base.createLoadingLayoutProxy(includeStart, includeEnd);

            if (mListViewExtrasEnabled)
            {
                Mode mode = getMode();

                if (includeStart && PtrModeHelper.showHeaderLoadingLayout(mode))
                {
                    proxy.addLayout(mHeaderLoadingView);
                }
                if (includeEnd && PtrModeHelper.showFooterLoadingLayout(mode))
                {
                    proxy.addLayout(mFooterLoadingView);
                }
            }

            return proxy;
        }

        protected ListView createListView(Context context, IAttributeSet attrs)
        {
            ListView lv;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Gingerbread)
            {
                lv = new InternalListViewSDK9(context, attrs, this);
            }
            else
            {
                lv = new InternalListView(context, attrs, this);
            }
            return lv;
        }

        //@Override
        protected override ListView createRefreshableView(Context context, IAttributeSet attrs)
        {
            ListView lv = createListView(context, attrs);

            // Set it to this so it can be used in ListActivity/ListFragment
            lv.Id = Android.Resource.Id.List;

            return lv;
        }

        //@Override
        protected override void handleStyledAttributes(TypedArray a)
        {
            base.handleStyledAttributes(a);

            mListViewExtrasEnabled = a.GetBoolean(Resource.Styleable.PullToRefresh_ptrListViewExtrasEnabled, true);

            if (mListViewExtrasEnabled)
            {
                FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MatchParent,
                        FrameLayout.LayoutParams.WrapContent, GravityFlags.CenterHorizontal);

                // Create Loading Views ready for use later
                FrameLayout frame = new FrameLayout(Context);
                mHeaderLoadingView = createLoadingLayout(Context, Mode.PULL_FROM_START, a);
                mHeaderLoadingView.Visibility = ViewStates.Gone;
                frame.AddView(mHeaderLoadingView, lp);

                ((ListView)mRefreshableView).AddHeaderView(frame, null, false);

                mLvFooterLoadingFrame = new FrameLayout(Context);
                mFooterLoadingView = createLoadingLayout(Context, Mode.PULL_FROM_END, a);
                mFooterLoadingView.Visibility = ViewStates.Gone;
                mLvFooterLoadingFrame.AddView(mFooterLoadingView, lp);

                /**
                 * If the value for Scrolling While Refreshing hasn't been
                 * explicitly set via XML, enable Scrolling While Refreshing.
                 */
                if (!a.HasValue(Resource.Styleable.PullToRefresh_ptrScrollingWhileRefreshingEnabled))
                {
                    setScrollingWhileRefreshingEnabled(true);
                }
            }
        }

        [TargetApi(Value=9)]
        public class InternalListViewSDK9 : InternalListView
        {

            public InternalListViewSDK9(Context context, IAttributeSet attrs, PullToRefreshListView instance)
                : base(context, attrs, instance)
            {
            }


            //@Override
            protected override bool OverScrollBy(int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX,
                    int scrollRangeY, int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
            {

                bool returnValue = base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX,
                        scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);

                // Does all of the hard work...

                OverscrollHelper.overScrollBy(inst, deltaX, scrollX, deltaY, scrollY, isTouchEvent);

                return returnValue;
            }
        }

        public class InternalListView : ListView, IEmptyViewMethodAccessor
        {

            private bool mAddedLvFooter = false;
            protected PullToRefreshListView inst;

            public InternalListView(Context context, IAttributeSet attrs, PullToRefreshListView instance)
                : base(context, attrs)
            {
                this.inst = instance;
            }

            protected override void DispatchDraw(Canvas canvas)
            {
                /**
                 * This is a bit hacky, but Samsung's ListView has got a bug in it
                 * when using Header/Footer Views and the list is empty. This masks
                 * the issue so that it doesn't cause an FC. See Issue #66.
                 */
                try
                {
                    base.DispatchDraw(canvas);
                }
                catch (Java.Lang.IndexOutOfBoundsException e)
                {
                    e.PrintStackTrace();
                }
            }

            //@Override
            public override bool DispatchTouchEvent(MotionEvent ev)
            {
                /**
                 * This is a bit hacky, but Samsung's ListView has got a bug in it
                 * when using Header/Footer Views and the list is empty. This masks
                 * the issue so that it doesn't cause an FC. See Issue #66.
                 */
                try
                {
                    return base.DispatchTouchEvent(ev);
                }
                catch (Java.Lang.IndexOutOfBoundsException e)
                {
                    e.PrintStackTrace();
                    return false;
                }
            }

            //@Override
            public override void SetAdapter(IListAdapter adapter)
            {
                // Add the Footer View at the last possible moment
                if (null != inst.mLvFooterLoadingFrame && !mAddedLvFooter)
                {
                    AddFooterView(inst.mLvFooterLoadingFrame, null, false);
                    mAddedLvFooter = true;
                }

                base.SetAdapter(adapter);
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
                //base.setEmptyView(emptyView);
            }

        }

    }
}