using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using FreeHand.Message.Mail;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "AddGmailAccountActivity",Theme = "@style/MyTheme.Mrkeys")]
    public class AddGmailAccountActivity : Activity
    {
        static readonly string TAG = typeof(AddGmailAccountActivity).FullName;

        Button _login;
        EditText _user, _pwd;
        TextView _warning;
        Config _cfg;
        ProgressDialog _progressDialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.gmail_login_layout);

            // Create your application here
            _login = FindViewById<Button>(Resource.Id.login);
            _user = FindViewById<EditText>(Resource.Id.user);
            _pwd = FindViewById<EditText>(Resource.Id.pwd);
            _warning = FindViewById<TextView>(Resource.Id.warning);

            _login.Click += _login_Click;
            _user.TextChanged += _user_TextChanged;

        }

        void _login_Click(object sender, System.EventArgs e)
        {
            string usr = _user.Text;
            if (string.IsNullOrEmpty(usr) || isValidEmail(usr)){
                GmailAction gmail = new GmailAction(_user.Text, _pwd.Text);
                ShowProgress();
                Task.Run(() =>
                {
                    gmail.Login();
                    _progressDialog.Cancel();
                    if (gmail.isLogin())
                    {         
                        gmail.SetActive(true);
                        Intent myIntent = new Intent(this, typeof(SettingAccount));                      
                        myIntent.PutExtra("usr", gmail.GetNameLogin());
                        myIntent.PutExtra("pwd", gmail.GetPwd());
                        SetResult(Result.Ok, myIntent);
                        Finish();
                    }
                    else
                    {

                    }
                });               
            }
            else 
            {
                /* 
                 * Todo
                 * Add popup display email invalid here
                 */
                Log.Info(TAG, "Email is invalid");

            }
        }

        private void ShowProgress()
        {
            _progressDialog = new ProgressDialog(this);
            _progressDialog.Indeterminate = true;
            _progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            _progressDialog.SetMessage("Login...");
            _progressDialog.SetCancelable(false);
            _progressDialog.Show();
               
        }

        void _user_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            string usr = e.Text.ToString();
            if (string.IsNullOrEmpty(usr) || isValidEmail(usr))
                _warning.Visibility = Android.Views.ViewStates.Invisible;
            else
                _warning.Visibility = Android.Views.ViewStates.Visible;
        }

        bool isValidEmail(string email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }

    }
}
