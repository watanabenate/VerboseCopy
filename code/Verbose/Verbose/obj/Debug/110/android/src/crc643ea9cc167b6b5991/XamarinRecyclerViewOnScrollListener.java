package crc643ea9cc167b6b5991;


public class XamarinRecyclerViewOnScrollListener
	extends androidx.recyclerview.widget.RecyclerView.OnScrollListener
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onScrolled:(Landroidx/recyclerview/widget/RecyclerView;II)V:GetOnScrolled_Landroidx_recyclerview_widget_RecyclerView_IIHandler\n" +
			"";
		mono.android.Runtime.register ("Verbose.XamarinRecyclerViewOnScrollListener, Verbose", XamarinRecyclerViewOnScrollListener.class, __md_methods);
	}


	public XamarinRecyclerViewOnScrollListener ()
	{
		super ();
		if (getClass () == XamarinRecyclerViewOnScrollListener.class)
			mono.android.TypeManager.Activate ("Verbose.XamarinRecyclerViewOnScrollListener, Verbose", "", this, new java.lang.Object[] {  });
	}

	public XamarinRecyclerViewOnScrollListener (androidx.recyclerview.widget.LinearLayoutManager p0)
	{
		super ();
		if (getClass () == XamarinRecyclerViewOnScrollListener.class)
			mono.android.TypeManager.Activate ("Verbose.XamarinRecyclerViewOnScrollListener, Verbose", "AndroidX.RecyclerView.Widget.LinearLayoutManager, Xamarin.AndroidX.RecyclerView", this, new java.lang.Object[] { p0 });
	}


	public void onScrolled (androidx.recyclerview.widget.RecyclerView p0, int p1, int p2)
	{
		n_onScrolled (p0, p1, p2);
	}

	private native void n_onScrolled (androidx.recyclerview.widget.RecyclerView p0, int p1, int p2);

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
