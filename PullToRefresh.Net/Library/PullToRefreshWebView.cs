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
//import android.os.Bundle;
//import android.util.AttributeSet;
//import android.util.FloatMath;
//import android.webkit.WebChromeClient;
//import android.webkit.WebView;


using Android.Content;
using Android.Util;
using Android.Webkit;
using WebChromeClient=Android.Webkit.WebChromeClient;
using PTROrientation = Com.Handmark.PullToRefresh.Library.PtrOrientation;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using Android.OS;
using System;


namespace Com.Handmark.PullToRefresh.Library
{

    public class PullToRefreshWebView : PullToRefreshBase<WebView>
    {

        //private static OnRefreshListener<WebView> defaultOnRefreshListener = new OnRefreshListener<WebView>() ;
        //{

        //    //@Override
        //    public override void onRefresh(PullToRefreshBase<WebView> refreshView) {
        //        refreshView.getRefreshableView().Reload();
        //    }

        //};

        class OnRefreshListenerImpl : OnRefreshListener<WebView>
        {

            public void onRefresh(PullToRefreshBase<WebView> refreshView)
            {
                refreshView.getRefreshableView().Reload();
            }
        }

        private OnRefreshListener<WebView> defaultOnRefreshListener = new OnRefreshListenerImpl();


        class WebChromeClientImpl : WebChromeClient
        {
            PullToRefreshWebView inst;
            public WebChromeClientImpl(PullToRefreshWebView instance)
                : base()
            {
                inst = instance;
            }
            public override void OnProgressChanged(WebView view, int newProgress)
            {
                if (newProgress == 100)
                {
                    inst.onRefreshComplete();
                }
                base.OnProgressChanged(view, newProgress);
            }
        }

        private WebChromeClient defaultWebChromeClient;
        //{

        //    //@Override
        //    public override void onProgressChanged(WebView view, int newProgress) {
        //        if (newProgress == 100) {
        //            onRefreshComplete();
        //        }
        //    }

        //};

        private void SetClient()
        {
            setOnRefreshListener(defaultOnRefreshListener);

            if (defaultWebChromeClient == null)
            {
                defaultWebChromeClient = new WebChromeClientImpl(this);
            }
            mRefreshableView.SetWebChromeClient(defaultWebChromeClient);
        }

        public PullToRefreshWebView(Context context)
            : base(context)
        {
            //super(context);

            /**
             * Added so that by default, Pull-to-Refresh refreshes the page
             */
            SetClient();
        }

        public PullToRefreshWebView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            //super(context, attrs);

            /**
             * Added so that by default, Pull-to-Refresh refreshes the page
             */
            SetClient();
        }

        public PullToRefreshWebView(Context context, Mode mode)
            : base(context, mode)
        {
            /**
             * Added so that by default, Pull-to-Refresh refreshes the page
             */
            SetClient();
        }

        public PullToRefreshWebView(Context context, Mode mode, AnimationStyle style)
            : base(context, mode, style)
        {

            /**
             * Added so that by default, Pull-to-Refresh refreshes the page
             */
            SetClient();
        }

        //@Overrize
        public override PTROrientation getPullToRefreshScrollDirection()
        {
            return PTROrientation.VERTICAL;
        }

        //@Override
        protected override WebView createRefreshableView(Context context, IAttributeSet attrs)
        {
            WebView webView;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Gingerbread)
            {
                webView = new InternalWebViewSDK9(context, attrs, this);
            }
            else
            {
                webView = new WebView(context, attrs);
            }

            webView.Id = Resource.Id.webview;
            return webView;
        }

        //@Override
        protected override bool isReadyForPullStart()
        {
            return mRefreshableView.ScrollY == 0;
        }

        //@Override
        protected override bool isReadyForPullEnd()
        {
            float exactContentHeight = FloatMath.Floor(mRefreshableView.ContentHeight * mRefreshableView.Scale);
            return mRefreshableView.ScrollY >= (exactContentHeight - mRefreshableView.Height);
        }

        //@Override
        protected override void onPtrRestoreInstanceState(Bundle savedInstanceState)
        {
            base.onPtrRestoreInstanceState(savedInstanceState);
            mRefreshableView.RestoreState(savedInstanceState);
        }

        //@Override
        protected override void onPtrSaveInstanceState(Bundle saveState)
        {
            base.onPtrSaveInstanceState(saveState);
            mRefreshableView.SaveState(saveState);
        }

        //@TargetApi(9)
        class InternalWebViewSDK9 : WebView
        {

            // WebView doesn't always scroll back to it's edge so we add some
            // fuzziness
            static readonly int OVERSCROLL_FUZZY_THRESHOLD = 2;

            // WebView seems quite reluctant to overscroll so we use the scale
            // factor to scale it's value
            static readonly float OVERSCROLL_SCALE_FACTOR = 1.5f;

            PullToRefreshWebView inst;

            public InternalWebViewSDK9(Context context, IAttributeSet attrs, PullToRefreshWebView instance)
                : base(context, attrs)
            {
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
                        getScrollRange(), OVERSCROLL_FUZZY_THRESHOLD, OVERSCROLL_SCALE_FACTOR, isTouchEvent);

                return returnValue;
            }

            private int getScrollRange()
            {
                return (int)Math.Max(0, FloatMath.Floor(inst.mRefreshableView.ContentHeight * inst.mRefreshableView.Scale)
                        - (Height - PaddingBottom - PaddingTop));
            }
        }
    }
}