﻿using System;
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

namespace FreeHand
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    //[IntentFilter(new string[] { "android.provider.Telephony.SMS_RECEIVED" })]
    public class SMSBroadcastReceiver : BroadcastReceiver, IRecognitionListener
    {

        private static readonly string TAG = "SMSBroadcastReceiver";
        private Context _context;
        private static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";
        private Model.MessengeQueue _messengeQueue;
        private TextToSpeechLib ttsLib;
        private STTLib _stt;
        private SpeechRecognizer _speech;
        private TaskCompletionSource<Java.Lang.Object> _tcs;
        private Config _config;
        private string _answer = "";

		public SMSBroadcastReceiver() { }
        public SMSBroadcastReceiver(Context context)
        {
            Log.Info(TAG, "test contructor broadcast service");
            _messengeQueue = Model.MessengeQueue.GetInstance();
            _config = Config.Instance();
            _context = context;
            //_activity = act;
            ttsLib = TextToSpeechLib.Instance();
            _stt = STTLib.Instance();
        }

        private async Task SMSHandleSpeak()
        {            
            Contract.Ensures(Contract.Result<Task>() != null);
            //         STTLib _stt = STTLib.Instance();
            //SpeechRecognizer speech = SpeechRecognizer.CreateSpeechRecognizer(_context);
            //speech.SetRecognitionListener(this);
            //speech.StartListening(_stt.IntentSTT());

            Model.IMessengeData messengeData = null;
            _config.RunningSpeech = true;
            int status;
            status = -1;
            while (!_messengeQueue.Empty())
            {
                if (_config.UpdateConfig) 
                {
                    _config.UpdateConfig = false;
                    await ttsLib.ReInitTTS();   
                }
                messengeData = _messengeQueue.DequeueMessengeQueue();
                //Speak content SMS comming
                await SpeakContentSMS(messengeData);
                int tryNumber = 3;
                //Listen request of user : Yes or No
                while (status != 0 && tryNumber > 0)
                {
                    status = await listenRequest();
                    await Task.Delay(500);
                    if (status != 0)
                    {
                        tryNumber--;
                        await SpeakMsg("I can't hear you, please try again");
                        await Task.Delay(500);
                    }
                }
                if (status == 0)
                {
                    Log.Info(TAG, "status " + status.ToString());
                    Log.Info(TAG, "Listen Answer "+_answer);
                    //await Task.Delay(1000);
                    //Listen msg reply
                    await SpeakMsg("Please speak messenge after beep ");
                    await Task.Delay(500);
                    status = -1;
                    tryNumber = 3;
                    while (status != 0)
                    {
                        status = await listenRequest();
                        await Task.Delay(500);
                        if (status != 0 && tryNumber >= 0)
                        {
                            tryNumber--;
                            await SpeakMsg("I can't hear you, please try again");
                            await Task.Delay(500);
                        }
                    }
                    Log.Info(TAG,"Listen reply "+ _answer);
                    //messengeData.Reply(_answer);
                }
            }
            _config.RunningSpeech = false;

        }
        private async Task<int> listenRequest()
        {
			_speech = SpeechRecognizer.CreateSpeechRecognizer(_context);
			_speech.SetRecognitionListener(this);
            _tcs = new TaskCompletionSource<Java.Lang.Object>();
            try{
                _speech.StartListening(_stt.IntentSTT());   
            }			    
            catch(Exception e){}
            return (int) await _tcs.Task;
		}
        private async Task SpeakContentSMS(Model.IMessengeData messengeData)
        {			
            //messengeData.Reply("aaa");
			Log.Info(TAG, messengeData.GetAddrSender());
			await SpeakMsg("Get new SMS messeger ");
            await SpeakMsg("From ");
			await SpeakMsg(messengeData.GetAddrSender());
            await SpeakMsg("Name Sender ");
            await SpeakMsg(messengeData.GetNameSender());
			await SpeakMsg("Content of Messenger");
			//Log.Info(TAG, messengeData.GetMessengeContent());
			await SpeakMsg(messengeData.GetMessengeContent());

			await SpeakMsg("Do you want reply");
        }
        private async Task SpeakMsg(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            int status = await ttsLib.SpeakMessenger(msg);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            //InvokeASendBroadcasttBroadcast();
            Log.Info(TAG, "Intent received: " + intent.Action);
            Toast.MakeText(context, "New Messenge", ToastLength.Short).Show();
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
                    string nameSender = GetNameFromPhoneNumber(_smsData.GetAddrSender());
                    Log.Info(TAG, "name contact " + nameSender);
                    _smsData.SetNameSender(nameSender);                    
                    _messengeQueue.EnqueueMessengeQueue(_smsData);
                    //Toast.MakeText(context, sb.ToString(), ToastLength.Long).Show();
                    Log.Info(TAG, sb.ToString());
                }
                if (!_config.RunningSpeech) SMSHandleSpeak();

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


            var loader = new CursorLoader(_context, uri, projection, ContactsContract.CommonDataKinds.Phone.Number + "=" + number, null, null);
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

		//Speech Interface Implement
		public void OnBeginningOfSpeech()
        {
            Log.Info(TAG, "onBeginningOfSpeech");
        }

        public void OnBufferReceived(Byte[] buffer)
        {
            Log.Info(TAG, "onBufferReceived: " + buffer);
        }

        public void OnEndOfSpeech()
        {
            Log.Info(TAG, "onEndOfSpeech");
        }

        public void OnError(SpeechRecognizerError err)
        {
            Log.Debug(TAG, "FAILED " + err.ToString());
            _tcs.TrySetResult(-1);


        }

        public void OnEvent(Int32 flag, Bundle bundle)
        {
            Log.Info(TAG, "onEvent");
        }

        public void OnPartialResults(Bundle bundle)
        {
            Log.Info(TAG, "onPartialResults");
        }

        public void OnReadyForSpeech(Bundle bundel)
        {
            Log.Info(TAG, "onReadyForSpeech");
        }

        public void OnResults(Bundle bundle)
        {
			IList<string> result = bundle.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
			Log.Info(TAG, "onResults ");
            _answer = result[0];
            _tcs.SetResult(0);

		}

        public void OnRmsChanged(Single single)
        {
            Log.Info(TAG, "onRmsChanged: " + single);
            //progressBarControl();

        }
    }
}
    
