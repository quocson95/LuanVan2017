using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace FreeHand.ActivityClass.SettingClass
{

    public class ListPhoneDetailAdapter : BaseAdapter<Tuple<string, string>>
    {
        IList<Tuple<string, string>> _items;
        Activity _context;
        public ListPhoneDetailAdapter(Activity context, IList<Tuple<string, string>> items)
        {
            this._items = items;
            this._context = context;
        }
        public override Tuple<string, string> this[int position] 
        {
            get { return _items[position]; }
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.phone_details_layout, null);
            
            var contactNumber = view.FindViewById<TextView>(Resource.Id.number);
            var contactName = view.FindViewById<TextView>(Resource.Id.name);

            contactNumber.Text = item.Item1;
            contactName.Text = item.Item2;           
            return view;
        }

        public void Update(IList<Tuple<string,string>> items)
        {
            _items.Clear();
            _items = items;
            NotifyDataSetChanged();
        }
    }
}
