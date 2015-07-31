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
//import android.content.res.TypedArray;
//import android.graphics.Matrix;
//import android.graphics.drawable.Drawable;
//import android.view.View;
//import android.view.ViewGroup;
//import android.view.animation.Animation;
//import android.view.animation.RotateAnimation;
//import android.widget.ImageView.ScaleType;

//import com.handmark.pulltorefresh.library.PullToRefreshBase.Mode;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.Orientation;
//import com.handmark.pulltorefresh.library.R;

using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using System;
using Android.Widget;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using PTROrientation = Com.Handmark.PullToRefresh.Library.PtrOrientation;
using Com.Handmark.PullToRefresh;

namespace Com.Handmark.PullToRefresh.Library.Internal
{

//@SuppressLint("ViewConstructor")
public class FlipLoadingLayout : LoadingLayout {

	static readonly int FLIP_ANIMATION_DURATION = 150;

	private readonly Animation mRotateAnimation, mResetRotateAnimation;

	public FlipLoadingLayout(Context context, Mode mode, PTROrientation scrollDirection, TypedArray attrs) 
       :base(context, mode, scrollDirection, attrs)
    {
		//super(context, mode, scrollDirection, attrs);

		int rotateAngle = mode == Mode.PULL_FROM_START ? -180 : 180;
        
		mRotateAnimation = new RotateAnimation(0, rotateAngle, Dimension.RelativeToSelf, 0.5f,
				Dimension.RelativeToSelf, 0.5f);
		mRotateAnimation.Interpolator=ANIMATION_INTERPOLATOR;
		mRotateAnimation.Duration=FLIP_ANIMATION_DURATION;
		mRotateAnimation.FillAfter=true;

		mResetRotateAnimation = new RotateAnimation(rotateAngle, 0, Dimension.RelativeToSelf, 0.5f,
				Dimension.RelativeToSelf, 0.5f);
		mResetRotateAnimation.Interpolator=ANIMATION_INTERPOLATOR;
		mResetRotateAnimation.Duration=FLIP_ANIMATION_DURATION;
		mResetRotateAnimation.FillAfter=true;
	}

	//@Override
	protected override void onLoadingDrawableSet(Drawable imageDrawable) {
		if (null != imageDrawable) {
            
			int dHeight = imageDrawable.IntrinsicHeight;
			int dWidth = imageDrawable.IntrinsicWidth;

			/**
			 * We need to set the width/height of the ImageView so that it is
			 * square with each side the size of the largest drawable dimension.
			 * This is so that it doesn't clip when rotated.
			 */
			ViewGroup.LayoutParams lp = mHeaderImage.LayoutParameters;
			lp.Width = lp.Height = Math.Max(dHeight, dWidth);
			mHeaderImage.RequestLayout();

			/**
			 * We now rotate the Drawable so that is at the correct rotation,
			 * and is centered.
			 */
            
			mHeaderImage.SetScaleType(ImageView.ScaleType.Matrix);
			Matrix matrix = new Matrix();
			matrix.PostTranslate((lp.Width - dWidth) / 2f, (lp.Height - dHeight) / 2f);
			matrix.PostRotate(getDrawableRotationAngle(), lp.Width / 2f, lp.Height / 2f);
			mHeaderImage.ImageMatrix=matrix;
            
		}
	}

	//@Override
	protected override void onPullImpl(float scaleOfLayout) {
		// NO-OP
	}

	//@Override
	protected override void pullToRefreshImpl() {
		// Only start reset Animation, we've previously show the rotate anim
		if (mRotateAnimation == mHeaderImage.Animation) {
			mHeaderImage.StartAnimation(mResetRotateAnimation);
		}
	}

	//@Override
	protected override void refreshingImpl() {
		mHeaderImage.ClearAnimation();
		mHeaderImage.Visibility=ViewStates.Invisible;
		mHeaderProgress.Visibility=ViewStates.Visible;
	}

	//@Override
	protected override void releaseToRefreshImpl() {
		mHeaderImage.StartAnimation(mRotateAnimation);
	}

	//@Override
	protected override void resetImpl() {
		mHeaderImage.ClearAnimation();
		mHeaderProgress.Visibility=ViewStates.Gone;
		mHeaderImage.Visibility=ViewStates.Visible;
	}

	//@Override
	protected override int getDefaultDrawableResId() {
		return Resource.Drawable.default_ptr_flip;
	}

	private float getDrawableRotationAngle() {
		float angle = 0f;
		switch (mMode) {
			case Mode.PULL_FROM_END:
				if (mScrollDirection == PtrOrientation.HORIZONTAL) {
					angle = 90f;
				} else {
					angle = 180f;
				}
				break;

			case Mode.PULL_FROM_START:
				if (mScrollDirection == PtrOrientation.HORIZONTAL) {
					angle = 270f;
				}
				break;

			default:
				break;
		}

		return angle;
	}

}

}