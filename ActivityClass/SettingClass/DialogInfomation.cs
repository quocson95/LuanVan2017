
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace FreeHand.ActivityClass.SettingClass
{
    public class DialogInfomation : DialogFragment
    {
        int _layout;
        public DialogInfomation(int layout){
            this._layout = layout;
        }
       public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(_layout, container, false);
            return view;
        }

        public override void OnActivityCreated(Bundle saveIns)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.SetCanceledOnTouchOutside(true);
            Dialog.SetCancelable(true);
            base.OnActivityCreated(saveIns);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_aim;
        }
    }
}
