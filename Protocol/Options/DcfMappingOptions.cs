using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
	/// <summary>
	/// Provide PIDs that will hold Mapping of all Connections and Properties Managed by this Element. Leaving PIDs out will create a more efficient DCFHelper Object but with limited functionality.
	/// For Example: Only defining the CurrentConnectionsPID will allow a user to Add and Remove Connections but it will not be possible to Manipulate any Properties.
	/// </summary>
	//[DISCodeLibrary(Version = 1)]
	public class DcfMappingOptions : DcfRemovalOptions
	{
		
	}
}