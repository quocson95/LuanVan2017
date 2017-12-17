
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
    public class SettingActivity : Activity
    {
        private static readonly string TAG = "TestActivity";
        TextView _tvSpeech, _tvMessenge, _tvPhone, _tvAccount, _tvAbout, _tvTerm, _tvPolicy, _tvContact;
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
            //Button btn = (Button)FindViewById(Resource.Id.item_1);
            //btn.Click += delegate {
            //    Log.Info(TAG, "press");
            //    Intent settingSpeech = new Intent(this, typeof(Setting_Speech));
            //    StartActivity(settingSpeech);
            //};

            //         Spinner spinner = (Spinner)FindViewById(Resource.Id.spinner1);
            //// Create your application here
            //string[] ITEMS = { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6" };
            //var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, ITEMS);
            //spinner.Adapter = adapter;   

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
                       
        }

        private void SetListenerUI()
        {
            _tvSpeech.Click += delegate
            {
                Intent intent = new Intent(this, typeof(Setting_Speech));
                StartActivity(intent);
            };

            _tvMessenge.Click += delegate
            {
                Intent intent = new Intent(this, typeof(Setting_Messenge));
                StartActivity(intent);
            };

            _tvPhone.Click += delegate 
            {
                Intent intent = new Intent(this, typeof(PhoneSettingActivity));
                StartActivity(intent);
            };

            _tvAccount.Click += delegate 
            {
                Intent intent = new Intent(this, typeof(ManageInternetAccountActivity));
                StartActivity(intent);
            };

            _tvAbout.Click += ActionInfo;
            _tvTerm.Click += ActionInfo;
            _tvPolicy.Click += ActionInfo;
            _tvContact.Click += ActionInfo;
        }

        void ActionInfo(object sender, EventArgs e)
        {
            TextView v = (TextView)sender;
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            int layout = 0;
            if (v.Equals(_tvAbout)){
                layout = Resource.Layout.about_layout;
            }
            else if (v.Equals(_tvTerm))
            {
                layout = Resource.Layout.terms_layout;
            }
            else if (v.Equals(_tvPolicy))
            {
                layout = Resource.Layout.privacy_layout;
            }
            else if (v.Equals(_tvContact))
            {
                layout = Resource.Layout.contact_layout;
            }


            DialogInfomation dialog = new DialogInfomation(layout);
            dialog.Show(transaction, "Diaglog");
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }

}
