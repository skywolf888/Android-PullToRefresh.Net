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
//package com.handmark.pulltorefresh.samples;

//import android.app.Activity;
//import android.os.Bundle;
//import android.webkit.WebView;
//import android.webkit.WebViewClient;

//import com.handmark.pulltorefresh.library.PullToRefreshWebView;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Mode=Com.Handmark.PullToRefresh.Library.PtrMode;
using Java.Lang;
using Com.Handmark.PullToRefresh.Library;
using Com.Handmark.PullToRefresh.Library.Extras;
using Android.Webkit;

namespace PullToRefresh.Net.Example
{
    [Activity(Label = "PullToRefreshWebViewActivity")]

    public sealed class PullToRefreshWebViewActivity : Activity
    {

        PullToRefreshWebView mPullRefreshWebView;
        WebView mWebView;

        /** Called when the activity is first created. */
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_ptr_webview);

            mPullRefreshWebView = (PullToRefreshWebView)FindViewById(Resource.Id.pull_refresh_webview);
            mWebView = mPullRefreshWebView.getRefreshableView();

            mWebView.Settings.JavaScriptEnabled = true;
            mWebView.SetWebViewClient(new SampleWebViewClient());
            mWebView.LoadUrl("http://caipiao.163.com/t");

        }

        private class SampleWebViewClient : WebViewClient
        {
            //@Override
            public bool shouldOverrideUrlLoading(WebView view, string url)
            {
                view.LoadUrl(url);
                return true;
            }
        }

    }
}