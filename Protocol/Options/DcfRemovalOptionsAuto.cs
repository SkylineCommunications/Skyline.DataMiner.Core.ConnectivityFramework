namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
	/// <summary>
	/// Represents automatic DCF removal options.
	/// </summary>
	public class DcfRemovalOptionsAuto : DcfRemovalOptionsBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DcfRemovalOptionsAuto" /> class.
		/// </summary>
		public DcfRemovalOptionsAuto()
		{
			HelperType = SyncOption.EndOfPolling;
		}
	}
}