﻿namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Logs
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// The DCF log type.
	/// </summary>
	//[DISCodeLibrary(Version = 1)]
	public enum DcfLogType
	{
		/// <summary>
		/// Indicates information about a change.
		/// </summary>
		Change,

		/// <summary>
		/// Indicates information about the setup.
		/// </summary>
		Setup,

		/// <summary>
		/// Indicates information about something staying the same.
		/// </summary>
		Same,

		/// <summary>
		/// Indicates default info.
		/// </summary>
		Info,
	}
}
