
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace FreeHand.Messenge.Mail.Gmail
{
    [Activity(Label = "WebViewActi")]
    public class WebViewActi : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.WebView);
            WebView wv = FindViewById<WebView>(Resource.Id.webView1);
            wv.Settings.JavaScriptEnabled = true;
            wv.LoadUrl(this.Intent.DataString);
        }
    }
}
