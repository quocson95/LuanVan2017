using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Util;
using Android.Telephony;
using Android.Runtime;

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
        RingerMode _stateRingMode;
        Model.ScriptLang _scriptLang;
        public PhoneCallBroadcastReceiver()        
        {
            Log.Info(TAG, "Initializing");           
            _config = Config.Instance();
            _tts = TTSLib.Instance();
            _scriptLang = Model.ScriptLang.Instance();
        }

         async Task PhoneCallHanler()
        {
            SetSilentRingMode();
            if (_config.IsUpdateCfg)
            {
                _config.IsUpdateCfg = false;
                await _tts.ReInitTTS();
            }

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


        public override void OnReceive(Context context, Intent intent)
        {
            _context = context;
            Log.Info(TAG, "OnReceive");
            // ensure there is information
            if (intent.Extras != null)
            {
                // get the incoming call state
                string state = intent.GetStringExtra(TelephonyManager.ExtraState);
                // check the current state
                if (state == TelephonyManager.ExtraStateRinging)
                {
                    _acceptCall = false;
                    Log.Info(TAG, "Phone ExtraStateRinging");
                    _telephone = intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);
                    Log.Info(TAG, "Incoming Numer " + _telephone);                                     
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
                    _config.phone.IsHandlePhoneRunnig = false;
                    _config.phone.MissedCall++;
                    Log.Info(TAG, "Phone ExtraStateIdle,SMS running "+_config.sms.IsHandleSMSRunnig.ToString()  ); 
                    if (_config.sms.IsHandleSMSRunnig)
                    {
                        Log.Info(TAG, "Send Speak Broadcast"); 
                        _config.sms.IsHandleSMSRunnig = false;
                        var speakSMSIntent = new Intent("FreeHand.QueueMessenge.Invoke");
                        context.SendBroadcast(speakSMSIntent);
                    }                  

                    if (_config.phone.AutoReply && !_acceptCall)
                    {
                        Log.Info(TAG,"Send sms inform miss call {0}",_config.phone.ContentReply);
                        SendReply(_telephone);
                    }
                    // incoming call end
                }
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
