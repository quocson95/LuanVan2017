using Android.Widget;
using Android.App;
using Android.OS;
using FreeHand;
using System;
using Android.Content;
using Android.Util;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "Setting_Messenge")]
    public class Setting_Messenge : Activity
    {
        private readonly string TAG = typeof(Setting_Messenge).FullName;
        private Switch _swEnable_sms, _swEnable_mail;
        private Switch _swAllowSpeakNameSMS, _swAllowSpeakNumberSMS, _swAllowSpeakContentSMS, _swAllowAutoReplySMS;
        private Switch _swAllowAutoReplyMail;
        private TextView _customSMSReply, _customMailReply;
        private TextView _tvContentSMSReply, _tvContentMailReply;
        private Config _cfg;
        private Android.Graphics.Color color_sw_disale, color_sw_enable,color_content_enable;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Setting_Messenge_Layout);
            _cfg = Config.Instance();
            InitUI();
            // Create your application here
        }

        private void InitUI()
        {
            //init color
            color_sw_disale = new Android.Graphics.Color(185, 185, 185);
            color_sw_enable = new Android.Graphics.Color(125, 199, 192);
            color_content_enable = new Android.Graphics.Color(0, 0, 0);
            //SMS
            _swEnable_sms = FindViewById<Switch>(Resource.Id.sw_enable_messenge);
            _swAllowSpeakNameSMS = FindViewById<Switch>(Resource.Id.sw_allow_speak_namesender_sms);
            _swAllowSpeakNumberSMS = FindViewById<Switch>(Resource.Id.sw_allow_speak_numsender_sms);
            _swAllowSpeakContentSMS = FindViewById<Switch>(Resource.Id.sw_allow_speak_content_sms);
            _swAllowAutoReplySMS = FindViewById<Switch>(Resource.Id.sw_allow_reply_sms);

            _customSMSReply = FindViewById<TextView>(Resource.Id.tv_custom_sms_reply);
            _tvContentSMSReply = FindViewById<TextView>(Resource.Id.tv_content_custom_reply);              

            //Mail
            _swEnable_mail = FindViewById<Switch>(Resource.Id.sw_enable_mail);
            _swAllowAutoReplyMail = FindViewById<Switch>(Resource.Id.sw_allow_reply_mail);
            _tvContentMailReply = FindViewById<TextView>(Resource.Id.tv_custom_mail_reply);
            InitListenerUI();
            InitDataUI();

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
            smsCfg.Restore();
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
            _swEnable_sms.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) => 
            {                
                _cfg.smsConfig.Enable = e.IsChecked;
                if (e.IsChecked){
                    RestoreStateSwSMS();
                }
                else {
                    DisableAllServiceSMS();
                }
            };

            //Speak Name
            _swAllowSpeakNameSMS.CheckedChange += CheckedChange;

            //Speak Number
            _swAllowSpeakNumberSMS.CheckedChange += CheckedChange;

            //Speak Content
            _swAllowSpeakContentSMS.CheckedChange += CheckedChange;

            //Auto Reply
            _swAllowAutoReplySMS.CheckedChange += CheckedChange;

            _customSMSReply.Click += delegate {
                Intent intent = new Intent(this, typeof(Custom_Reply_Messenge));
                intent.PutExtra("type","sms");
                StartActivityForResult(intent,Model.Constants.Code_Setting_Messenge_SMS);
            };
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode.Equals(Model.Constants.Code_Setting_Messenge_SMS))
            {
                if(resultCode == Result.Ok)
                {
                    _cfg.smsConfig.CustomContetnReply = data.GetStringExtra("sms_reply_ok");
                    _tvContentSMSReply.Text = _cfg.smsConfig.CustomContetnReply;
                }
            }
        }
        private void CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Switch sw = (Switch)sender;
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
        }
            

        private void HandleSWAutoReplySMS(bool isChecked)
        {
            _cfg.smsConfig.AllowAutoReply = isChecked;
            if (isChecked)
            {
                _customSMSReply.SetTextColor(color_sw_enable);
                _tvContentSMSReply.SetTextColor(color_content_enable);
            }
            else
            {
                _customSMSReply.SetTextColor(color_sw_disale);
                _tvContentSMSReply.SetTextColor(color_sw_disale);
            }
        }

        private void DisableAllServiceSMS()
        {
            _cfg.smsConfig.Backup();
            _swAllowSpeakNameSMS.Checked = false;
            _swAllowSpeakNumberSMS.Checked = false;
            _swAllowSpeakContentSMS.Checked = false;
            _swAllowAutoReplySMS.Checked = false;
            
        }


        protected override void OnDestroy()
        {
            Log.Info(TAG,"Destroy");
            base.OnDestroy();
            _cfg.SaveSMSConfig();
        }
    }
}
