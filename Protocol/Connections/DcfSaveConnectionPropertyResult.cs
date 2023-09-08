using Skyline.DataMiner.Scripting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections
{
    /// <summary>
    /// A result object after performing a SaveConnectionsProperties calls
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfSaveConnectionPropertyResult
    {
        /// <summary>
        /// The property field
        /// </summary>
        private ConnectivityConnectionProperty property;

        /// <summary>
        /// The success field
        /// </summary>
        private bool success;

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionPropertyResult" /> class.
        /// </summary>
        /// <param name="result">The result parameter</param>  
        public DcfSaveConnectionPropertyResult(bool result)
        {
            success = result;
            property = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionPropertyResult" /> class.
        /// </summary>
        /// <param name="result">The result parameter</param>
        /// <param name="prop">The prop parameter</param>  
        public DcfSaveConnectionPropertyResult(bool result, ConnectivityConnectionProperty prop)
        {
            success = result;
            property = prop;
        }

        /// <summary>
        /// Gets the Success property
        /// </summary>  
        public bool Success
        {
            get { return success; }
            private set { success = value; }
        }

        /// <summary>
        /// Gets the Property property
        /// </summary>  
        public ConnectivityConnectionProperty Property
        {
            get { return property; }
            private set { property = value; }
        }
    }
}
