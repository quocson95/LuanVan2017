using Android.App;
using Android.OS;
using Android.Views;

namespace FreeHand.ActivityClass.SettingClass
{
    public class DialogAccount : DialogFragment
    {
       public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
           base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.Diaglog_Select_Account, container, false);
            Android.Widget.Button google = view.FindViewById<Android.Widget.Button>(Resource.Id.google_button);
            google.Click += delegate {
                LoginMethod activity = (LoginMethod)Activity;
                activity.updateResult("google");
                Dismiss();
            };
            return view;
        }

        public override void OnActivityCreated(Bundle saveIns)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.SetCanceledOnTouchOutside(true);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_aim;

            base.OnActivityCreated(saveIns);           
        }
    }

    public interface LoginMethod
    {
        void updateResult(string method);
    }
}
