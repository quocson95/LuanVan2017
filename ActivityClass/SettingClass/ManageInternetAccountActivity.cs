
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

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "ManageInternetAccountActivity")]
    public class ManageInternetAccountActivity : Activity
    {
        Button _btnAddAccount;
        ListView listView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
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
            items.Add("first");
            items.Add("first");
            items.Add("first");
            items.Add("first");
            items.Add("first");items.Add("first");items.Add("first");items.Add("first");items.Add("first");items.Add("first");items.Add("first");
            listView.Adapter = new Messenge.Mail.ListAccountAdapter(this, items);
            SetListenerUI();
        }

        private void SetListenerUI()
        {
            _btnAddAccount.Click += delegate {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                DialogAccount dialog = new DialogAccount();
                dialog.Show(transaction, "Diaglog");
            };
        }
    }
}
