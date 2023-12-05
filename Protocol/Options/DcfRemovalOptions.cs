namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
	/// <summary>
	/// Represents the DCF removal options.
	/// </summary>
	public abstract class DcfRemovalOptions : DcfRemovalOptionsBase
	{
		/// <summary>
		/// Gets or sets the PIDnewConnections property
		/// </summary>
		public int PIDnewConnections { get; set; } = -1;

		/// <summary>
		/// Gets or sets the PIDnewConnectionProperties property
		/// </summary>
		public int PIDnewConnectionProperties { get; set; } = -1;

		/// <summary>
		/// Gets or sets the PIDnewInterfaceProperties property
		/// </summary>
		public int PIDnewInterfaceProperties { get; set; } = -1;
	}
}