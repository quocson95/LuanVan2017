
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Speech;
using Android.Media;
using Android.Util;
using Android.Telephony;
using DeviceMotion.Plugin;
using Android.Runtime;

namespace FreeHand.Phone
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class PhoneCallBroadcastReceiver : BroadcastReceiver
    {
        private static readonly string TAG = "PhoneCallBroadcastReceiver";
       
        private Config _config;
        TTSLib _tts;
        string _telephone,_answer;
        public PhoneCallBroadcastReceiver()        
        {
            Log.Info(TAG, "Initializing");           
            _config = Config.Instance();
            _tts = TTSLib.Instance();
        }

        private async Task PhoneCallHanler()
        {
            if (_config.UpdateConfig)
            {
                _config.UpdateConfig = false;
                await _tts.ReInitTTS();
            }

            string nameCaller;
            nameCaller = Model.Commom.GetNameFromPhoneNumber(_telephone);

            await _tts.SpeakMessenger("You Has New Call From ");
            await Task.Delay(500);
            await _tts.SpeakMessenger(_telephone);
            await Task.Delay(500);
            await _tts.SpeakMessenger("Name Caller ");
            await Task.Delay(500);
            await _tts.SpeakMessenger(nameCaller);

            //Intent buttonDown = new Intent(Intent.ActionMediaButton);
            //buttonDown.PutExtra(Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Up, Keycode.Headsethook));
            //Application.Context.SendOrderedBroadcast(buttonDown, Android.Manifest.Permission.CallPrivileged);
        }

      
        public override void OnReceive(Context context, Intent intent)
        {            
            bool acceptCall;
            acceptCall = false;
            Log.Info(TAG, "OnReceive");
            // ensure there is information
            if (intent.Extras != null)
            {
                // get the incoming call state
                string state = intent.GetStringExtra(TelephonyManager.ExtraState);
                // check the current state
                if (state == TelephonyManager.ExtraStateRinging)
                {                    
                    Log.Info(TAG, "Phone ExtraStateRinging");
                    _telephone = intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);
                    Log.Info(TAG, "Incoming Numer " + _telephone);                                     
                    CheckCall(_telephone, context);                                                        
                }
                else if (state == TelephonyManager.ExtraStateOffhook)
                {
                    // incoming call answer
                    acceptCall = true;
                    Log.Info(TAG, "Phone ExtraStateOffhook");

                }
                else if (state == TelephonyManager.ExtraStateIdle)
                {
                    _config.phoneConfig.IsHandlePhoneRunnig = false;
                    _config.phoneConfig.MissedCall++;
                    Log.Info(TAG, "Phone ExtraStateIdle,SMS running "+_config.smsConfig.IsHandleSMSRunnig.ToString()  ); 
                    if (_config.smsConfig.IsHandleSMSRunnig)
                    {
                        Log.Info(TAG, "Send Speak Broadcast"); 
                        _config.smsConfig.IsHandleSMSRunnig = false;
                        var speakSMSIntent = new Intent("FreeHand.QueueMessenge.Invoke");
                        context.SendBroadcast(speakSMSIntent);
                    }                  

                    // incoming call end
                }
            }
        }

        private async void WaitAccepCall(string telephone)
        {
            if (_config.GetPermissionRun(Config.PERMISSION_RUN.PHONE))
            {             
                  _config.phoneConfig.IsHandlePhoneRunnig = true;
                  await PhoneCallHanler();
            }
        }

        private async void CheckCall(string telephone,Context context)
        {
            if (_config.phoneConfig.BlockAll)
                EndCall(context);
            else if (_config.phoneConfig.BlockInList)
            {
                bool isBlock = BlockCall(telephone,context);
                if (!isBlock) await PhoneCallHanler();
            }
            else 
                await PhoneCallHanler();
        }

        private bool BlockCall(string telephone, Context context)
        {
            bool inBlackList = false;
            foreach (var item in _config.phoneConfig.BlackList)
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

        private void SendReply(string telephone)
        {
            SmsManager.Default.SendTextMessage(telephone, null, _config.phoneConfig.ContentReply, null, null);
        }



        private void EndCall(Context context)
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
