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
//import android.graphics.drawable.Drawable;
//import android.os.Build.VERSION;
//import android.os.Build.VERSION_CODES;
//import android.os.Bundle;
//import android.os.Parcelable;
//import android.util.AttributeSet;
//import android.util.Log;
//import android.view.Gravity;
//import android.view.MotionEvent;
//import android.view.View;
//import android.view.ViewConfiguration;
//import android.view.ViewGroup;
//import android.view.animation.DecelerateInterpolator;
//import android.view.animation.Interpolator;
//import android.widget.FrameLayout;
//import android.widget.LinearLayout;

//import com.handmark.pulltorefresh.library.internal.FlipLoadingLayout;
//import com.handmark.pulltorefresh.library.internal.LoadingLayout;
//import com.handmark.pulltorefresh.library.internal.RotateLoadingLayout;
//import com.handmark.pulltorefresh.library.internal.Utils;
//import com.handmark.pulltorefresh.library.internal.ViewCompat;



using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Runtime;
using Android.Graphics.Drawables;
using Android.Content.Res;
using Android.OS;
using System;
using Android.Views.Animations;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using PTROrientation = Com.Handmark.PullToRefresh.Library.PtrOrientation;
using Com.Handmark.PullToRefresh.Library.Internal;


namespace Com.Handmark.PullToRefresh.Library
{

	public abstract class PullToRefreshBase<T> : LinearLayout, IPullToRefresh<T> where T : View
	{

		// ===========================================================
		// Constants
		// ===========================================================

		public static bool DEBUG = true;

		static bool USE_HW_LAYERS = false;

		public static string LOG_TAG = "PullToRefresh";

		static float FRICTION = 2.0f;

		public static int SMOOTH_SCROLL_DURATION_MS = 200;
		public static int SMOOTH_SCROLL_LONG_DURATION_MS = 325;
		static int DEMO_SCROLL_INTERVAL = 225;

		static string STATE_STATE = "ptr_state";
		static string STATE_MODE = "ptr_mode";
		static string STATE_CURRENT_MODE = "ptr_current_mode";
		static string STATE_SCROLLING_REFRESHING_ENABLED = "ptr_disable_scrolling";
		static string STATE_SHOW_REFRESHING_VIEW = "ptr_show_refreshing_view";
		static string STATE_SUPER = "ptr_super";

		// ===========================================================
		// Fields
		// ===========================================================

		private int mTouchSlop;
		private float mLastMotionX, mLastMotionY;
		private float mInitialMotionX, mInitialMotionY;

		private bool mIsBeingDragged = false;
		private State mState = State.RESET;
		private PtrMode mMode = PtrModeHelper.getDefault();

		private PtrMode mCurrentMode;
		protected T mRefreshableView;
		private FrameLayout mRefreshableViewWrapper;

		private bool mShowViewWhileRefreshing = true;
		private bool mScrollingWhileRefreshingEnabled = false;
		private bool mFilterTouchEvents = true;
		private bool mOverScrollEnabled = true;
		private bool mLayoutVisibilityChangesEnabled = true;

		private static IInterpolator mScrollAnimationInterpolator;
		private AnimationStyle mLoadingAnimationStyle = AnimationStyleHelper.getDefault();

		private LoadingLayout mHeaderLayout;
		private LoadingLayout mFooterLayout;

		private OnRefreshListener<T> mOnRefreshListener;
		private OnRefreshListener2<T> mOnRefreshListener2;
		private OnPullEventListener<T> mOnPullEventListener;

		private SmoothScrollRunnable mCurrentSmoothScrollRunnable;

		// ===========================================================
		// Constructors
		// ===========================================================

		public PullToRefreshBase(Context context)
			: base(context)
		{
			//super(context);
			init(context, null);
		}

		public PullToRefreshBase(Context context, IAttributeSet attrs)
			: base(context, attrs)
		{
			//super(context, attrs);
			init(context, attrs);
		}

		public PullToRefreshBase(Context context, Mode mode)
			: base(context)
		{
			//super(context);
			mMode = mode;
			init(context, null);
		}

		public PullToRefreshBase(Context context, Mode mode, AnimationStyle animStyle)
			: base(context)
		{
			//super(context);
			mMode = mode;
			mLoadingAnimationStyle = animStyle;
			init(context, null);
		}

		//@Override
		public override void AddView(View child, int index, ViewGroup.LayoutParams lparams)
		{
			if (DEBUG)
			{
				Log.Debug(LOG_TAG, "addView: " + child.GetType().Name);//  .getSimpleName());
			}

			//T refreshableView = getRefreshableView();
			View refreshableView = getRefreshableView();

			if (refreshableView is ViewGroup)
			{
				((ViewGroup)refreshableView).AddView(child, index, lparams);
			}
			else
			{
				//throw new  UnsupportedOperationException("Refreshable View is not a ViewGroup so can't addView");
				throw new Exception("Refreshable View is not a ViewGroup so can't addView");
			}
		}

		//@Override
		public bool demo()
		{
			if (PtrModeHelper.showHeaderLoadingLayout(mMode) && isReadyForPullStart())
			{
				smoothScrollToAndBack(-getHeaderSize() * 2);
				return true;
			}
			else if (PtrModeHelper.showFooterLoadingLayout(mMode) && isReadyForPullEnd())
			{
				smoothScrollToAndBack(getFooterSize() * 2);
				return true;
			}

			return false;
		}

		//@Override
		public PtrMode getCurrentMode()
		{
			return mCurrentMode;
		}

		//@Override
		public bool getFilterTouchEvents()
		{
			return mFilterTouchEvents;
		}

		//@Override
		public ILoadingLayout getLoadingLayoutProxy()
		{
			return getLoadingLayoutProxy(true, true);
		}

		//@Override
		public ILoadingLayout getLoadingLayoutProxy(bool includeStart, bool includeEnd)
		{
			return createLoadingLayoutProxy(includeStart, includeEnd);
		}

		//@Override
		public PtrMode getMode()
		{
			return mMode;
		}

		//@Override
		public T getRefreshableView()
		{
			return mRefreshableView;
		}

		//@Override
		public bool getShowViewWhileRefreshing()
		{
			return mShowViewWhileRefreshing;
		}

		//@Override
		public State getState()
		{
			
			return mState;
		}

		/**
		 * @deprecated See {@link #isScrollingWhileRefreshingEnabled()}.
		 */
		public bool isDisableScrollingWhileRefreshing()
		{
			return !isScrollingWhileRefreshingEnabled();
		}

		//@Override
		public bool isPullToRefreshEnabled()
		{
			return PtrModeHelper.permitsPullToRefresh(mMode);
		}

		//@Override
		public bool isPullToRefreshOverScrollEnabled()
		{
			return Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Gingerbread && mOverScrollEnabled
					&& OverscrollHelper.isAndroidOverScrollEnabled(mRefreshableView);
		}

		//@Override
		public bool isRefreshing()
		{
			return mState == State.REFRESHING || mState == State.MANUAL_REFRESHING;
		}

		//@Override
		public bool isScrollingWhileRefreshingEnabled()
		{
			return mScrollingWhileRefreshingEnabled;
		}

		//@Override
		public override bool OnInterceptTouchEvent(MotionEvent mevent)
		{
			

			if (!isPullToRefreshEnabled())
			{
				return false;
			}

			var action = mevent.Action;

			if (action == MotionEventActions.Cancel || action == MotionEventActions.Up)
			{
				mIsBeingDragged = false;
				return false;
			}

			if (action != MotionEventActions.Down && mIsBeingDragged)
			{
				return true;
			}

			switch (action)
			{
				case MotionEventActions.Move:
					{
						// If we're refreshing, and the flag is set. Eat all MOVE events
						if (!mScrollingWhileRefreshingEnabled && isRefreshing())
						{
							return true;
						}

						if (isReadyForPull())
						{

							float y = mevent.GetY(), x = mevent.GetX();
							float diff, oppositeDiff, absDiff;

							// We need to use the correct values, based on scroll
							// direction
							switch (getPullToRefreshScrollDirection())
							{
								case PTROrientation.HORIZONTAL:
									diff = x - mLastMotionX;
									oppositeDiff = y - mLastMotionY;
									break;
								case PTROrientation.VERTICAL:
								default:
									diff = y - mLastMotionY;
									oppositeDiff = x - mLastMotionX;
									break;
							}
							absDiff = Math.Abs(diff);

							if (absDiff > mTouchSlop && (!mFilterTouchEvents || absDiff > Math.Abs(oppositeDiff)))
							{
								if (PtrModeHelper.showHeaderLoadingLayout(mMode) && diff >= 1f && isReadyForPullStart())
								{
									mLastMotionY = y;
									mLastMotionX = x;
									mIsBeingDragged = true;
									if (mMode == Mode.BOTH)
									{
										mCurrentMode = Mode.PULL_FROM_START;
									}
								}
								else if (PtrModeHelper.showFooterLoadingLayout(mMode) && diff <= -1f && isReadyForPullEnd())
								{
									mLastMotionY = y;
									mLastMotionX = x;
									mIsBeingDragged = true;
									if (mMode == Mode.BOTH)
									{
										mCurrentMode = Mode.PULL_FROM_END;
									}
								}
							}
						}
						break;
					}

				case MotionEventActions.Down:
					{
						if (isReadyForPull())
						{
							mLastMotionY = mInitialMotionY = mevent.GetY();
							mLastMotionX = mInitialMotionX = mevent.GetX();
							mIsBeingDragged = false;
						}
						break;
					}
			}

			return mIsBeingDragged;
		}

		//@Override
		public void onRefreshComplete()
		{
			
			if (isRefreshing())
			{
				setState(State.RESET);
			}
		}

		//@Override
		public override bool OnTouchEvent(MotionEvent mevent)
		{
			

			if (!isPullToRefreshEnabled())
			{
				return false;
			}

			// If we're refreshing, and the flag is set. Eat the event
			if (!mScrollingWhileRefreshingEnabled && isRefreshing())
			{
				return true;
			}

			if (mevent.Action == MotionEventActions.Down && mevent.EdgeFlags != 0)
			{
				return false;
			}

			switch (mevent.Action)
			{
				case MotionEventActions.Move:
					{
						if (mIsBeingDragged)
						{
							mLastMotionY = mevent.GetY();
							mLastMotionX = mevent.GetX();
							pullEvent();
							return true;
						}
						break;
					}

				case MotionEventActions.Down:
					{
						if (isReadyForPull())
						{
							mLastMotionY = mInitialMotionY = mevent.GetY();
							mLastMotionX = mInitialMotionX = mevent.GetX();
							return true;
						}
						break;
					}

				case MotionEventActions.Cancel:
				case MotionEventActions.Up:
					{
						if (mIsBeingDragged)
						{
							mIsBeingDragged = false;

							if (mState == State.RELEASE_TO_REFRESH
									&& (null != mOnRefreshListener || null != mOnRefreshListener2))
							{
								setState(State.REFRESHING, true);
								return true;
							}

							// If we're already refreshing, just scroll back to the top
							if (isRefreshing())
							{
								smoothScrollTo(0);
								return true;
							}

							// If we haven't returned by here, then we're not in a state
							// to pull, so just reset
							setState(State.RESET);

							return true;
						}
						break;
					}
			}

			return false;
		}


		public void setScrollingWhileRefreshingEnabled(bool allowScrollingWhileRefreshing)
		{
			mScrollingWhileRefreshingEnabled = allowScrollingWhileRefreshing;
		}

		/**
		 * @deprecated See {@link #setScrollingWhileRefreshingEnabled(bool)}
		 */
		public void setDisableScrollingWhileRefreshing(bool disableScrollingWhileRefreshing)
		{
			setScrollingWhileRefreshingEnabled(!disableScrollingWhileRefreshing);
		}



		//@Override
		public void setFilterTouchEvents(bool filterEvents)
		{
			mFilterTouchEvents = filterEvents;
		}

		/**
		 * @deprecated You should now call this method on the result of
		 *             {@link #getLoadingLayoutProxy()}.
		 */
		public void setLastUpdatedLabel(string label)
		{
			getLoadingLayoutProxy().setLastUpdatedLabel(label);
		}

		/**
		 * @deprecated You should now call this method on the result of
		 *             {@link #getLoadingLayoutProxy()}.
		 */
		public void setLoadingDrawable(Drawable drawable)
		{
			getLoadingLayoutProxy().setLoadingDrawable(drawable);
		}

		/**
		 * @deprecated You should now call this method on the result of
		 *             {@link #getLoadingLayoutProxy(bool, bool)}.
		 */
		public void setLoadingDrawable(Drawable drawable, Mode mode)
		{
			getLoadingLayoutProxy(PtrModeHelper.showHeaderLoadingLayout(mode), PtrModeHelper.showFooterLoadingLayout(mode)).setLoadingDrawable(
					drawable);
		}

		//@Override
		public void setLongClickable(bool longClickable)
		{
			getRefreshableView().LongClickable = longClickable;
			//getRefreshableView().setLongClickable(longClickable);
		}

		//@Override
		public void setMode(Mode mode)
		{
			if (mode != mMode)
			{
				if (DEBUG)
				{
					Log.Debug(LOG_TAG, "Setting mode to: " + mode);
				}
				mMode = mode;
				updateUIForMode();
			}
		}

		public void setOnPullEventListener(OnPullEventListener<T> listener)
		{
			mOnPullEventListener = listener;
		}

		//@Override
		public void setOnRefreshListener(OnRefreshListener<T> listener)
		{
			mOnRefreshListener = listener;
			mOnRefreshListener2 = null;
		}

		//@Override
		public void setOnRefreshListener(OnRefreshListener2<T> listener)
		{
			mOnRefreshListener2 = listener;
			mOnRefreshListener = null;
		}

		/**
		 * @deprecated You should now call this method on the result of
		 *             {@link #getLoadingLayoutProxy()}.
		 */
		public void setPullLabel(string pullLabel)
		{            
			getLoadingLayoutProxy().setPullLabel(pullLabel);
		}

		/**
		 * @deprecated You should now call this method on the result of
		 *             {@link #getLoadingLayoutProxy(bool, bool)}.
		 */
		public void setPullLabel(string pullLabel, Mode mode)
		{
			getLoadingLayoutProxy(PtrModeHelper.showHeaderLoadingLayout(mode), PtrModeHelper.showFooterLoadingLayout(mode)).setPullLabel(pullLabel);
		}

		/**
		 * @param enable Whether Pull-To-Refresh should be used
		 * @deprecated This simple calls setMode with an appropriate mode based on
		 *             the passed value.
		 */
		public void setPullToRefreshEnabled(bool enable)
		{
			setMode(enable ? PtrModeHelper.getDefault() : Mode.DISABLED);
		}

		//@Override
		public void setPullToRefreshOverScrollEnabled(bool enabled)
		{
			mOverScrollEnabled = enabled;
		}
		

		//@Override
		public void setRefreshing()
		{
			setRefreshing(true);
		}

		//@Override
		public void setRefreshing(bool doScroll)
		{
			if (!isRefreshing())
			{
				setState(State.MANUAL_REFRESHING, doScroll);
			}
		}

		/**
		 * @deprecated You should now call this method on the result of
		 *             {@link #getLoadingLayoutProxy()}.
		 */
		public void setRefreshingLabel(string refreshingLabel)
		{
			getLoadingLayoutProxy().setRefreshingLabel(refreshingLabel);
		}

		/**
		 * @deprecated You should now call this method on the result of
		 *             {@link #getLoadingLayoutProxy(bool, bool)}.
		 */
		public void setRefreshingLabel(string refreshingLabel, Mode mode)
		{
			getLoadingLayoutProxy(PtrModeHelper.showHeaderLoadingLayout(mMode), PtrModeHelper.showFooterLoadingLayout(mMode)).setRefreshingLabel(
					refreshingLabel);
		}

		/**
		 * @deprecated You should now call this method on the result of
		 *             {@link #getLoadingLayoutProxy()}.
		 */
		public void setReleaseLabel(string releaseLabel)
		{
			setReleaseLabel(releaseLabel, Mode.BOTH);
		}

		/**
		 * @deprecated You should now call this method on the result of
		 *             {@link #getLoadingLayoutProxy(bool, bool)}.
		 */
		public void setReleaseLabel(string releaseLabel, Mode mode)
		{
			getLoadingLayoutProxy(PtrModeHelper.showHeaderLoadingLayout(mode), PtrModeHelper.showFooterLoadingLayout(mode)).setReleaseLabel(
					releaseLabel);
		}

		public void setScrollAnimationInterpolator(IInterpolator interpolator)
		{
			mScrollAnimationInterpolator = interpolator;
		}

		//@Override
		public void setShowViewWhileRefreshing(bool showView)
		{
			mShowViewWhileRefreshing = showView;
		}

		/**
		 * @return Either {@link Orientation#VERTICAL} or
		 *         {@link Orientation#HORIZONTAL} depending on the scroll direction.
		 */
		public abstract PtrOrientation getPullToRefreshScrollDirection();

		 public void setState(State state,  bool lparams=false) {
			mState = state;
			if (DEBUG) {
				//Log.Debug(LOG_TAG, "State: " + mState.name());
				Log.Debug(LOG_TAG, "State: " + mState.ToString());
			}

			switch (mState) {
				case State.RESET:
					onReset();
					break;
				case State.PULL_TO_REFRESH:
					onPullToRefresh();
					break;
				case State.RELEASE_TO_REFRESH:
					onReleaseToRefresh();
					break;
				case State.REFRESHING:
				case State.MANUAL_REFRESHING:
					onRefreshing(lparams);
					break;
				case State.OVERSCROLLING:
					// NO-OP
					break;
			}

			// Call OnPullEventListener
			if (null != mOnPullEventListener) {
				mOnPullEventListener.onPullEvent(this, mState, mCurrentMode);
			}
		}

		/**
		 * Used internally for adding view. Need because we override addView to
		 * pass-through to the Refreshable View
		 */
		protected void addViewInternal(View child, int index, ViewGroup.LayoutParams lparams)
		//:base addView(child, index, lparams);
		{
			//super.addView(child, index, params);
			base.AddView(child, index, lparams);
		}

		/**
		 * Used internally for adding view. Need because we override addView to
		 * pass-through to the Refreshable View
		 */
		protected void addViewInternal(View child, ViewGroup.LayoutParams lparams)
		{
			base.AddView(child, -1, lparams);
		}

		protected LoadingLayout createLoadingLayout(Context context, Mode mode, TypedArray attrs)
		{
		   
			LoadingLayout layout =AnimationStyleHelper.createLoadingLayout(mLoadingAnimationStyle,context, mode,
					getPullToRefreshScrollDirection(), attrs);
			layout.Visibility = ViewStates.Invisible;
			return layout;
		}

		/**
		 * Used internally for {@link #getLoadingLayoutProxy(bool, bool)}.
		 * Allows derivative classes to include any extra LoadingLayouts.
		 */
		protected virtual LoadingLayoutProxy createLoadingLayoutProxy(bool includeStart, bool includeEnd)
		{
			LoadingLayoutProxy proxy = new LoadingLayoutProxy();

			if (includeStart && PtrModeHelper.showHeaderLoadingLayout(mMode))
			{
				proxy.addLayout(mHeaderLayout);
			}
			if (includeEnd && PtrModeHelper.showFooterLoadingLayout(mMode))
			{
				proxy.addLayout(mFooterLayout);
			}

			return proxy;
		}

		/**
		 * This is implemented by derived classes to return the created View. If you
		 * need to use a custom View (such as a custom ListView), override this
		 * method and return an instance of your custom class.
		 * <p/>
		 * Be sure to set the ID of the view in this method, especially if you're
		 * using a ListActivity or ListFragment.
		 * 
		 * @param context Context to create view with
		 * @param attrs AttributeSet from wrapped class. Means that anything you
		 *            include in the XML layout declaration will be routed to the
		 *            created View
		 * @return New instance of the Refreshable View
		 */
		protected abstract T createRefreshableView(Context context, IAttributeSet attrs);

		protected void disableLoadingLayoutVisibilityChanges()
		{
			mLayoutVisibilityChangesEnabled = false;
		}

		protected LoadingLayout getFooterLayout()
		{
			return mFooterLayout;
		}

		protected int getFooterSize()
		{
			return mFooterLayout.getContentSize();
		}

		protected LoadingLayout getHeaderLayout()
		{
			return mHeaderLayout;
		}

		protected int getHeaderSize()
		{
			return mHeaderLayout.getContentSize();
		}

		protected int getPullToRefreshScrollDuration()
		{
			return SMOOTH_SCROLL_DURATION_MS;
		}

		protected int getPullToRefreshScrollDurationLonger()
		{
			return SMOOTH_SCROLL_LONG_DURATION_MS;
		}

		protected FrameLayout getRefreshableViewWrapper()
		{
			return mRefreshableViewWrapper;
		}

		/**
		 * Allows Derivative classes to handle the XML Attrs without creating a
		 * TypedArray themsevles
		 * 
		 * @param a - TypedArray of PullToRefresh Attributes
		 */
		protected virtual void handleStyledAttributes(TypedArray a)
		{
		}

		/**
		 * Implemented by derived class to return whether the View is in a state
		 * where the user can Pull to Refresh by scrolling from the end.
		 * 
		 * @return true if the View is currently in the correct state (for example,
		 *         bottom of a ListView)
		 */
		protected abstract bool isReadyForPullEnd();

		/**
		 * Implemented by derived class to return whether the View is in a state
		 * where the user can Pull to Refresh by scrolling from the start.
		 * 
		 * @return true if the View is currently the correct state (for example, top
		 *         of a ListView)
		 */
		protected abstract bool isReadyForPullStart();

		/**
		 * Called by {@link #onRestoreInstanceState(Parcelable)} so that derivative
		 * classes can handle their saved instance state.
		 * 
		 * @param savedInstanceState - Bundle which contains saved instance state.
		 */
		protected virtual void onPtrRestoreInstanceState(Bundle savedInstanceState)
		{
		}

		/**
		 * Called by {@link #onSaveInstanceState()} so that derivative classes can
		 * save their instance state.
		 * 
		 * @param saveState - Bundle to be updated with saved state.
		 */
		protected virtual void onPtrSaveInstanceState(Bundle saveState)
		{
		}

		/**
		 * Called when the UI has been to be updated to be in the
		 * {@link State#PULL_TO_REFRESH} state.
		 */
		protected virtual void onPullToRefresh()
		{
			switch (mCurrentMode)
			{
				case Mode.PULL_FROM_END:
					mFooterLayout.pullToRefresh();
					break;
				case Mode.PULL_FROM_START:
					mHeaderLayout.pullToRefresh();
					break;
				default:
					// NO-OP
					break;
			}
		}


		class SmoothScrollFinishedImpl : OnSmoothScrollFinishedListener
		{
			PullToRefreshBase<T> inst;
			public SmoothScrollFinishedImpl(PullToRefreshBase<T> instance)
			{
				inst = instance;
			}
			public void onSmoothScrollFinished()
			{
				inst.callRefreshListener();
			}
		}

		/**
		 * Called when the UI has been to be updated to be in the
		 * {@link State#REFRESHING} or {@link State#MANUAL_REFRESHING} state.
		 * 
		 * @param doScroll - Whether the UI should scroll for this event.
		 */
		protected void onRefreshing(bool doScroll)
		{
			if (PtrModeHelper.showHeaderLoadingLayout(mMode))
			{
				mHeaderLayout.refreshing();
			}
			if (PtrModeHelper.showFooterLoadingLayout(mMode))
			{
				mFooterLayout.refreshing();
			}

			if (doScroll) {
				if (mShowViewWhileRefreshing) {

				//    // Call Refresh Listener when the Scroll has finished
				//    OnSmoothScrollFinishedListener listener = new OnSmoothScrollFinishedListener() {
				//        //@Override
				//        public void onSmoothScrollFinished() {
				//            callRefreshListener();
				//        }
				//    };

					OnSmoothScrollFinishedListener listener = new SmoothScrollFinishedImpl(this);

					switch (mCurrentMode) {
						case Mode.MANUAL_REFRESH_ONLY:
						case Mode.PULL_FROM_END:
							smoothScrollTo(getFooterSize(), listener);
							break;
						default:
						case Mode.PULL_FROM_START:
							smoothScrollTo(-getHeaderSize(), listener);
							break;
					}
				} else {
					smoothScrollTo(0);
				}
			} else {
				// We're not scrolling, so just call Refresh Listener now
				callRefreshListener();
			}
		}

		/**
		 * Called when the UI has been to be updated to be in the
		 * {@link State#RELEASE_TO_REFRESH} state.
		 */
		protected virtual void onReleaseToRefresh()
		{
			switch (mCurrentMode)
			{
				case PtrMode.PULL_FROM_END:
					mFooterLayout.releaseToRefresh();
					break;
				case PtrMode.PULL_FROM_START:
					mHeaderLayout.releaseToRefresh();
					break;
				default:
					// NO-OP
					break;
			}
		}

		/**
		 * Called when the UI has been to be updated to be in the
		 * {@link State#RESET} state.
		 */
		protected virtual void onReset()
		{
			mIsBeingDragged = false;
			mLayoutVisibilityChangesEnabled = true;

			// Always reset both layouts, just in case...
			mHeaderLayout.reset();
			mFooterLayout.reset();

			smoothScrollTo(0);
		}        

		//@Override
		protected override void OnRestoreInstanceState(IParcelable state)
		{
			if (state is Bundle)
			{
				
				Bundle bundle = (Bundle)state;

				setMode((Mode)bundle.GetInt(STATE_MODE, 0));
				mCurrentMode = (Mode)bundle.GetInt(STATE_CURRENT_MODE, 0);

				mScrollingWhileRefreshingEnabled = bundle.GetBoolean(STATE_SCROLLING_REFRESHING_ENABLED, false);
				mShowViewWhileRefreshing = bundle.GetBoolean(STATE_SHOW_REFRESHING_VIEW, true);

				// Let super Restore Itself
				base.OnRestoreInstanceState(bundle.GetParcelable(STATE_SUPER) as IParcelable);

				State viewState = (State)bundle.GetInt(STATE_STATE, 0);
				if (viewState == State.REFRESHING || viewState == State.MANUAL_REFRESHING)
				{
					setState(viewState, true);
				}

				// Now let derivative classes restore their state
				onPtrRestoreInstanceState(bundle);
				return;
			}

			base.OnRestoreInstanceState(state);
		}

		//@Override
		protected override IParcelable OnSaveInstanceState()
		{
			Bundle bundle = new Bundle();

			// Let derivative classes get a chance to save state first, that way we
			// can make sure they don't overrite any of our values
			onPtrSaveInstanceState(bundle);

			bundle.PutInt(STATE_STATE, (int)mState);
			bundle.PutInt(STATE_MODE, (int)mMode);
			bundle.PutInt(STATE_CURRENT_MODE, (int)mCurrentMode);
			bundle.PutBoolean(STATE_SCROLLING_REFRESHING_ENABLED, mScrollingWhileRefreshingEnabled);
			bundle.PutBoolean(STATE_SHOW_REFRESHING_VIEW, mShowViewWhileRefreshing);
			bundle.PutParcelable(STATE_SUPER, base.OnSaveInstanceState());

			return bundle;
		}

		//@Override
		protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
		{
			if (DEBUG)
			{
				Log.Debug(LOG_TAG, string.Format("onSizeChanged. W: %d, H: %d", w, h));
			}

			base.OnSizeChanged(w, h, oldw, oldh);

			// We need to update the header/footer when our size changes
			refreshLoadingViewsSize();

			// Update the Refreshable View layout
			refreshRefreshableViewSize(w, h);

			/**
			 * As we're currently in a Layout Pass, we need to schedule another one
			 * to layout any changes we've made here
			 */
			//throw new System.Exception("");
			//post(new Runnable() {
			//    //@Override
			//    public override void run() {
			//        requestLayout();
			//    }
			//});


            Post(new Java.Lang.Runnable(() =>{
                RequestLayout();
            }));

             
		}

		/**
		 * Re-measure the Loading Views height, and adjust internal padding as
		 * necessary
		 */
		protected void refreshLoadingViewsSize()
		{
			int maximumPullScroll = (int)(getMaximumPullScroll() * 1.2f);

			int pLeft = PaddingLeft;
			int pTop = PaddingTop;
			int pRight = PaddingRight;
			int pBottom = PaddingBottom;

			switch (getPullToRefreshScrollDirection())
			{
				case PTROrientation.HORIZONTAL:
					if (PtrModeHelper.showHeaderLoadingLayout(mMode))
					{
						mHeaderLayout.setWidth(maximumPullScroll);
						pLeft = -maximumPullScroll;
					}
					else
					{
						pLeft = 0;
					}

					if (PtrModeHelper.showFooterLoadingLayout(mMode))
					{
						mFooterLayout.setWidth(maximumPullScroll);
						pRight = -maximumPullScroll;
					}
					else
					{
						pRight = 0;
					}
					break;

				case PTROrientation.VERTICAL:
					if (PtrModeHelper.showHeaderLoadingLayout(mMode))
					{
						mHeaderLayout.setHeight(maximumPullScroll);
						pTop = -maximumPullScroll;
					}
					else
					{
						pTop = 0;
					}

					if (PtrModeHelper.showFooterLoadingLayout(mMode))
					{
						mFooterLayout.setHeight(maximumPullScroll);
						pBottom = -maximumPullScroll;
					}
					else
					{
						pBottom = 0;
					}
					break;
			}

			if (DEBUG)
			{
				Log.Debug(LOG_TAG, String.Format("Setting Padding. L: %d, T: %d, R: %d, B: %d", pLeft, pTop, pRight, pBottom));
			}
			SetPadding(pLeft, pTop, pRight, pBottom);
		}

		protected void refreshRefreshableViewSize(int width, int height)
		{
			// We need to set the Height of the Refreshable View to the same as
			// this layout
			LinearLayout.LayoutParams lp = (LinearLayout.LayoutParams)mRefreshableViewWrapper.LayoutParameters;

			switch (getPullToRefreshScrollDirection())
			{
				case PTROrientation.HORIZONTAL:
					if (lp.Width != width)
					{
						lp.Width = width;
						mRefreshableViewWrapper.RequestLayout();
					}
					break;
				case PTROrientation.VERTICAL:
					if (lp.Height != height)
					{
						lp.Height = height;
						mRefreshableViewWrapper.RequestLayout();
					}
					break;
			}
		}

		/**
		 * Helper method which just calls scrollTo() in the correct scrolling
		 * direction.
		 * 
		 * @param value - New Scroll value
		 */
		public void setHeaderScroll(int value)
		{
			if (DEBUG)
			{
				Log.Debug(LOG_TAG, "setHeaderScroll: " + value);
			}

			// Clamp value to with pull scroll range
			int maximumPullScroll = getMaximumPullScroll();
			value = Math.Min(maximumPullScroll, Math.Max(-maximumPullScroll, value));

			if (mLayoutVisibilityChangesEnabled)
			{
				if (value < 0)
				{
					mHeaderLayout.Visibility = ViewStates.Visible;
				}
				else if (value > 0)
				{
					mFooterLayout.Visibility = ViewStates.Visible;
				}
				else
				{
					mHeaderLayout.Visibility = ViewStates.Invisible;
					mFooterLayout.Visibility = ViewStates.Invisible;
				}
			}

			if (USE_HW_LAYERS)
			{
				/**
				 * Use a Hardware Layer on the Refreshable View if we've scrolled at
				 * all. We don't use them on the Header/Footer Views as they change
				 * often, which would negate any HW layer performance boost.
				 */
				 
				//ViewCompat.setLayerType(mRefreshableViewWrapper, value != 0 ? View.LAYER_TYPE_HARDWARE
				//        : View.LAYER_TYPE_NONE);
				ViewCompat.setLayerType(mRefreshableViewWrapper, value != 0 ? (int)LayerType.Hardware :(int) LayerType.None);
			}

			switch (getPullToRefreshScrollDirection())
			{
				case PTROrientation.VERTICAL:
					ScrollTo(0, value);
					break;
				case PTROrientation.HORIZONTAL:
					ScrollTo(value, 0);
					break;
			}
		}

		/**
		 * Smooth Scroll to position using the default duration of
		 * {@value #SMOOTH_SCROLL_DURATION_MS} ms.
		 * 
		 * @param scrollValue - Position to scroll to
		 */
		protected void smoothScrollTo(int scrollValue)
		{
			smoothScrollTo(scrollValue, getPullToRefreshScrollDuration());
		}

		/**
		 * Smooth Scroll to position using the default duration of
		 * {@value #SMOOTH_SCROLL_DURATION_MS} ms.
		 * 
		 * @param scrollValue - Position to scroll to
		 * @param listener - Listener for scroll
		 */
		protected void smoothScrollTo(int scrollValue, OnSmoothScrollFinishedListener listener)
		{
			smoothScrollTo(scrollValue, getPullToRefreshScrollDuration(), 0, listener);
		}

		/**
		 * Smooth Scroll to position using the longer default duration of
		 * {@value #SMOOTH_SCROLL_LONG_DURATION_MS} ms.
		 * 
		 * @param scrollValue - Position to scroll to
		 */
		protected void smoothScrollToLonger(int scrollValue)
		{
			smoothScrollTo(scrollValue, getPullToRefreshScrollDurationLonger());
		}

		/**
		 * Updates the View State when the mode has been set. This does not do any
		 * checking that the mode is different to current state so always updates.
		 */
		protected void updateUIForMode()
		{
			// We need to use the correct LayoutParam values, based on scroll
			// direction
			LinearLayout.LayoutParams lp = getLoadingLayoutLayoutParams();

			// Remove Header, and then add Header Loading View again if needed
			if (this == mHeaderLayout.Parent)// getParent())
			{
				RemoveView(mHeaderLayout);
			}
			if (PtrModeHelper.showHeaderLoadingLayout(mMode))
			{
				addViewInternal(mHeaderLayout, 0, lp);
			}

			// Remove Footer, and then add Footer Loading View again if needed
			if (this == mFooterLayout.Parent)
			{
				RemoveView(mFooterLayout);
			}
			if (PtrModeHelper.showFooterLoadingLayout(mMode))
			{
				addViewInternal(mFooterLayout, lp);
			}

			// Hide Loading Views
			refreshLoadingViewsSize();

			// If we're not using Mode.BOTH, set mCurrentMode to mMode, otherwise
			// set it to pull down
			mCurrentMode = (mMode != Mode.BOTH) ? mMode : Mode.PULL_FROM_START;
		}

		private void addRefreshableView(Context context, T refreshableView)
		{
			mRefreshableViewWrapper = new FrameLayout(context);
			mRefreshableViewWrapper.AddView(refreshableView, ViewGroup.LayoutParams.MatchParent,
					ViewGroup.LayoutParams.MatchParent);

			addViewInternal(mRefreshableViewWrapper, new LinearLayout.LayoutParams(LayoutParams.MatchParent,
					LayoutParams.MatchParent));
		}

		private void callRefreshListener()
		{
			if (null != mOnRefreshListener)
			{
				mOnRefreshListener.onRefresh(this);
			}
			else if (null != mOnRefreshListener2)
			{
				if (mCurrentMode == Mode.PULL_FROM_START)
				{
					mOnRefreshListener2.onPullDownToRefresh(this);
				}
				else if (mCurrentMode == Mode.PULL_FROM_END)
				{
					mOnRefreshListener2.onPullUpToRefresh(this);
				}
			}
		}

		//@SuppressWarnings("deprecation")
		private void init(Context context, IAttributeSet attrs)
		{
			switch (getPullToRefreshScrollDirection())
			{
				case PTROrientation.HORIZONTAL:
					
					//setOrientation(LinearLayout.HORIZONTAL);
					Orientation = Android.Widget.Orientation.Horizontal;
					break;
				case PTROrientation.VERTICAL:
				default:
					//setOrientation(LinearLayout.VERTICAL);
					Orientation= Android.Widget.Orientation.Vertical;
					break;
			}

			SetGravity(GravityFlags.Center);

			ViewConfiguration config = ViewConfiguration.Get(context);
			mTouchSlop = config.ScaledTouchSlop;

			// Styleables from XML
			TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.PullToRefresh);

			if (a.HasValue(Resource.Styleable.PullToRefresh_ptrMode))
			{
				//                mMode = Mode.mapIntToValue(a.GetInteger(R.styleable.PullToRefresh_ptrMode, 0));
				mMode = (Mode)a.GetInteger(Resource.Styleable.PullToRefresh_ptrMode, 0);
			}

			if (a.HasValue(Resource.Styleable.PullToRefresh_ptrAnimationStyle))
			{
				mLoadingAnimationStyle = (AnimationStyle)a.GetInteger(
						Resource.Styleable.PullToRefresh_ptrAnimationStyle, 0);
			}

			// Refreshable View
			// By passing the attrs, we can add ListView/GridView params via XML
			mRefreshableView = createRefreshableView(context, attrs);
			
			addRefreshableView(context, mRefreshableView);

			// We need to create now layouts now
			mHeaderLayout = createLoadingLayout(context, Mode.PULL_FROM_START, a);
			mFooterLayout = createLoadingLayout(context, Mode.PULL_FROM_END, a);

			/**
			 * Styleables from XML
			 */
			if (a.HasValue(Resource.Styleable.PullToRefresh_ptrRefreshableViewBackground))
			{
				Drawable background = a.GetDrawable(Resource.Styleable.PullToRefresh_ptrRefreshableViewBackground);
				if (null != background)
				{
					mRefreshableView.SetBackgroundDrawable(background);
				}
			}
			else if (a.HasValue(Resource.Styleable.PullToRefresh_ptrAdapterViewBackground))
			{

				//Utils.warnDeprecation("ptrAdapterViewBackground", "ptrRefreshableViewBackground");


				Drawable background = a.GetDrawable(Resource.Styleable.PullToRefresh_ptrAdapterViewBackground);
				if (null != background)
				{
					mRefreshableView.SetBackgroundDrawable(background);
				}
			}

			if (a.HasValue(Resource.Styleable.PullToRefresh_ptrOverScroll))
			{
				mOverScrollEnabled = a.GetBoolean(Resource.Styleable.PullToRefresh_ptrOverScroll, true);
			}

			if (a.HasValue(Resource.Styleable.PullToRefresh_ptrScrollingWhileRefreshingEnabled))
			{
				mScrollingWhileRefreshingEnabled = a.GetBoolean(
						Resource.Styleable.PullToRefresh_ptrScrollingWhileRefreshingEnabled, false);
			}

			// Let the derivative classes have a go at handling attributes, then
			// recycle them...
			handleStyledAttributes(a);
			a.Recycle();

			// ly update the UI for the modes
			updateUIForMode();
		}

		private bool isReadyForPull()
		{
			switch (mMode)
			{
				case Mode.PULL_FROM_START:
					return isReadyForPullStart();
				case Mode.PULL_FROM_END:
					return isReadyForPullEnd();
				case Mode.BOTH:
					return isReadyForPullEnd() || isReadyForPullStart();
				default:
					return false;
			}
		}

		/**
		 * Actions a Pull Event
		 * 
		 * @return true if the Event has been handled, false if there has been no
		 *         change
		 */
		private void pullEvent()
		{
			int newScrollValue;
			int itemDimension;
			float initialMotionValue, lastMotionValue;

			switch (getPullToRefreshScrollDirection())
			{
				case PTROrientation.HORIZONTAL:
					initialMotionValue = mInitialMotionX;
					lastMotionValue = mLastMotionX;
					break;
				case PTROrientation.VERTICAL:
				default:
					initialMotionValue = mInitialMotionY;
					lastMotionValue = mLastMotionY;
					break;
			}

			switch (mCurrentMode)
			{
				case Mode.PULL_FROM_END:
					newScrollValue = (int)Math.Round(Math.Max(initialMotionValue - lastMotionValue, 0) / FRICTION);
					itemDimension = getFooterSize();
					break;
				case Mode.PULL_FROM_START:
				default:
					newScrollValue = (int)Math.Round(Math.Min(initialMotionValue - lastMotionValue, 0) / FRICTION);
					itemDimension = getHeaderSize();
					break;
			}

			setHeaderScroll(newScrollValue);

			if (newScrollValue != 0 && !isRefreshing())
			{
				float scale = Math.Abs(newScrollValue) / (float)itemDimension;
				switch (mCurrentMode)
				{
					case Mode.PULL_FROM_END:
						mFooterLayout.onPull(scale);
						break;
					case Mode.PULL_FROM_START:
					default:
						mHeaderLayout.onPull(scale);
						break;
				}

				if (mState != State.PULL_TO_REFRESH && itemDimension >= Math.Abs(newScrollValue))
				{
					setState(State.PULL_TO_REFRESH);
				}
				else if (mState == State.PULL_TO_REFRESH && itemDimension < Math.Abs(newScrollValue))
				{
					setState(State.RELEASE_TO_REFRESH);
				}
			}
		}

		private LinearLayout.LayoutParams getLoadingLayoutLayoutParams()
		{
			switch (getPullToRefreshScrollDirection())
			{
				case PTROrientation.HORIZONTAL:
					return new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent,
							LinearLayout.LayoutParams.MatchParent);
				case PTROrientation.VERTICAL:
				default:
					return new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
							LinearLayout.LayoutParams.WrapContent);
			}
		}

		private int getMaximumPullScroll()
		{
			switch (getPullToRefreshScrollDirection())
			{
				case PTROrientation.HORIZONTAL:
					return (int)Math.Round(Width / FRICTION);
				case PTROrientation.VERTICAL:
				default:
					return (int)Math.Round(Height / FRICTION);
			}
		}

		/**
		 * Smooth Scroll to position using the specific duration
		 * 
		 * @param scrollValue - Position to scroll to
		 * @param duration - Duration of animation in milliseconds
		 */
		private void smoothScrollTo(int scrollValue, long duration)
		{
			smoothScrollTo(scrollValue, duration, 0, null);
		}

		private void smoothScrollTo(int newScrollValue, long duration, long delayMillis,
				OnSmoothScrollFinishedListener listener)
		{
			if (null != mCurrentSmoothScrollRunnable)
			{
				mCurrentSmoothScrollRunnable.stop();
			}

			int oldScrollValue;
			switch (getPullToRefreshScrollDirection())
			{
				case PTROrientation.HORIZONTAL:
					oldScrollValue =ScrollX;// getScrollX();
					break;
				case PTROrientation.VERTICAL:
				default:
					oldScrollValue =ScrollY;// getScrollY();
					break;
			}

			if (oldScrollValue != newScrollValue)
			{
				if (null == mScrollAnimationInterpolator)
				{
					// Default interpolator is a Decelerate Interpolator
					mScrollAnimationInterpolator = new DecelerateInterpolator();
				}
				mCurrentSmoothScrollRunnable = new SmoothScrollRunnable(this,oldScrollValue, newScrollValue, duration, listener);

				if (delayMillis > 0)
				{

					PostDelayed(mCurrentSmoothScrollRunnable as Java.Lang.IRunnable, delayMillis);
				}
				else
				{
					Post(mCurrentSmoothScrollRunnable as Java.Lang.IRunnable);
				}
			}
		}

        class smoothScrollToAndBackImpl: OnSmoothScrollFinishedListener
        {
            private  PullToRefreshBase<T> inst;
            public smoothScrollToAndBackImpl(PullToRefreshBase<T> instance)
            {
                inst=instance;
            }
            public void onSmoothScrollFinished()
             {
 	            inst.smoothScrollTo(0, SMOOTH_SCROLL_DURATION_MS, DEMO_SCROLL_INTERVAL, null);
             }
            }

		private void smoothScrollToAndBack(int y)
		{
            //smoothScrollTo(y, SMOOTH_SCROLL_DURATION_MS, 0, new OnSmoothScrollFinishedListener() {

            //    //@Override
            //    public override void onSmoothScrollFinished() {
            //        smoothScrollTo(0, SMOOTH_SCROLL_DURATION_MS, DEMO_SCROLL_INTERVAL, null);
            //    }
            //});

            

            smoothScrollTo(y, SMOOTH_SCROLL_DURATION_MS, 0,  new smoothScrollToAndBackImpl(this));
		}

		public enum AnimationStyle
		{
			/**
			 * This is the default for Android-PullToRefresh. Allows you to use any
			 * drawable, which is automatically rotated and used as a Progress Bar.
			 */
			ROTATE,

			/**
			 * This is the old default, and what is commonly used on iOS. Uses an
			 * arrow image which flips depending on where the user has scrolled.
			 */
			FLIP

			//static AnimationStyle getDefault() {
			//    return ROTATE;
			//}

			///**
			// * Maps an int to a specific mode. This is needed when saving state, or
			// * inflating the view from XML where the mode is given through a attr
			// * int.
			// * 
			// * @param modeInt - int to map a Mode to
			// * @return Mode that modeInt maps to, or ROTATE by default.
			// */
			//static AnimationStyle mapIntToValue(int modeInt) {
			//    switch (modeInt) {
			//        case 0x0:
			//        default:
			//            return ROTATE;
			//        case 0x1:
			//            return FLIP;
			//    }
			//}

			//LoadingLayout createLoadingLayout(Context context, Mode mode, Orientation scrollDirection, TypedArray attrs) {
			//    switch (this) {
			//        case ROTATE:
			//        default:
			//            return new RotateLoadingLayout(context, mode, scrollDirection, attrs);
			//        case FLIP:
			//            return new FlipLoadingLayout(context, mode, scrollDirection, attrs);
			//    }
			//}
		}


		public static class AnimationStyleHelper {

			public static AnimationStyle getDefault()
			{
				return AnimationStyle.ROTATE;
			}

			public static LoadingLayout createLoadingLayout(AnimationStyle astyle,Context context, Mode mode, PtrOrientation scrollDirection, TypedArray attrs)
			{
				switch (astyle)
				{
					case AnimationStyle.ROTATE:
					default:
						return new RotateLoadingLayout(context, mode, scrollDirection, attrs);
					case AnimationStyle.FLIP:
						return new FlipLoadingLayout(context, mode, scrollDirection, attrs);
				}
			}
		}		

		class SmoothScrollRunnable :  Java.Lang.Object, Java.Lang.IRunnable
		{
			private IInterpolator mInterpolator;
			private int mScrollToY;
			private int mScrollFromY;
			private long mDuration;
			private OnSmoothScrollFinishedListener mListener;

			private bool mContinueRunning = true;
			private long mStartTime = -1;
			private int mCurrentY = -1;
			private PullToRefreshBase<T> inst;
			public SmoothScrollRunnable(PullToRefreshBase<T> instance,int fromY, int toY, long duration, OnSmoothScrollFinishedListener listener)
			{
				inst = instance;
				mScrollFromY = fromY;
				mScrollToY = toY;
				mInterpolator = mScrollAnimationInterpolator;
				mDuration = duration;
				mListener = listener;
			}
			

			//@Override
			public void Run() {

			/**
			 * Only set mStartTime if this is the first time we're starting,
			 * else actually calculate the Y delta
			 */
			if (mStartTime == -1) {
				
				mStartTime = DateHelper.CurrentTimeMillis();
				
			} else {

				/**
				 * We do do all calculations in long to reduce software float
				 * calculations. We use 1000 as it gives us good accuracy and
				 * small rounding errors
				 */
				long normalizedTime = (1000 * (DateHelper.CurrentTimeMillis() - mStartTime)) / mDuration;
				normalizedTime = Math.Max(Math.Min(normalizedTime, 1000), 0);

				 int deltaY =(int) Math.Round((mScrollFromY - mScrollToY)
						* mInterpolator.GetInterpolation(normalizedTime / 1000f));
				mCurrentY = mScrollFromY - deltaY;
				
				inst.setHeaderScroll(mCurrentY);
			}

			// If we're not at the target Y, keep going...
			if (mContinueRunning && mScrollToY != mCurrentY) {
				ViewCompat.postOnAnimation(inst as View, this );
			} else {
				if (null != mListener) {
					mListener.onSmoothScrollFinished();
				}
			}
		}

			public void stop()
			{
				mContinueRunning = false;
				inst.RemoveCallbacks(this);
				//inst.removeCallbacks(this);
			}


		}

		public interface OnSmoothScrollFinishedListener
		{
			void onSmoothScrollFinished();
		}

	}

	public enum PtrOrientation
	{
		VERTICAL, HORIZONTAL
	}

	public enum PtrMode
	{

		/**
		 * Disable all Pull-to-Refresh gesture and Refreshing handling
		 */
		DISABLED = 0x0,

		/**
		 * Only allow the user to Pull from the start of the Refreshable View to
		 * refresh. The start is either the Top or Left, depending on the
		 * scrolling direction.
		 */
		PULL_FROM_START = 0x1,

		/**
		 * Only allow the user to Pull from the end of the Refreshable View to
		 * refresh. The start is either the Bottom or Right, depending on the
		 * scrolling direction.
		 */
		PULL_FROM_END = 0x2,

		/**
		 * Allow the user to both Pull from the start, from the end to refresh.
		 */
		BOTH = 0x3,

		/**
		 * Disables Pull-to-Refresh gesture handling, but allows manually
		 * setting the Refresh state via
		 * {@link PullToRefreshBase#setRefreshing() setRefreshing()}.
		 */
		MANUAL_REFRESH_ONLY = 0x4


		/**
		 * @deprecated Use {@link #PULL_FROM_START} from now on.
		 */
		//public static Mode PULL_DOWN_TO_REFRESH = Mode.PULL_FROM_START;

		/**
		 * @deprecated Use {@link #PULL_FROM_END} from now on.
		 */
		//public static Mode PULL_UP_TO_REFRESH = Mode.PULL_FROM_END;

		/**
		 * Maps an int to a specific mode. This is needed when saving state, or
		 * inflating the view from XML where the mode is given through a attr
		 * int.
		 * 
		 * @param modeInt - int to map a Mode to
		 * @return Mode that modeInt maps to, or PULL_FROM_START by default.
		 */
		//static Mode mapIntToValue( int modeInt) {
		//    for (Mode value : Mode.values()) {
		//        if (modeInt == value.getIntValue()) {
		//            return value;
		//        }
		//    }

		//    // If not, return default
		//    return getDefault();
		//}

		//public static PullToRefreshMode getDefault() {
		//    return PULL_FROM_START;
		//}

		//private int mIntValue;

		//// The modeInt values need to match those from attrs.xml
		//PullToRefreshMode(int modeInt) {
		//    mIntValue = modeInt;
		//}

		///**
		// * @return true if the mode permits Pull-to-Refresh
		// */
		//public bool permitsPullToRefresh()
		//{
		//    return !(this == DISABLED || this == MANUAL_REFRESH_ONLY);
		//}

		///**
		// * @return true if this mode wants the Loading Layout Header to be shown
		// */
		//public bool showHeaderLoadingLayout() {
		//    return this == PULL_FROM_START || this == BOTH;
		//}

		///**
		// * @return true if this mode wants the Loading Layout Footer to be shown
		// */
		//public bool showFooterLoadingLayout() {
		//    return this == PULL_FROM_END || this == BOTH || this == MANUAL_REFRESH_ONLY;
		//}

		//int getIntValue() {
		//    return mIntValue;
		//}

	}

	public static class PtrModeHelper
	{
		public static PtrMode getDefault()
		{
			return Mode.PULL_FROM_START;
		}

		/**
		 * @return true if the mode permits Pull-to-Refresh
		 */
		public static bool permitsPullToRefresh(Mode mode)
		{
			return !(mode == Mode.DISABLED || mode == Mode.MANUAL_REFRESH_ONLY);
		}

		/**
		 * @return true if this mode wants the Loading Layout Header to be shown
		 */
		public static bool showHeaderLoadingLayout(Mode mode)
		{
			return mode == Mode.PULL_FROM_START || mode == Mode.BOTH;
		}

		/**
		 * @return true if this mode wants the Loading Layout Footer to be shown
		 */
		public static bool showFooterLoadingLayout(Mode mode)
		{
			return mode == Mode.PULL_FROM_END || mode == Mode.BOTH || mode == Mode.MANUAL_REFRESH_ONLY;
		}
	}

	public static class DateHelper
	{
		public static long CurrentTimeMillis()
		{
			return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		}

	}

	public enum State
	{

		/**
		 * When the UI is in a state which means that user is not interacting
		 * with the Pull-to-Refresh function.
		 */
		RESET = 0x0,

		/**
		 * When the UI is being pulled by the user, but has not been pulled far
		 * enough so that it refreshes when released.
		 */
		PULL_TO_REFRESH = 0x1,

		/**
		 * When the UI is being pulled by the user, and <strong>has</strong>
		 * been pulled far enough so that it will refresh when released.
		 */
		RELEASE_TO_REFRESH = 0x2,

		/**
		 * When the UI is currently refreshing, caused by a pull gesture.
		 */
		REFRESHING = 0x8,

		/**
		 * When the UI is currently refreshing, caused by a call to
		 * {@link PullToRefreshBase#setRefreshing() setRefreshing()}.
		 */
		MANUAL_REFRESHING = 0x9,

		/**
		 * When the UI is currently overscrolling, caused by a fling on the
		 * Refreshable View.
		 */
		OVERSCROLLING = 0x10

		/**
		 * Maps an int to a specific state. This is needed when saving state.
		 * 
		 * @param stateInt - int to map a State to
		 * @return State that stateInt maps to
		 */
		//static State mapIntToValue( int stateInt) {
		//    for (State value : State.values()) {
		//        if (stateInt == value.getIntValue()) {
		//            return value;
		//        }
		//    }

		//    // If not, return default
		//    return RESET;
		//}

		//private int mIntValue;

		//State(int intValue) {
		//    mIntValue = intValue;
		//}

		//int getIntValue() {
		//    return mIntValue;
		//}
	}



	/**
	 * Simple Listener to listen for any callbacks to Refresh.
	 * 
	 * @author Chris Banes
	 */
		
	public interface OnRefreshListener<T> where T : View
	{

		/**
		 * onRefresh will be called for both a Pull from start, and Pull from
		 * end
		 */
		void onRefresh(PullToRefreshBase<T> refreshView);

	}

	/**
		 * An advanced version of the Listener to listen for callbacks to Refresh.
		 * This listener is different as it allows you to differentiate between Pull
		 * Ups, and Pull Downs.
		 * 
		 * @author Chris Banes
		 */
	public interface OnRefreshListener2<T> where T : View
	{
		// TODO These methods need renaming to START/END rather than DOWN/UP

		/**
		 * onPullDownToRefresh will be called only when the user has Pulled from
		 * the start, and released.
		 */
		void onPullDownToRefresh(PullToRefreshBase<T> refreshView);

		/**
		 * onPullUpToRefresh will be called only when the user has Pulled from
		 * the end, and released.
		 */
		void onPullUpToRefresh(PullToRefreshBase<T> refreshView);

	}


	// ===========================================================
	// Inner, Anonymous Classes, and Enumerations
	// ===========================================================

	/**
	 * Simple Listener that allows you to be notified when the user has scrolled
	 * to the end of the AdapterView. See (
	 * {@link PullToRefreshAdapterViewBase#setOnLastItemVisibleListener}.
	 * 
	 * @author Chris Banes
	 */
	public interface OnLastItemVisibleListener
	{

		/**
		 * Called when the user has scrolled to the end of the list
		 */
		void onLastItemVisible();

	}

	/**
	 * Listener that allows you to be notified when the user has started or
	 * finished a touch event. Useful when you want to append extra UI events
	 * (such as sounds). See (
	 * {@link PullToRefreshAdapterViewBase#setOnPullEventListener}.
	 * 
	 * @author Chris Banes
	 */
	public interface OnPullEventListener<T> where T:View
	{

		/**
		 * Called when the internal state has been changed, usually by the user
		 * pulling.
		 * 
		 * @param refreshView - View which has had it's state change.
		 * @param state - The new state of View.
		 * @param direction - One of {@link Mode#PULL_FROM_START} or
		 *            {@link Mode#PULL_FROM_END} depending on which direction
		 *            the user is pulling. Only useful when <var>state</var> is
		 *            {@link State#PULL_TO_REFRESH} or
		 *            {@link State#RELEASE_TO_REFRESH}.
		 */
		void onPullEvent(PullToRefreshBase<T> refreshView, State state, Mode direction);
	}		

}