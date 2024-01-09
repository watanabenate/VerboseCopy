using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verbose.Data
{
    /// <summary>
    /// Taken from https://damienaicheh.github.io/xamarin/xamarin.android/2018/05/26/xamarin-android-parcelable-en.html
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericParcelableCreator<T> : Java.Lang.Object, IParcelableCreator where T : Java.Lang.Object, new()
    {
        /// <summary>
        /// Function for the creation of a parcel.
        /// </summary>
        private readonly Func<Parcel, T> _createFunc;

        /// <summary>
        /// Initialize an instance of the GenericParcelableCreator.
        /// </summary>
        public GenericParcelableCreator(Func<Parcel, T> createFromParcelFunc)
        {
            _createFunc = createFromParcelFunc;
        }

        /// <summary>
        /// Create a parcelable from a parcel.
        /// </summary>
        public Java.Lang.Object CreateFromParcel(Parcel parcel)
        {
            return _createFunc(parcel);
        }

        /// <summary>
        /// Create an array from the parcelable class.
        /// </summary>
        public Java.Lang.Object[] NewArray(int size)
        {
            return new T[size];
        }
    }

}