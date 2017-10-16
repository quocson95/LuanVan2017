using System;
using Android.Content;
using Android.App;
using Android.Provider;
using Android.Database;

using System.Collections;
using System.Collections.Generic;

namespace FreeHand.Model
{
    public class MessengeQueue  : Activity
    {
        private static MessengeQueue instance;
        private Queue _queueMessenge;
        private List<string> _phoneBook = new List<string>();
        private Activity _activity;
        public static MessengeQueue GetInstance(Activity act = null){
            if (instance == null) instance = new MessengeQueue(act);
            return instance;
        }
        private MessengeQueue(Activity act)
        {
            _queueMessenge = new Queue();
            this._activity = act;
        }

        public void SetActivity(Activity act)
        {
            this._activity = act;
        }

        public void EnqueueMessengeQueue(IMessengeData mess){
            _queueMessenge.Enqueue(mess);
        }
        public IMessengeData DequeueMessengeQueue(){
            return (IMessengeData)_queueMessenge.Dequeue();
        }

        public bool Empty(){
            return (_queueMessenge.Count == 0);
        }

        public bool Clear(){
            _queueMessenge.Clear();
            return Empty();
        }
		public string GetNameFromPhoneNumber(string number)
		{
			String selection = ContactsContract.CommonDataKinds.Contactables.InterfaceConsts.HasPhoneNumber + " = " + 1;
			var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
			string[] projection = {
				ContactsContract.Contacts.InterfaceConsts.Id,
				ContactsContract.Contacts.InterfaceConsts.DisplayName ,
				ContactsContract.CommonDataKinds.Phone.Number
                        //ContactsContract.CommonDataKinds.Phone
                        
            };
			//var cursor = act.Con(uri, projection, null, null, null);
			//var cursor = ApplicationContext.ContentResolver.Query(uri, projection, null, null, null);
            using (var cursor = _activity.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null))
				//var loader = new CursorLoader(_activity, uri, projection, null, null, null);
				//var cursor = (ICursor)loader.LoadInBackground();
				if (cursor.MoveToFirst())
				{
					do
					{
						//var Id = cursor.GetLong(cursor.GetColumnIndex(projection[0]));
						var DisplayName = cursor.GetString(cursor.GetColumnIndex(projection[1]));
						var Number = cursor.GetString(cursor.GetColumnIndex(projection[2]));
						if (number == Number) return DisplayName;
					} while (cursor.MoveToNext());
				}
			return null;
		}
    }
}
