
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
    [Activity(Label = "CallSettingActivity", Theme = "@style/MyTheme.Mrkeys")]
    public class CallSettingActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //Loader UI
            SetContentView(Resource.Layout.layout_call_setting);
        }
    }
}
