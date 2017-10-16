
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
using Calligraphy;

namespace FreeHand
{
    [Activity(Label = "TestActivity",Theme = "@android:style/Theme.NoTitleBar")]
    public class TestActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main_layout);
			CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
			.SetDefaultFontPath("Fonts/HELR45W.ttf")
			.SetFontAttrId(Resource.Attribute.fontPath)
			.Build());
   //         Spinner spinner = (Spinner)FindViewById(Resource.Id.spinner1);
			//// Create your application here
			//string[] ITEMS = { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6" };
			//var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, ITEMS);
			//spinner.Adapter = adapter;   

        }
		protected override void AttachBaseContext(Android.Content.Context @base)
		{
			base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
		}
    }

}
