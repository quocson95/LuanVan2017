
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Speech;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TipCalculator.Droid
{
    [Activity(Label = "VoiceRecognize")]
    public class VoiceRecognize : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			// Create your application here
			Intent intent = new Intent();
            intent.PutExtra(RecognizerIntent.ExtraResults, "test");
            SetResult(Result.Ok, intent);
            Finish();//finishing activity 			
        }
    }
}
