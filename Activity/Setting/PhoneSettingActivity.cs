
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace FreeHand
{
    [Activity(Label = "PhoneSettingActivity")]
    public class PhoneSettingActivity : Activity
    {
        private static readonly string TAG = "PhoneSettingActivity";
        private Switch _swEnable, _swAutoAcceptCall, _swSmartAlert;
        private SeekBar _skTimeAcceptCall;
        private TextView _label_TimeAcceptCall;
        private int _timeAcceptCall;
        private Config _config;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Phone_Layout);
            _config = Config.Instance();
            InitUI();
            // Create your application here
        }

        private void InitUI()
        {
            _swEnable = (Switch)FindViewById(Resource.Id.sw_enable);
            _swAutoAcceptCall = (Switch)FindViewById(Resource.Id.sw_auto_accept_call);
            _swSmartAlert = (Switch)FindViewById(Resource.Id.sw_smartAlert);
            _skTimeAcceptCall = (SeekBar)FindViewById(Resource.Id.sk_time_auto_accept_call);
            _label_TimeAcceptCall = (TextView)FindViewById(Resource.Id.label_time_auto_accept_call);

            _swEnable.Checked = _config.phoneConfig.Enable;
            _swSmartAlert.Checked = _config.phoneConfig.SmartAlert;
            _swAutoAcceptCall.Checked = _config.phoneConfig.AllowAutoAcceptCall;
            _timeAcceptCall = _config.phoneConfig.TimeAutoAcceptCall;
            _label_TimeAcceptCall.Text = _timeAcceptCall.ToString() + "s";
            _skTimeAcceptCall.Progress = _timeAcceptCall * 10 / 3;
            SetListenerUI();
        }

        private void SetListenerUI()
        {
            _swEnable.CheckedChange += delegate {
                _config.phoneConfig.Enable = _swEnable.Checked;
            };

            _swSmartAlert.CheckedChange += delegate {
                _config.phoneConfig.SmartAlert = _swSmartAlert.Checked;
            };

            _swAutoAcceptCall.CheckedChange += delegate {
                _config.phoneConfig.AllowAutoAcceptCall = _swAutoAcceptCall.Checked;
            };

            _skTimeAcceptCall.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
                if (e.FromUser)
                {                    
                    _timeAcceptCall = e.Progress * 3 / 10;
                    Console.WriteLine("Time Accept Call Change "+_timeAcceptCall);
                    _label_TimeAcceptCall.Text = _timeAcceptCall.ToString() + "s";
                    _config.phoneConfig.TimeAutoAcceptCall = _timeAcceptCall;
                }
            };    
        }

        private void HandleSwitchEnable()
        {
            _config.phoneConfig.Enable = _swEnable.Checked;
            if (_swEnable.Checked == true) 
            {
                Log.Info(TAG,"swEnable = true");
            }
            else 
            {
                Log.Info(TAG, "swEnable = false");
            }
        }

        protected override void OnStop()
        {
            Log.Info(TAG, "OnStop");
            _config.save();
            base.OnStop();
        }
    }
}
