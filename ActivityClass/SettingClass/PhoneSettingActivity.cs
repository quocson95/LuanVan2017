
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
         static readonly string TAG = "PhoneSettingActivity";
         Switch _swEnable, _swAutoAcceptCall, _swSmartAlert, _swAutoReply, _swRejectCall;
         SeekBar _skTimeAcceptCall;
         TextView _labelTimeAcceptCall, _contentPhoneReply, _labelAcceptCall, _labelContentReply;
         int _timeAcceptCall;
         Config _config;
         Android.Graphics.Color color_text_disale, color_text_enable,color_black;

        delegate void Del();
        Del DbackUp;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Phone_Layout);
            _config = Config.Instance();
            DbackUp += _config.phoneConfig.Backup;
            InitUI();
           

            // Create your application here
        }

         void InitData()
        {            
            _contentPhoneReply.Text = _config.phoneConfig.ContentReply;
            _labelTimeAcceptCall.Text = _config.phoneConfig.TimeAutoAcceptCall.ToString() + "s";
            _skTimeAcceptCall.Progress = _config.phoneConfig.TimeAutoAcceptCall * 10 / 3;
            _swEnable.Checked = _config.phoneConfig.Enable;
        }

         void InitUI()
        {
            color_text_disale = new Android.Graphics.Color(185, 185, 185);
            color_text_enable = new Android.Graphics.Color(125, 199, 192);
            color_black = new Android.Graphics.Color(0, 0, 0);

            _swEnable = (Switch)FindViewById(Resource.Id.sw_enable);
            _swAutoAcceptCall = (Switch)FindViewById(Resource.Id.sw_auto_accept_call);
            _swSmartAlert = (Switch)FindViewById(Resource.Id.sw_smartAlert);
            _skTimeAcceptCall = (SeekBar)FindViewById(Resource.Id.sk_time_auto_accept_call);
            _labelTimeAcceptCall = (TextView)FindViewById(Resource.Id.label_time_auto_accept_call);
            _swAutoReply = FindViewById<Switch>(Resource.Id.auto_reply_when_miss_call);
            _contentPhoneReply = FindViewById<TextView>(Resource.Id.content_phone_reply);
            _labelAcceptCall = FindViewById<TextView>(Resource.Id.label_auto_accept_call);
            _swRejectCall = FindViewById<Switch>(Resource.Id.auto_reject_call);
            _labelContentReply = FindViewById<TextView>(Resource.Id.lable_content_phone_reply);
            //_swEnable.Checked = _config.phoneConfig.Enable;
            //_swSmartAlert.Checked = _config.phoneConfig.SmartAlert;
            //_swAutoAcceptCall.Checked = _config.phoneConfig.AllowAutoAcceptCall;
            //_timeAcceptCall = _config.phoneConfig.TimeAutoAcceptCall;
            //_labelTimeAcceptCall.Text = _timeAcceptCall.ToString() + "s";
            //_skTimeAcceptCall.Progress = _timeAcceptCall * 10 / 3;
            SetListenerUI();
            InitData();

        }

         void SetListenerUI()
        {
            _swEnable.CheckedChange += CheckedChangeHandle;

            _swSmartAlert.CheckedChange += CheckedChangeHandle;

            _swAutoAcceptCall.CheckedChange += CheckedChangeHandle;

            _skTimeAcceptCall.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {                            
                    _timeAcceptCall = e.Progress * 3 / 10;
                    Console.WriteLine("Time Accept Call Change "+_timeAcceptCall);
                    _labelTimeAcceptCall.Text = _timeAcceptCall.ToString() + "s";
                    _config.phoneConfig.TimeAutoAcceptCall = _timeAcceptCall;
            };    
            _swAutoReply.CheckedChange += CheckedChangeHandle;
            _swRejectCall.CheckedChange += CheckedChangeHandle;
        }

        void DisableAllServicePhone()
        {
            DbackUp -= _config.phoneConfig.Backup;
            _swAutoReply.Checked = false;
            _swSmartAlert.Checked = false;
            _swAutoAcceptCall.Checked = false;
            _swRejectCall.Checked = false;
            DbackUp += _config.phoneConfig.Backup;

        }

         void RestoreStateSwPhone()
        {                        
            _swAutoReply.Checked = _config.phoneConfig.AutoReply;
            _swSmartAlert.Checked = _config.phoneConfig.SmartAlert;
            _swAutoAcceptCall.Checked = _config.phoneConfig.AllowAutoAcceptCall;
            _swRejectCall.Checked = _config.phoneConfig.AutoRejectCall;

        }


         void CheckedChangeHandle(object sender, CompoundButton.CheckedChangeEventArgs e)
        {                        
            Switch sw = (Switch)sender;
            if (sw.Equals(_swEnable))
                HandleSwEnable(e.IsChecked);
            else
            {
                _swEnable.CheckedChange -= CheckedChangeHandle;
                _swEnable.Checked = _swAutoReply.Checked || _swSmartAlert.Checked || _swAutoAcceptCall.Checked || _swRejectCall.Checked;
                _config.phoneConfig.Enable = _swEnable.Checked;
                _swEnable.CheckedChange += CheckedChangeHandle;

                if (sw.Equals(_swAutoReply))
                {
                    HandleSwAutoReply(e.IsChecked);
                }
                else if (sw.Equals(_swSmartAlert))
                {
                    _config.phoneConfig.SmartAlert = e.IsChecked;
                }
                else if (sw.Equals(_swAutoAcceptCall))
                {
                    HandleSwAutoAccepCall(e.IsChecked);
                }
                else if (sw.Equals(_swRejectCall))
                {
                    HandleSwRejectCall(e.IsChecked);
                }
                if (DbackUp != null) DbackUp();
            }
           

        }

        private void HandleSwRejectCall(bool isChecked)
        {
            _config.phoneConfig.AutoRejectCall = isChecked;
            UpdateUIColorReply();
        }

        void HandleSwEnable(bool isChecked)
        {
            _config.phoneConfig.Enable = isChecked;
            if (isChecked)
            {
                _config.phoneConfig.Restore();
                RestoreStateSwPhone();
            }
            else
            {
                _config.phoneConfig.Backup();
                DisableAllServicePhone();
            }
                
        }

        void HandleSwAutoAccepCall(bool isChecked)
        {
            _config.phoneConfig.AllowAutoAcceptCall = isChecked;
            if (isChecked)
            {
                _labelAcceptCall.SetTextColor(color_text_enable);
                _labelTimeAcceptCall.SetTextColor(color_text_enable);
            }
            else
            {
                _labelAcceptCall.SetTextColor(color_text_disale);
                _labelTimeAcceptCall.SetTextColor(color_text_disale);
            }
        }

        void HandleSwAutoReply(bool isChecked)
        {
            _config.phoneConfig.AutoReply = isChecked;
            UpdateUIColorReply();
        }

        private void UpdateUIColorReply()
        {
            if (_swAutoReply.Checked || _swRejectCall.Checked)
            {
                _contentPhoneReply.Click += ActionCustomContentReply;
                _labelContentReply.Click += ActionCustomContentReply;
                _contentPhoneReply.SetTextColor(color_black);
                _labelContentReply.SetTextColor(color_text_enable);
            }
            else
            {
                _contentPhoneReply.Click -= ActionCustomContentReply;
                _labelContentReply.Click -= ActionCustomContentReply;
                _contentPhoneReply.SetTextColor(color_text_disale);
                _labelContentReply.SetTextColor(color_text_disale);

            }
        }

        void ActionCustomContentReply(object sender, EventArgs e)
        {
            
        }
        protected override void OnStop()
        {
            Log.Info(TAG, "OnStop");
            _config.SavePhoneConfig();
            base.OnStop();
        }
    }
}
