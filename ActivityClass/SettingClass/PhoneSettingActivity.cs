
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
        private Switch _swEnable, _swAutoAcceptCall, _swSmartAlert, _swAutoReply;
        private SeekBar _skTimeAcceptCall;
        private TextView _labelTimeAcceptCall, _contentPhoneReply;
        private int _timeAcceptCall;
        private Config _config;
        private Android.Graphics.Color color_text_disale, color_text_enable;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Phone_Layout);
            _config = Config.Instance();
            InitUI();
            SetListenerUI();
            InitData();
            // Create your application here
        }

        private void InitData()
        {
            _config.phoneConfig.Restore();
            _swEnable.Checked = _config.phoneConfig.Enable;
            _contentPhoneReply.Text = _config.phoneConfig.ContentReply;
        }

        private void InitUI()
        {
            color_text_disale = new Android.Graphics.Color(185, 185, 185);
            color_text_enable = new Android.Graphics.Color(125, 199, 192);

            _swEnable = (Switch)FindViewById(Resource.Id.sw_enable);
            _swAutoAcceptCall = (Switch)FindViewById(Resource.Id.sw_auto_accept_call);
            _swSmartAlert = (Switch)FindViewById(Resource.Id.sw_smartAlert);
            _skTimeAcceptCall = (SeekBar)FindViewById(Resource.Id.sk_time_auto_accept_call);
            _labelTimeAcceptCall = (TextView)FindViewById(Resource.Id.label_time_auto_accept_call);
            _swAutoReply = FindViewById<Switch>(Resource.Id.auto_reply_when_miss_call);
            _contentPhoneReply = FindViewById<TextView>(Resource.Id.content_custom_phone_reply);

            _swEnable.Checked = _config.phoneConfig.Enable;
            _swSmartAlert.Checked = _config.phoneConfig.SmartAlert;
            _swAutoAcceptCall.Checked = _config.phoneConfig.AllowAutoAcceptCall;
            _timeAcceptCall = _config.phoneConfig.TimeAutoAcceptCall;
            _labelTimeAcceptCall.Text = _timeAcceptCall.ToString() + "s";
            _skTimeAcceptCall.Progress = _timeAcceptCall * 10 / 3;

        }

        private void SetListenerUI()
        {
            _swEnable.CheckedChange += CheckedChange;         
            //    _config.phoneConfig.Enable = _swEnable.Checked;
            //    if (_swEnable.Checked)
            //    {                    
            //        _config.phoneConfig.Restore();
            //        RestoreStateSwPhone();
            //    }
            //    else 
            //    {      
            //        _config.phoneConfig.Backup();
            //        DisableAllServiceSMS();
            //    }
            //};

            _swSmartAlert.CheckedChange += CheckedChange;

            _swAutoAcceptCall.CheckedChange += CheckedChange;

            _skTimeAcceptCall.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {                            
                    _timeAcceptCall = e.Progress * 3 / 10;
                    Console.WriteLine("Time Accept Call Change "+_timeAcceptCall);
                    _labelTimeAcceptCall.Text = _timeAcceptCall.ToString() + "s";
                    _config.phoneConfig.TimeAutoAcceptCall = _timeAcceptCall;
            };    
            _swAutoReply.CheckedChange += CheckedChange;
        }

        private void DisableAllServicePhone()
        {
            _config.phoneConfig.Backup();
            _swAutoReply.Checked = false;
            _swSmartAlert.Checked = false;
            _swAutoAcceptCall.Checked = false;

        }

        private void RestoreStateSwPhone()
        {
            _config.phoneConfig.Restore();
            _swAutoReply.Checked = _config.phoneConfig.AutoReply;
            _swSmartAlert.Checked = _config.phoneConfig.SmartAlert;
            _swAutoAcceptCall.Checked = _config.phoneConfig.AllowAutoAcceptCall;

        }


        private void CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {                        
            Switch sw = (Switch)sender;
            if (sw.Equals(_swAutoReply))
            {
                _config.phoneConfig.AutoReply = _swAutoReply.Checked;
                if (e.IsChecked)
                {
                    _contentPhoneReply.Click += ActionCustomContentReply;
                    _contentPhoneReply.SetTextColor(color_text_enable);
                }
                else
                {
                    _contentPhoneReply.Click -= ActionCustomContentReply;
                    _contentPhoneReply.SetTextColor(color_text_disale);
                }
            }
            else if (sw.Equals(_swSmartAlert))
            {
                _config.phoneConfig.SmartAlert = _swSmartAlert.Checked;
            }
            else if (sw.Equals(_swAutoAcceptCall))
            {
                _config.phoneConfig.AllowAutoAcceptCall = _swAutoAcceptCall.Checked;
            }
            else if (sw.Equals(_swEnable))
            {
                _config.phoneConfig.Enable = _swEnable.Checked;
            }
            CheckAllSw(sw);

        }

        private void CheckAllSw(Switch sender)
        {
            bool result = _swAutoReply.Checked || _swSmartAlert.Checked || _swAutoAcceptCall.Checked;

            if (sender.Equals(_swEnable))
            {
                if (_swEnable.Checked)
                    RestoreStateSwPhone();                
                else 
                    DisableAllServicePhone();
            }
            else 
            {
                _swEnable.CheckedChange -= CheckedChange;
                _swEnable.Checked = result;
                _swEnable.CheckedChange += CheckedChange;
            }
           

        }


        private void ActionCustomContentReply(object sender, EventArgs e)
        {
            
        }
        protected override void OnStop()
        {
            Log.Info(TAG, "OnStop");
            _config.Save();
            base.OnStop();
        }
    }
}
