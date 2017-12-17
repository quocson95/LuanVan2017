using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using static Android.Provider.Contacts;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "BlockNumberActivity", Theme = "@style/MyTheme.Mrkeys")]
    public class BlockNumberActivity : Activity
    {      

        readonly string TAG = typeof(BlockNumberActivity).FullName;
        Switch _blockAll, _blockInList;
        Button _selectFromContact, _addNumber;
        EditText _inputNumber;
        TextView _warning;
        Config _cfg;
        private Model.Constants.TYPE _type;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.BlockNumberLayout);
            _cfg = Config.Instance();
            string type = Intent.GetStringExtra("type");
            _type = JsonConvert.DeserializeObject<Model.Constants.TYPE>(type);

            InitUI();
            SetListenerUI();
            InitData();
        }

        private void InitData()
        {
            _blockAll.Checked = _cfg.phoneConfig.BlockAll;
            _blockInList.Checked = _cfg.phoneConfig.BlockInList;
            _inputNumber.TextChanged += InputTextChange;
        }

        void InitUI()
        {

            _inputNumber = FindViewById<EditText>(Resource.Id.input_numer);
            _warning = FindViewById<TextView>(Resource.Id.input_warning);
            _selectFromContact = FindViewById<Button>(Resource.Id.select_from_contact);
            _addNumber = FindViewById<Button>(Resource.Id.add_number);
            _blockAll = FindViewById<Switch>(Resource.Id.block_all);
            _blockInList = FindViewById<Switch>(Resource.Id.block_only_blacklist);

            switch (_type)
            {
                case Model.Constants.TYPE.SMS:
                    break;
                case Model.Constants.TYPE.PHONE:
                    _blockAll.Visibility = ViewStates.Visible;
                    _blockInList.Visibility = ViewStates.Visible;
                    break;
                case Model.Constants.TYPE.MAIL:
                    
            default:
                    break;
            }           
        }

        void SetListenerUI()
        {
            _blockAll.CheckedChange += CheckedChangeHandle;
            _blockInList.CheckedChange += CheckedChangeHandle;

            _selectFromContact.Click += SelectFromContact;
            _addNumber.Click += AddNumber;

        }

        void CheckedChangeHandle(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Switch sw = (Switch)sender;
            if (sw.Equals(_blockAll)){
                _cfg.phoneConfig.BlockAll = e.IsChecked;
                _blockInList.CheckedChange -= CheckedChangeHandle;
                _blockInList.Checked = false;
                _blockInList.CheckedChange += CheckedChangeHandle;
            }
            else if (sw.Equals(_blockInList))
            {
                _cfg.phoneConfig.BlockInList = e.IsChecked;
                _blockAll.CheckedChange -= CheckedChangeHandle;
                _blockAll.Checked = false;
                _blockAll.CheckedChange += CheckedChangeHandle;
            }

        }
        void SelectFromContact(object sender, EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionPick);
            intent.SetType(ContactsContract.Contacts.ContentType);
            StartActivityForResult(intent, Model.Constants.CODE_PICK_CONTACT);
        }



        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode.Equals(Model.Constants.CODE_PICK_CONTACT))
            {                
                if (resultCode == Result.Ok)
                {
                    var cursor = Android.App.Application.Context.ContentResolver.Query(data.Data, null, null, null, null);

                    if (cursor != null)
                    {
                        if (cursor.MoveToFirst())
                        {
                            int hasPhoneNumber = cursor.GetInt(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber));
                            if (hasPhoneNumber.Equals(1))
                            {
                                string id = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id));
                                AddNumberToList(id);
                            }
                        }
                        cursor.Close();                       
                    }
                }
            }
        }

        private void AddNumberToList(string id)
        {
            string a = Model.Commom.GetNumberFromId(id);
        
        }

        void AddNumber(object sender, EventArgs e)
        {
            string number;
            number = _inputNumber.Text;
            if (ValidatePhone(number))
            {
                _cfg.phoneConfig.BlackList.Add(number);
            }
        }

        void InputTextChange(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (ValidatePhone(e.Text.ToString()))
            {
                _warning.Visibility = ViewStates.Invisible;
            }
            else 
            {
                if (!string.IsNullOrEmpty(e.Text.ToString()))
                    _warning.Visibility = ViewStates.Visible;
                else _warning.Visibility = ViewStates.Invisible;
            }
        }

        bool ValidatePhone(string number)
        {            
            return Patterns.Phone.Matcher(number).Matches();
        }

        protected override void  OnStop()
        {
            Log.Info(TAG, "OnStop");
            _cfg.phoneConfig.AutoReply = _blockAll.Checked || _blockInList.Checked;
            _cfg.phoneConfig.PrevAutoReply = _cfg.phoneConfig.AutoReply;
            base.OnStop();
        }

    }
}
