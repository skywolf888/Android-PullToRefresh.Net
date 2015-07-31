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

//import android.content.Context;
//import android.content.res.TypedArray;
//import android.util.AttributeSet;
//import android.util.Log;
//import android.view.Gravity;
//import android.view.View;
//import android.view.ViewGroup;
//import android.view.ViewParent;
//import android.widget.AbsListView;
//import android.widget.AbsListView.OnScrollListener;
//import android.widget.Adapter;
//import android.widget.AdapterView;
//import android.widget.AdapterView.OnItemClickListener;
//import android.widget.FrameLayout;
//import android.widget.LinearLayout;
//import android.widget.ListAdapter;

//import com.handmark.pulltorefresh.library.internal.EmptyViewMethodAccessor;
//import com.handmark.pulltorefresh.library.internal.IndicatorLayout;

using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using OnScrollListener=Android.Widget.AbsListView.IOnScrollListener;
using OnItemClickListener=Android.Widget.AdapterView.IOnItemClickListener;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using Android.Content.Res;
using Com.Handmark.PullToRefresh.Library.Internal;

namespace Com.Handmark.PullToRefresh.Library
{
	public abstract class PullToRefreshAdapterViewBase<T> : PullToRefreshBase<T>, OnScrollListener where T : AbsListView
	{

		private static FrameLayout.LayoutParams convertEmptyViewLayoutParams(ViewGroup.LayoutParams lp)
		{
			FrameLayout.LayoutParams newLp = null;

			if (null != lp)
			{
				newLp = new FrameLayout.LayoutParams(lp);

				//if (lp instanceof LinearLayout.LayoutParams) {
				//    newLp.gravity = ((LinearLayout.LayoutParams) lp).gravity;
				//} else {
				//    newLp.gravity = Gravity.CENTER;
				//}
				if (lp is LinearLayout.LayoutParams)
				{
					newLp.Gravity = ((LinearLayout.LayoutParams)lp).Gravity;
				}
				else
				{
					newLp.Gravity = GravityFlags.Center;
				}
			}

			return newLp;
		}

		private bool mLastItemVisible;
		private OnScrollListener mOnScrollListener;
		private OnLastItemVisibleListener mOnLastItemVisibleListener;
		private View mEmptyView;

		private IndicatorLayout mIndicatorIvTop;
		private IndicatorLayout mIndicatorIvBottom;

		private bool mShowIndicator;
		private bool mScrollEmptyView = true;

		public PullToRefreshAdapterViewBase(Context context)
			: base(context)
		{
			//super(context);
			mRefreshableView.SetOnScrollListener(this);
		}
       
		public PullToRefreshAdapterViewBase(Context context, IAttributeSet attrs)
		    :base(context,attrs)
        {
			//super(context, attrs);
			mRefreshableView.SetOnScrollListener(this);
		}

		public PullToRefreshAdapterViewBase(Context context, Mode mode)
            :base(context,mode)
		{
			//super(context, mode);
			mRefreshableView.SetOnScrollListener(this);
		}

		public PullToRefreshAdapterViewBase(Context context, Mode mode, AnimationStyle animStyle)
            :base(context, mode, animStyle)
		{
			//super(context, mode, animStyle);
			mRefreshableView.SetOnScrollListener(this);
		}

		/**
		 * Gets whether an indicator graphic should be displayed when the View is in
		 * a state where a Pull-to-Refresh can happen. An example of this state is
		 * when the Adapter View is scrolled to the top and the mode is set to
		 * {@link Mode#PULL_FROM_START}. The default value is <var>true</var> if
		 * {@link PullToRefreshBase#isPullToRefreshOverScrollEnabled()
		 * isPullToRefreshOverScrollEnabled()} returns false.
		 * 
		 * @return true if the indicators will be shown
		 */
		public bool getShowIndicator()
		{
			return mShowIndicator;
		}

		public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount,
				 int totalItemCount)
		{

			if (DEBUG)
			{
				Log.Debug(LOG_TAG, "First Visible: " + firstVisibleItem + ". Visible Count: " + visibleItemCount
						+ ". Total Items:" + totalItemCount);
			}

			/**
			 * Set whether the Last Item is Visible. lastVisibleItemIndex is a
			 * zero-based index, so we minus one totalItemCount to check
			 */
			if (null != mOnLastItemVisibleListener)
			{
				mLastItemVisible = (totalItemCount > 0) && (firstVisibleItem + visibleItemCount >= totalItemCount - 1);
			}

			// If we're showing the indicator, check positions...
			if (getShowIndicatorInternal())
			{
				updateIndicatorViewsVisibility();
			}

			// Finally call OnScrollListener if we have one
			if (null != mOnScrollListener)
			{
				mOnScrollListener.OnScroll(view, firstVisibleItem, visibleItemCount, totalItemCount);
			}
		}

        public void OnScrollStateChanged(AbsListView view, ScrollState state)
		{
			/**
			 * Check that the scrolling has stopped, and that the last item is
			 * visible.
			 */

            if (state == ScrollState.Idle && null != mOnLastItemVisibleListener && mLastItemVisible)
			{
				mOnLastItemVisibleListener.onLastItemVisible();
			}

			if (null != mOnScrollListener)
			{             
				mOnScrollListener.OnScrollStateChanged(view, state);
			}
		}

		/**
		 * Pass-through method for {@link PullToRefreshBase#getRefreshableView()
		 * getRefreshableView()}.
		 * {@link AdapterView#setAdapter(android.widget.Adapter)}
		 * setAdapter(adapter)}. This is just for convenience!
		 * 
		 * @param adapter - Adapter to set
		 */
		public void setAdapter(IListAdapter adapter)
		{            
			((AdapterView<IListAdapter>)mRefreshableView).Adapter=adapter;
		}

		/**
		 * Sets the Empty View to be used by the Adapter View.
		 * <p/>
		 * We need it handle it ourselves so that we can Pull-to-Refresh when the
		 * Empty View is shown.
		 * <p/>
		 * Please note, you do <strong>not</strong> usually need to call this method
		 * yourself. Calling setEmptyView on the AdapterView will automatically call
		 * this method and set everything up. This includes when the Android
		 * Framework automatically sets the Empty View based on it's ID.
		 * 
		 * @param newEmptyView - Empty View to be used
		 */
		public void setEmptyView(View newEmptyView) {
		FrameLayout refreshableViewWrapper = getRefreshableViewWrapper();

		if (null != newEmptyView) {
			// New view needs to be clickable so that Android recognizes it as a
			// target for Touch Events
            
			//newEmptyView.setClickable(true);
            newEmptyView.Clickable=true;
			IViewParent newEmptyViewParent = newEmptyView.Parent;
			if (null != newEmptyViewParent && newEmptyViewParent is ViewGroup) {
				((ViewGroup) newEmptyViewParent).RemoveView(newEmptyView);
			}

			// We need to convert any LayoutParams so that it works in our
			// FrameLayout
			FrameLayout.LayoutParams lp = convertEmptyViewLayoutParams(newEmptyView.LayoutParameters);
			if (null != lp) {
				refreshableViewWrapper.AddView(newEmptyView, lp);
			} else {
				refreshableViewWrapper.AddView(newEmptyView);
			}
		}

		if (mRefreshableView is IEmptyViewMethodAccessor) {
			((IEmptyViewMethodAccessor) mRefreshableView).setEmptyViewInternal(newEmptyView);
		} else {
			mRefreshableView.EmptyView=newEmptyView;
		}
		mEmptyView = newEmptyView;
	}

		/**
		 * Pass-through method for {@link PullToRefreshBase#getRefreshableView()
		 * getRefreshableView()}.
		 * {@link AdapterView#setOnItemClickListener(OnItemClickListener)
		 * setOnItemClickListener(listener)}. This is just for convenience!
		 * 
		 * @param listener - OnItemClickListener to use
		 */
		public void setOnItemClickListener(Android.Widget.AdapterView.IOnItemClickListener listener)
		{
			mRefreshableView.OnItemClickListener=listener;
		}

		public void setOnLastItemVisibleListener(OnLastItemVisibleListener listener)
		{
			mOnLastItemVisibleListener = listener;
		}

		public void setOnScrollListener(OnScrollListener listener)
		{
			mOnScrollListener = listener;
		}

		public void setScrollEmptyView(bool doScroll)
		{
			mScrollEmptyView = doScroll;
		}

		/**
		 * Sets whether an indicator graphic should be displayed when the View is in
		 * a state where a Pull-to-Refresh can happen. An example of this state is
		 * when the Adapter View is scrolled to the top and the mode is set to
		 * {@link Mode#PULL_FROM_START}
		 * 
		 * @param showIndicator - true if the indicators should be shown.
		 */
		public void setShowIndicator(bool showIndicator)
		{
			mShowIndicator = showIndicator;

			if (getShowIndicatorInternal())
			{
				// If we're set to Show Indicator, add/update them
				addIndicatorViews();
			}
			else
			{
				// If not, then remove then
				removeIndicatorViews();
			}
		}

		//;

		//@Override
		protected override void onPullToRefresh()
		{
			base.onPullToRefresh();

			if (getShowIndicatorInternal())
			{
				switch (getCurrentMode())
				{
					case Mode.PULL_FROM_END:
						mIndicatorIvBottom.pullToRefresh();
						break;
					case Mode.PULL_FROM_START:
						mIndicatorIvTop.pullToRefresh();
						break;
					default:
						// NO-OP
						break;
				}
			}
		}

		protected virtual void onRefreshing(bool doScroll)
		{
			base.onRefreshing(doScroll);
			if (getShowIndicatorInternal())
			{                
				updateIndicatorViewsVisibility();
			}
		}

		//@Override
		protected override void onReleaseToRefresh()
		{
			base.onReleaseToRefresh();

			if (getShowIndicatorInternal())
			{
				switch (getCurrentMode())
				{
					case Mode.PULL_FROM_END:
						mIndicatorIvBottom.releaseToRefresh();
						break;
                    case Mode.PULL_FROM_START:
						mIndicatorIvTop.releaseToRefresh();
						break;
					default:
						// NO-OP
						break;
				}
			}
		}

		//@Override
		protected override void onReset()
		{
			base.onReset();

			if (getShowIndicatorInternal())
			{
				updateIndicatorViewsVisibility();
			}
		}
        
		//@Override
		protected override void handleStyledAttributes(TypedArray a)
		{
             
			// Set Show Indicator to the XML value, or default value
			mShowIndicator = a.GetBoolean(Resource.Styleable.PullToRefresh_ptrShowIndicator, !isPullToRefreshOverScrollEnabled());
		}

		protected override bool isReadyForPullStart()
		{            
			return isFirstItemVisible();
		}


		protected override bool isReadyForPullEnd()
		{            
			return isLastItemVisible();
		}

		//@Override
        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
		{
			base.OnScrollChanged(l, t, oldl, oldt);
			if (null != mEmptyView && !mScrollEmptyView)
			{
				mEmptyView.ScrollTo(-l, -t);
			}
		}

		//@Override
		protected void updateUIForMode()
		{
            base.updateUIForMode();

			// Check Indicator Views consistent with new Mode
			if (getShowIndicatorInternal())
			{
				addIndicatorViews();
			}
			else
			{
				removeIndicatorViews();
			}
		}

		private void addIndicatorViews() {
		Mode mode = getMode();
		FrameLayout refreshableViewWrapper = getRefreshableViewWrapper();

		if (PtrModeHelper.showHeaderLoadingLayout(mode) && null == mIndicatorIvTop) {
			// If the mode can pull down, and we don't have one set already
            
			mIndicatorIvTop = new IndicatorLayout(Context, Mode.PULL_FROM_START);
			FrameLayout.LayoutParams lparams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
					ViewGroup.LayoutParams.WrapContent);
			lparams.RightMargin = Resources.GetDimensionPixelSize(Resource.Dimension.indicator_right_padding);
			lparams.Gravity = GravityFlags.Top | GravityFlags.Right;
			refreshableViewWrapper.AddView(mIndicatorIvTop, lparams);

		} else if (!PtrModeHelper.showHeaderLoadingLayout(mode) && null != mIndicatorIvTop) {
			// If we can't pull down, but have a View then remove it
			refreshableViewWrapper.RemoveView(mIndicatorIvTop);
			mIndicatorIvTop = null;
		}

		if (PtrModeHelper.showFooterLoadingLayout(mode) && null == mIndicatorIvBottom) {
			// If the mode can pull down, and we don't have one set already
			mIndicatorIvBottom = new IndicatorLayout(Context, Mode.PULL_FROM_END);
			FrameLayout.LayoutParams lparams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
					ViewGroup.LayoutParams.WrapContent);
			lparams.RightMargin = Resources.GetDimensionPixelSize(Resource.Dimension.indicator_right_padding);
			lparams.Gravity = GravityFlags.Bottom | GravityFlags.Right;
			refreshableViewWrapper.AddView(mIndicatorIvBottom, lparams);

        }
        else if (!PtrModeHelper.showFooterLoadingLayout(mode) && null != mIndicatorIvBottom)
        {
			// If we can't pull down, but have a View then remove it
			refreshableViewWrapper.RemoveView(mIndicatorIvBottom);
			mIndicatorIvBottom = null;
		}
	}

		private bool getShowIndicatorInternal()
		{
			return mShowIndicator && isPullToRefreshEnabled();
		}

		private bool isFirstItemVisible() {
             
		IListAdapter adapter = mRefreshableView.Adapter;

		if (null == adapter || adapter.IsEmpty) {
			if (DEBUG) {
				Log.Debug(LOG_TAG, "isFirstItemVisible. Empty View.");
			}
			return true;

		} else {

			/**
			 * This check should really just be:
			 * mRefreshableView.getFirstVisiblePosition() == 0, but PtRListView
			 * internally use a HeaderView which messes the positions up. For
			 * now we'll just add one to account for it and rely on the inner
			 * condition which checks getTop().
			 */
			if (mRefreshableView.FirstVisiblePosition <= 1) {
				View firstVisibleChild = mRefreshableView.GetChildAt(0);
				if (firstVisibleChild != null) {
					return firstVisibleChild.Top >= mRefreshableView.Top;
				}
			}
		}

		return false;
	}

		private bool isLastItemVisible()
		{
			IListAdapter adapter = mRefreshableView.Adapter;

			if (null == adapter || adapter.IsEmpty)
			{
				if (DEBUG)
				{
					Log.Debug(LOG_TAG, "isLastItemVisible. Empty View.");
				}
				return true;
			}
			else
			{
				int lastItemPosition = mRefreshableView.Count - 1;
				int lastVisiblePosition = mRefreshableView.LastVisiblePosition;

				if (DEBUG)
				{
					Log.Debug(LOG_TAG, "isLastItemVisible. Last Item Position: " + lastItemPosition + " Last Visible Pos: "
							+ lastVisiblePosition);
				}

				/**
				 * This check should really just be: lastVisiblePosition ==
				 * lastItemPosition, but PtRListView internally uses a FooterView
				 * which messes the positions up. For me we'll just subtract one to
				 * account for it and rely on the inner condition which checks
				 * getBottom().
				 */
				if (lastVisiblePosition >= lastItemPosition - 1)
				{
					int childIndex = lastVisiblePosition - mRefreshableView.FirstVisiblePosition;
					View lastVisibleChild = mRefreshableView.GetChildAt(childIndex);
					if (lastVisibleChild != null)
					{
						return lastVisibleChild.Bottom <= mRefreshableView.Bottom;
					}
				}
			}

			return false;
		}

		private void removeIndicatorViews()
		{
			if (null != mIndicatorIvTop)
			{
				getRefreshableViewWrapper().RemoveView(mIndicatorIvTop);
				mIndicatorIvTop = null;
			}

			if (null != mIndicatorIvBottom)
			{
				getRefreshableViewWrapper().RemoveView(mIndicatorIvBottom);
				mIndicatorIvBottom = null;
			}
		}

		private void updateIndicatorViewsVisibility()
		{
			if (null != mIndicatorIvTop)
			{
				if (!isRefreshing() && isReadyForPullStart())
				{
					if (!mIndicatorIvTop.isVisible())
					{
						mIndicatorIvTop.show();
					}
				}
				else
				{
					if (mIndicatorIvTop.isVisible())
					{
						mIndicatorIvTop.hide();
					}
				}
			}

			if (null != mIndicatorIvBottom)
			{
				if (!isRefreshing() && isReadyForPullEnd())
				{
					if (!mIndicatorIvBottom.isVisible())
					{
						mIndicatorIvBottom.show();
					}
				}
				else
				{
					if (mIndicatorIvBottom.isVisible())
					{
						mIndicatorIvBottom.hide();
					}
				}
			}
		}
	}
}