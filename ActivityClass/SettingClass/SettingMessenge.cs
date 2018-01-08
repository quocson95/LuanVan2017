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
    public class SettingMessenge : Activity
    {
        private readonly string TAG = typeof(SettingMessenge).FullName;
        private Switch _swEnable_sms, _swEnable_mail;
        private Switch _swAllowSpeakNameSMS, _swAllowSpeakNumberSMS, _swAllowSpeakContentSMS, _swAllowAutoReplySMS;
        private Switch _swAllowAutoReplyMail,_swAllowSpeakAddrMail, _swAllowSpeakNameMail, _swAllowSpeakSubjectMail;

        private TextView _customSMSReply, _tvContentMailReply, _labelMail;
        private TextView _tvContentSMSReply, _customMailReply, _blockSMS,_labelSMS;

        private Config _cfg;
        private Android.Graphics.Color color_grey, color_black;
        delegate void Del();
        Del DbackUpSMS, DbackUpMail;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);                      

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                                          .SetDefaultFontPath("Fonts/HELR45W.ttf")
                                          .SetFontAttrId(Resource.Attribute.fontPath)
                                          .Build());

            SetContentView(Resource.Layout.Setting_Messenge_Layout);
            _cfg = Config.Instance();
            DbackUpSMS += _cfg.sms.Backup;
            DbackUpMail += _cfg.mail.Backup;
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
            _labelSMS = FindViewById<TextView>(Resource.Id.label_sms);

            //Mail
            _swEnable_mail = FindViewById<Switch>(Resource.Id.sw_enable_mail);
            _swAllowAutoReplyMail = FindViewById<Switch>(Resource.Id.sw_allow_reply_mail);
            _swAllowSpeakAddrMail = FindViewById<Switch>(Resource.Id.sw_allow_speak_addr_sender_mail);
            _swAllowSpeakNameMail = FindViewById<Switch>(Resource.Id.sw_allow_speak_name_sender_mail);
            _swAllowSpeakSubjectMail = FindViewById<Switch>(Resource.Id.sw_allow_speak_content_mail);
            _customMailReply = FindViewById<TextView>(Resource.Id.tv_custom_mail_reply);
            _labelMail = FindViewById<TextView>(Resource.Id.label_mail);
            _tvContentMailReply = FindViewById<TextView>(Resource.Id.tv_content_mail_custom_reply);
        }

        private void InitDataUI()
        {
            /*
             * SMS
             */           
            _tvContentSMSReply.Text = _cfg.sms.CustomContetnReply;
            _swEnable_sms.Checked = _cfg.sms.Enable;
            _swEnable_mail.Checked = _cfg.mail.Enable;
        }

        private void RestoreStateSwSMS()
        {
            var smsCfg = _cfg.sms;
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
            InitListenerSMS();
            InitListenerMail();

            /*
             * MAIL
             */

           
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode.Equals(Model.Constants.CODE_SETTING_CONTENT_REPLY))
            {
                if(resultCode == Result.Ok)
                {
                    string type = data.GetStringExtra("type");
                    string content = data.GetStringExtra("content_reply_ok");;
                    if (type.Equals("sms"))
                    {
                        _cfg.sms.CustomContetnReply = content;
                        _tvContentSMSReply.Text = _cfg.sms.CustomContetnReply;
                    }
                    else if (type.Equals("mail"))
                    {
                        _cfg.mail.ContentReply = content;
                        _tvContentMailReply.Text = content;
                    }
                }
            }
        }
        private void CheckedChangeHandleSMS(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Switch sw = (Switch)sender;

            if (sw.Equals(_swEnable_sms))
            {
                HandleSwEnable(e.IsChecked);
            }
            else
            {         
                if (e.IsChecked && !_swEnable_sms.Checked)
                {
                    _swEnable_sms.Checked = true;
                    _cfg.sms.Enable = true;
                }
               //bool tmp = _swAllowAutoReplySMS.Checked ||
                //    _swAllowSpeakNameSMS.Checked ||
                //    _swAllowSpeakNumberSMS.Checked ||
                //    _swAllowSpeakContentSMS.Checked;
                //if (tmp && !_swEnable_sms.Checked)
                //{
                //    _swEnable_sms.Checked = tmp;
                //    _cfg.sms.Enable = _swEnable_sms.Checked;
                //}

                if (sw.Equals(_swAllowSpeakNameSMS))
                {
                    _cfg.sms.AllowSpeakName = e.IsChecked;
                }
                else if (sw.Equals(_swAllowSpeakNumberSMS))
                {
                    _cfg.sms.AllowSpeakNumber = e.IsChecked;
                }
                else if (sw.Equals(_swAllowSpeakContentSMS))
                {
                    _cfg.sms.AllowSpeakContent = e.IsChecked;
                }
                else if (sw.Equals(_swAllowAutoReplySMS))
                {
                    HandleSWAutoReplySMS(e.IsChecked);
                }

                DbackUpSMS?.Invoke();
            }
        }

        void CheckedChangeHandleMail(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Switch sw = (Switch)sender;
            if (sw.Equals(_swEnable_mail))
            {
                HandleSwEnableMail(e.IsChecked);
            }
            else 
            {                

                if (e.IsChecked && !_swEnable_mail.Checked)
                {
                    _swEnable_mail.Checked = true;
                    _cfg.mail.Enable = true;
                }

                if (sw.Equals(_swAllowAutoReplyMail))
                {
                    _cfg.mail.AutoReply = e.IsChecked;
                }
                else if (sw.Equals(_swAllowSpeakAddrMail))
                {
                    _cfg.mail.AllowSpeakAddr = e.IsChecked;
                }
                else if (sw.Equals(_swAllowSpeakNameMail))
                {
                    _cfg.mail.AllowSpeakName = e.IsChecked;
                }
                else if (sw.Equals(_swAllowSpeakSubjectMail))
                {
                    _cfg.mail.AllowSpeakContent = e.IsChecked;
                }

                DbackUpMail?.Invoke();
            }

        }

        private void HandleSwEnableMail(bool isChecked)
        {
            _swEnable_mail.Enabled = false;
            _swEnable_mail.CheckedChange -= CheckedChangeHandleMail;
            _cfg.mail.Enable = isChecked;
            if (isChecked)
            {
                Model.Commom.StartMailSerive();
                _tvContentMailReply.SetTextColor(color_black);
                _customMailReply.SetTextColor(color_black);
                bool check = _swAllowAutoReplyMail.Checked ||
                              _swAllowSpeakAddrMail.Checked ||
                              _swAllowSpeakNameMail.Checked ||
                              _swAllowSpeakSubjectMail.Checked;                              
                if (!check)
                {
                    _cfg.mail.Restore();
                    RestoreStateSwMail();
                }
            }
            else 
            {
                Model.Commom.StopMailService();
                _tvContentMailReply.SetTextColor(color_grey);
                _customMailReply.SetTextColor(color_grey);
                _cfg.mail.Backup();
                DisableAllServiceMail();

            }
            _swEnable_mail.CheckedChange += CheckedChangeHandleMail;
            _swEnable_mail.Enabled = true;
        }

        private void DisableAllServiceMail()
        {
            DbackUpMail -= _cfg.mail.Backup;
            _swAllowAutoReplyMail.Checked = false;
            _swAllowSpeakAddrMail.Checked = false;
            _swAllowSpeakNameMail.Checked = false;
            _swAllowSpeakSubjectMail.Checked = false;
            DbackUpMail += _cfg.mail.Backup;
        }

        private void RestoreStateSwMail()
        {
            var mailCfg = _cfg.mail;
            _swAllowAutoReplyMail.Checked = mailCfg.AutoReply;
            _swAllowSpeakAddrMail.Checked = mailCfg.AllowSpeakAddr;
            _swAllowSpeakNameMail.Checked = mailCfg.AllowSpeakName;
            _swAllowSpeakSubjectMail.Checked = mailCfg.AllowSpeakContent;

        }

        private void HandleSwEnable(bool isChecked)
        {
            _cfg.sms.Enable = isChecked;
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
                    _cfg.sms.Restore();
                    RestoreStateSwSMS();
                }

            }
            else
            {
                Model.Commom.StopSMSService();
                _cfg.sms.Backup();
                _blockSMS.SetTextColor(color_grey);
                DisableAllServiceSMS();
            }
        }

        private void HandleSWAutoReplySMS(bool isChecked)
        {
            _cfg.sms.AllowAutoReply = isChecked;
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
            DbackUpSMS -= _cfg.sms.Backup;
            _swAllowSpeakNameSMS.Checked = false;
            _swAllowSpeakNumberSMS.Checked = false;
            _swAllowSpeakContentSMS.Checked = false;
            _swAllowAutoReplySMS.Checked = false;
            DbackUpSMS += _cfg.sms.Backup;
            
        }

        private void InitListenerSMS()
        {
            _swEnable_sms.CheckedChange += CheckedChangeHandleSMS;

            //Speak Name
            _swAllowSpeakNameSMS.CheckedChange += CheckedChangeHandleSMS;

            //Speak Number
            _swAllowSpeakNumberSMS.CheckedChange += CheckedChangeHandleSMS;

            //Speak Content
            _swAllowSpeakContentSMS.CheckedChange += CheckedChangeHandleSMS;

            //Auto Reply
            _swAllowAutoReplySMS.CheckedChange += CheckedChangeHandleSMS;

            _customSMSReply.Click += ActionCustomContentReply;
            _tvContentSMSReply.Click += ActionCustomContentReply;

            _blockSMS.Click += delegate {
                Intent intent = new Intent(this, typeof(BlockNumberActivity));
                string type = JsonConvert.SerializeObject(Model.Constants.TYPE.SMS);
                intent.PutExtra("type", type);
                StartActivity(intent);
            };
        }


        private void InitListenerMail()
        {
            _swEnable_mail.CheckedChange += CheckedChangeHandleMail;
            _swAllowAutoReplyMail.CheckedChange += CheckedChangeHandleMail;
            _swAllowSpeakAddrMail.CheckedChange += CheckedChangeHandleMail;
            _swAllowSpeakNameMail.CheckedChange += CheckedChangeHandleMail;
            _swAllowSpeakSubjectMail.CheckedChange += CheckedChangeHandleMail;

            _customMailReply.Click += delegate {
                Intent intent = new Intent(this, typeof(Custom_Reply_Messenge));
                string type = "mail";
                intent.PutExtra("type", type);
                StartActivityForResult(intent, Model.Constants.CODE_SETTING_CONTENT_REPLY);
            };
        }

        void ActionCustomContentReply(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(Custom_Reply_Messenge));
            intent.PutExtra("type", "sms");
            StartActivityForResult(intent, Model.Constants.CODE_SETTING_CONTENT_REPLY);
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetLanguage();
        }

        private void SetLanguage()
        {            
            _labelSMS.SetText(Resource.String.label_setting_message_sms);
            _swEnable_sms.SetText(Resource.String.label_setting_message_sms_enable);
            _swAllowAutoReplySMS.SetText(Resource.String.label_setting_message_sms_autoreply);
            _swAllowSpeakNameSMS.SetText(Resource.String.label_setting_message_sms_speakname);
            _swAllowSpeakNumberSMS.SetText(Resource.String.label_setting_message_sms_speaknumber);
            _swAllowSpeakContentSMS.SetText(Resource.String.label_setting_message_sms_speakcontent);
            _customSMSReply.SetText(Resource.String.label_setting_message_sms_contentsmsreply);
            _blockSMS.SetText(Resource.String.label_setting_message_sms_blocksms);

            //Mail
            _labelMail.SetText(Resource.String.label_setting_message_email);
            _swEnable_mail.SetText(Resource.String.label_setting_message_email_enable);
            _swAllowAutoReplyMail.SetText(Resource.String.label_setting_message_email_autoreply);
            _swAllowSpeakAddrMail.SetText(Resource.String.label_setting_message_email_speakaddr);
            _swAllowSpeakNameMail.SetText(Resource.String.label_setting_message_email_speakname);
            _swAllowSpeakSubjectMail.SetText(Resource.String.label_setting_message_email_speaksubject);
            _customMailReply.SetText(Resource.String.label_setting_message_sms_contentmailreply);

        }

        protected override void OnStop()
        {
            Log.Info(TAG,"OnStop");
            if (_swEnable_sms.Checked) _cfg.sms.Backup();
            _cfg.SaveSMSConfig();
            _cfg.SaveMailConfig();
            base.OnStop();

        }
       

        protected override void AttachBaseContext(Context @base)
        {
            Context c = Model.LocaleHelper.onAttach(@base);
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(c));
        }

       
               
    }
}
