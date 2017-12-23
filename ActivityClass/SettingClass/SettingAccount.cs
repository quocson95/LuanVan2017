using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using FreeHand.Message.Mail;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "ManageInternetAccountActivity", Theme = "@style/MyTheme.Mrkeys")]
    public class SettingAccount : Activity,LoginMethod
    {
        static readonly string TAG = typeof(SettingAccount).FullName;
        Button _btnAddAccount;
        ListView listView;
        Toast toast;
        Config _cfg;
        ListAccountAdapter lstViewAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Calligraphy.CalligraphyConfig.InitDefault(new Calligraphy.CalligraphyConfig.Builder()
                                          .SetDefaultFontPath("Fonts/HELR45W.ttf")
                                          .SetFontAttrId(Resource.Attribute.fontPath)
                                          .Build());
            
            SetContentView(Resource.Layout.Setting_Email_Layout);

            _cfg = Config.Instance();
            InitUI();
            // Create your application here
        }

        private void InitUI()
        {
            _btnAddAccount = FindViewById<Button>(Resource.Id.btn_add_account);
            listView = FindViewById<ListView>(Resource.Id.ListViewAccount);           
            lstViewAdapter = new ListAccountAdapter(this, _cfg.account.LstMail);
            listView.Adapter = lstViewAdapter;
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
                StartActivityForResult(intent,Model.Constants.CODE_ADD_ACCOUNT_GOOGLE);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode.Equals(Model.Constants.CODE_ADD_ACCOUNT_GOOGLE))
            {
                if (resultCode.Equals(Result.Ok))
                {
                    string usr = data.GetStringExtra("usr");
                    string pwd = data.GetStringExtra("pwd");
                    Tuple<string, string> item = new Tuple<string, string>(usr, pwd);
                    _cfg.mail.lstAccount.Add(item);
                    IMailAction account = new GmailAction(usr, pwd);
                    _cfg.account.LstMail.Add(account);
                    lstViewAdapter.NotifyDataSetChanged();
                    DisplayToast("Add account success");
                }
                else
                    DisplayToast("Has occur error");
            }
        }

        private void DisplayToast(string v)
        {
            if (toast != null) toast.Cancel();
            toast = Toast.MakeText(this, v, ToastLength.Short);
            toast.Show();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Log.Info(TAG,"OnDestroy");
        }
    }
}
