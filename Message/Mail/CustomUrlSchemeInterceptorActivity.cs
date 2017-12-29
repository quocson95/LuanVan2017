

using System;
using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.OS;

namespace FreeHand.Message.Mail
{
    [Activity(Label = "CustomUrlSchemeInterceptorActivity",NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(
        new[] { Intent.ActionView},
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataSchemes = new[] { "com.googleusercontent.apps.896076308445-4l5u94fiaiq46md94st5opev4vrpqcc4" },
    DataPath = "/oauth2redirect")]
    public class CustomUrlSchemeInterceptorActivity : Activity
    {        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Console.WriteLine("CustomUrlSchemeInterceptorActivity");
            // Create your application here
            var uri = new Uri(Intent.Data.ToString());
            // Load redirectUrl page
            AuthenticationState.Authenticator.OnPageLoading(uri);
            Finish();
        }
    }
}
