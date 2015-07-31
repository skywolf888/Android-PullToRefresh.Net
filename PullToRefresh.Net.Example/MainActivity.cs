using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace PullToRefresh.Net.Example
{
    //[Activity(Label = "PullToRefresh.Net.Example", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            Intent intent;
             
            button.Click += delegate { 
                intent = new Intent(this, typeof(PullToRefreshListActivity));

                
                StartActivity(intent);
            };
        }
    }
}

