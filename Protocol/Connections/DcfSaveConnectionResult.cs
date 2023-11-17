using Skyline.DataMiner.Scripting;

using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections
{
	/// <summary>
	/// A result object from performing a SaveConnections method
	/// </summary>
	//[DISCodeLibrary(Version = 1)]
	public class DcfSaveConnectionResult
	{
		/// <summary>
		/// The sourceConnection field
		/// </summary>
		private ConnectivityConnection sourceConnection;

		/// <summary>
		/// The sourceConnectionID field
		/// </summary>
		private int sourceConnectionID;

		/// <summary>
		/// The destinationConnection field
		/// </summary>
		private ConnectivityConnection destinationConnection;

		/// <summary>
		/// The destinationConnectionID field
		/// </summary>
		private int destinationConnectionID;

		/// <summary>
		/// The internalConnection field
		/// </summary>
		private bool internalConnection;

		/// <summary>
		/// The updated field
		/// </summary>
		private bool updated;

		/// <summary>
		/// The propertyResults field
		/// </summary>
		private DcfSaveConnectionPropertyResult[] propertyResults;

		/// <summary>
		/// Initializes a new instance of the <see cref="DcfSaveConnectionResult" /> class.
		/// </summary>
		/// <param name="sourceConnection">The sourceConnection parameter</param>
		/// <param name="destinationConnection">The destinationConnection parameter</param>
		/// <param name="internalConnection">The internalConnection parameter</param>
		/// <param name="updated">The updated parameter</param>
		/// <param name="propertyResults">The propertyResults parameter</param>  
		public DcfSaveConnectionResult(ConnectivityConnection sourceConnection, ConnectivityConnection destinationConnection, bool internalConnection, bool updated, DcfSaveConnectionPropertyResult[] propertyResults)
		{
			this.sourceConnection = sourceConnection;
			if (sourceConnection != null)
			{
				sourceConnectionID = sourceConnection.ConnectionId;
			}
			else
			{
				sourceConnectionID = -1;
			}
			this.destinationConnection = destinationConnection;

			if (destinationConnection != null)
			{
				destinationConnectionID = destinationConnection.ConnectionId;
			}
			else
			{
				destinationConnectionID = -1;
			}
			this.internalConnection = internalConnection;
			this.updated = updated;
			this.propertyResults = propertyResults;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DcfSaveConnectionResult" /> class.
		/// </summary>
		/// <param name="sourceConnection">The sourceConnection parameter</param>
		/// <param name="destinationConnection">The destinationConnection parameter</param>
		/// <param name="internalConnection">The internalConnection parameter</param>
		/// <param name="updated">The updated parameter</param>
		/// <param name="propertyResults">The propertyResults parameter</param>  
		public DcfSaveConnectionResult(int sourceConnectionID, int destinationConnectionID, bool internalConnection, bool updated, DcfSaveConnectionPropertyResult[] propertyResults)
		{
			sourceConnection = null;
			this.sourceConnectionID = sourceConnectionID;
			destinationConnection = null;
			this.destinationConnectionID = destinationConnectionID;
			this.internalConnection = internalConnection;
			this.updated = updated;
			this.propertyResults = propertyResults;
		}


		/// <summary>
		/// Gets the SourceConnection ID
		/// </summary>  
		public int SourceConnectionID
		{
			get { return sourceConnectionID; }
			private set { sourceConnectionID = value; }
		}

		/// <summary>
		/// Gets the DestinationConnection ID
		/// </summary>  
		public int DestinationConnectionID
		{
			get { return destinationConnectionID; }
			private set { destinationConnectionID = value; }
		}

		/// <summary>
		/// Gets the SourceConnection property
		/// </summary>  
		public ConnectivityConnection SourceConnection
		{
			get { return sourceConnection; }
			private set { sourceConnection = value; }
		}

		/// <summary>
		/// Gets the DestinationConnection property
		/// </summary>  
		public ConnectivityConnection DestinationConnection
		{
			get { return destinationConnection; }
			private set { destinationConnection = value; }
		}

		/// <summary>
		/// Gets the InternalConnection property
		/// </summary>  
		public bool InternalConnection
		{
			get { return internalConnection; }
			private set { internalConnection = value; }
		}

		/// <summary>
		/// Gets the Updated property
		/// </summary>  
		public bool Updated
		{
			get { return updated; }
			private set { updated = value; }
		}

		/// <summary>
		/// Gets the DcfSaveConnectionPropertyResults
		/// </summary>
		public DcfSaveConnectionPropertyResult[] DcfSaveConnectionPropertyResults { get { return propertyResults; } }
	}
}