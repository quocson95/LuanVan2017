using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace FreeHand.ActivityClass.SettingClass
{
    public class DialogEditPhoneDetail : DialogFragment
    {
        Model.Constants.TYPE _type;
        Tuple<string, string> _item,_result;
        TextView _etName;
        bool _isClickOK;
        int _id;
        public DialogEditPhoneDetail(Model.Constants.TYPE type, Tuple<string,string> item,int id)
        {
            _type = type;
            _item = item;
            _isClickOK = false;
            _id = id;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.dialog_phone_detail, container, false);


            SetViewFollowType(view);

            return view;
        }

        private void SetViewFollowType(View view)
        {
            TextView _tvTitle = view.FindViewById<TextView>(Resource.Id.title);
            TextView _etPhone = view.FindViewById<TextView>(Resource.Id.phone);
            _etName = view.FindViewById<TextView>(Resource.Id.name);
            TextView _cacel = view.FindViewById<TextView>(Resource.Id.cancel);
            Button _ok = view.FindViewById<Button>(Resource.Id.ok);

            _cacel.Click += delegate {
                _isClickOK = false;
                Dismiss();
            };

            _ok.Click += delegate {
                Tuple<string, string> item = new Tuple<string, string>(_etPhone.Text, _etName.Text);
                EditDialogListener activity = (EditDialogListener)Activity;
                activity.updateResult(item,_id);
                Dismiss();
                
            };

            _tvTitle.Text = "Edit Phone Detail";
            _etPhone.Text = _item.Item1;
            _etName.Text = _item.Item2;

            _etPhone.TextChanged += _etPhone_TextChanged;

            switch (_type)
            {
                case Model.Constants.TYPE.SMS:
                    
                    break;
                case Model.Constants.TYPE.PHONE:
                    break;
                case Model.Constants.TYPE.MAIL:
                    break;
            }
        }

        void _etPhone_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var phone = String.Concat(e.Text);
            bool isValid = Android.Util.Patterns.Phone.Matcher(phone).Matches();
            if (isValid ){                
                _etName.Text = Model.Commom.GetNameFromPhoneNumber(phone);
            }
        }

        public bool IsClickOK()
        {
            return _isClickOK;
        }

        public Tuple<string,string> Result()
        {
            return _result;
        }

        public override void OnActivityCreated(Bundle saveIns)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.SetCanceledOnTouchOutside(true);
            Dialog.SetCancelable(true);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_aim;                      

            base.OnActivityCreated(saveIns);

        }
    }

    public interface EditDialogListener
    {
        void updateResult(Tuple<string,string> item,int id);
    }
}
