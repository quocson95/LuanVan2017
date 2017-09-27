
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Telephony;

using Android.Util;
namespace FreeHand
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    //[IntentFilter(new string[] { "android.provider.Telephony.SMS_RECEIVED" })]
    public class SMSBroadcastReceiver : BroadcastReceiver
    {
        
        private static readonly string TAG = "SMSBroadcastReceiver";
        private static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

		public override void OnReceive(Context context, Intent intent)
		{
			//InvokeAbortBroadcast();
			Log.Info(TAG, "Intent received: " + intent.Action);
            Toast.MakeText(context,"alo alo", ToastLength.Long).Show();
			//try
			//{
			//	if (intent.Action != IntentAction) return;

			//	var bundle = intent.Extras;

			//	if (bundle == null) return;

			//	var pdus = bundle.Get("pdus");
   //             var castedPdus = JNIEnv.GetArray<Java.Lang.Object>(pdus.Handle);

			//	var msgs = new SmsMessage[castedPdus.Length];

			//	var sb = new StringBuilder();
			//	String sender = null;
			//	for (var i = 0; i < msgs.Length; i++)
			//	{
			//		var bytes = new byte[JNIEnv.GetArrayLength(castedPdus[i].Handle)];
			//		JNIEnv.CopyArray(castedPdus[i].Handle, bytes);

			//		msgs[i] = SmsMessage.CreateFromPdu(bytes);
			//		if (sender == null)
			//			sender = msgs[i].OriginatingAddress;
			//		sb.Append(string.Format("SMS From: {0}{1}Body: {2}{1}", msgs[i].OriginatingAddress,
			//			Environment.NewLine, msgs[i].MessageBody));

			//		Toast.MakeText(context, sb.ToString(), ToastLength.Long).Show();
			//	}
			//}
			//catch (Exception ex)
			//{
			//	Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
			//}
		}
    }
}
