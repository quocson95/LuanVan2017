package md56a24daee81cfa3afbd1a8aebe02e5ec8;


public class TextToSpeechLib
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer,
		android.speech.tts.TextToSpeech.OnInitListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onActivityResult:(IILandroid/content/Intent;)V:GetOnActivityResult_IILandroid_content_Intent_Handler\n" +
			"n_onInit:(I)V:GetOnInit_IHandler:Android.Speech.Tts.TextToSpeech/IOnInitListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("FreeHand.TextToSpeechLib, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", TextToSpeechLib.class, __md_methods);
	}


	public TextToSpeechLib () throws java.lang.Throwable
	{
		super ();
		if (getClass () == TextToSpeechLib.class)
			mono.android.TypeManager.Activate ("FreeHand.TextToSpeechLib, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public TextToSpeechLib (android.content.Context p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == TextToSpeechLib.class)
			mono.android.TypeManager.Activate ("FreeHand.TextToSpeechLib, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public void onActivityResult (int p0, int p1, android.content.Intent p2)
	{
		n_onActivityResult (p0, p1, p2);
	}

	private native void n_onActivityResult (int p0, int p1, android.content.Intent p2);


	public void onInit (int p0)
	{
		n_onInit (p0);
	}

	private native void n_onInit (int p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
