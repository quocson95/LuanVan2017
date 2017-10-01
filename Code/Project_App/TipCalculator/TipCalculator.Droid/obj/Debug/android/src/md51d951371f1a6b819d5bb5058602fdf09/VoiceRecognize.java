package md51d951371f1a6b819d5bb5058602fdf09;


public class VoiceRecognize
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("TipCalculator.Droid.VoiceRecognize, TipCalculator.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", VoiceRecognize.class, __md_methods);
	}


	public VoiceRecognize () throws java.lang.Throwable
	{
		super ();
		if (getClass () == VoiceRecognize.class)
			mono.android.TypeManager.Activate ("TipCalculator.Droid.VoiceRecognize, TipCalculator.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
