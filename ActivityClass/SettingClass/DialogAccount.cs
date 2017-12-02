﻿
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
    public class DialogAccount : DialogFragment
    {
       public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.Diaglog_Select_Account, container, false);
            return view;
        }

        public override void OnActivityCreated(Bundle saveIns)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.SetCanceledOnTouchOutside(true);
            base.OnActivityCreated(saveIns);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_aim;
        }
    }
}
