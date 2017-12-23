using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace FreeHand.Message.Mail
{    
    public class ListAccountAdapter : BaseAdapter<IMailAction>
    {
        IList<IMailAction> items;
        Activity context;
        public ListAccountAdapter(Activity context, IList<IMailAction> items): base()
        {
                   this.context = context;
                   this.items = items;
        }
        public override IMailAction this[int position] 
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
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.listAccount, null);            
            return view;
        }

      
    }
}
