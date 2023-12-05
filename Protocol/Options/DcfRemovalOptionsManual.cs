namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
	/// <summary>
	/// Represents manual DCF removal options.
	/// </summary>
	public class DcfRemovalOptionsManual : DcfRemovalOptionsBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DcfRemovalOptionsManual" /> class.
		/// </summary>
		public DcfRemovalOptionsManual()
		{
			HelperType = SyncOption.Custom;
		}
	}
}