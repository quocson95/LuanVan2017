package md56a24daee81cfa3afbd1a8aebe02e5ec8;


public class TextToSpeechLib_UtteranceProgressLs
	extends android.speech.tts.UtteranceProgressListener
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onStart:(Ljava/lang/String;)V:GetOnStart_Ljava_lang_String_Handler\n" +
			"n_onError:(Ljava/lang/String;)V:GetOnError_Ljava_lang_String_Handler\n" +
			"n_onDone:(Ljava/lang/String;)V:GetOnDone_Ljava_lang_String_Handler\n" +
			"";
		mono.android.Runtime.register ("FreeHand.TextToSpeechLib+UtteranceProgressLs, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", TextToSpeechLib_UtteranceProgressLs.class, __md_methods);
	}


	public TextToSpeechLib_UtteranceProgressLs () throws java.lang.Throwable
	{
		super ();
		if (getClass () == TextToSpeechLib_UtteranceProgressLs.class)
			mono.android.TypeManager.Activate ("FreeHand.TextToSpeechLib+UtteranceProgressLs, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public TextToSpeechLib_UtteranceProgressLs (md56a24daee81cfa3afbd1a8aebe02e5ec8.TextToSpeechLib p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == TextToSpeechLib_UtteranceProgressLs.class)
			mono.android.TypeManager.Activate ("FreeHand.TextToSpeechLib+UtteranceProgressLs, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "FreeHand.TextToSpeechLib, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void onStart (java.lang.String p0)
	{
		n_onStart (p0);
	}

	private native void n_onStart (java.lang.String p0);


	public void onError (java.lang.String p0)
	{
		n_onError (p0);
	}

	private native void n_onError (java.lang.String p0);


	public void onDone (java.lang.String p0)
	{
		n_onDone (p0);
	}

	private native void n_onDone (java.lang.String p0);

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
