package md5ba3999f25c238555190cf577d9e17e81;


public class MessengeQueue
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("FreeHand.Model.MessengeQueue, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", MessengeQueue.class, __md_methods);
	}


	public MessengeQueue () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MessengeQueue.class)
			mono.android.TypeManager.Activate ("FreeHand.Model.MessengeQueue, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public MessengeQueue (android.app.Activity p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == MessengeQueue.class)
			mono.android.TypeManager.Activate ("FreeHand.Model.MessengeQueue, FreeHand, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.App.Activity, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}

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
