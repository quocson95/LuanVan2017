
using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.OS;

using Android.Provider;
using System.Collections;
using System.Collections.Generic;

namespace FreeHand.Model
{
    public class MessengeQueue  : Activity
    {
        private static MessengeQueue instance;
        private Queue _queueMessenge;
        private List<string> _phoneBook = new List<string>();
        public static MessengeQueue GetInstance(){
            if (instance == null) instance = new MessengeQueue();
            return instance;
        }
        private MessengeQueue()
        {
            _queueMessenge = new Queue();
        }

        public void EnqueueMessengeQueue(IMessengeData mess){
            _queueMessenge.Enqueue(mess);
        }
        public IMessengeData DequeueMessengeQueu(){
            return (IMessengeData)_queueMessenge.Dequeue();
        }

        public bool Empty(){
            return (_queueMessenge.Count == 0);
        }

        public bool Clear(){
            _queueMessenge.Clear();
            return Empty();
        }

        public void RetrievePhoneBook(Activity ac)
        {            
            var uri = ContactsContract.Contacts.ContentUri;
            string[] projection = {
                                    ContactsContract.Contacts.InterfaceConsts.Id, 
                                    ContactsContract.Contacts.InterfaceConsts.DisplayName 
                                  };
            var cursor = ac.ManagedQuery(uri, projection, null, null, null);
            if (cursor.MoveToFirst())
            {
                do
                {
                    _phoneBook.Add(cursor.GetString(
                            cursor.GetColumnIndex(projection[1])));
                } while (cursor.MoveToNext());
            }
        }
    }
}
