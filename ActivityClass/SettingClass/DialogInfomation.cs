using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace FreeHand.ActivityClass.SettingClass
{
    public class DialogInfomation : DialogFragment
    {
        int _layout;
        DIALOG_TYPE _type;
        public DialogInfomation(int layout,DIALOG_TYPE type){
            this._layout = layout;
            _type = type;
        }
       public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(_layout, container, false);
            switch (_type)
            {
                case DIALOG_TYPE.About:
                    SetLanguageAbout(view);
                    break;
                case DIALOG_TYPE.Term:
                    SetLanguageTerm(view);
                    break;
                case DIALOG_TYPE.Privacy:
                    SetLanguagePrivacy(view);
                    break;
                case DIALOG_TYPE.Contact:
                    SetLanguageContact(view);
                    break;
            }
            return view;
        }

        private void SetLanguageContact(View v)
        {
            TextView _title = v.FindViewById<TextView>(Resource.Id.title);

            _title.SetText(Resource.String.label_setting_contact_label);
        }

        private void SetLanguagePrivacy(View v)
        {            
            TextView _title = v.FindViewById<TextView>(Resource.Id.title);
            TextView _privacy = v.FindViewById<TextView>(Resource.Id.cont_privacy);

            _title.SetText(Resource.String.label_setting_privacypolicy);
            _privacy.SetText(Resource.String.label_setting_privacy_content);
        }

        private void SetLanguageTerm(View v)
        {
            TextView _term = v.FindViewById<TextView>(Resource.Id.cont_terms);
            TextView _title = v.FindViewById<TextView>(Resource.Id.title);

            _title.SetText(Resource.String.label_setting_termsandconditions);
            _term.SetText(Resource.String.label_setting_termandconditions_content);
        }

        private void SetLanguageAbout(View v)
        {
            TextView _version = v.FindViewById<TextView>(Resource.Id.version);
            TextView _copyright = v.FindViewById<TextView>(Resource.Id.copyright);

            _version.SetText(Resource.String.label_setting_about_version);
            _copyright.SetText(Resource.String.label_setting_about_copyright);
        }

        public override void OnActivityCreated(Bundle saveIns)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.SetCanceledOnTouchOutside(true);
            Dialog.SetCancelable(true);
            base.OnActivityCreated(saveIns);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_aim;
        }
    }

    public enum DIALOG_TYPE{
        About,
        Term,
        Privacy,
        Contact
    }
}
