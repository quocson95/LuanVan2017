using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "ManageInternetAccountActivity", Theme = "@style/MyTheme.Mrkeys")]
    public class SettingAccount : Activity,LoginMethod
    {
        static readonly string TAG = typeof(SettingAccount).FullName;
        Button _btnAddAccount;
        ListView listView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Calligraphy.CalligraphyConfig.InitDefault(new Calligraphy.CalligraphyConfig.Builder()
                                          .SetDefaultFontPath("Fonts/HELR45W.ttf")
                                          .SetFontAttrId(Resource.Attribute.fontPath)
                                          .Build());
            
            SetContentView(Resource.Layout.Setting_Email_Layout);
            InitUI();
            // Create your application here
        }

        private void InitUI()
        {
            _btnAddAccount = FindViewById<Button>(Resource.Id.btn_add_account);
            listView = FindViewById<ListView>(Resource.Id.ListViewAccount);
            var items = new List<string>();
            items.Add("first");
            listView.Adapter = new Messenge.Mail.ListAccountAdapter(this, items);
            SetListenerUI();
        }

        private void SetListenerUI()
        {
            _btnAddAccount.Click += delegate
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                DialogAccount dialog = new DialogAccount();
                dialog.Show(transaction, "Diaglog");
            };

        }

        protected override void OnResume()
        {
            base.OnResume();
            Android.Util.Log.Info(TAG,"OnResume");
            SetLanguage();
        }

        private void SetLanguage()
        {
            _btnAddAccount.SetText(Resource.String.label_setting_account_addaccount);
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            Android.Content.Context c = Model.LocaleHelper.onAttach(@base);
            base.AttachBaseContext(Calligraphy.CalligraphyContextWrapper.Wrap(c));
        }

        public void updateResult(string method)
        {
            if (method.Equals("google")){
                Intent intent = new Intent(this, typeof(AddGmailAccountActivity));
                StartActivity(intent);
            }
        }
    }
}
