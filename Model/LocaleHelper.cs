using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Java.Util;

namespace FreeHand.Model
{
    public class LocaleHelper
    {

        private static readonly string SELECTED_LANGUAGE = typeof(LocaleHelper).FullName;

        public static Context onAttach(Context context)
        {
            string lang = getPersistedData(context, Locale.Default.Language);
            Log.Info("LocaleHelper", lang);
            return setLocale(context, lang);
        }

        public static Context onAttach(Context context, string defaultLanguage)
        {
            string lang = getPersistedData(context, defaultLanguage);
            return setLocale(context, lang);
        }

        public static string getLanguage(Context context)
        {
            return getPersistedData(context, Locale.Default.Language);
        }

        public static Context setLocale(Context context, string language)
        {
            persist(context, language);

            //if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
            //{
            //    return updateResources(context, language);
            //}

            return updateResourcesLegacy(context, language);
        }

        private static string getPersistedData(Context context, string defaultLanguage)
        {
            var preferences = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            return preferences.GetString(SELECTED_LANGUAGE, defaultLanguage);
        }

        private static void persist(Context context, string language)
        {
            var preferences = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            var editor = preferences.Edit();
            editor.PutString(SELECTED_LANGUAGE, language);
            editor.Apply();
        }


        private static Context updateResources(Context context, string language)
        {
            Locale locale = new Locale(language);
            Locale.Default = locale;

            Configuration configuration = context.Resources.Configuration;
            configuration.SetLocale(locale);
            configuration.SetLayoutDirection(locale);
            return context.CreateConfigurationContext(configuration);
        }


        private static Context updateResourcesLegacy(Context context, string language)
        {
            Locale locale = new Locale(language);
            Locale.Default = locale;

            Resources resources = context.Resources;

            Configuration configuration = resources.Configuration;
            configuration.Locale = locale;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
            {
                configuration.SetLayoutDirection(locale);
            }
            resources.UpdateConfiguration(configuration, resources.DisplayMetrics);

            return context;
        }
    }
}