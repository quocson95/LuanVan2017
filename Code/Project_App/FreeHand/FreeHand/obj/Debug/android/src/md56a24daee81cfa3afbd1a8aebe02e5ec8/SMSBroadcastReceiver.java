package md56a24daee81cfa3afbd1a8aebe02e5ec8;


public class SMSBroadcastReceiver
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("FreeHand.SMSBroadcastReceiver, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", SMSBroadcastReceiver.class, __md_methods);
	}


	public SMSBroadcastReceiver () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SMSBroadcastReceiver.class)
			mono.android.TypeManager.Activate ("FreeHand.SMSBroadcastReceiver, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public SMSBroadcastReceiver (android.content.Context p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == SMSBroadcastReceiver.class)
			mono.android.TypeManager.Activate ("FreeHand.SMSBroadcastReceiver, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

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
