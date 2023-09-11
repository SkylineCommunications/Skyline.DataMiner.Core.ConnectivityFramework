using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
    /// <summary>
    /// Provide PIDs that will hold Mapping of all Connections & Properties Managed by this Element. Leaving PIDs out will create a more efficient DCFHelper Object but with limited functionality. 
    /// For Example: Only defining the CurrentConnectionsPID will allow a user to Add and Remove Connections but it will not be possible to Manipulate any Properties.
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfMappingOptions : DcfRemovalOptions
    {
        /// <summary>
        /// Gets or sets the HelperType property
        /// </summary>  
        public new SyncOption HelperType
        {
            get { return (SyncOption)base.HelperType; }
            set { base.HelperType = (SyncOption)value; }
        }

        /// <summary>
        /// Gets or sets the PIDnewConnections property
        /// </summary>  
        public new int PIDnewConnections
        {
            get { return base.PIDnewConnections; }
            set { base.PIDnewConnections = value; }
        }

        /// <summary>
        /// Gets or sets the PIDnewConnectionProperties property
        /// </summary>  
        public new int PIDnewConnectionProperties
        {
            get { return base.PIDnewConnectionProperties; }
            set { base.PIDnewConnectionProperties = value; }
        }

        /// <summary>
        /// Gets or sets the PIDnewInterfaceProperties property
        /// </summary>  
        public new int PIDnewInterfaceProperties
        {
            get { return base.PIDnewInterfaceProperties; }
            set { base.PIDnewInterfaceProperties = value; }
        }

        /// <summary>
        /// Gets or sets the PIDcurrentConnections property
        /// </summary>  
        public new int PIDcurrentConnections
        {
            get { return base.PIDcurrentConnections; }
            set { base.PIDcurrentConnections = value; }
        }

        /// <summary>
        /// Gets or sets the PIDcurrentConnectionProperties property
        /// </summary>  
        public new int PIDcurrentConnectionProperties
        {
            get { return base.PIDcurrentConnectionProperties; }
            set { base.PIDcurrentConnectionProperties = value; }
        }

        /// <summary>
        /// Gets or sets the PIDcurrentInterfaceProperties property
        /// </summary>  
        public new int PIDcurrentInterfaceProperties
        {
            get { return base.PIDcurrentInterfaceProperties; }
            set { base.PIDcurrentInterfaceProperties = value; }
        }
    }
}
