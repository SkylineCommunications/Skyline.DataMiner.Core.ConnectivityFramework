namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections
{
	/// <summary>
	/// Indicates if the connection is internal, external or both.
	/// </summary>
	//[DISCodeLibrary(Version = 1)]
	public enum ConnectionType
	{
		/// <summary>
		/// Internal.
		/// </summary>
		Internal,

		/// <summary>
		/// External.
		/// </summary>
		External,

		/// <summary>
		/// Both.
		/// </summary>
		Both
	}
}