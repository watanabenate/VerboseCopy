package crc64daa11d757e4f5dbf;


public class PodcastViewHolder
	extends androidx.recyclerview.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Verbose.src.viewholders.PodcastViewHolder, Verbose", PodcastViewHolder.class, __md_methods);
	}


	public PodcastViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == PodcastViewHolder.class)
			mono.android.TypeManager.Activate ("Verbose.src.viewholders.PodcastViewHolder, Verbose", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
