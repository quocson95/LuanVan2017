using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Speech.Tts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Java.Util;
using Android.Util;

namespace FreeHand
{
    [Activity(Label = "TtsActivity")]
    public class TtsActivity : Activity,TextToSpeech.IOnInitListener
    {
        private static readonly string TAG = "TtsActivity";
        private TextToSpeech textToSpeech;
        private Spinner spinLanguages;
        private EditText txt_input;
        private Java.Util.Locale lang;
        private List<string> langAvailable;
        private TaskCompletionSource<Java.Lang.Object> _tcs;
        private readonly int NeedLang = 103;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_SpeechRecognizingActivity);
            // Create your application here
            Button btn_tts = (Button)FindViewById(Resource.Id.btn_start);
            spinLanguages = (Spinner) FindViewById(Resource.Id.spinLanguage);
            txt_input = (EditText)FindViewById(Resource.Id.txt_input);
            textToSpeech = new TextToSpeech(this, this, "com.google.android.tts");

			btn_tts.Click += delegate {
                string txt = txt_input.Text;
                StartTTS(txt);
            };
            spinLanguages.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                lang = Java.Util.Locale.GetAvailableLocales().FirstOrDefault(t => t.DisplayLanguage == langAvailable[(int)e.Id]);
                SetLangTTs(lang);
            };

        }
		protected async Task<TextToSpeech> CreateTtsAsync(Context context, string engName)
		{
			_tcs = new TaskCompletionSource<Java.Lang.Object>();
			var tts = new TextToSpeech(context, this, engName);
			if ((int) await _tcs.Task != (int)OperationResult.Success)
			{
				Log.Debug(TAG, "Engine: " + engName + " failed to initialize.");
				tts = null;
			}
			_tcs = null;
			return tts;
		}

        void SetLangTTs(Java.Util.Locale loc){                        
            textToSpeech.SetLanguage(loc);
		}

        void SetSpeedAndPitch(){
            //textToSpeech.SetPitch(.5f);
			//textToSpeech.SetSpeechRate(.5f);
        }
        private List<string> GetLanguageSuppport(){
            //using defalut tts
            //textextToSpeech = new TextToSpeech(this, this); 
            //using special tts
            //Task<TextToSpeech> _tts;
            //_tts = CreateTtsAsync(this, "com.google.android.tts");
            //textToSpeech = _tts.Result;
            //textToSpeech = new TextToSpeech(this, null);

			IList<TextToSpeech.EngineInfo> engines = textToSpeech.Engines;
			var intent = new Intent(TextToSpeech.Engine.ActionCheckTtsData);
            StartActivityForResult(intent, 10);
			langAvailable = new List<string> { "Default" };
			var localesAvailable = Java.Util.Locale.GetAvailableLocales().ToList();
			foreach (var locale in localesAvailable)
			{
				var res = textToSpeech.IsLanguageAvailable(locale);
				switch (res)
				{
					case LanguageAvailableResult.Available:
						langAvailable.Add(locale.DisplayLanguage);
						break;
					case LanguageAvailableResult.CountryAvailable:
						langAvailable.Add(locale.DisplayLanguage);
						break;
					case LanguageAvailableResult.CountryVarAvailable:
						langAvailable.Add(locale.DisplayLanguage);
						break;
				}
			}

			langAvailable = langAvailable.OrderBy(t => t).Distinct().ToList();
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, langAvailable);
            spinLanguages.Adapter = adapter;
            return langAvailable;
        }

        void InstallNewLanguage(){            
			var checkTTSIntent = new Intent();
			checkTTSIntent.SetAction(TextToSpeech.Engine.ActionCheckTtsData);
			StartActivityForResult(checkTTSIntent, NeedLang);
        }
		void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
		{			
            // if we get an error, default to the default language
            if (status == OperationResult.Error)
            {
                Log.Debug(TAG, "Set lang error");
                textToSpeech.SetLanguage(Java.Util.Locale.Default);
            }
            // if the listener is ok, set the lang
            if (status == OperationResult.Success)
            {
				//lang = new Locale("vie-vnm");
				//textToSpeech.SetLanguage(lang);
				//Log.Debug(TAG, "Set lang success");
				GetLanguageSuppport();				
				SetSpeedAndPitch();
            }
		}

        void StartTTS(string text){
            //text = "Hello";
            text = "Và tôi cầm lấy đóm, vo viên một điếu. Tôi rít một hơi xong, thông điếu\nrồi mới đặt vào lòng lão. Lão bỏ thuốc, nhưng chưa hút vội. Lão cầm lấy\nđóm, gạt tàn, và bảo :\n- Có lẽ tôi bán con chó đấy, ông giáo ạ !\n\nLão đặt xe điếu, hút. Tôi vừa thở khói, vừa gà gà đôi mắt của người say,\nnhìn lão, nhì để làm ra vẻ chú ý đến câu nói của lão đó thôi. Thật ra thì\ntrong lòng tôi rất dửng dưng. Tôi nghe câu ấy đã nhàm rồi. Tôi lại biết\nrằng : lão nói là nói để có đấy thôi ; chẳng bao giờ lão bán đâu. Vả lại,\ncó bán thật nữa thì đã sao ? Làm quái gì một con chó mà lão có vẻ băn\nkhoăn quá thế..\n\nLão hút xong, đặt xe điếu xuống, quay ra ngoài, thở khói. Sau một điếu\nthuốc lào, óc người ta tê dại đi trong một nỗi đê mê nhẹ nhõm. Lão Hạc\nngồi lặng lẽ, hưởng chút khoái lạc con con ấy. Tôi cũng ngồi lặng lẽ. Tôi\nnghĩ đến mấy quyển sách quý của tôi. Hồi bị ốm nặng ở Sài Gòn tôi bán gần\nhết cả áo quần, nhưng vẫn không chịu bán cho ai một quyển. ốm dậy, tôi về\nquê, hành lý chỉ vẻn vẹn có một cái va-ly đựng toàn những sách. Ôi những\nquyển sách rất nâng niu ! Tôi đã nguyện giữ chúng suốt đời, để lưu lại cái\nkỷ niệm một thời chăm chỉ, hăng hái và tin tưởng đầy những say mê đẹp và\ncao vọng : mỗi lần mở một quyển ra, chưa kịp đọc dòng nào, tôi đã thấy bừng\nlên trong lòng tôi như một rạng đông, cái hình ảnh tuổi hai mươi trong\ntrẻo, biết yêu và biết ghét... Nhưng đời người ta không chỉ khổ một lần.\nMỗi lần cùng đường đất sinh nhai, và bán hết mọi thứ rồi, tôi lại phải bán\nđi một ít sách của tôi. Sau cùng chỉ còn có năm quyển, tôi nhất định, dù\ncó phải chết cũng không chịu bán. ấy thế mà tôi cũng bán ! Mới cách đây có\nhơn một tháng thôi, đứa con nhỏ của tôi bị chứng lỵ gần kiệt sức... Không\n! Lão Hạc ơi ! Ta có quyền giữ cho ta một tý gì đâu ? Lão quý c";
            if (!string.IsNullOrEmpty(text)){
                //CheckLangHasIntall();
                textToSpeech.Speak(text,QueueMode.Flush,null);
            }

        }

		protected override void OnActivityResult(int req, Result res, Intent data)
		{
			if (req == NeedLang)
			{
				// we need a new language installed
				var installTTS = new Intent();
				installTTS.SetAction(TextToSpeech.Engine.ActionInstallTtsData);
				StartActivity(installTTS);
			}
            if (req == 10){
                IList<String> voices = data.GetStringArrayListExtra(TextToSpeech.Engine.ExtraAvailableVoices);
            }
			
		}

	}
}
