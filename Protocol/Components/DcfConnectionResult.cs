using Skyline.DataMiner.Scripting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Components
{
    /// <summary>
    /// A result from performing a GetConnections using a ConnectionFilter
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfConnectionResult
    {
        /// <summary>
        /// The filter field
        /// </summary>
        private DcfConnectionFilter filter;

        /// <summary>
        /// The connections field
        /// </summary>
        private ConnectivityConnection[] connections;

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfConnectionResult" /> class.
        /// </summary>  
        public DcfConnectionResult()
        {
            filter = null;
            connections = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfConnectionResult" /> class.
        /// </summary>
        /// <param name="filter">The filter parameter</param>
        /// <param name="connections">The connections parameter</param>  
        public DcfConnectionResult(DcfConnectionFilter filter, ConnectivityConnection[] connections)
        {
            this.filter = filter;
            this.connections = connections;
        }

        /// <summary>
        /// Gets the Filter property
        /// </summary>  
        public DcfConnectionFilter Filter
        {
            get { return filter; }
        }

        /// <summary>
        /// Gets the Connections property
        /// </summary>  
        public ConnectivityConnection[] Connections
        {
            get { return connections; }
        }
    }

}
