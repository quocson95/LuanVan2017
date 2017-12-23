
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

namespace FreeHand.Message.Mail
{
    [Activity(Label = "MailActivity")]
    public class MailActivity : Activity
    {
        Button login;
        IMailAction gmail = new GmailAction("","");
        MailSerivce mailService;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Mail_Setting_Layout);

            //login = FindViewById<Button>(Resource.Id.btn_login);

            //login.Click += Login_Click;
            //mailService = new MailSerivce();
        }

        void Login_Click(object sender, EventArgs e)
        {
            //mailService.AddAccount(gmail);
            //mailService.SyncMail();
        }
    }
}
