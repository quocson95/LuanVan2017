
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Calligraphy;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "SettingActivity", Theme = "@style/MyTheme.Mrkeys")]
    public class Setting : Activity
    {
        private static readonly string TAG = typeof(Setting).FullName;
        TextView _tvSpeech, _tvMessenge, _tvPhone, _tvAccount, _tvAbout, _tvTerm, _tvPolicy, _tvContact, _tvOption;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Setting);

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
            .SetDefaultFontPath("Fonts/HELR45W.ttf")
            .SetFontAttrId(Resource.Attribute.fontPath)
            .Build());

            InitUI();
            SetListenerUI();
        }

        private void InitUI()
        {
            _tvSpeech = FindViewById<TextView>(Resource.Id.speech_setting);
            _tvMessenge = FindViewById<TextView>(Resource.Id.messenge_setting);
            _tvPhone = FindViewById<TextView>(Resource.Id.phone_setting);
            _tvAccount = FindViewById<TextView>(Resource.Id.account_setting);
            _tvAbout = FindViewById<TextView>(Resource.Id.about_setting);
            _tvTerm = FindViewById<TextView>(Resource.Id.term_setting);
            _tvPolicy = FindViewById<TextView>(Resource.Id.policy_setting);
            _tvContact = FindViewById<TextView>(Resource.Id.contact_setting);
            _tvOption = FindViewById<TextView>(Resource.Id.option_setting);
                       
        }

        private void SetListenerUI()
        {
            _tvSpeech.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SettingSpeech));
                StartActivity(intent);
            };

            _tvMessenge.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SettingMessenge));
                StartActivity(intent);
            };

            _tvPhone.Click += delegate 
            {
                Intent intent = new Intent(this, typeof(SettingPhone));
                StartActivity(intent);
            };

            _tvAccount.Click += delegate 
            {
                Intent intent = new Intent(this, typeof(SettingAccount));
                StartActivity(intent);
            };

            _tvAbout.Click += ActionInfo;
            _tvTerm.Click += ActionInfo;
            _tvPolicy.Click += ActionInfo;
            _tvContact.Click += ActionInfo;
            _tvOption.Click += _tvOption_Click;
        }

        void ActionInfo(object sender, EventArgs e)
        {
            TextView v = (TextView)sender;
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            int layout = 0;
            DialogInfomation dialog = null;
            if (v.Equals(_tvAbout)){
                layout = Resource.Layout.about_layout;
                dialog = new DialogInfomation(layout,DIALOG_TYPE.About);
            }
            else if (v.Equals(_tvTerm))
            {
                layout = Resource.Layout.terms_layout;
                dialog = new DialogInfomation(layout, DIALOG_TYPE.Term);
            }
            else if (v.Equals(_tvPolicy))
            {
                layout = Resource.Layout.privacy_layout;
                dialog = new DialogInfomation(layout, DIALOG_TYPE.Privacy);
            }
            else if (v.Equals(_tvContact))
            {
                layout = Resource.Layout.contact_layout;
                dialog = new DialogInfomation(layout, DIALOG_TYPE.Contact);
            }

            if (dialog != null)
                dialog.Show(transaction, "Diaglog");
        }

        void _tvOption_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SettingOption));
            StartActivity(intent);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Info(TAG,"OnResume");
            //TextView _tvSpeech, _tvMessenge, _tvPhone, _tvAccount, _tvAbout, _tvTerm, _tvPolicy, _tvContact, _tvOption;
            _tvSpeech.SetText(Resource.String.label_setting_speech);
            _tvMessenge.SetText(Resource.String.label_setting_message);
            _tvPhone.SetText(Resource.String.label_setting_phone);
            _tvAccount.SetText(Resource.String.label_setting_account);
            _tvAbout.SetText(Resource.String.label_setting_about);
            _tvTerm.SetText(Resource.String.label_setting_termsandconditions);
            _tvPolicy.SetText(Resource.String.label_setting_privacypolicy);
            _tvContact.SetText(Resource.String.label_setting_contact);
            _tvOption.SetText(Resource.String.label_setting_option);

        }

        protected override void AttachBaseContext(Context @base)
        {
            Context c = Model.LocaleHelper.onAttach(@base);
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(c));
        }
    }

}
