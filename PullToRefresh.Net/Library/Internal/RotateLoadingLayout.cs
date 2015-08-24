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

//import android.content.Context;
//import android.content.res.TypedArray;
//import android.graphics.Matrix;
//import android.graphics.drawable.Drawable;
//import android.view.animation.Animation;
//import android.view.animation.RotateAnimation;
//import android.widget.ImageView.ScaleType;

//import com.handmark.pulltorefresh.library.PullToRefreshBase.Mode;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.Orientation;
//import com.handmark.pulltorefresh.library.R;

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views.Animations;
using Android.Widget;
using System;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;

namespace Com.Handmark.PullToRefresh.Library.Internal
{

    public class RotateLoadingLayout : LoadingLayout
    {

        const int ROTATION_ANIMATION_DURATION = 1200;

        private readonly Animation mRotateAnimation;
        private readonly Matrix mHeaderImageMatrix;

        private float mRotationPivotX, mRotationPivotY;

        private readonly bool mRotateDrawableWhilePulling;

        public RotateLoadingLayout(Context context, Mode mode, PtrOrientation scrollDirection, TypedArray attrs)
            : base(context, mode, scrollDirection, attrs)
        {
            //super(context, mode, scrollDirection, attrs);

            mRotateDrawableWhilePulling = attrs.GetBoolean(Resource.Styleable.PullToRefresh_ptrRotateDrawableWhilePulling, true);

            mHeaderImage.SetScaleType(ImageView.ScaleType.Matrix);
            mHeaderImageMatrix = new Matrix();
            mHeaderImage.ImageMatrix = mHeaderImageMatrix;

            mRotateAnimation = new RotateAnimation(0, 720, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf,
                    0.5f);

            mRotateAnimation.Interpolator = ANIMATION_INTERPOLATOR;
            mRotateAnimation.Duration = ROTATION_ANIMATION_DURATION;
            mRotateAnimation.RepeatCount = Animation.Infinite;
            mRotateAnimation.RepeatMode = RepeatMode.Restart;

        }

        protected override void onLoadingDrawableSet(Drawable imageDrawable)
        {
            if (null != imageDrawable)
            {
                mRotationPivotX = (int)Math.Round(imageDrawable.IntrinsicWidth / 2f);
                mRotationPivotY = (int)Math.Round(imageDrawable.IntrinsicHeight / 2f);
            }
        }


        protected override void onPullImpl(float scaleOfLayout)
        {
            float angle;
            if (mRotateDrawableWhilePulling)
            {
                angle = scaleOfLayout * 90f;
            }
            else
            {
                angle = Math.Max(0f, Math.Min(180f, scaleOfLayout * 360f - 180f));
            }

            mHeaderImageMatrix.SetRotate(angle, mRotationPivotX, mRotationPivotY);
            mHeaderImage.ImageMatrix = mHeaderImageMatrix;
        }

        //@Override
        protected override void refreshingImpl()
        {
            mHeaderImage.StartAnimation(mRotateAnimation);
        }

        //@Override
        protected override void resetImpl()
        {
            mHeaderImage.ClearAnimation();
            resetImageRotation();
        }

        private void resetImageRotation()
        {
            if (null != mHeaderImageMatrix)
            {
                mHeaderImageMatrix.Reset();
                mHeaderImage.ImageMatrix = mHeaderImageMatrix;
            }
        }

        //@Override
        protected override void pullToRefreshImpl()
        {
            // NO-OP
        }

        //@Override
        protected override void releaseToRefreshImpl()
        {
            // NO-OP
        }

        //@Override
        protected override int getDefaultDrawableResId()
        {
            return Resource.Drawable.default_ptr_rotate;
        }

    }

}