
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Calligraphy;
using Newtonsoft.Json;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "PhoneSettingActivity", Theme = "@style/MyTheme.Mrkeys")]
    public class SettingPhone : Activity
    {
         static readonly string TAG = "PhoneSettingActivity";
        Switch _swEnable,  _swSmartAlert, _swAutoReply;         
        TextView _contentPhoneReply, _labelContentReply, _declineCall, _labelPhone;         
        Config _config;
        Android.Graphics.Color color_grey, color_black;

        delegate void Del();
        Del DbackUp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Phone_Layout);

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                                          .SetDefaultFontPath("Fonts/HELR45W.ttf")
                                          .SetFontAttrId(Resource.Attribute.fontPath)
                                          .Build());


            _config = Config.Instance();
            DbackUp += _config.phone.Backup;
            InitUI();
            SetListenerUI();
            InitData();
           

            // Create your application here
        }
               

         void InitUI()
        {
            color_grey = new Android.Graphics.Color(185, 185, 185);
            color_black = new Android.Graphics.Color(0, 0, 0);

            _swEnable = (Switch)FindViewById(Resource.Id.sw_enable);
            _swSmartAlert = (Switch)FindViewById(Resource.Id.sw_smartAlert);           
            _swAutoReply = FindViewById<Switch>(Resource.Id.auto_reply_when_miss_call);
            _contentPhoneReply = FindViewById<TextView>(Resource.Id.content_phone_reply);
            _labelContentReply = FindViewById<TextView>(Resource.Id.lable_content_phone_reply);
            _declineCall = FindViewById<TextView>(Resource.Id.decline_call);
            _labelPhone = FindViewById<TextView>(Resource.Id.label_phone);                      

        }

         void SetListenerUI()
        {
            _swEnable.CheckedChange += CheckedChangeHandle;
            _swSmartAlert.CheckedChange += CheckedChangeHandle;
            _swAutoReply.CheckedChange += CheckedChangeHandle;

            _declineCall.Click += ActionBlockSMSActivity;

            _labelContentReply.Click += ActionCustomContentReply;
            _contentPhoneReply.Click += ActionCustomContentReply;
        }

        void InitData()
        {
            _contentPhoneReply.Text = _config.phone.ContentReply;
            _swEnable.Checked = _config.phone.Enable;
        }

        void DisableAllServicePhone()
        {
            DbackUp -= _config.phone.Backup;
            _swAutoReply.Checked = false;
            _swSmartAlert.Checked = false;
            DbackUp += _config.phone.Backup;

        }

         void RestoreStateSwPhone()
        {                        
            _swAutoReply.Checked = _config.phone.AutoReply;
            _swSmartAlert.Checked = _config.phone.SmartAlert;
        }


        void CheckedChangeHandle(object sender, CompoundButton.CheckedChangeEventArgs e)
        {                        
            Switch sw = (Switch)sender;
            if (sw.Equals(_swEnable))
                HandleSwEnable(e.IsChecked);
            else
            {               
                _swEnable.Checked = _swAutoReply.Checked || _swSmartAlert.Checked;//|| _swAutoAcceptCall.Checked ;
                _config.phone.Enable = _swEnable.Checked;

                if (sw.Equals(_swAutoReply))
                {
                    HandleSwAutoReply(e.IsChecked);
                }
                else if (sw.Equals(_swSmartAlert))
                {
                    HandleSwSmartAlert(e.IsChecked);
                }               
               
                if (DbackUp != null) DbackUp();
            }
           

        }

        private void HandleSwSmartAlert(bool isChecked)
        {
            _config.phone.SmartAlert = isChecked;

            if (isChecked)
                Model.Commom.StartSmartAlert();            
            else
                Model.Commom.StopSmartAlert();
        }

        void HandleSwEnable(bool isChecked)
        {
            _config.phone.Enable = isChecked;
            if (isChecked)
            {
                Model.Commom.StartPhoneSerive();
                _declineCall.SetTextColor(color_black);               
                bool check = _swAutoReply.Checked || _swSmartAlert.Checked;
                if (!check)
                {
                    _config.phone.Restore();
                    RestoreStateSwPhone();
                }
            }
            else
            {
                Model.Commom.StopPhoneService();
                _config.phone.Backup();
                DisableAllServicePhone();
                _declineCall.SetTextColor(color_grey);
            }
                
        }


        void HandleSwAutoReply(bool isChecked)
        {
            _config.phone.AutoReply = isChecked;
            UpdateUIColorReply();
        }

        private void UpdateUIColorReply()
        {
            if (_swAutoReply.Checked)
            {
                //_contentPhoneReply.Click += ActionCustomContentReply;
                //_labelContentReply.Click += ActionCustomContentReply;
                _contentPhoneReply.SetTextColor(color_black);
                _labelContentReply.SetTextColor(color_black);
            }
            else
            {
                //_contentPhoneReply.Click -= ActionCustomContentReply;
                //_labelContentReply.Click -= ActionCustomContentReply;
                _contentPhoneReply.SetTextColor(color_grey);
                _labelContentReply.SetTextColor(color_grey);

            }
        }

        void ActionCustomContentReply(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ActivityClass.SettingClass.Custom_Reply_Messenge));
            intent.PutExtra("type", "phone");
            StartActivityForResult(intent, Model.Constants.CODE_SETTING_CONTENT_REPLY);
        }

        void ActionBlockSMSActivity(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(BlockNumberActivity));
            string type = JsonConvert.SerializeObject(Model.Constants.TYPE.PHONE);
            intent.PutExtra("type", type);
            StartActivity(intent);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode.Equals(Model.Constants.CODE_SETTING_CONTENT_REPLY))
            {
                if (resultCode == Result.Ok)
                {                   
                    _config.phone.ContentReply = data.GetStringExtra("content_reply_ok");
                    _contentPhoneReply.Text = _config.phone.ContentReply;
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Info(TAG,"OnResume");
            SetLanguage();
        }

        private void SetLanguage()
        {
        //    Switch _swEnable,  _swSmartAlert, _swAutoReply;         
        //TextView _contentPhoneReply, _labelContentReply, _declineCall; 
            _labelPhone.SetText(Resource.String.label_setting_phone);
            _swEnable.SetText(Resource.String.label_setting_phone_enable);
            _swSmartAlert.SetText(Resource.String.label_setting_phone_speakmisscall);
            _swAutoReply.SetText(Resource.String.label_setting_phone_sendsms);
            _labelContentReply.SetText(Resource.String.label_setting_phone_contentreply);
            _declineCall.SetText(Resource.String.label_setting_phone_declinecall);

        }


        protected override void OnStop()
        {
            Log.Info(TAG, "OnStop");
            _config.SavePhoneConfig();
            base.OnStop();
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}
