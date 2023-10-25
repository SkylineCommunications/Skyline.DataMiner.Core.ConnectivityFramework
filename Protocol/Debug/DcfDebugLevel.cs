namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Debug
{
	//[DISCodeLibrary(Version = 1)]
	public enum DcfDebugLevel
	{
		/// <summary>
		/// Log all DCF Debug Info.
		/// </summary>
		All,

		/// <summary>
		/// Log only the Changes and the Setup
		/// </summary>
		Changes_And_Setup,

		/// <summary>
		/// Log only when something changes.
		/// </summary>
		Only_Changes,

		/// <summary>
		/// Log only the setup (buffers, startup checks).
		/// </summary>
		Only_Setup,

		/// <summary>
		/// No DCF Debug Info.
		/// </summary>
		None
	}
}