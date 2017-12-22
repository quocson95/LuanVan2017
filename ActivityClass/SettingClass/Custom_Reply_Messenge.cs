
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "Custom_Reply_SMS", Theme = "@style/MyTheme.Mrkeys")]
    public class Custom_Reply_Messenge : Activity
    {
        string[] items;
        EditText editText;
        Config _cfg;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Custom_Reply_SMS);

            InitItem();

            var listView = FindViewById<ListView>(Resource.Id.ListView);
            listView.Adapter = new ArrayAdapter<String>(this, Resource.Layout.listText, items);
            listView.ItemClick += OnListItemClick;
            editText = FindViewById<EditText>(Resource.Id.textArea_content_sms_auto_reply);
            _cfg = Config.Instance();
            string type = Intent.GetStringExtra("type");
            SetDataEditText(type);

            Button OK = FindViewById<Button>(Resource.Id.btn_ok);
            Button Cancel = FindViewById<Button>(Resource.Id.btn_cancel);

            Intent callBackIntent = new Intent(this, typeof(SettingMessenge));

            OK.Click += delegate
            {
                callBackIntent.PutExtra("content_reply_ok", editText.Text);
                SetResult(Result.Ok, callBackIntent);
                Finish();
            };

            Cancel.Click += delegate
            {
                SetResult(Result.Canceled);
                Finish();
            };
           
            // Create your application here
        }

        private void InitItem()
        {
            items = new string[5]{
                this.GetString(Resource.String.item1),
                this.GetString(Resource.String.item2),
                this.GetString(Resource.String.item3),
                this.GetString(Resource.String.item4),
                this.GetString(Resource.String.item5)
            };         
        }

        private void SetDataEditText(string type)
        {
            switch(type)
            {
                case "sms":
                    editText.Text = _cfg.sms.CustomContetnReply;
                    break;
                case "mail":
                    //TODO
                    break;
                case "phone":
                    editText.Text = _cfg.phone.ContentReply;
                    break;
            }
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {            
            var t = items[e.Position];
            Console.WriteLine(t);
            editText.Text = t;
        }
    }
}
