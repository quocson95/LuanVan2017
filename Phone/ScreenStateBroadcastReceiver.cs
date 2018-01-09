using System;
using Android.Content;
using Android.Util;
using Android.Provider;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using Plugin.Vibrate;
using Android.OS;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace FreeHand.Phone
{
    [BroadcastReceiver]
    public class ScreenStateBroadcastReceiver : BroadcastReceiver
    {
        private static readonly string TAG = "ScreenStateBroadcastReceiver";
        private string queryFilter = String.Format("{0}={1}", CallLog.Calls.Type, (int)CallType.Missed);
        string querySorter = String.Format("{0} desc limit 3", CallLog.Calls.Date);
        private DeviceMotionImplementation sensor;
        private string _lastValueX;
        IList<string> _lastValue;
        CircularBuffer<string> _buffer;
        private TTSLib _tts;
        private Config _config;
        public ScreenStateBroadcastReceiver()       
        {            
            _lastValueX = null;
            sensor = new DeviceMotionImplementation();
            _tts = TTSLib.Instance();
            _config = Config.Instance();
            _buffer = new CircularBuffer<string>(5);


        }

        public override void OnReceive(Context context, Intent intent)
        {
            Log.Info(TAG, "Intent received: " + intent.Action);
            //
            switch (intent.Action)
            {
                case (Intent.ActionScreenOff):
                    HandleScreenOffEvent(context);
                    break;
                case (Intent.ActionScreenOn):
                    HandleScreenOnEvent();
                    break;
                default:
                    break;
            }
        }

        private void HandleScreenOffEvent(Context context)
        {
            Log.Info(TAG,"HandleScreenOffEvent");
            if (IsMissCallLogExist(context))
            {
                StartMonitorSensor(context);
            }
        }

        private void HandleScreenOnEvent()
        {
            if (sensor.IsActive(MotionSensorType.Accelerometer))
                sensor.Stop(MotionSensorType.Accelerometer);
        }

        private bool IsMissCallLogExist(Context context)
        {
            bool result = false;
            Android.Database.ICursor queryData = context.ContentResolver.Query(CallLog.Calls.ContentUri, null, queryFilter, null, querySorter);
            if (queryData.MoveToNext())
            {
                result = true;
            }
            return result;
        }

        private async void StartMonitorSensor(Context context)
        {            
            sensor.Start(MotionSensorType.Accelerometer,MotionSensorDelay.Default);
            sensor.SensorValueChanged += async (s, a) =>
                {
                    switch (a.SensorType)
                    {
                        case MotionSensorType.Accelerometer:
                            Console.WriteLine("A: {0},{1},{2}", ((MotionVector)a.Value).X, ((MotionVector)a.Value).Y, ((MotionVector)a.Value).Z);
                            if (DectectDeviceMotion((MotionVector)a.Value))
                            {                                                      
                                Log.Info(TAG, "Device Motion");
                                //sensor.Stop(MotionSensorType.Accelerometer);
                                int n = PhoneCallBroadcastReceiver.CountMissCall();         
                                if (n > 0)
                                {
                                    var v = CrossVibrate.Current;
                                    if (v.CanVibrate) v.Vibration(); //Default 500ms

                                    var powerManager = (PowerManager)context.GetSystemService(Context.PowerService);
                                    var wakeLock = powerManager.NewWakeLock(WakeLockFlags.ScreenDim | WakeLockFlags.AcquireCausesWakeup, "Mrkeys");
                                    wakeLock.Acquire();
                                    wakeLock.Release();

                                    if (_config.GetPermissionRun(Config.PERMISSION_RUN.NOTIFY_MISS_CALL))
                                    {
                                        StringBuilder builder = new StringBuilder(Model.ScriptLang.Instance().tts_you_has_n_miss_call);                                  
                                    builder.Replace("xxx", n.ToString()); // Replaces 'xxx' with 'number miss call'.
                                        await _tts.SpeakMessenger(builder.ToString());
                                    }

                                }

                            }
                            break;
                    }

                };

        }

        private bool DectectDeviceMotion(MotionVector x)
        {
            bool isMotion;
            string value;
            value = x.X.ToString();
            //_buffer.pu
            isMotion = false;
            float t1, t2;
            t1 = 0;t2 = 0;
            if (_lastValueX != null){
                t1 = float.Parse(_lastValueX, CultureInfo.InvariantCulture.NumberFormat);
                t2 = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
            }
            Log.Debug(TAG, "Last value {0} value {1} compare {2} t1 - t2 {3}", _lastValueX, value, string.Compare(_lastValueX, value), t1 - t2);
            //if (_lastValueX != null && string.Compare(_lastValueX,value) != 0)
            //{
            //    isMotion = true;
            //}
            if (Math.Abs(t1 - t2) >= 2)
                isMotion = true;
            _lastValueX = value;
            return isMotion;
        }

       
    }
}

