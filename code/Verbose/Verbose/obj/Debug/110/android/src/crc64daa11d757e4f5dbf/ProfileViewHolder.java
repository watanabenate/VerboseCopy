package crc64daa11d757e4f5dbf;


public class ProfileViewHolder
	extends androidx.recyclerview.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Verbose.src.viewholders.ProfileViewHolder, Verbose", ProfileViewHolder.class, __md_methods);
	}


	public ProfileViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == ProfileViewHolder.class)
			mono.android.TypeManager.Activate ("Verbose.src.viewholders.ProfileViewHolder, Verbose", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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