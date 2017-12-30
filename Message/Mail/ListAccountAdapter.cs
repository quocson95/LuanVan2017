using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace FreeHand.Message.Mail
{    
    public class ListAccountAdapter : BaseAdapter<Tuple<User, object>>
    {
        IList<Tuple<User, object>> items;
        Activity context;
        public ListAccountAdapter(Activity context, IList<Tuple<User, object>> items): base()
        {
                   this.context = context;
                   this.items = items;
        }
        public override Tuple<User, object> this[int position] 
        {
            get { return items[position]; }
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            IMailAction mail = (GmailAction)item.Item2;
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.listAccount, null);
            TextView name_login = view.FindViewById<TextView>(Resource.Id.name_login);
            TextView nameDisplay = view.FindViewById<TextView>(Resource.Id.nameDisplay);
            Switch swActive = view.FindViewById<Switch>(Resource.Id.sw_active);

            name_login.Text = mail.GetNameLogin();
            nameDisplay.Text = item.Item1.Name;
            swActive.Selected = mail.GetActive();
            return view;;
        }

      
    }
}
