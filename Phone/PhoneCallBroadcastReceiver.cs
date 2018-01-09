using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Util;
using Android.Telephony;
using Android.Runtime;
using Android.Provider;

namespace FreeHand.Phone
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class PhoneCallBroadcastReceiver : BroadcastReceiver
    {
         static readonly string TAG = "PhoneCallBroadcastReceiver";
       
         Config _config;
        TTSLib _tts;
        string _telephone,_answer;
        bool _acceptCall;
        Context _context;
        int _missCallBeforeIncomingCall;
        RingerMode _stateRingMode;
        Model.ScriptLang _scriptLang;
        public PhoneCallBroadcastReceiver()
        {
            Log.Info(TAG, "Initializing");
            _config = Config.Instance();
            _tts = TTSLib.Instance();
            _scriptLang = Model.ScriptLang.Instance();
            _missCallBeforeIncomingCall = 0;
        }


         async Task PhoneCallHanler()
        {
            SetSilentRingMode();
            //if (_config.IsUpdateCfg)
            //{
            //    _config.IsUpdateCfg = false;
            //    await _tts.ReInitTTS();
            //}

            await _tts.GetTTS();
            string nameCaller;
            nameCaller = Model.Commom.GetNameFromPhoneNumber(_telephone);
            if (string.IsNullOrEmpty(nameCaller)){
                nameCaller = _scriptLang.tts_name_sender_content;
            }
            string script;
            //await _tts.SpeakMessenger("You Has New Call From ");
            //script = _context.GetString(Resource.String.tts_new_call);
            await _tts.SpeakMessenger(_scriptLang.tts_new_call);
            await Task.Delay(500);

            //script = _context.GetString(Resource.String.tts_from);
            //await _tts.SpeakMessenger(script);
            await _tts.SpeakMessenger(_scriptLang.tts_from);
            await Task.Delay(500);

            await _tts.SpeakMessenger(_telephone);
            await Task.Delay(500);

            //await _tts.SpeakMessenger("Name Caller ");
            //script = _context.GetString(Resource.String.tts_name_caller);
            //await _tts.SpeakMessenger(script);
            await _tts.SpeakMessenger(_scriptLang.tts_name_caller);
            await Task.Delay(500);
            await _tts.SpeakMessenger(nameCaller);

            //Intent buttonDown = new Intent(Intent.ActionMediaButton);
            //buttonDown.PutExtra(Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Up, Keycode.Headsethook));
            //Application.Context.SendOrderedBroadcast(buttonDown, Android.Manifest.Permission.CallPrivileged);
        }

         void SetSilentRingMode()
        {
            AudioManager am;
            am = (AudioManager)Application.Context.GetSystemService(Context.AudioService);
            _stateRingMode = am.RingerMode;
            am.RingerMode = RingerMode.Silent;
        }

         void RestoreRingMode()
        {
            AudioManager am;
            am = (AudioManager)Application.Context.GetSystemService(Context.AudioService);
            am.RingerMode = _stateRingMode;
        }

        public static int CountMissCall()
        {
            // filter call logs by type = missed
            string queryFilter = String.Format("{0}={1}", CallLog.Calls.Type, (int)CallType.Missed);
            // filter in desc order limit by no
            string querySorter = String.Format("{0} desc ", CallLog.Calls.Date);
            // CallLog.Calls.ContentUri is the path where data is saved
            Android.Database.ICursor queryData = Application.Context.ContentResolver.Query(CallLog.Calls.ContentUri, null, queryFilter, null, querySorter);
            int missCall = 0;
            while (queryData.MoveToNext())
            {
                //---phone number---
                string callNumber = queryData.GetString(queryData.GetColumnIndex(CallLog.Calls.Number));

                //---date of call---
                string callDate = queryData.GetString(queryData.GetColumnIndex(CallLog.Calls.Date));

                //---1-incoming; 2-outgoing; 3-missed---
                String callType = queryData.GetString(queryData.GetColumnIndex(CallLog.Calls.Type));
                String callNew= queryData.GetString(queryData.GetColumnIndex(CallLog.Calls.New));
                //Log.Info(TAG,"Number {0} date {1} type {2} isNew {3} ",callNumber,callDate,callType,callNew);
                if (callType.Equals("3") && callNew.Equals("1"))
                    missCall++;
            }
            Log.Info(TAG,"Has {0} misscall",missCall);
            return missCall;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            _context = context;
            Log.Info(TAG, "OnReceive");
            // ensure there is information
            if (intent.Extras != null)
            {
                _config.phone.IsHandlePhoneRunnig = true;
                // get the incoming call state
                string state = intent.GetStringExtra(TelephonyManager.ExtraState);
                // check the current state
                if (state == TelephonyManager.ExtraStateRinging)
                {
                    _acceptCall = false;
                    Log.Info(TAG, "Phone ExtraStateRinging");
                    _telephone = intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);
                    Log.Info(TAG, "Incoming Numer " + _telephone);
                    _missCallBeforeIncomingCall = CountMissCall();
                    CheckCall(_telephone, context);                   
                }
                else if (state == TelephonyManager.ExtraStateOffhook)
                {
                    // incoming call answer
                    _acceptCall = true;
                    Log.Info(TAG, "Phone ExtraStateOffhook");

                }
                else if (state == TelephonyManager.ExtraStateIdle)
                {
                    RestoreRingMode();
                    _tts.Stop();
                    _config.phone.IsHandlePhoneRunnig = false;
                    _config.phone.MissedCall++;
                    Log.Info(TAG, "Phone ExtraStateIdle,SMS running "+_config.sms.IsHandleSMSRunnig.ToString()  ); 
                    Log.Info(TAG, "Send Speak Broadcast"); 
                    if (_config.sms.IsHandleSMSRunnig)
                    {                        
                        _config.sms.IsHandleSMSRunnig = false;

                    }      
                    var speakSMSIntent = new Intent("FreeHand.QueueMessenge.Invoke");
                    context.SendBroadcast(speakSMSIntent);                                                         
                }
                if (_config.phone.AutoReply && !_acceptCall)
                {

                    if (CountMissCall() > _missCallBeforeIncomingCall)
                    {
                        Log.Info(TAG, "Send sms inform miss call, content sms:  {0}", _config.phone.ContentReply);
                        SendReply(_telephone);
                    }
                }
                _missCallBeforeIncomingCall = 0;

            }
        }                     

        async void CheckCall(string telephone,Context context)
        {
            if (_config.phone.BlockAll)
                EndCall(context);
            else if (_config.phone.BlockInList)
            {
                bool isBlock = BlockCall(telephone,context);
                if (!isBlock) await PhoneCallHanler();
            }
            else 
                await PhoneCallHanler();
        }

        bool BlockCall(string telephone, Context context)
        {
            bool inBlackList = false;
            foreach (var item in _config.phone.BlackList)
            {
                if (PhoneNumberUtils.Compare(telephone,item.Item1))
                {
                    inBlackList = true;
                    break;
                }
            }

            if (inBlackList)
                EndCall(context);
            
            return inBlackList;
        }

        void SendReply(string telephone)
        {
            SmsManager.Default.SendTextMessage(telephone, null, _config.phone.ContentReply, null, null);
        }



        void EndCall(Context context)
        {
            Log.Info(TAG,"End call");
            var manager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
            IntPtr TelephonyManager_getITelephony = JNIEnv.GetMethodID(
                manager.Class.Handle,
                "getITelephony",
                "()Lcom/android/internal/telephony/ITelephony;");
            IntPtr telephony = JNIEnv.CallObjectMethod(manager.Handle, TelephonyManager_getITelephony);
            IntPtr ITelephony_class = JNIEnv.GetObjectClass(telephony);
            IntPtr ITelephony_endCall = JNIEnv.GetMethodID(
                ITelephony_class,
                "endCall",
                "()Z");
            JNIEnv.CallBooleanMethod(telephony, ITelephony_endCall);
            JNIEnv.DeleteLocalRef(telephony);
            JNIEnv.DeleteLocalRef(ITelephony_class);



        }
    }
}
