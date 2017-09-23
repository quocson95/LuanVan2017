using System;
using Xamarin.Forms;
using Android.Speech;
using Android.Content;
using Android.App;
using Android.Util;
using System.Threading.Tasks;

[assembly : Dependency(typeof(TipCalculator.Droid.Test))]
namespace TipCalculator.Droid
{	
    public class Test :MainActivity, ITest

    {
        string speech_txt;
        public Test() { }
		public bool GetSpeechSDKSupport(){
            return true;
        }

        public bool GetMicroPhoneSupport()
        {
            var rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
		
        	

        public Task<string> SpeechToText()
        {
            try
            {
                //var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                var voiceIntent = new Intent(this, typeof(VoiceRecognize));
                //var voiceIntent = new Intent(RecognizerIntent.ActionVoiceSearchHandsFree);
                //voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Hello Baby");
                //voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                //voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                //voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                //voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 5);
                ////voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.English);
                //voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, "en-US");
                var activity = Forms.Context as MainActivity;
                var listener = new ActivityResultListener(activity);     
                //Log.Info("application", "check");
                activity.StartActivityForResult(voiceIntent,10);
                ////activity.StartActivityForResult(voiceIntent, 10);
                ////activity.StartActivity(voiceIntent,OnActivityResult);   
                //return listener.Task;

                return listener.Task;
            }
            catch(Exception e){
                Log.Error("Speech ","err" +  e.ToString());
                return null;
            }



        }
        //private void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    if (resultCode == Result.Ok)
        //    {
        //        var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
        //        speech_txt = matches[0];
        //        Log.Info("Speech",speech_txt);
        //    }
        //}
		private class ActivityResultListener
		{
            private TaskCompletionSource<string> Complete = new TaskCompletionSource<string>();
            public Task<string> Task { get { return this.Complete.Task; } }

			public ActivityResultListener(MainActivity activity)
			{                
				// subscribe to activity results
				activity.ActivityResult += OnActivityResult;
			}
			private void OnActivityResult(int requestCode, Result resultCode, Intent data)
			{
				 //unsubscribe from activity results
				var context = Forms.Context;
				var activity = (MainActivity)context;
				activity.ActivityResult -= OnActivityResult;



                 //process result
                if (resultCode != Result.Ok)
                {
                    this.Complete.TrySetResult("not found");
                }
                else
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches != null)
                    {
                        string txt = matches[0];
                        //speech_txt = txt;
                        Log.Info("Speech Found ", txt);
                        this.Complete.TrySetResult(txt);
                    }
                    else this.Complete.TrySetResult("not found");

                }
			}
		}
    }
}
