using Android.Widget;
using Android.App;
using Android.OS;
using FreeHand;
using System;
using Android.Content;
using Android.Util;
using Calligraphy;
using Newtonsoft.Json;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "Setting_Messenge",Theme = "@style/MyTheme.Mrkeys")]
    public class Setting_Messenge : Activity
    {
        private readonly string TAG = typeof(Setting_Messenge).FullName;
        private Switch _swEnable_sms, _swEnable_mail;
        private Switch _swAllowSpeakNameSMS, _swAllowSpeakNumberSMS, _swAllowSpeakContentSMS, _swAllowAutoReplySMS;
        private Switch _swAllowAutoReplyMail;
        private TextView _customSMSReply, _customMailReply;
        private TextView _tvContentSMSReply, _tvContentMailReply, _blockSMS;
        private Config _cfg;
        private Android.Graphics.Color color_grey, color_black;
        delegate void Del();
        Del DbackUp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);                      

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                                          .SetDefaultFontPath("Fonts/HELR45W.ttf")
                                          .SetFontAttrId(Resource.Attribute.fontPath)
                                          .Build());

            SetContentView(Resource.Layout.Setting_Messenge_Layout);
            _cfg = Config.Instance();
            DbackUp += _cfg.smsConfig.Backup;
            InitUI();
            InitListenerUI();
            InitDataUI();
            // Create your application here
        }

        private void InitUI()
        {
            //init color
            color_grey = new Android.Graphics.Color(185, 185, 185);
            color_black = new Android.Graphics.Color(0, 0, 0);

            //SMS
            _swEnable_sms = FindViewById<Switch>(Resource.Id.sw_enable_messenge);
            _swAllowSpeakNameSMS = FindViewById<Switch>(Resource.Id.sw_allow_speak_namesender_sms);
            _swAllowSpeakNumberSMS = FindViewById<Switch>(Resource.Id.sw_allow_speak_numsender_sms);
            _swAllowSpeakContentSMS = FindViewById<Switch>(Resource.Id.sw_allow_speak_content_sms);
            _swAllowAutoReplySMS = FindViewById<Switch>(Resource.Id.sw_allow_reply_sms);
            _customSMSReply = FindViewById<TextView>(Resource.Id.tv_custom_sms_reply);
            _tvContentSMSReply = FindViewById<TextView>(Resource.Id.tv_content_custom_reply);
            _blockSMS = FindViewById<TextView>(Resource.Id.block_sms);

            //Mail
            _swEnable_mail = FindViewById<Switch>(Resource.Id.sw_enable_mail);
            _swAllowAutoReplyMail = FindViewById<Switch>(Resource.Id.sw_allow_reply_mail);
            _tvContentMailReply = FindViewById<TextView>(Resource.Id.tv_custom_mail_reply);

        }

        private void InitDataUI()
        {
            /*
             * SMS
             */           
            _tvContentSMSReply.Text = _cfg.smsConfig.CustomContetnReply;
            _swEnable_sms.Checked = _cfg.smsConfig.Enable;
        }

        private void RestoreStateSwSMS()
        {
            var smsCfg = _cfg.smsConfig;
            _swAllowSpeakNameSMS.Checked = smsCfg.AllowSpeakName;
            _swAllowSpeakNumberSMS.Checked = smsCfg.AllowSpeakNumber;
            _swAllowSpeakContentSMS.Checked = smsCfg.AllowSpeakContent;
            _swAllowAutoReplySMS.Checked = smsCfg.AllowAutoReply;
        }

        private void InitListenerUI()
        {
            /*
             * SMS
             */
            //Enable
            _swEnable_sms.CheckedChange += CheckedChangeHandle;

            //Speak Name
            _swAllowSpeakNameSMS.CheckedChange += CheckedChangeHandle;

            //Speak Number
            _swAllowSpeakNumberSMS.CheckedChange += CheckedChangeHandle;

            //Speak Content
            _swAllowSpeakContentSMS.CheckedChange += CheckedChangeHandle;

            //Auto Reply
            _swAllowAutoReplySMS.CheckedChange += CheckedChangeHandle;

            _customSMSReply.Click += ActionCustomContentReply;
            _tvContentSMSReply.Click += ActionCustomContentReply;

            _blockSMS.Click += delegate {
                Intent intent = new Intent(this, typeof(BlockNumberActivity));
                string type = JsonConvert.SerializeObject(Model.Constants.TYPE.SMS);
                intent.PutExtra("type",type);
                StartActivity(intent);
            };
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode.Equals(Model.Constants.CODE_SETTING_CONTENT_REPLY))
            {
                if(resultCode == Result.Ok)
                {
                    _cfg.smsConfig.CustomContetnReply = data.GetStringExtra("content_reply_ok");
                    _tvContentSMSReply.Text = _cfg.smsConfig.CustomContetnReply;
                }
            }
        }
        private void CheckedChangeHandle(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Switch sw = (Switch)sender;

            if (sw.Equals(_swEnable_sms))
            {
                HandleSwEnable(e.IsChecked);
            }
            else
            {              
                _swEnable_sms.Checked = _swAllowAutoReplySMS.Checked ||
                    _swAllowSpeakNameSMS.Checked ||
                    _swAllowSpeakNumberSMS.Checked ||
                    _swAllowSpeakContentSMS.Checked;
                _cfg.smsConfig.Enable = _swEnable_sms.Checked;           

                if (sw.Equals(_swAllowSpeakNameSMS))
                {
                    _cfg.smsConfig.AllowSpeakName = e.IsChecked;
                }
                else if (sw.Equals(_swAllowSpeakNumberSMS))
                {
                    _cfg.smsConfig.AllowSpeakNumber = e.IsChecked;
                }
                else if (sw.Equals(_swAllowSpeakContentSMS))
                {
                    _cfg.smsConfig.AllowSpeakContent = e.IsChecked;
                }
                else if (sw.Equals(_swAllowAutoReplySMS))
                {
                    HandleSWAutoReplySMS(e.IsChecked);
                }

                if (DbackUp != null) DbackUp();
            }
        }

        private void HandleSwEnable(bool isChecked)
        {
            _cfg.smsConfig.Enable = isChecked;
            if (isChecked)
            {           
                _blockSMS.SetTextColor(color_black);
                Model.Commom.StartSMSService();
                bool check = _swAllowAutoReplySMS.Checked ||
                    _swAllowSpeakNameSMS.Checked ||
                    _swAllowSpeakNumberSMS.Checked ||
                    _swAllowSpeakContentSMS.Checked;
                if (!check)
                {
                    _cfg.smsConfig.Restore();
                    RestoreStateSwSMS();
                }

            }
            else
            {
                Model.Commom.StopSMSService();
                _cfg.smsConfig.Backup();
                _blockSMS.SetTextColor(color_grey);
                DisableAllServiceSMS();
            }
        }

        private void HandleSWAutoReplySMS(bool isChecked)
        {
            _cfg.smsConfig.AllowAutoReply = isChecked;
            if (isChecked)
            {
                _customSMSReply.SetTextColor(color_black);
                _tvContentSMSReply.SetTextColor(color_black);
            }
            else
            {
                _customSMSReply.SetTextColor(color_grey);
                _tvContentSMSReply.SetTextColor(color_grey);
            }
        }

        private void DisableAllServiceSMS()
        {
            DbackUp -= _cfg.smsConfig.Backup;
            _swAllowSpeakNameSMS.Checked = false;
            _swAllowSpeakNumberSMS.Checked = false;
            _swAllowSpeakContentSMS.Checked = false;
            _swAllowAutoReplySMS.Checked = false;
            DbackUp += _cfg.smsConfig.Backup;
            
        }

        void ActionCustomContentReply(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(Custom_Reply_Messenge));
            intent.PutExtra("type", "sms");
            StartActivityForResult(intent, Model.Constants.CODE_SETTING_CONTENT_REPLY);
        }

        protected override void OnStop()
        {
            Log.Info(TAG,"OnStop");
            if (_swEnable_sms.Checked) _cfg.smsConfig.Backup();
            _cfg.SaveSMSConfig();
            base.OnStop();

        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

    }
}
