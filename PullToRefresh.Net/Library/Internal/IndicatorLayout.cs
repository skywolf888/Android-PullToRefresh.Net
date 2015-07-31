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
//import android.graphics.Matrix;
//import android.graphics.drawable.Drawable;
//import android.view.View;
//import android.view.animation.Animation;
//import android.view.animation.Animation.AnimationListener;
//import android.view.animation.AnimationUtils;
//import android.view.animation.Interpolator;
//import android.view.animation.LinearInterpolator;
//import android.view.animation.RotateAnimation;
//import android.widget.FrameLayout;
//import android.widget.ImageView;
//import android.widget.ImageView.ScaleType;

using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Views.Animations;
//import com.handmark.pulltorefresh.library.PullToRefreshBase;
//import com.handmark.pulltorefresh.library.R;
using Android.Widget;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;

namespace Com.Handmark.PullToRefresh.Library.Internal
{
//@SuppressLint("ViewConstructor")
public class IndicatorLayout : FrameLayout , Android.Views.Animations.Animation.IAnimationListener {

	const int DEFAULT_ROTATION_ANIMATION_DURATION = 150;

	private Animation mInAnim, mOutAnim;
	private ImageView mArrowImageView;

	private readonly Animation mRotateAnimation, mResetRotateAnimation;

	public IndicatorLayout(Context context, Mode mode) 
    
        :base(context)
    {
		//super(context);
		mArrowImageView = new ImageView(context);
        
		Drawable arrowD =Resources.GetDrawable(Resource.Drawable.indicator_arrow);
		mArrowImageView.SetImageDrawable(arrowD);

		int padding = Resources.GetDimensionPixelSize(Resource.Dimension.indicator_internal_padding);
		mArrowImageView.SetPadding(padding, padding, padding, padding);
		AddView(mArrowImageView);

		int inAnimResId, outAnimResId;
		switch (mode) {
			case Mode.PULL_FROM_END:
				inAnimResId = Resource.Animation.slide_in_from_bottom;
				outAnimResId = Resource.Animation.slide_out_to_bottom;
				SetBackgroundResource(Resource.Drawable.indicator_bg_bottom);

				// Rotate Arrow so it's pointing the correct way
				mArrowImageView.SetScaleType(Android.Widget.ImageView.ScaleType.Matrix);
				Matrix matrix = new Matrix();
                
				matrix.SetRotate(180f, arrowD.IntrinsicWidth/ 2f, arrowD.IntrinsicHeight/ 2f);              
				mArrowImageView.ImageMatrix=matrix;
				break;
			default:
			case Mode.PULL_FROM_START:
				inAnimResId = Resource.Animation.slide_in_from_top;
				outAnimResId = Resource.Animation.slide_out_to_top;
				SetBackgroundResource(Resource.Drawable.indicator_bg_top);
				break;
		}

		mInAnim = AnimationUtils.LoadAnimation(context, inAnimResId);
		mInAnim.SetAnimationListener(this);

		mOutAnim = AnimationUtils.LoadAnimation(context, outAnimResId);
		mOutAnim.SetAnimationListener(this);

		IInterpolator interpolator = new LinearInterpolator();
        
        //mRotateAnimation = new RotateAnimation(0, -180, Animation.RELATIVE_TO_SELF, 0.5f, Animation.RELATIVE_TO_SELF,0.5f);
        mRotateAnimation = new RotateAnimation(0, -180, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);

       
		mRotateAnimation.Interpolator=interpolator;
		mRotateAnimation.Duration=DEFAULT_ROTATION_ANIMATION_DURATION;
		mRotateAnimation.FillAfter=true;

        mResetRotateAnimation = new RotateAnimation(-180, 0, Dimension.RelativeToSelf, 0.5f,
                Dimension.RelativeToSelf, 0.5f);
		mResetRotateAnimation.Interpolator=interpolator;
        mResetRotateAnimation.Duration=DEFAULT_ROTATION_ANIMATION_DURATION;
		mResetRotateAnimation.FillAfter=true;

	}

	public bool isVisible() {

        Animation currentAnim = Animation;
		if (null != currentAnim) {
			return mInAnim == currentAnim;
		}

		return Visibility == ViewStates.Visible;
	}

	public void hide() {
		StartAnimation(mOutAnim);
	}

	public void show() {
		mArrowImageView.ClearAnimation();
		StartAnimation(mInAnim);
	}

	//@Override
	public void OnAnimationEnd(Animation animation) {
		if (animation == mOutAnim) {
			mArrowImageView.ClearAnimation();
			Visibility=ViewStates.Gone;
		} else if (animation == mInAnim) {
			Visibility=ViewStates.Visible;
		}

		ClearAnimation();
	}

	//@Override
	public void OnAnimationRepeat(Animation animation) {
		// NO-OP
	}

	//@Override
	public void OnAnimationStart(Animation animation) {
		Visibility=ViewStates.Visible;
	}

	public void releaseToRefresh() {
		mArrowImageView.StartAnimation(mRotateAnimation);
	}

	public void pullToRefresh() {
		mArrowImageView.StartAnimation(mResetRotateAnimation);
	}

}

}