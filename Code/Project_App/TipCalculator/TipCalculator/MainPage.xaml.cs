using System;
using Xamarin.Forms;

using System.Threading.Tasks;
using TipCalculator;

namespace TipCalculator
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            //Remove Title App ; 
            NavigationPage.SetHasNavigationBar(this, false);
            btn2.Clicked += A;

        }
        public void OpenSMS_SettingPage(object sender, EventArgs arg){
            Navigation.PushAsync(new SMS_Settings());
            
        }

		public void OpenCall_SettingPage(object sender, EventArgs arg)
		{			
            Navigation.PushAsync(new Call_Setting());

		}

        public async void A(object obj, EventArgs arg){
            await Check();
        }
        public async Task Check(){
            ITest test = DependencyService.Get<ITest>();
            microphone.Text = test.GetMicroPhoneSupport().ToString();
            var result = await test.SpeechToText();
            if (result != null) speech.Text = result;
        }

        //public void Check(object obj, EventArgs arg)
        //{
        //    Xamarin.Forms.DependencyService.Register<ITest>();
        //    ITest test = DependencyService.Get<ITest>();
        //    string speech_txt = test.SpeechToText();
        //    speech.Text = speech_txt;
        //}
    }
}