
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FreeHand
{
    [Activity(Label = "MailSetting")]
    public class MailSetting : Activity
    {
        Button btn_login, btn_sync_mail,btn_print;
        IMailAction gmail;
        MailManager mailMng;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Mail_Setting_Layout);
            InitUI();
            gmail = new GmailAction("dangquocson1995@gmail.com", "kakula@7#9");
            mailMng = MailManager.Instance();
            SetListenerUI();
            // Create your application here
        }
        private void InitUI()
        {
            btn_login = (Button)FindViewById(Resource.Id.btn_login);
            btn_sync_mail = (Button)FindViewById(Resource.Id.btn_sync_mail);
            btn_print = (Button)FindViewById(Resource.Id.btn_print);
        }

        private void SetListenerUI()
        {
            btn_login.Click += delegate {
                Task task = new Task(delegate ()
                {
                    gmail.Login();
                    if (gmail.isLogin()) mailMng.AddMailAccount(gmail);
                });
                task.Start();
                task.Wait();

            };
            btn_sync_mail.Click += async delegate {
                await mailMng.StartAutoCheckMail();
            };
            btn_print.Click += delegate {
                
            };
        }


    }
}