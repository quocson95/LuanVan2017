using System;
using System.Threading.Tasks;
namespace TipCalculator
{
    public interface ITest
    {
        bool GetMicroPhoneSupport();
        bool GetSpeechSDKSupport();
        //Task<string> SpeechToText();
        Task<string> SpeechToText();
    }
}
