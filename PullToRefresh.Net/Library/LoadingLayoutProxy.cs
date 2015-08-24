//package com.handmark.pulltorefresh.library;

//import java.util.HashSet;

//import android.graphics.Typeface;
//import android.graphics.drawable.Drawable;

using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Com.Handmark.PullToRefresh.Library.Internal;
//import com.handmark.pulltorefresh.library.internal.LoadingLayout;
using System.Collections.Generic;
namespace Com.Handmark.PullToRefresh.Library
{
    public class LoadingLayoutProxy : ILoadingLayout
    {

        private HashSet<LoadingLayout> mLoadingLayouts;

        public LoadingLayoutProxy()
        {
            mLoadingLayouts = new HashSet<LoadingLayout>();
        }

        /**
         * This allows you to add extra LoadingLayout instances to this proxy. This
         * is only necessary if you keep your own instances, and want to have them
         * included in any
         * {@link PullToRefreshBase#createLoadingLayoutProxy(boolean, boolean)
         * createLoadingLayoutProxy(...)} calls.
         * 
         * @param layout - LoadingLayout to have included.
         */
        public void addLayout(LoadingLayout layout)
        {
            if (null != layout)
            {
                mLoadingLayouts.Add(layout);
            }
        }

        //@Override
        public void setLastUpdatedLabel(string label)
        {

            foreach (var layout in mLoadingLayouts)
            {
                layout.setLastUpdatedLabel(label);
            }

            //for (LoadingLayout layout : mLoadingLayouts) {
            //    layout.setLastUpdatedLabel(label);
            //}
        }

        //@Override
        public void setLoadingDrawable(Drawable drawable)
        {
            //for (LoadingLayout layout : mLoadingLayouts) {
            //    layout.setLoadingDrawable(drawable);
            //}
            foreach (var layout in mLoadingLayouts)
            {
                layout.setLoadingDrawable(drawable);
            }
        }

        //@Override
        public void setRefreshingLabel(string refreshingLabel)
        {
            //for (LoadingLayout layout : mLoadingLayouts) {
            //    layout.setRefreshingLabel(refreshingLabel);
            //}
            foreach (var layout in mLoadingLayouts)
            {
                layout.setRefreshingLabel(refreshingLabel);
            }
        }

        //@Override
        public void setPullLabel(string label)
        {
            //for (LoadingLayout layout : mLoadingLayouts) {
            //    layout.setPullLabel(label);
            //}
            foreach (var layout in mLoadingLayouts)
            {
                layout.setPullLabel(label);
            }
        }

        //@Override
        public void setReleaseLabel(string label)
        {
            //for (LoadingLayout layout : mLoadingLayouts) {
            //    layout.setReleaseLabel(label);
            //}
            foreach (var layout in mLoadingLayouts)
            {
                layout.setReleaseLabel(label);
            }
        }

        public void setTextTypeface(Typeface tf)
        {
            //for (LoadingLayout layout : mLoadingLayouts) {
            //    layout.setTextTypeface(tf);
            //}
            foreach (var layout in mLoadingLayouts)
            {
                layout.setTextTypeface(tf);
            }
        }
    }

}