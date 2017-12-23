using System;
using Android.Content;
using Android.Provider;

namespace FreeHand.Model
{
    public static class Commom
    {

        public static string GetNameFromPhoneNumber(string number)
        {
            Android.Net.Uri uri = Android.Net.Uri.WithAppendedPath(Android.Provider.ContactsContract.PhoneLookup.ContentFilterUri, Android.Net.Uri.Encode(number));

            String[] projection = new String[] { Android.Provider.ContactsContract.PhoneLookup.InterfaceConsts.DisplayName };

            String contactName;
            contactName = "";
            var cursor = Android.App.Application.Context.ContentResolver.Query(uri, projection, null, null, null);

            if (cursor != null)
            {
                if (cursor.MoveToFirst())
                {
                    contactName = cursor.GetString(0);
                }
                cursor.Close();
            }
            return contactName;
        }

        public static string GetNumberFromId(string id)
        {
            Android.Net.Uri uri = Android.Provider.ContactsContract.CommonDataKinds.Phone.ContentUri;

            string[] projection = new String[] { Android.Provider.ContactsContract.CommonDataKinds.Phone.Number };

            string selection = "_ID=" + id;

            string phone;
            phone = "";
            var cursor = Android.App.Application.Context.ContentResolver.Query(uri, projection, selection, null, null);

            if (cursor != null)
            {
                if (cursor.MoveToFirst())
                {
                    phone = cursor.GetString(0);
                }
                cursor.Close();
            }
            return phone;
        }


        public static string GetNumberFromIdNew(string id)
        {
            string phone ="";
            var cursor = Android.App.Application.Context.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri,null,ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId +" =?",
                                                                                                                                          new string[]{id},null);
            if (cursor != null)
            {
                if (cursor.MoveToFirst())
                {
                    phone = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));
                }
                cursor.Close();
            }
            return phone;
        }


        public static void StartMainService()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_START_MAIN_SERVICE);
            Android.App.Application.Context.StartService(intent);
        }

        public static void StopMainService()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_STOP_MAIN_SERVICE);
            Android.App.Application.Context.StartService(intent);
        }

        public static void StartMessageService()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_START_MESSAGE_SERVICE);
            Android.App.Application.Context.StartService(intent);

        }

        public static void StopMessageService()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_STOP_MESSAGE_SERVICE);
            Android.App.Application.Context.StartService(intent);

        }

        public static void StartSMSService()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_START_SMS_SERVICE);
            Android.App.Application.Context.StartService(intent);
        }

        public static void StopSMSService()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_STOP_SMS_SERVICE);
            Android.App.Application.Context.StartService(intent);
        }

        public static void StartPhoneSerive()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_START_PHONE_SERVICE);
            Android.App.Application.Context.StartService(intent);
        }

        public static void StopPhoneService()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_STOP_PHONE_SERVICE);
            Android.App.Application.Context.StartService(intent);
        }

        public static void StartSmartAlert()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_START_PHONE_SMART_ALERT);
            Android.App.Application.Context.StartService(intent);
        }

        public static void StopSmartAlert()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_STOP_PHONE_SMART_ALERT);
            Android.App.Application.Context.StartService(intent);
        }

        /* 
         * Mail Service
         */
        public static void StartMailSerive()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_START_MAIL_SERVICE);
            Android.App.Application.Context.StartService(intent);
        }


        public static void StopMailService()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainService));
            intent.SetAction(Constants.ACTION_STOP_MAIL_SERVICE);
            Android.App.Application.Context.StartService(intent);
        }
    }
}