//package com.handmark.pulltorefresh.samples;

//import java.util.Arrays;

//import android.app.Activity;
//import android.content.Context;
//import android.os.AsyncTask;
//import android.os.Bundle;
//import android.support.v4.view.PagerAdapter;
//import android.support.v4.view.ViewPager;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.ViewGroup;
//import android.view.ViewGroup.LayoutParams;
//import android.widget.ArrayAdapter;
//import android.widget.ListAdapter;
//import android.widget.ListView;

//import com.handmark.pulltorefresh.library.PullToRefreshBase;
//import com.handmark.pulltorefresh.library.PullToRefreshBase.OnRefreshListener;
//import com.handmark.pulltorefresh.library.PullToRefreshListView;

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

//using Mode = Com.Handmark.PullToRefresh.Library.PtrMode;
using Java.Lang;
using Com.Handmark.PullToRefresh.Library;
using Com.Handmark.PullToRefresh.Extras.Listfragment;
using Android.Support.V4.App;
using Android.Support.V4.View;

namespace PullToRefresh.Net.Example
{

    [Activity(Label = "PullToRefreshListInViewPagerActivity")]
    public class PullToRefreshListInViewPagerActivity : Activity, OnRefreshListener<ListView>
    {

        private static string[] STRINGS = { "Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam", "Abondance",
			"Ackawi", "Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale", "Aisy Cendre",
			"Allgauer Emmentaler", "Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam", "Abondance", "Ackawi",
			"Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale", "Aisy Cendre",
			"Allgauer Emmentaler" };

        private ViewPager mViewPager;

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_ptr_list_in_vp);
            View inst = FindViewById(Resource.Id.vp_list);
            string t = inst.GetType().FullName;
            mViewPager = FindViewById<ViewPager>(Resource.Id.vp_list);
            mViewPager.Adapter=new ListViewPagerAdapter(this);
        }

        private class ListViewPagerAdapter : PagerAdapter
        {
            private PullToRefreshListInViewPagerActivity inst;

            public ListViewPagerAdapter(PullToRefreshListInViewPagerActivity instance)
            {
                inst = instance;
            }
            public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
            {
                    Context context = container.Context;

                    PullToRefreshListView plv = (PullToRefreshListView) LayoutInflater.From(context).Inflate(
                            Resource.Layout.layout_listview_in_viewpager, container, false);
                
                    IListAdapter adapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleListItem1,
                             STRINGS.ToList());
                    plv.setAdapter(adapter);

                    plv.setOnRefreshListener(inst);

                    // Now just add ListView to ViewPager and return it
                    container.AddView(plv, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

                    return plv;
            }

            //@Override
            //public View instantiateItem(ViewGroup container, int position) {
            //    Context context = container.getContext();

            //    PullToRefreshListView plv = (PullToRefreshListView) LayoutInflater.from(context).inflate(
            //            R.layout.layout_listview_in_viewpager, container, false);

            //    ListAdapter adapter = new ArrayAdapter<String>(context, android.R.layout.simple_list_item_1,
            //            Arrays.asList(STRINGS));
            //    plv.setAdapter(adapter);

            //    plv.setOnRefreshListener(PullToRefreshListInViewPagerActivity.this);

            //    // Now just add ListView to ViewPager and return it
            //    container.addView(plv, LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);

            //    return plv;
            //}

            public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
            {
                container.RemoveView((View) @object);
            }

            //@Override
            //public void destroyItem(ViewGroup container, int position, Object object) {
            //    container.removeView((View) object);
            //}

            public override bool IsViewFromObject(View view, Java.Lang.Object @object)
            {
                return view == @object;
            }

            //@Override
            //public boolean isViewFromObject(View view, Object object) {
            //    return view == object;
            //}
            public override int Count
            {
                get { return 3; }
            }

            //@Override
            //public int getCount() {
            //    return 3;
            //}

        }

        //@Override
        public void onRefresh(PullToRefreshBase<ListView> refreshView)
        {
            new GetDataTask(refreshView).Execute();
        }

        private class GetDataTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, Java.Lang.Void>
        {

            PullToRefreshBase<ListView> mRefreshedView;

            public GetDataTask(PullToRefreshBase<ListView> refreshedView) {
                mRefreshedView = refreshedView;
            }

            protected override Java.Lang.Void RunInBackground(params Java.Lang.Void[] @params)
            {
                try
                {
                    Thread.Sleep(4000);
                }
                catch (InterruptedException e)
                {
                }
                return null;
            }             

            protected override void OnPostExecute(Java.Lang.Object result)
            {
                mRefreshedView.onRefreshComplete();                
                base.OnPostExecute(result);
            }
            
        }

    }
}