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
//package com.handmark.pulltorefresh.library.internal;

//import android.annotation.SuppressLint;
//import android.content.Context;
//import android.content.res.ColorStateList;
//import android.content.res.TypedArray;
//import android.graphics.Typeface;
//import android.graphics.drawable.AnimationDrawable;
//import android.graphics.drawable.Drawable;
//import android.text.TextUtils;
//import android.util.TypedValue;
//import android.view.Gravity;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.ViewGroup;
//import android.view.animation.Interpolator;
//import android.view.animation.LinearInterpolator;
//import android.widget.FrameLayout;
//import android.widget.ImageView;
//import android.widget.ProgressBar;
//import android.widget.TextView;

//import com.handmark.pulltorefresh.library.ILoadingLayout;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.Mode;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.Orientation;
//import com.handmark.pulltorefresh.library.R;

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;

namespace Com.Handmark.PullToRefresh.Library.Internal
{

    //@SuppressLint("ViewConstructor")
    public abstract class LoadingLayout : FrameLayout, ILoadingLayout
    {

        const string LOG_TAG = "PullToRefresh-LoadingLayout";

        protected static readonly IInterpolator ANIMATION_INTERPOLATOR = new LinearInterpolator();

        private FrameLayout mInnerLayout;

        protected readonly ImageView mHeaderImage;
        protected readonly ProgressBar mHeaderProgress;

        private bool mUseIntrinsicAnimation;

        private readonly TextView mHeaderText;
        private readonly TextView mSubHeaderText;

        protected readonly Mode mMode;
        protected readonly PtrOrientation mScrollDirection;

        private string mPullLabel;
        private string mRefreshingLabel;
        private string mReleaseLabel;

        public LoadingLayout(Context context, Mode mode, PtrOrientation scrollDirection, TypedArray attrs)
            : base(context)
        {
            //base(context);
            mMode = mode;
            mScrollDirection = scrollDirection;

            switch (scrollDirection)
            {
                case PtrOrientation.HORIZONTAL:
                    LayoutInflater.From(context).Inflate(Resource.Layout.pull_to_refresh_header_horizontal, this);
                    break;
                case PtrOrientation.VERTICAL:
                default:
                    LayoutInflater.From(context).Inflate(Resource.Layout.pull_to_refresh_header_vertical, this);
                    break;
            }

            mInnerLayout = (FrameLayout)FindViewById(Resource.Id.fl_inner);
            mHeaderText = (TextView)mInnerLayout.FindViewById(Resource.Id.pull_to_refresh_text);
            mHeaderProgress = (ProgressBar)mInnerLayout.FindViewById(Resource.Id.pull_to_refresh_progress);
            mSubHeaderText = (TextView)mInnerLayout.FindViewById(Resource.Id.pull_to_refresh_sub_text);
            mHeaderImage = (ImageView)mInnerLayout.FindViewById(Resource.Id.pull_to_refresh_image);

            FrameLayout.LayoutParams lp = (FrameLayout.LayoutParams)mInnerLayout.LayoutParameters;

            switch (mode)
            {
                case Mode.PULL_FROM_END:
                    lp.Gravity = scrollDirection == PtrOrientation.VERTICAL ? GravityFlags.Top : GravityFlags.Left;

                    // Load in labels
                    mPullLabel = context.GetString(Resource.String.pull_to_refresh_from_bottom_pull_label);
                    mRefreshingLabel = context.GetString(Resource.String.pull_to_refresh_from_bottom_refreshing_label);
                    mReleaseLabel = context.GetString(Resource.String.pull_to_refresh_from_bottom_release_label);
                    break;

                case Mode.PULL_FROM_START:
                default:
                    lp.Gravity = scrollDirection == PtrOrientation.VERTICAL ? GravityFlags.Bottom : GravityFlags.Right;

                    // Load in labels
                    mPullLabel = context.GetString(Resource.String.pull_to_refresh_pull_label);
                    mRefreshingLabel = context.GetString(Resource.String.pull_to_refresh_refreshing_label);
                    mReleaseLabel = context.GetString(Resource.String.pull_to_refresh_release_label);
                    break;
            }



            if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrHeaderBackground))
            {
                Drawable background = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrHeaderBackground);
                if (null != background)
                {
                    ViewCompat.setBackground(this, background);
                }
            }

            if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrHeaderTextAppearance))
            {
                TypedValue styleID = new TypedValue();
                attrs.GetValue(Resource.Styleable.PullToRefresh_ptrHeaderTextAppearance, styleID);
                setTextAppearance(styleID.Data);
            }
            if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrSubHeaderTextAppearance))
            {
                TypedValue styleID = new TypedValue();
                attrs.GetValue(Resource.Styleable.PullToRefresh_ptrSubHeaderTextAppearance, styleID);
                setSubTextAppearance(styleID.Data);
            }

            // Text Color attrs need to be set after TextAppearance attrs
            if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrHeaderTextColor))
            {
                ColorStateList colors = attrs.GetColorStateList(Resource.Styleable.PullToRefresh_ptrHeaderTextColor);
                if (null != colors)
                {
                    setTextColor(colors);
                }
            }
            if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrHeaderSubTextColor))
            {
                ColorStateList colors = attrs.GetColorStateList(Resource.Styleable.PullToRefresh_ptrHeaderSubTextColor);
                if (null != colors)
                {
                    setSubTextColor(colors);
                }
            }

            // Try and get defined drawable from Attrs
            Drawable imageDrawable = null;
            if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrDrawable))
            {
                imageDrawable = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrDrawable);
            }

            // Check Specific Drawable from Attrs, these overrite the generic
            // drawable attr above
            switch (mode)
            {
                case Mode.PULL_FROM_START:
                default:

                    if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrDrawableStart))
                    {
                        imageDrawable = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrDrawableStart);
                    }
                    else if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrDrawableTop))
                    {
                        Utils.warnDeprecation("ptrDrawableTop", "ptrDrawableStart");
                        imageDrawable = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrDrawableTop);
                    }
                    break;

                case Mode.PULL_FROM_END:
                    if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrDrawableEnd))
                    {
                        imageDrawable = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrDrawableEnd);
                    }
                    else if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrDrawableBottom))
                    {
                        Utils.warnDeprecation("ptrDrawableBottom", "ptrDrawableEnd");
                        imageDrawable = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrDrawableBottom);
                    }
                    break;
            }

            // If we don't have a user defined drawable, load the default
            if (null == imageDrawable)
            {

                imageDrawable = context.Resources.GetDrawable(getDefaultDrawableResId());
            }

            // Set Drawable, and save width/height
            setLoadingDrawable(imageDrawable);

            reset();
        }

        public void setHeight(int height)
        {
            ViewGroup.LayoutParams lp = (ViewGroup.LayoutParams)this.LayoutParameters;
            lp.Height = height;
            RequestLayout();
        }

        public void setWidth(int width)
        {

            ViewGroup.LayoutParams lp = (ViewGroup.LayoutParams)this.LayoutParameters;
            lp.Width = width;
            RequestLayout();

        }

        public int getContentSize()
        {

            switch (mScrollDirection)
            {
                case PtrOrientation.HORIZONTAL:
                    return mInnerLayout.Width;
                case PtrOrientation.VERTICAL:
                default:
                    return mInnerLayout.Height;
            }
        }

        public void hideAllViews()
        {

            if (ViewStates.Visible == mHeaderText.Visibility)
            {
                mHeaderText.Visibility = ViewStates.Invisible;
            }
            if (ViewStates.Visible == mHeaderProgress.Visibility)
            {
                mHeaderProgress.Visibility = ViewStates.Invisible;
            }
            if (ViewStates.Visible == mHeaderImage.Visibility)
            {
                mHeaderImage.Visibility = ViewStates.Invisible;
            }
            if (ViewStates.Visible == mSubHeaderText.Visibility)
            {
                mSubHeaderText.Visibility = ViewStates.Invisible;
            }
        }

        public void onPull(float scaleOfLayout)
        {
            if (!mUseIntrinsicAnimation)
            {
                onPullImpl(scaleOfLayout);
            }
        }

        public void pullToRefresh()
        {
            if (null != mHeaderText)
            {
                mHeaderText.Text = mPullLabel;
            }

            // Now call the callback
            pullToRefreshImpl();
        }

        public void refreshing()
        {
            if (null != mHeaderText)
            {
                mHeaderText.Text = mRefreshingLabel;
            }

            if (mUseIntrinsicAnimation)
            {
                ((AnimationDrawable)mHeaderImage.Drawable).Start();
            }
            else
            {
                // Now call the callback
                refreshingImpl();
            }

            if (null != mSubHeaderText)
            {
                mSubHeaderText.Visibility = ViewStates.Gone;
            }
        }

        public void releaseToRefresh()
        {
            if (null != mHeaderText)
            {
                mHeaderText.Text = mReleaseLabel;
            }

            // Now call the callback
            releaseToRefreshImpl();
        }

        public void reset()
        {
            if (null != mHeaderText)
            {
                mHeaderText.Text = mPullLabel;
            }
            mHeaderImage.Visibility = ViewStates.Visible;

            if (mUseIntrinsicAnimation)
            {

                ((AnimationDrawable)mHeaderImage.Drawable).Stop();
            }
            else
            {
                // Now call the callback
                resetImpl();
            }

            if (null != mSubHeaderText)
            {

                if (TextUtils.IsEmpty(mSubHeaderText.Text))
                {
                    mSubHeaderText.Visibility = ViewStates.Gone;
                }
                else
                {
                    mSubHeaderText.Visibility = ViewStates.Visible;
                }
            }
        }

        //@Override
        public void setLastUpdatedLabel(string label)
        {
            setSubHeaderText(label);
        }

        public void setLoadingDrawable(Drawable imageDrawable)
        {
            // Set Drawable
            mHeaderImage.SetImageDrawable(imageDrawable);
            mUseIntrinsicAnimation = (imageDrawable is AnimationDrawable);

            // Now call the callback
            onLoadingDrawableSet(imageDrawable);
        }

        public void setPullLabel(string pullLabel)
        {
            mPullLabel = pullLabel;
        }

        public void setRefreshingLabel(string refreshingLabel)
        {
            mRefreshingLabel = refreshingLabel;
        }

        public void setReleaseLabel(string releaseLabel)
        {
            mReleaseLabel = releaseLabel;
        }

        //@Override
        public void setTextTypeface(Typeface tf)
        {
            mHeaderText.Typeface = tf;
        }

        public void showInvisibleViews()
        {
            if (ViewStates.Invisible == mHeaderText.Visibility)
            {
                mHeaderText.Visibility = ViewStates.Visible;
            }
            if (ViewStates.Invisible == mHeaderProgress.Visibility)
            {
                mHeaderProgress.Visibility = ViewStates.Visible;
            }
            if (ViewStates.Invisible == mHeaderImage.Visibility)
            {
                mHeaderImage.Visibility = ViewStates.Visible;
            }
            if (ViewStates.Invisible == mSubHeaderText.Visibility)
            {
                mSubHeaderText.Visibility = ViewStates.Visible;
            }
        }

        /**
         * Callbacks for derivative Layouts
         */

        protected abstract int getDefaultDrawableResId();

        protected abstract void onLoadingDrawableSet(Drawable imageDrawable);

        protected abstract void onPullImpl(float scaleOfLayout);

        protected abstract void pullToRefreshImpl();

        protected abstract void refreshingImpl();

        protected abstract void releaseToRefreshImpl();

        protected abstract void resetImpl();

        private void setSubHeaderText(string label)
        {
            if (null != mSubHeaderText)
            {

                if (TextUtils.IsEmpty(label))
                {
                    mSubHeaderText.Visibility = ViewStates.Gone;
                }
                else
                {
                    mSubHeaderText.Text = label;

                    // Only set it to Visible if we're GONE, otherwise VISIBLE will
                    // be set soon
                    if (ViewStates.Gone == mSubHeaderText.Visibility)
                    {
                        mSubHeaderText.Visibility = ViewStates.Visible;
                    }
                }
            }
        }

        private void setSubTextAppearance(int value)
        {
            if (null != mSubHeaderText)
            {
                mSubHeaderText.SetTextAppearance(this.Context, value);
            }
        }

        private void setSubTextColor(ColorStateList color)
        {
            if (null != mSubHeaderText)
            {
                mSubHeaderText.SetTextColor(color);
            }
        }

        private void setTextAppearance(int value)
        {
            if (null != mHeaderText)
            {
                mHeaderText.SetTextAppearance(this.Context, value);
            }
            if (null != mSubHeaderText)
            {
                mSubHeaderText.SetTextAppearance(this.Context, value);
            }
        }

        private void setTextColor(ColorStateList color)
        {
            if (null != mHeaderText)
            {
                mHeaderText.SetTextColor(color);
            }
            if (null != mSubHeaderText)
            {
                mSubHeaderText.SetTextColor(color);
            }
        }

    }
}