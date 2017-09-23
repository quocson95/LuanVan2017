using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;

using Android.OS;
using Android.Content;

namespace TipCalculator.Droid
{
    [Activity(Label = "TipCalculator", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
		public event Action<int, Result, Intent> ActivityResult;

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
		    if (this.ActivityResult != null)
		        this.ActivityResult(requestCode, resultCode, data);
		}

		//private Action<int, Result, Intent> _resultCallback;

		//public void StartActivity(Intent intent, Action<int, Result, Intent> resultCallback)
		//{
		//	_resultCallback = resultCallback;

		//	StartActivityForResult(intent, 0);
		//}

		//protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		//{
		//	base.OnActivityResult(requestCode, resultCode, data);
		//	if (_resultCallback != null)
		//	{
		//		_resultCallback(requestCode, resultCode, data);
		//		_resultCallback = null;
		//	}

		//}

    }
}

