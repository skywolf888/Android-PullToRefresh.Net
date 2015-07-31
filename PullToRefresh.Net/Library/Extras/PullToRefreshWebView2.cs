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
//package com.handmark.pulltorefresh.library.extras;

//import java.util.concurrent.atomic.AtomicBoolean;

//import android.content.Context;
//import android.util.AttributeSet;
//import android.webkit.WebView;

//import com.handmark.pulltorefresh.library.PullToRefreshWebView;

/**
 * An advanced version of {@link PullToRefreshWebView} which delegates the
 * triggering of the PullToRefresh gesture to the Javascript running within the
 * WebView. This means that you should only use this class if:
 * <p/>
 * <ul>
 * <li>{@link PullToRefreshWebView} doesn't work correctly because you're using
 * <code>overflow:scroll</code> or something else which means
 * {@link WebView#getScrollY()} doesn't return correct values.</li>
 * <li>You control the web content being displayed, as you need to write some
 * Javascript callbacks.</li>
 * </ul>
 * <p/>
 * <p/>
 * The way this call works is that when a PullToRefresh gesture is in action,
 * the following Javascript methods will be called:
 * <code>isReadyForPullDown()</code> and <code>isReadyForPullUp()</code>, it is
 * your job to calculate whether the view is in a state where a PullToRefresh
 * can happen, and return the result via the callback mechanism. An example can
 * be seen below:
 * <p/>
 * 
 * <pre>
 * function isReadyForPullDown() {
 *   var result = ...  // Probably using the .scrollTop DOM attribute
 *   ptr.isReadyForPullDownResponse(result);
 * }
 * 
 * function isReadyForPullUp() {
 *   var result = ...  // Probably using the .scrollBottom DOM attribute
 *   ptr.isReadyForPullUpResponse(result);
 * }
 * </pre>
 * 
 * @author Chris Banes
 */

using Android.Content;
using Android.Util;
using Android.Webkit;
using Java.Util.Concurrent.Atomic;
using PTROrientation = Com.Handmark.PullToRefresh.Library.PtrOrientation;
using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;


namespace Com.Handmark.PullToRefresh.Library.Extras
{
    public class PullToRefreshWebView2 : PullToRefreshWebView
    {

        static readonly string JS_INTERFACE_PKG = "ptr";
        static readonly string DEF_JS_READY_PULL_DOWN_CALL = "javascript:isReadyForPullDown();";
        static readonly string DEF_JS_READY_PULL_UP_CALL = "javascript:isReadyForPullUp();";

        public PullToRefreshWebView2(Context context)
            :base(context)
        {
            //super(context);
        }

        public PullToRefreshWebView2(Context context, IAttributeSet attrs)
            :base(context,attrs)
        {
            //super(context, attrs);
        }

        public PullToRefreshWebView2(Context context, Mode mode)
            :base(context,mode)
        {
            //super(context, mode);
        }

        private JsValueCallback mJsCallback;
        protected readonly AtomicBoolean mIsReadyForPullDown = new AtomicBoolean(false);
        protected readonly AtomicBoolean mIsReadyForPullUp = new AtomicBoolean(false);

        //@Override
        protected override WebView createRefreshableView(Context context, IAttributeSet attrs)
        {
            WebView webView = base.createRefreshableView(context, attrs);

            // Need to add JS Interface so we can get the response back
            mJsCallback = new JsValueCallback(this);
            webView.AddJavascriptInterface(mJsCallback, JS_INTERFACE_PKG);

            return webView;
        }

        //@Override
        protected override bool isReadyForPullStart()
        {
            // Call Javascript...
            getRefreshableView().LoadUrl(DEF_JS_READY_PULL_DOWN_CALL);

            // Response will be given to JsValueCallback, which will update
            // mIsReadyForPullDown

            return mIsReadyForPullDown.Get();
        }

        //@Override
        protected override bool isReadyForPullEnd()
        {
            // Call Javascript...
            getRefreshableView().LoadUrl(DEF_JS_READY_PULL_UP_CALL);

            // Response will be given to JsValueCallback, which will update
            // mIsReadyForPullUp

            return mIsReadyForPullUp.Get();
        }

        /**
         * Used for response from Javascript
         * 
         * @author Chris Banes
         */

        //[Export]
        [JavascriptInterface]
        class JsValueCallback: Java.Lang.Object
        {
            private PullToRefreshWebView2 inst;

            public JsValueCallback(PullToRefreshWebView2 instance)
            {
                inst = instance;
            }

            public void isReadyForPullUpResponse(bool response)
            {
                inst.mIsReadyForPullUp.Set(response);
            }

            public void isReadyForPullDownResponse(bool response)
            {
                inst.mIsReadyForPullDown.Set(response);
            }
        }
    }

}