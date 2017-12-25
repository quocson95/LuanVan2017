using System;
using Android.App;
using Android.Content;

namespace FreeHand.Model
{
    public class ScriptLang
    {
        static readonly string TAG = typeof(ScriptLang).FullName;
        public string tts_continous_speak_prev_mess;
        public string tts_can_not_hear_voice;
        public string tts_do_you_want_rep;
        public string tts_content_mess;
        public string tts_subject_mail;
        public string tts_name_sender;
        public string tts_you_get_new_mess;
        public string tts_from;
        public string tts_new_call;
        public string tts_name_caller;
        public string tts_ask_for_reply;
        public string tts_name_sender_content;
        public string tts_you_get_new_mail;
        ScriptLang()
        {
            if (getPersistedData("en").Equals("vi"))
            {
                vi_VN();
            }
            else
                Default();
        }
        private static ScriptLang instance;
        public static ScriptLang Instance(){
            if (instance == null)
                instance = new ScriptLang();
            return instance;
        }

        public void Default(){
            tts_continous_speak_prev_mess = "Continuos Speak Previous Messenger";
            tts_can_not_hear_voice = "I can not hear you, please try again";
            tts_do_you_want_rep = "Do you want reply";
            tts_content_mess = "Content of Messenge";
            tts_subject_mail = "Subject of email";
            tts_name_sender = "Name of Sender";
            tts_you_get_new_mess = "You get a new message";
            tts_from = "From";
            tts_new_call = "You has new call";
            tts_name_caller = "Name of caller";
            tts_ask_for_reply = "Please speak your reply after sound beep";
            tts_name_sender_content = "Unknow";
            tts_you_get_new_mail = "You get a new email";

            persist("en");
        }

        public void vi_VN()
        {
            tts_continous_speak_prev_mess = "Tiếp tục với tin nhắn trước đó";
            tts_can_not_hear_voice = "Tôi không thể nghe được giọng bạn, vui lòng thử lại";
            tts_do_you_want_rep = "Bạn có muốn trả lời tin nhắn này hay không";
            tts_content_mess = "Nội dung tin nhắn";
            tts_subject_mail = "Chủ đề của email";
            tts_name_sender = "Tên người gửi";
            tts_you_get_new_mess = "Bạn nhận được một tin nhắn mới";
            tts_from = "Từ";
            tts_new_call = "Bạn có một cuộc gọi mới";
            tts_name_caller = "Tên người gọi";
            tts_ask_for_reply = "Vui lòng đọc nôi dung trả lời sau tiếng bíp";
            tts_name_sender_content = "Không rõ";
            tts_you_get_new_mail = "Bạn có n cuộc gọi nhỡ";
            persist("vi");
        }

        private string getPersistedData(string defaultLanguage)
        {
            var preferences = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            return preferences.GetString(TAG, defaultLanguage);
        }

        private static void persist(string language)
        {
            var preferences = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            var editor = preferences.Edit();
            editor.PutString(TAG, language);
            editor.Apply();
        }
    }
}
