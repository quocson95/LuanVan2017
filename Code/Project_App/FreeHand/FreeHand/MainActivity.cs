using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Util;
using System.Threading.Tasks;
using FreeHand.Model;
using GR.Net.Maroulis.Library;
using Android.Views;
using Android.Support.V7.App;
using Android.Graphics;

namespace FreeHand
{

    [Activity(Label = "FreeHand", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity
	{
        //private static readonly int CHECK_PERMISSION = 1000;
        /* Permissions required to read and write contacts.Used to get PhoneBook.
        */
        //SMSBroadcastReceiver smsReceiver;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            var config = new EasySplashScreen(this)
                .WithFullScreen()
                .WithTargetActivity(Java.Lang.Class.FromType(typeof(SplashActivity)))
                .WithHeaderText("Now Loading...")
                .WithLogo(Resource.Drawable.load);
            config.HeaderTextView.SetTextColor(Color.Black);
            View view = config.Create();
            SetContentView(view);
          

        }
		

	}
}

