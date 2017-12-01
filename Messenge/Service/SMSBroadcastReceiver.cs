using System;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using Android.Telephony;
using Android.Util;

namespace FreeHand.Messenge.Service
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    //[IntentFilter(new string[] { "android.provider.Telephony.SMS_RECEIVED" })]
    public class SMSBroadcastReceiver : BroadcastReceiver
    {

        private static readonly string TAG = typeof(SMSBroadcastReceiver).FullName;
        private static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";
        private Model.MessengeQueue _messengeQueue;
        //private TextToSpeechLib ttsLib;
        //private STTLib _stt;       
        private Config _config;
        private string _answer = "";		
        public SMSBroadcastReceiver()
        {
            Log.Info(TAG, "Initializing");
            _messengeQueue = Model.MessengeQueue.GetInstance();
            _config = Config.Instance();
            //_activity = act;
            //ttsLib = TextToSpeechLib.Instance();
            //_stt = STTLib.Instance();
        }
               
       public override void OnReceive(Context context, Intent intent)
        {
            //InvokeASendBroadcasttBroadcast();
            Log.Info(TAG, "Intent received: " + intent.Action);
            //Toast.MakeText(context, "New Messenge", ToastLength.Short).Show();
            try
            {
                if (intent.Action != IntentAction) return;

                var bundle = intent.Extras;

                if (bundle == null) return;
                var pdus = bundle.Get("pdus");
                var castedPdus = JNIEnv.GetArray<Java.Lang.Object>(pdus.Handle);

                var msgs = new SmsMessage[castedPdus.Length];

                String sender = null;
                for (var i = 0; i < msgs.Length; i++)
                {
                    var bytes = new byte[JNIEnv.GetArrayLength(castedPdus[i].Handle)];
                    JNIEnv.CopyArray(castedPdus[i].Handle, bytes);

                    msgs[i] = SmsMessage.CreateFromPdu(bytes);
                    if (sender == null)
                        sender = msgs[i].OriginatingAddress;                   
                    Model.IMessengeData _smsData = new Model.SMSData(msgs[i].OriginatingAddress, msgs[i].MessageBody);
                    string nameSender = GetNameFromPhoneNumber(context,_smsData.GetAddrSender());
                    Log.Info(TAG, "name contact " + nameSender);
                    _smsData.SetNameSender(nameSender);                    
                    _messengeQueue.EnqueueMessengeQueue(_smsData);
                    //Toast.MakeText(context, sb.ToString(), ToastLength.Long).Show();
                    Log.Info(TAG,"Num Messenger: "+ _messengeQueue.Count().ToString());
                }
                //if (!_config.RunningSMSHandle) SMSHandleSpeak();

                //Send Broadcast for handle new messenge in queue
                //            string nameBroadcast = context.Resources.GetString(Resource.String.Speak_SMS_Broadcast_Receiver);
                var speakSMSIntent = new Intent("FreeHand.QueueMessenge.Invoke");               
                //speakSMSIntent.PutExtra("someKey", "someValue");
                context.SendBroadcast(speakSMSIntent);
            }
            catch (Exception ex)
            {
                //Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
                Log.Info(TAG, ex.Message);
            }
        }

        private string GetNameFromPhoneNumber(Context context, string number)
        {
            Android.Net.Uri uri = Android.Net.Uri.WithAppendedPath(ContactsContract.PhoneLookup.ContentFilterUri, Android.Net.Uri.Encode(number));

            String[] projection = new String[] { ContactsContract.PhoneLookup.InterfaceConsts.DisplayName };

            String contactName;
            contactName = "UNKNOW";
            var cursor = Android.App.Application.Context.ContentResolver.Query(uri, projection, null, null, null);

            if (cursor != null)
            {
                if (cursor.MoveToFirst())
                {
                    contactName = cursor.GetString(0);
                }
                cursor.Close();
            }
            Log.Info(TAG, "Detect Contact Name " + contactName);
            return contactName;
        }
    }
}
    
