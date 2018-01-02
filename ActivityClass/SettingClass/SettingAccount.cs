using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class SettingAccount : Activity,InterfaceDialogCallback
    {
        static readonly string TAG = typeof(SettingAccount).FullName;
        Button _btnAddAccount;
        ListView listView;
        Toast toast;
        Config _cfg;
#pragma warning disable CS0618 // Type or member is obsolete
        ProgressDialog _progressDialog;
#pragma warning restore CS0618 // Type or member is obsolete

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
            // Create your application here
            InitUI();
            SetListenerUI();
            InitDataUI();
        }

        private void InitDataUI()
        {                        
            lstViewAdapter = new ListAccountAdapter(this, _cfg.mail.AccountEmail);
            listView.Adapter = lstViewAdapter;
        }

        private void InitUI()
        {
           
            _btnAddAccount = FindViewById<Button>(Resource.Id.btn_add_account);
            listView = FindViewById<ListView>(Resource.Id.ListViewAccount);           
        }

        private void SetListenerUI()
        {   
            _btnAddAccount.Click += delegate
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                DialogAccount dialog = new DialogAccount();
                dialog.Show(transaction, "Diaglog");
            };

            listView.ItemClick += ListView_Click;

        }

        private void ListView_Click(object sender, AdapterView.ItemClickEventArgs e)
        {
            PopupMenu menu = new PopupMenu(this, e.View);
            menu.Inflate(Resource.Layout.pop_up_menu);

            menu.MenuItemClick += (s1, arg1) =>
            {
                string title = arg1.Item.TitleFormatted.ToString();
                if (title.Equals("Edit"))
                {
                    Console.WriteLine("menu Edit");
                    //FragmentTransaction transaction = FragmentManager.BeginTransaction();
                    //DialogEditPhoneDetail dialog = new DialogEditPhoneDetail(_type, _lstAdapter[(int)e.Id], (int)e.Id);
                    //dialog.Show(transaction, "Diaglog");
                }
                else
                {
                    Log.Debug(TAG, "select id {0}", e.Id);
                    var item = _cfg.mail.AccountEmail[(int)e.Id];
                    Log.Debug(TAG, item.ToString());
                    Model.Commom.DelMailAccount(item.Item1.Email);
                    _cfg.mail.AccountEmail.Remove(item);
                    lstViewAdapter.NotifyDataSetChanged();
                }

            };

            menu.DismissEvent += (s2, arg2) => {
                Console.WriteLine("menu dismissed");
            };
            menu.Show();
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

        protected override void AttachBaseContext(Context @base)
        {
            Context c = Model.LocaleHelper.onAttach(@base);
            base.AttachBaseContext(Calligraphy.CalligraphyContextWrapper.Wrap(c));
        }

        public void ICallbackDialog(string method)
        {
            ShowProgress();
            if (method.Equals("google")){
                GoogleAuthenticate();
            }
        }

        private void GoogleAuthenticate()
        {
            Google google = new Google();
            google.Completed += Google_Completed;
            google.Authenticate();
        }

        async void Google_Completed(object sender, Xamarin.Auth.AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator == null)
            {
                Log.Debug(TAG,"Google authenticator is null");
            }
            else
            {
                //get basic info of user
                User user = null;
                var request = new OAuth2Request("GET", new Uri(Model.Constants.UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    string userJson = await response.GetResponseTextAsync();
                    user = JsonConvert.DeserializeObject<User>(userJson);
                    if (_cfg.mail.AccountEmail.FirstOrDefault(item => item.Item1.Email.Equals(user.Email)) == null)
                    {
                        Tuple<User, Account> newAccount = new Tuple<User, Account>(user, e.Account);
                        _cfg.mail.AccountEmail.Add(newAccount);
                        //GmailAction mail = new GmailAction("","");
                        //mail.Login_v2(user.Email,e.Account.Properties["access_token"]);    
                        Model.Commom.AddMailAccount(user.Email,e.Account.Properties["access_token"]);
                        lstViewAdapter.NotifyDataSetChanged();
                        DisplayToast("Add Account Success");
                    }
                    else 
                    {
                        Log.Debug(TAG, "Account has exist");
                        DisplayToast("Account has already exist");
                    }
                }
                else 
                {
                    Log.Debug(TAG, "Can not get basic infomation of user, dismiss");
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

        private bool AddAccount(Intent data)
        {
            string usr = data.GetStringExtra("usr");
            string pwd = data.GetStringExtra("pwd");
            Tuple<string, string> item = new Tuple<string, string>(usr, pwd);
            if (_cfg.mail.lstAccount.IndexOf(item) == -1)
            {
                _cfg.mail.lstAccount.Add(item);
                IMailAction account = new GmailAction(usr, pwd);
                _cfg.account.LstMail.Add(account);
                lstViewAdapter.NotifyDataSetChanged();
                return true;
            }
            else return false;
        }

        private void ShowProgress()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            _progressDialog = new ProgressDialog(this)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                Indeterminate = true
            };
            _progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            _progressDialog.SetMessage("Please wait ...");
            _progressDialog.SetCancelable(false);
            _progressDialog.Show();

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
