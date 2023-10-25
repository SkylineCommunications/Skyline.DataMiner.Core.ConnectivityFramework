namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections
{
	/// <summary>
	/// Determines how a connection type should be saved.
	/// </summary>
	public enum SaveConnectionType
	{
		/// <summary>
		/// Indicates that the Source Interface can only have a single Connection.
		/// </summary>
		Unique_Source,

		/// <summary>
		/// Indicates that the Destination Interface can only have a single Connection.
		/// </summary>
		Unique_Destination,

		/// <summary>
		/// Indicates that the name of a connection is Unique.
		/// </summary>
		Unique_Name,

		/// <summary>
		/// Indicates that there can only be a single connection between a single source and destination interface.
		/// </summary>
		Unique_SourceAndDestination,
	}
}