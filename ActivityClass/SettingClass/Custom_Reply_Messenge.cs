
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
    [Activity(Label = "Custom_Reply_SMS")]
    public class Custom_Reply_Messenge : Activity
    {
        string[] items;
        EditText editText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Custom_Reply_SMS);

            Task configWork = new Task(() =>
            {
                items = new string[] { "Vegetables", "Fruits", "Flower Buds", "Legumes", "Bulbs", "Tubers" };
                var listView = FindViewById<ListView>(Resource.Id.ListView);
                listView.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, items);
                listView.ItemClick += OnListItemClick;
                editText = FindViewById<EditText>(Resource.Id.textArea_content_sms_auto_reply);
                Button OK = FindViewById<Button>(Resource.Id.btn_ok);
                Button Cancel = FindViewById<Button>(Resource.Id.btn_cancel);

                Intent callBackIntent = new Intent(this, typeof(Setting_Messenge));

                OK.Click += delegate
                {
                    callBackIntent.PutExtra("sms_reply_ok",editText.Text);
                    SetResult(Result.Ok, callBackIntent);
                    Finish();
                };

                Cancel.Click += delegate
                {
                    SetResult(Result.Canceled);
                    Finish();
                };
            });
            configWork.Start();

            // Create your application here
        }
        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {            
            var t = items[e.Position];
            Console.WriteLine(t);
            editText.Text = t;
        }
    }
}
