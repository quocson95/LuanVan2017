using System;

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
    }
}
