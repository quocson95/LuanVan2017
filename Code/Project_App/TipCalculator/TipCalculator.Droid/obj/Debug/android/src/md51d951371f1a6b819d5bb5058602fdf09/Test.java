package md51d951371f1a6b819d5bb5058602fdf09;


public class Test
	extends md51d951371f1a6b819d5bb5058602fdf09.MainActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("TipCalculator.Droid.Test, TipCalculator.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Test.class, __md_methods);
	}


	public Test () throws java.lang.Throwable
	{
		super ();
		if (getClass () == Test.class)
			mono.android.TypeManager.Activate ("TipCalculator.Droid.Test, TipCalculator.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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
