using System;
using Android.Content;
using Android.App;
using Android.Provider;
using Android.Database;

using System.Collections;
using System.Collections.Generic;
using Java.Util;

namespace FreeHand.Model
{
    public class MessengeQueue  : Activity
    {
        private static MessengeQueue instance;
        private LinkedList<IMessengeData> _queueMessenge;
        private Activity _activity;
        LinkedList<string> a;
        public static MessengeQueue GetInstance(Activity act = null){
            if (instance == null) instance = new MessengeQueue(act);
            return instance;
        }
        private MessengeQueue(Activity act)
        {
            _queueMessenge = new LinkedList<IMessengeData>();
            this._activity = act;
        }

        public void SetActivity(Activity act)
        {
            this._activity = act;
        }

        public void EnqueueMessengeQueue(IMessengeData mess){
            lock (this)
            {
                _queueMessenge.AddLast(mess);
            }
        }

        public void EnMessengeListQueue(IList<IMessengeData> mess)
        {
            lock (this)
            {
                foreach (var item in mess)
                    _queueMessenge.AddLast(item);
            }
        }

        public IMessengeData DequeueMessengeQueue(){
            IMessengeData item;
            lock (this)
            {
                item = _queueMessenge.First.Value;
                _queueMessenge.RemoveFirst();
            }
            return item;
        }

        public bool Empty(){
            return (_queueMessenge.Count == 0);
        }

        public bool Clear(){
            lock (this)
            {
                _queueMessenge.Clear();
            }
            return Empty();
        }

        public int Count(){
            return _queueMessenge.Count;
        }

        public void CleanSMS()
        {
            lock (this)
            {
                var item = _queueMessenge.First;
                while (item != null)
                {
                    var next = item.Next;
                    if (item.Value.Type().Equals(TYPE_MESSAGE.SMS))
                    {
                        _queueMessenge.Remove(item);
                    }
                    item = next;
                }
            }
        }

        public void CleanMail()
        {
            lock (this)
            {              
                var item = _queueMessenge.First;
                while (item != null)
                {
                    var next = item.Next;
                    if (item.Value.Type().Equals(TYPE_MESSAGE.MAIL))
                    {
                        _queueMessenge.Remove(item);
                    }
                    item = next;
                }
            }
        }
		
    }
}
