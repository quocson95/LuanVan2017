using System;
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
            contactName = "UNKNOW";
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
            Android.Net.Uri uri = Android.Net.Uri.WithAppendedPath(Android.Provider.ContactsContract.PhoneLookup.ContentFilterUri, id);

            String[] projection = new String[] { Android.Provider.ContactsContract.PhoneLookup.InterfaceConsts.Number };

            String phone;
            phone = "";
            var cursor = Android.App.Application.Context.ContentResolver.Query(uri, projection, null, null, null);

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
    }
}
