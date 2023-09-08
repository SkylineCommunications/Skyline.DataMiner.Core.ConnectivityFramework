using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
    /// <summary>
    /// Indicates the DCFHelper is running in 'buffer' mode. It will buffer all changes until such a time the LastInBuffer is set to true or RemovalOptionsAuto is used.
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfRemovalOptionsBuffer : DcfRemovalOptions
    {
        /// <summary>
        /// The lastInBuffer field
        /// </summary>
        private bool lastInBuffer = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfRemovalOptionsBuffer" /> class.
        /// </summary>
        /// <param name="lastInBuffer">The lastInBuffer parameter</param>  
        public DcfRemovalOptionsBuffer(bool lastInBuffer)
        {
            LastInBuffer = lastInBuffer;
        }

        /// <summary>
        /// Gets or sets the LastInBuffer property
        /// </summary>  
        public bool LastInBuffer
        {
            get
            {
                return lastInBuffer;
            }

            set
            {
                if (value)
                {
                    HelperType = SyncOption.EndOfPolling;
                }
                else
                {
                    HelperType = SyncOption.PollingSync;
                }

                lastInBuffer = value;
            }
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
