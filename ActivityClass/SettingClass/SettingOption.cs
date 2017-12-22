using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "SettingOptionActivity", Theme = "@style/MyTheme.Mrkeys")]
    public class SettingOption : Activity
    {
        static readonly string TAG = typeof(SettingOption).FullName;

        Spinner _spnSelectLang;
        TextView _labelSelLang;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Calligraphy.CalligraphyConfig.InitDefault(new Calligraphy.CalligraphyConfig.Builder()
                                        .SetDefaultFontPath("Fonts/HELR45W.ttf")
                                        .SetFontAttrId(Resource.Attribute.fontPath)
                                        .Build());

            SetContentView(Resource.Layout.setting_option_layout);
            // Create your application here

            _spnSelectLang = FindViewById<Spinner>(Resource.Id.select_lang);
            _labelSelLang = FindViewById<TextView>(Resource.Id.label_select_lang);

            IList<string> lstLang = new List<string>(2);
            lstLang.Add("vi");
            lstLang.Add("en");
            int index = lstLang.IndexOf(Model.LocaleHelper.getLanguage(this));
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, lstLang);
            _spnSelectLang.Adapter = adapter;
            _spnSelectLang.SetSelection(index);
            _spnSelectLang.ItemSelected += (sender, e) => 
            {
                Model.LocaleHelper.setLocale(this,lstLang[(int)e.Id]);
                Log.Info(TAG, "lang select " + lstLang[(int)e.Id]);
                SetLanguage();
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
            _labelSelLang.SetText(Resource.String.label_setting_option_lang);
        }

        protected override void AttachBaseContext(Context @base)
        {
            Context c = Model.LocaleHelper.onAttach(@base);
            base.AttachBaseContext(Calligraphy.CalligraphyContextWrapper.Wrap(c));
        }
    }
}
