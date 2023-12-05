namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public abstract class DcfRemovalOptionsBase
	{
		/// <summary>
		/// Gets or sets the HelperType property
		/// </summary>
		public SyncOption HelperType { get; set; } = SyncOption.Custom;

		/// <summary>
		/// Gets or sets the PIDcurrentConnections property
		/// </summary>  
		public int PIDcurrentConnections { get; set; } = -1;

		/// <summary>
		/// Gets or sets the PIDcurrentConnectionProperties property
		/// </summary>  
		public int PIDcurrentConnectionProperties { get; set; } = -1;

		/// <summary>
		/// Gets or sets the PIDcurrentInterfaceProperties property
		/// </summary>  
		public int PIDcurrentInterfaceProperties { get; set; } = -1;
	}
}
