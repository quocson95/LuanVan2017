using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "BlockNumberActivity", Theme = "@style/MyTheme.Mrkeys")]
    public class BlockNumberActivity : Activity
    {
        readonly string TAG = typeof(BlockNumberActivity).FullName;
        Switch _blockAll, _blockInList;
        Button _selectFromContact, _addNumber;
        EditText _inputNumber;
        TextView _warning, _clean;
        ListView _lstView;
        ListPhoneDetailAdapter lstAdapter;
        Config _cfg;
        Toast toast;
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
            switch (_type)
            {
                case Model.Constants.TYPE.SMS:
                    lstAdapter = new ListPhoneDetailAdapter(this, _cfg.smsConfig.BlockList);
                    _lstView.Adapter = lstAdapter;
                    break;
                case Model.Constants.TYPE.PHONE:
                    lstAdapter = new ListPhoneDetailAdapter(this, _cfg.phoneConfig.BlackList);
                    _lstView.Adapter = lstAdapter;
                    break;
                case Model.Constants.TYPE.MAIL:
                    break;
            }
        }

        void InitUI()
        {

            _inputNumber = FindViewById<EditText>(Resource.Id.input_numer);
            _warning = FindViewById<TextView>(Resource.Id.input_warning);
            _clean = FindViewById<TextView>(Resource.Id.clean);
            _selectFromContact = FindViewById<Button>(Resource.Id.select_from_contact);
            _addNumber = FindViewById<Button>(Resource.Id.add_number);
            _blockAll = FindViewById<Switch>(Resource.Id.block_all);
            _blockInList = FindViewById<Switch>(Resource.Id.block_only_blacklist);
            _lstView = FindViewById<ListView>(Resource.Id.listView);

            _warning.Visibility = ViewStates.Invisible;
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
            _addNumber.Click += _addNumber_Click;
            _clean.Click += _clean_Click;

        }

        void _addNumber_Click(object sender, EventArgs e)
        {
            if (!_warning.Visibility.Equals(ViewStates.Visible) 
                && !string.IsNullOrEmpty(_inputNumber.Text))
            {
                Tuple<string, string> item;
                string name = Model.Commom.GetNameFromPhoneNumber(_inputNumber.Text);
                item = new Tuple<string, string>(_inputNumber.Text, name);
                switch (_type)
                {
                    case Model.Constants.TYPE.SMS:
                        AddToSMSBlockList(item);
                        break;
                    case Model.Constants.TYPE.PHONE:
                        AddToPhoneBlackList(item);
                        break;
                    case Model.Constants.TYPE.MAIL:
                        break;
                }
                _inputNumber.Text = "";
            }
            else DisplayToast("Phone Number is invalid");

        }


        void _clean_Click(object sender, EventArgs e)
        {
            switch (_type)
            {
                case Model.Constants.TYPE.SMS:
                    _cfg.smsConfig.BlockList.Clear();
                    break;
                case Model.Constants.TYPE.PHONE:
                    _cfg.phoneConfig.BlackList.Clear();
                    break;
                case Model.Constants.TYPE.MAIL:
                    break;
            }

            lstAdapter.NotifyDataSetChanged();
        }

        void CheckedChangeHandle(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Switch sw = (Switch)sender;
            if (sw.Equals(_blockAll))
            {
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
                            string name = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                            if (hasPhoneNumber.Equals(1))
                            {
                                string id = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id));
                                AddItemToList(id, name);
                            }
                        }
                        cursor.Close();
                    }
                }
            }
        }

        private void AddItemToList(string id, string name)
        {
            string number = Model.Commom.GetNumberFromId(id);
            if (!string.IsNullOrEmpty(number))
            {
                Tuple<string, string> item = new Tuple<string, string>(number, name);
                switch (_type)
                {
                    case Model.Constants.TYPE.SMS:                        
                        AddToSMSBlockList(item);                     
                        break;
                    case Model.Constants.TYPE.PHONE:
                        AddToPhoneBlackList(item);
                        break;
                    case Model.Constants.TYPE.MAIL:
                        break;
                }
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


        void AddToSMSBlockList(Tuple<string,string> item)
        {
            if (_cfg.smsConfig.BlockList.IndexOf(item) == -1)
            {
                _cfg.smsConfig.BlockList.Insert(0, item);     
                lstAdapter.NotifyDataSetChanged();
                DisplayToast("Add Success");
            }
            else
                DisplayToast("Number has exist in list");
            
        }

        private void AddToPhoneBlackList(Tuple<string, string> item)
        {
            if (_cfg.phoneConfig.BlackList.IndexOf(item) == -1)
            {
                _cfg.phoneConfig.BlackList.Insert(0, item);
                lstAdapter.NotifyDataSetChanged();
                DisplayToast("Add Success");
            }
            else
                DisplayToast("Number has exist in list");
        }


        private void DisplayToast(string v)
        {
            if (toast != null) toast.Cancel();
            toast = Toast.MakeText(this, v, ToastLength.Short);
            toast.Show();
        }

        protected override void OnStop()
        {
            Log.Info(TAG, "OnStop");
            switch (_type)
            {
                case Model.Constants.TYPE.SMS:
                    _cfg.SaveSMSConfig();
                    break;
                case Model.Constants.TYPE.PHONE:
                    _cfg.SavePhoneConfig();
                    break;
                case Model.Constants.TYPE.MAIL:
                    break;
            }
            base.OnStop();
        }

    }
}