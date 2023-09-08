using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections;
using Skyline.DataMiner.Scripting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Interfaces
{
    /// <summary>
    /// A result object after performing a SaveConnectionsProperties calls
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfSaveInterfacePropertyResult
    {
        /// <summary>
        /// The property field
        /// </summary>
        private ConnectivityInterfaceProperty property;

        /// <summary>
        /// The success field
        /// </summary>
        private bool success;

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionPropertyResult" /> class.
        /// </summary>
        /// <param name="result">The result parameter</param>  
        public DcfSaveInterfacePropertyResult(bool result)
        {
            success = result;
            property = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionPropertyResult" /> class.
        /// </summary>
        /// <param name="result">The result parameter</param>
        /// <param name="prop">The prop parameter</param>  
        public DcfSaveInterfacePropertyResult(bool result, ConnectivityInterfaceProperty prop)
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
        public ConnectivityInterfaceProperty Property
        {
            get { return property; }
            private set { property = value; }
        }
    }
}
