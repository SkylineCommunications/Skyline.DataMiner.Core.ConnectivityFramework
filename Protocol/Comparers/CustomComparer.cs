using System;
using System.Collections.Generic;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Comparers
{
	/// <summary>
	/// A custom comparer used in combination with some methods from the FastCollection class
	/// </summary>
	/// <typeparam name="T"></typeparam>
	//[DISCodeLibrary(Version = 1)]
	public class CustomComparer<T> : IEqualityComparer<T>
	{
		/// <summary>
		/// The keySelector field
		/// </summary>
		private Func<T, object> keySelector;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomComparer{T}" /> class.
		/// </summary>
		/// <param name="keySelector">The keySelector parameter</param>
		public CustomComparer(Func<T, object> keySelector)
		{
			this.keySelector = keySelector;
		}

		/// <summary>
		/// The Equals method
		/// </summary>
		/// <param name="x">The x parameter</param>
		/// <param name="y">The y parameter</param>
		/// <returns>The bool type object</returns>
		public bool Equals(T x, T y)
		{
			return keySelector(x).Equals(keySelector(y));
		}

		/// <summary>
		/// The GetHashCode method
		/// </summary>
		/// <param name="obj">The obj parameter</param>
		/// <returns>The int type object</returns>
		public int GetHashCode(T obj)
		{
			return keySelector(obj).GetHashCode();
		}
	}
}