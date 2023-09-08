using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
    public class DcfRemovalOptionsAuto : DcfRemovalOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DcfRemovalOptionsAuto" /> class.
        /// </summary>  
        public DcfRemovalOptionsAuto()
        {
            HelperType = SyncOption.EndOfPolling;
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
