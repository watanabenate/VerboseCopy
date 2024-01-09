package crc645d7077849783abec;


public class ListenedToParcelable
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.os.Parcelable
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_GetCreator:()Lcrc645d7077849783abec/GenericParcelableCreator_1;:__export__\n" +
			"n_describeContents:()I:GetDescribeContentsHandler:Android.OS.IParcelableInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_writeToParcel:(Landroid/os/Parcel;I)V:GetWriteToParcel_Landroid_os_Parcel_IHandler:Android.OS.IParcelableInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("Verbose.Data.ListenedToParcelable, Verbose.Data", ListenedToParcelable.class, __md_methods);
	}


	public ListenedToParcelable ()
	{
		super ();
		if (getClass () == ListenedToParcelable.class)
			mono.android.TypeManager.Activate ("Verbose.Data.ListenedToParcelable, Verbose.Data", "", this, new java.lang.Object[] {  });
	}

	public ListenedToParcelable (android.os.Parcel p0)
	{
		super ();
		if (getClass () == ListenedToParcelable.class)
			mono.android.TypeManager.Activate ("Verbose.Data.ListenedToParcelable, Verbose.Data", "Android.OS.Parcel, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public static crc645d7077849783abec.GenericParcelableCreator_1 CREATOR = GetCreator ();

	public static crc645d7077849783abec.GenericParcelableCreator_1 GetCreator ()
	{
		return n_GetCreator ();
	}

	private static native crc645d7077849783abec.GenericParcelableCreator_1 n_GetCreator ();


	public int describeContents ()
	{
		return n_describeContents ();
	}

	private native int n_describeContents ();


	public void writeToParcel (android.os.Parcel p0, int p1)
	{
		n_writeToParcel (p0, p1);
	}

	private native void n_writeToParcel (android.os.Parcel p0, int p1);

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
