using System;
using Android.Content;
using Android.Util;
using Android.Provider;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using Plugin.Vibrate;
using Android.OS;

namespace FreeHand.Phone
{
    [BroadcastReceiver]
    public class ScreenStateBroadcastReceiver : BroadcastReceiver
    {
        private static readonly string TAG = "ScreenStateBroadcastReceiver";
        private string queryFilter = String.Format("{0}={1}", CallLog.Calls.Type, (int)CallType.Missed);
        string querySorter = String.Format("{0} desc limit 3", CallLog.Calls.Date);
        private DeviceMotionImplementation sensor;
        private Context _context;
        private string _lastValueX;
        private TextToSpeechLib _tts;
        private Config _config;
        public ScreenStateBroadcastReceiver() {}
        public ScreenStateBroadcastReceiver(Context context)
        {
            _context = context;
            _lastValueX = null;
            sensor = new DeviceMotionImplementation();
            _tts = TextToSpeechLib.Instance();
            _config = Config.Instance();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Log.Info(TAG, "Intent received: " + intent.Action);
            //
            switch (intent.Action)
            {
                case (Intent.ActionScreenOff):
                    HandleScreenOffEvent();
                    break;
                case (Intent.ActionScreenOn):
                    HandleScreenOnEvent();
                    break;
                default:
                    break;
            }
        }

        private void HandleScreenOffEvent()
        {
            Log.Info(TAG,"HandleScreenOffEvent");
            if (IsMissCallLogExist())
            {
                StartMonitorSensor();
            }
        }

        private void HandleScreenOnEvent()
        {
        }

        private bool IsMissCallLogExist()
        {
            bool result = false;
            Android.Database.ICursor queryData = _context.ContentResolver.Query(CallLog.Calls.ContentUri, null, queryFilter, null, querySorter);
            if (queryData.MoveToNext())
            {
                result = true;
            }
            return result;
        }

        private void StartMonitorSensor()
        {            
            sensor.Start(MotionSensorType.Accelerometer,MotionSensorDelay.Default);
            sensor.SensorValueChanged += (s, a)  =>
                {
                    Console.WriteLine("A: {0},{1},{2}", ((MotionVector)a.Value).X, ((MotionVector)a.Value).Y, ((MotionVector)a.Value).Z);
                    if (CheckDeviceMotion((MotionVector)a.Value))
                    {                        
                        Log.Info(TAG, "Device Motion");
                        sensor.Stop(MotionSensorType.Accelerometer);
                        var v = CrossVibrate.Current;
                        if (v.CanVibrate) v.Vibration(); //Default 500ms
                        
                        var powerManager = (PowerManager)_context.GetSystemService(Context.PowerService);
                        var wakeLock = powerManager.NewWakeLock(WakeLockFlags.ScreenDim | WakeLockFlags.AcquireCausesWakeup, "StackOverflow");
                        wakeLock.Acquire();                        
                        wakeLock.Release();

                    if(_config.GetPermissionRun(Config.PERMISSION_RUN.NOTIFY_MISS_CALL))
                        _tts.SpeakMessenger("You have missed call");
                        
                    }
                    
                };

        }

        private bool CheckDeviceMotion(MotionVector x)
        {
            bool isMotion;
            string value;
            value = x.X.ToString();
            isMotion = false;
            if (_lastValueX != null && string.Compare(_lastValueX,value) != 0)
            {
                isMotion = true;
            }
            _lastValueX = value;
            return isMotion;
        }

       
    }
}

