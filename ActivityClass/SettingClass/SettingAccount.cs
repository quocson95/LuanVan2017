using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using FreeHand.Message.Mail;
using Newtonsoft.Json;
using Xamarin.Auth;

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
        AccountStore store;
        ProgressDialog _progressDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Calligraphy.CalligraphyConfig.InitDefault(new Calligraphy.CalligraphyConfig.Builder()
                                          .SetDefaultFontPath("Fonts/HELR45W.ttf")
                                          .SetFontAttrId(Resource.Attribute.fontPath)
                                          .Build());
            
            SetContentView(Resource.Layout.Setting_Email_Layout);
            store = AccountStore.Create(this);
            _cfg = Config.Instance();
            InitUI();
            // Create your application here
        }

        private void InitUI()
        {
            _btnAddAccount = FindViewById<Button>(Resource.Id.btn_add_account);
            listView = FindViewById<ListView>(Resource.Id.ListViewAccount);           
            lstViewAdapter = new ListAccountAdapter(this, _cfg.mail.lstAccount);
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
            Log.Info(TAG,"OnResume");
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

        public void IMethodLogin(string method)
        { 
            ShowProgress();
            if (method.Equals("google")){
                loginGoogle();
            }
        }

        private void loginGoogle()
        {
            Message.Mail.Oauth2.Google google = new Message.Mail.Oauth2.Google(this);
            google.Completed += Google_Completed;
            google.authenticate();
           
        }

        async void Google_Completed(object sender, Xamarin.Auth.AuthenticatorCompletedEventArgs e)
        {
            Log.Info(TAG,"Auth_Completed");
            //Get infomation
            User user = null;
            var request = new OAuth2Request("GET", new Uri(Model.Constants.UserInfoUrl), null, e.Account);
            var response = await request.GetResponseAsync();
            if (response != null)
            {
                string userJson = await response.GetResponseTextAsync();
                user = JsonConvert.DeserializeObject<User>(userJson);
                var x = _cfg.mail.lstAccount.FirstOrDefault(item => item.Equals(user.Email));
                if (_cfg.mail.lstAccount.FirstOrDefault(item => item.Item1.Email.Equals(user.Email)) != null)
                {
                    Log.Debug(TAG, "Account has exist");
                    DisplayToast("Account has already exist");
                }
                else
                {                    
                    IMailAction mail = new GmailAction(user.Email, e.Account);
                    mail.Login();
                    Tuple<User, object> account = new Tuple<User, object>(user, mail);
                    _cfg.mail.lstAccount.Add(account);
                    lstViewAdapter.NotifyDataSetChanged();
                    DisplayToast("Add Account Success");
                }
            }                     
            _progressDialog.Dismiss();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode.Equals(Model.Constants.CODE_ADD_ACCOUNT_GOOGLE))
            {
                if (resultCode.Equals(Result.Ok))
                {
                    if (AddAccount(data))
                        DisplayToast("Add account success");
                    else 
                        DisplayToast("Account already exist");
                }
                else
                    DisplayToast("Has occur error");
            }
        }

        private void ShowProgress()
        {
            _progressDialog = new ProgressDialog(this)
            {
                Indeterminate = true
            };
            _progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            _progressDialog.SetMessage("Please wait ...");
            _progressDialog.SetCancelable(false);
            _progressDialog.Show();

        }

        private bool AddAccount(Intent data)
        {
            //string usr = data.GetStringExtra("usr");
            //string pwd = data.GetStringExtra("pwd");
            //Tuple<string, string> item = new Tuple<string, string>(usr, pwd);
            //if (_cfg.mail.lstAccount.IndexOf(item) == -1)
            //{
            //    _cfg.mail.lstAccount.Add(item);
            //    IMailAction account = new GmailAction(usr, pwd);
            //    _cfg.account.LstMail.Add(account);
            //    lstViewAdapter.NotifyDataSetChanged();
            //    return true;
            //}
            //else return false;
            return false;
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
            _cfg.SaveMailConfig();
        }
    }
}
