using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
    public abstract class DcfRemovalOptions
    {
        /// <summary>
        /// The pidCurrentInterfaceProperties field
        /// </summary>
        private int pidCurrentInterfaceProperties = -1;

        /// <summary>
        /// The pidcurrentConnectionProperties field
        /// </summary>
        private int pidcurrentConnectionProperties = -1;

        /// <summary>
        /// The pidcurrentConnections field
        /// </summary>
        private int pidcurrentConnections = -1;

        /// <summary>
        /// The pidnewInterfaceProperties field
        /// </summary>
        private int pidnewInterfaceProperties = -1;

        /// <summary>
        /// The pidnewConnectionProperties field
        /// </summary>
        private int pidnewConnectionProperties = -1;

        /// <summary>
        /// The pidnewConnections field
        /// </summary>
        private int pidnewConnections = -1;

        /// <summary>
        /// The helperType field
        /// </summary>
        private SyncOption helperType = SyncOption.Custom;

        /// <summary>
        /// Gets or sets the HelperType property
        /// </summary>  
        public SyncOption HelperType
        {
            get { return helperType; }
            protected set { helperType = value; }
        }

        /// <summary>
        /// Gets or sets the PIDnewConnections property
        /// </summary>  
        protected int PIDnewConnections
        {
            get { return pidnewConnections; }
            set { pidnewConnections = value; }
        }

        /// <summary>
        /// Gets or sets the PIDnewConnectionProperties property
        /// </summary>  
        protected int PIDnewConnectionProperties
        {
            get { return pidnewConnectionProperties; }
            set { pidnewConnectionProperties = value; }
        }

        /// <summary>
        /// Gets or sets the PIDnewInterfaceProperties property
        /// </summary>  
        protected int PIDnewInterfaceProperties
        {
            get { return pidnewInterfaceProperties; }
            set { pidnewInterfaceProperties = value; }
        }

        /// <summary>
        /// Gets or sets the PIDcurrentConnections property
        /// </summary>  
        protected int PIDcurrentConnections
        {
            get { return pidcurrentConnections; }
            set { pidcurrentConnections = value; }
        }

        /// <summary>
        /// Gets or sets the PIDcurrentConnectionProperties property
        /// </summary>  
        protected int PIDcurrentConnectionProperties
        {
            get { return pidcurrentConnectionProperties; }
            set { pidcurrentConnectionProperties = value; }
        }

        /// <summary>
        /// Gets or sets the PIDcurrentInterfaceProperties property
        /// </summary>  
        protected int PIDcurrentInterfaceProperties
        {
            get { return pidCurrentInterfaceProperties; }
            set { pidCurrentInterfaceProperties = value; }
        }
    }
}
