//package com.handmark.pulltorefresh.library.internal;

//import android.util.Log;

using Android.Util;
namespace Com.Handmark.PullToRefresh.Library.Internal
{
    public class Utils
    {

        static string LOG_TAG = "PullToRefresh";

        public static void warnDeprecation(string depreacted, string replacement)
        {
            Log.Warn(LOG_TAG, "You're using the deprecated " + depreacted + " attr, please switch over to " + replacement);
        }

    }

}