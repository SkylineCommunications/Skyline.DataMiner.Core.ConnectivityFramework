using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Skyline.Protocol.Library.ProtocolDCF;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Components
{
    /// <summary>
    /// Objects of this class represent one or more unique Interfaces, specified by it's ParameterGroupID. In case of a table, the Key of the table must also be specified. In case of external element, the elementKey (DmaID/EleID) must also be specified.
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfInterfaceFilter
    {
        /// <summary>
        /// The parameterGroupID field
        /// </summary>        
        private int parameterGroupID;

        /// <summary>
        /// The tableKey field
        /// </summary>        
        private string tableKey;

        /// <summary>
        /// The elementKey field
        /// </summary>        
        private string elementKey;

        /// <summary>
        /// The custom field
        /// </summary>        
        private bool custom;

        /// <summary>
        /// The getAll field
        /// </summary>        
        private bool getAll;

        /// <summary>
        /// The interfaceName field
        /// </summary>
        private string interfaceName;

        /// <summary>
        /// The propertyFilter field
        /// </summary>
        private DcfPropertyFilter propertyFilter;

        /// <summary>
        /// Gets the PropertyFilter property
        /// </summary>  
        public DcfPropertyFilter PropertyFilter
        {
            get { return propertyFilter; }
            protected set { propertyFilter = value; }
        }

        /// <summary>
        /// Gets the TableKey property
        /// </summary>  
        public string TableKey
        {
            get { return tableKey; }
            protected set { tableKey = value; }
        }

        /// <summary>
        /// Gets the ElementKey property
        /// </summary>  
        public string ElementKey
        {
            get { return elementKey; }
            protected set { elementKey = value; }
        }

        /// <summary>
        /// Gets the GetAll property
        /// </summary>  
        public bool GetAll
        {
            get { return getAll; }
            protected set { getAll = value; }
        }

        /// <summary>
        /// Gets the Custom property
        /// </summary>  
        public bool Custom
        {
            get { return custom; }
            protected set { custom = value; }
        }

        /// <summary>
        /// Gets the ParameterGroupID property
        /// </summary>  
        public int ParameterGroupID
        {
            get { return parameterGroupID; }
            protected set { parameterGroupID = value; }
        }

        /// <summary>
        /// Gets the InterfaceName property
        /// </summary>  
        public string InterfaceName
        {
            get { return interfaceName; }
            protected set { interfaceName = value; }
        }
    }
}
