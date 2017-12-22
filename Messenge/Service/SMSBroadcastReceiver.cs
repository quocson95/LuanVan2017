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

        public SMSBroadcastReceiver()
        {
            Log.Info(TAG, "Initializing");
            _messengeQueue = Model.MessengeQueue.GetInstance();
            _config = Config.Instance();
        }
               
       public override void OnReceive(Context context, Intent intent)
        {
            //InvokeASendBroadcasttBroadcast();
            Log.Info(TAG, "Intent received: " + intent.Action);

            try
            {
                if (intent.Action != IntentAction) return;

                var bundle = intent.Extras;
                if (bundle == null) return;

                var pdus = bundle.Get("pdus");
                var castedPdus = JNIEnv.GetArray<Java.Lang.Object>(pdus.Handle);
                var msgs = new SmsMessage[castedPdus.Length];              

                for (var i = 0; i < msgs.Length; i++)
                {
                    var bytes = new byte[JNIEnv.GetArrayLength(castedPdus[i].Handle)];
                    JNIEnv.CopyArray(castedPdus[i].Handle, bytes);

                    msgs[i] = SmsMessage.CreateFromPdu(bytes);
                  
                    Model.IMessengeData _smsData = new Model.SMSData(msgs[i].OriginatingAddress, msgs[i].MessageBody);
                    string nameSender = Model.Commom.GetNameFromPhoneNumber(_smsData.GetAddrSender());
                    Log.Info(TAG, "Name Sender " + nameSender);

                    _smsData.SetNameSender(string.IsNullOrEmpty(nameSender)?"Unknow":nameSender);
                    if (!NumberInBlockList(msgs[i].OriginatingAddress))
                    {
                        _messengeQueue.EnqueueMessengeQueue(_smsData);
                        Log.Info(TAG, "Num Messenger: " + _messengeQueue.Count().ToString());

                        //Send Broadcast for handle new messenge in queue
                        var speakSMSIntent = new Intent("FreeHand.QueueMessenge.Invoke");
                        context.SendBroadcast(speakSMSIntent);
                    }
                    else
                        Log.Info(TAG, "Block SMS From " + msgs[i].OriginatingAddress);                    
                }
            }
            catch (Exception ex)
            {               
                Log.Info(TAG, ex.Message);
            }
        }

        private bool NumberInBlockList(string originatingAddress)
        {
            foreach(var item in _config.sms.BlockList)
            {
                if (PhoneNumberUtils.Compare(originatingAddress,item.Item1 ))
                    return true;
            }
            return false;
        }
    }
}
    
