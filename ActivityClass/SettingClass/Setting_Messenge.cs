using Android.Widget;
using Android.App;
using Android.OS;
using FreeHand;
using System;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "Setting_Messenge")]
    public class Setting_Messenge : Activity
    {
        private Switch _swEnable_sms, _swEnable_mail;
        private Switch _swAllowSpeakNameSMS, _swAllowSpeakNumberSMS, _swAllowSpeakContentSMS, _swAllowAutoReplySMS;
        private Switch _swAllowAutoReplyMail;
        private TextView _customSMSReply, _customMailReply;
        private TextView _tvContentSMSReply, _tvContentMailReply;
        private Config _cfg;
        private Android.Graphics.Color color_text_disale, color_text_enable,color_content_enable;
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
            color_text_disale = new Android.Graphics.Color(185, 185, 185);
            color_text_enable = new Android.Graphics.Color(125, 199, 192);
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

        }

        private void InitListenerUI()
        {
            //Enable
            _swEnable_sms.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) => 
            {
                _cfg.smsConfig.Enable = e.IsChecked;
                if (e.IsChecked){
                    EnableAllServiceSMS();
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
                _customSMSReply.SetTextColor(color_text_enable);
                _tvContentSMSReply.SetTextColor(color_content_enable);
            }
            else
            {
                _customSMSReply.SetTextColor(color_text_disale);
                _tvContentSMSReply.SetTextColor(color_text_disale);
            }
        }

        private void DisableAllServiceSMS()
        {
            _swAllowSpeakNameSMS.Checked = false;
            _swAllowSpeakNumberSMS.Checked = false;
            _swAllowSpeakContentSMS.Checked = false;
            _swAllowAutoReplySMS.Checked = false;
            
        }

        private void EnableAllServiceSMS()
        {
            _swAllowSpeakNameSMS.Checked = true;
            _swAllowSpeakNumberSMS.Checked = true;
            _swAllowSpeakContentSMS.Checked = true;
            _swAllowAutoReplySMS.Checked = true;
        }
    }
}
