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

        public void EnMessengeListQueue(IList<IMessengeData> mess)
        {
            foreach ( var item in mess)
                _queueMessenge.Enqueue(item);
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

        public int Count(){
            return _queueMessenge.Count;
        }
		
    }
}
