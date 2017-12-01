using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

using Android.Provider;
using Android.Database;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using Android.Telephony;
using Android.OS;
using Android.Speech;
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
                        System.Environment.NewLine, msgs[i].MessageBody));
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

        public string GetNameFromPhoneNumber(Context context,string number)
        {
            string result = "UNKNOWN";
            var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
            var uriii = ContactsContract.PhoneLookup.ContentFilterUri;

			string[] projection = {
               ContactsContract.Contacts.InterfaceConsts.Id,
               ContactsContract.Contacts.InterfaceConsts.DisplayName,
                //ContactsContract.Contacts.InterfaceConsts.PhotoId,
                //ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber,
                ContactsContract.CommonDataKinds.Phone.Number
            };
            //var num = PhoneNumberUtils.FormatNumber(number.ToString(), "VI");
            //var y = PhoneNumberUtils.FormatNumber(number);


            var loader = new CursorLoader(context, uri, projection, ContactsContract.CommonDataKinds.Phone.Number + "=" + number, null, null);
			var cursor = (ICursor)loader.LoadInBackground();

            if (cursor.MoveToFirst())
            {
                //do
                //{
                    var Id = cursor.GetLong(cursor.GetColumnIndex(projection[0]));
                    var DisplayName = cursor.GetString(cursor.GetColumnIndex(projection[1]));
                    //var PhotoId = cursor.GetString(cursor.GetColumnIndex(projection[2]));
                    //var hasPhone = cursor.GetString(cursor.GetColumnIndex(projection[3]));
                    var Number = cursor.GetString(cursor.GetColumnIndex(projection[2]));
                    Log.Info(TAG, Id.ToString());
					Log.Info(TAG, DisplayName);
                    Log.Info(TAG, Number);
                    result = DisplayName;
                    //break;
                //} while (cursor.MoveToNext());
            }




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
            return result;
        }

        /*
         * Get only number from 
         * Ex (097) 455-8367 to 09874558367
         */ 
        private void ConverNumber(string source, ref string des)
        {
            des = "";
            foreach (char it in source )
            {
                if (it > 47 && it < 58) des += it;
            }
        }

        /* Compare two contact
         * Ex 84974558367 is equal 0974558367, +84974558367
         */        	
    }
}
    
