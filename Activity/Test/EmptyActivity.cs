
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

namespace FreeHand
{
    [Activity(Label = "EmptyActivity",Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class EmptyActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Phone_Layout);
            // Create your application here
        }
    }
}
