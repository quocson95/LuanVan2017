
using System;
using System.Text;

using System.Threading.Tasks;
using Android.Content;
using Android.Runtime;
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
		private Context _context;
		private static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";
        private Model.MessengeQueue _messengeQueue;
		private bool isSMSHandleSpeakRunning = false;
		private TextToSpeechLib ttsLib;       

		public SMSBroadcastReceiver(){}
        public SMSBroadcastReceiver(Context context)
		{            
			Log.Info(TAG,"test contructor broadcast service");
            _messengeQueue = Model.MessengeQueue.GetInstance();

			_context = context;
            //_activity = act;
			ttsLib = TextToSpeechLib.Instance(_context);
		}
		
        private async Task SMSHandleSpeak()
		{
            Model.IMessengeData messengeData;
            int status;
            isSMSHandleSpeakRunning = true;
            while (!_messengeQueue.Empty())
            {
				messengeData = _messengeQueue.DequeueMessengeQueue();
                Log.Info(TAG,messengeData.GetMessengeContent());
                Log.Info(TAG, messengeData.GetAddrSender());
				string info = messengeData.GetNameSender();
				if (string.IsNullOrEmpty(info)) info = messengeData.GetAddrSender();
                status = await ttsLib.SpeakMessenger("Get messeger from");
                status = await ttsLib.SpeakMessenger(info);
                status = await ttsLib.SpeakMessenger("Content of Messenger");
				status = await ttsLib.SpeakMessenger(messengeData.GetMessengeContent());
                status = await ttsLib.SpeakMessenger("Do you want reply");

            }
            isSMSHandleSpeakRunning = false;

		}
		public override void OnReceive(Context context, Intent intent)
		{
			//InvokeASendBroadcasttBroadcast();
			Log.Info(TAG, "Intent received: " + intent.Action);
			Toast.MakeText(context, "alo alo", ToastLength.Long).Show();
			try
			{
				if (intent.Action != IntentAction) return;

				var bundle = intent.Extras;

				if (bundle == null) return;
				var pdus = bundle.Get("pdus");
				var castedPdus = JNIEnv.GetArray<Java.Lang.Object>(pdus.Handle);

				var msgs = new SmsMessage[castedPdus.Length];
				var sb = new StringBuilder();
				String sender = null;
				for (var i = 0; i < msgs.Length; i++)
				{
					var bytes = new byte[JNIEnv.GetArrayLength(castedPdus[i].Handle)];
					JNIEnv.CopyArray(castedPdus[i].Handle, bytes);

					msgs[i] = SmsMessage.CreateFromPdu(bytes);
					if (sender == null)
						sender = msgs[i].OriginatingAddress;
					sb.Append(string.Format("SMS From: {0}{1}Body: {2}{1}", msgs[i].OriginatingAddress,
						Environment.NewLine, msgs[i].MessageBody));
					Model.IMessengeData _smsData = new Model.SMSData(msgs[i].OriginatingAddress, msgs[i].MessageBody);
                    string nameSender = GetNameFromPhoneNumber(_smsData.GetAddrSender());
                    _smsData.SetNameSender(nameSender);
					_messengeQueue.EnqueueMessengeQueue(_smsData);

					//Toast.MakeText(context, sb.ToString(), ToastLength.Long).Show();
					Log.Info(TAG, sb.ToString());
				}
				if (!isSMSHandleSpeakRunning) SMSHandleSpeak();
				//Send Broadcast for handle new messenge in queue
				//            string nameBroadcast = context.Resources.GetString(Resource.String.Speak_SMS_Broadcast_Receiver);
				//            var speakSMSIntent = new Intent(nameBroadcast);
				//speakSMSIntent.PutExtra("someKey", "someValue");
				//context.SendBroadcast(speakSMSIntent);
			}
			catch (Exception ex)
			{
				//Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
				Log.Info(TAG, ex.Message);
			}
		}

		public string GetNameFromPhoneNumber(string number)
		{
			//String selection = ContactsContract.CommonDataKinds.Contactables.InterfaceConsts.HasPhoneNumber + " = " + 1;
			//var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
			//string[] projection = {
			//	ContactsContract.Contacts.InterfaceConsts.Id,
			//	ContactsContract.Contacts.InterfaceConsts.DisplayName ,
			//	ContactsContract.CommonDataKinds.Phone.Number
   //                     //ContactsContract.CommonDataKinds.Phone
                        
   //         };
			////var cursor = act.Con(uri, projection, null, null, null);
			////var cursor = ApplicationContext.ContentResolver.Query(uri, projection, null, null, null);
			//using (var cursor = _activity.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null))
				////var loader = new CursorLoader(_activity, uri, projection, null, null, null);
				////var cursor = (ICursor)loader.LoadInBackground();
				//if (cursor.MoveToFirst())
				//{
				//	do
				//	{
				//		//var Id = cursor.GetLong(cursor.GetColumnIndex(projection[0]));
				//		var DisplayName = cursor.GetString(cursor.GetColumnIndex(projection[1]));
				//		var Number = cursor.GetString(cursor.GetColumnIndex(projection[2]));
				//		if (number == Number) return DisplayName;
				//	} while (cursor.MoveToNext());
				//}
			return null;
		}
	}
}
