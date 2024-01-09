package crc643ea9cc167b6b5991;


public class ProfileSubsPageFragment
	extends androidx.fragment.app.Fragment
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreateView:(Landroid/view/LayoutInflater;Landroid/view/ViewGroup;Landroid/os/Bundle;)Landroid/view/View;:GetOnCreateView_Landroid_view_LayoutInflater_Landroid_view_ViewGroup_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Verbose.ProfileSubsPageFragment, Verbose", ProfileSubsPageFragment.class, __md_methods);
	}


	public ProfileSubsPageFragment ()
	{
		super ();
		if (getClass () == ProfileSubsPageFragment.class)
			mono.android.TypeManager.Activate ("Verbose.ProfileSubsPageFragment, Verbose", "", this, new java.lang.Object[] {  });
	}


	public ProfileSubsPageFragment (int p0)
	{
		super (p0);
		if (getClass () == ProfileSubsPageFragment.class)
			mono.android.TypeManager.Activate ("Verbose.ProfileSubsPageFragment, Verbose", "System.Int32, mscorlib", this, new java.lang.Object[] { p0 });
	}


	public android.view.View onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2)
	{
		return n_onCreateView (p0, p1, p2);
	}

	private native android.view.View n_onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2);

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
