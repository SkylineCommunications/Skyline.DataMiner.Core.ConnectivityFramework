using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Filters;
using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Interfaces;
using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Skyline.Protocol.Library.ProtocolDCF;
using Skyline.DataMiner.Scripting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections
{
    /// <summary>
    /// A filter representing one or more Connections
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfConnectionFilter
    {
        /// <summary>
        /// The connectionID field
        /// </summary>
        private int connectionID = -1;

        /// <summary>
        /// The connectionName field
        /// </summary>
        private string connectionName = null;

        /// <summary>
        /// The sourceFilter field
        /// </summary>
        private Interfaces.DcfInterfaceFilterSingle sourceFilter = null;

        /// <summary>
        /// The destinationFilter field
        /// </summary>
        private Interfaces.DcfInterfaceFilterSingle destinationFilter = null;

        /// <summary>
        /// The sourceInterface field
        /// </summary>
        private ConnectivityInterface sourceInterface = null;

        /// <summary>
        /// The destinationInterface field
        /// </summary>
        private ConnectivityInterface destinationInterface = null;

        /// <summary>
        /// The propertyFilter field
        /// </summary>
        private Filters.DcfPropertyFilter propertyFilter = null;

        /// <summary>
        /// The elementKey field
        /// </summary>
        private string elementKey = null;

        /// <summary>
        /// The type field
        /// </summary>
        private ConnectionType type = ConnectionType.Both;

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfConnectionFilter" /> class.
        /// </summary>
        /// <param name="elementKey">The elementKey parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>  
        public DcfConnectionFilter(string elementKey = "local", ConnectionType connectionType = ConnectionType.Both, Filters.DcfPropertyFilter propertyFilter = null)
        {
            type = connectionType;
            this.elementKey = elementKey;
            this.propertyFilter = propertyFilter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfConnectionFilter" /> class.
        /// </summary>
        /// <param name="connectionID">The connectionID parameter</param>
        /// <param name="elementKey">The elementKey parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>  
        public DcfConnectionFilter(int connectionID, string elementKey = "local", ConnectionType connectionType = ConnectionType.Both, Filters.DcfPropertyFilter propertyFilter = null)
        {
            type = connectionType;
            this.connectionID = connectionID;
            this.elementKey = elementKey;
            this.propertyFilter = propertyFilter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfConnectionFilter" /> class.
        /// </summary>
        /// <param name="connectionName">The connectionName parameter</param>
        /// <param name="elementKey">The elementKey parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>  
        public DcfConnectionFilter(string connectionName, string elementKey = "local", ConnectionType connectionType = ConnectionType.Both, Filters.DcfPropertyFilter propertyFilter = null)
        {
            type = connectionType;
            this.connectionName = connectionName;
            this.elementKey = elementKey;
            this.propertyFilter = propertyFilter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfConnectionFilter" /> class.
        /// </summary>
        /// <param name="filter">The filter parameter</param>
        /// <param name="type">The type parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>  
        public DcfConnectionFilter(Interfaces.DcfInterfaceFilterSingle filter, Interfaces.InterfaceType type = Interfaces.InterfaceType.Source, ConnectionType connectionType = ConnectionType.Both, Filters.DcfPropertyFilter propertyFilter = null)
        {
            this.propertyFilter = propertyFilter;
            this.type = connectionType;
            switch (type)
            {
                case Interfaces.InterfaceType.Source:
                    sourceFilter = filter;
                    break;
                case Interfaces.InterfaceType.Destination:
                    destinationFilter = filter;
                    break;
                default:
                    sourceFilter = filter;
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfConnectionFilter" /> class.
        /// </summary>
        /// <param name="itf">The itf parameter</param>
        /// <param name="type">The type parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>  
        public DcfConnectionFilter(ConnectivityInterface itf, Interfaces.InterfaceType type = Interfaces.InterfaceType.Source, ConnectionType connectionType = ConnectionType.Both, Filters.DcfPropertyFilter propertyFilter = null)
        {
            this.propertyFilter = propertyFilter;
            this.type = connectionType;
            switch (type)
            {
                case Interfaces.InterfaceType.Source:
                    sourceInterface = itf;
                    break;
                case Interfaces.InterfaceType.Destination:
                    destinationInterface = itf;
                    break;
                default:
                    sourceInterface = itf;
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfConnectionFilter" /> class.
        /// </summary>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>  
        public DcfConnectionFilter(Interfaces.DcfInterfaceFilterSingle source, Interfaces.DcfInterfaceFilterSingle destination, ConnectionType connectionType = ConnectionType.Both, Filters.DcfPropertyFilter propertyFilter = null)
        {
            type = connectionType;
            this.propertyFilter = propertyFilter;
            sourceFilter = source;
            destinationFilter = destination;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfConnectionFilter" /> class.
        /// </summary>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>  
        public DcfConnectionFilter(ConnectivityInterface source, ConnectivityInterface destination, ConnectionType connectionType = ConnectionType.Both, Filters.DcfPropertyFilter propertyFilter = null)
        {
            type = connectionType;
            this.propertyFilter = propertyFilter;
            sourceInterface = source;
            destinationInterface = destination;
        }

        /// <summary>
        /// Gets or sets the Type property
        /// </summary>  
        public ConnectionType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the ConnectionID property
        /// </summary>  
        public int ConnectionID
        {
            get { return connectionID; }
            set { connectionID = value; }
        }

        /// <summary>
        /// Gets or sets the ConnectionName property
        /// </summary>  
        public string ConnectionName
        {
            get { return connectionName; }
            set { connectionName = value; }
        }

        /// <summary>
        /// Gets or sets the SourceFilter property
        /// </summary>  
        public Interfaces.DcfInterfaceFilterSingle SourceFilter
        {
            get { return sourceFilter; }
            set { sourceFilter = value; }
        }

        /// <summary>
        /// Gets or sets the DestinationFilter property
        /// </summary>  
        public Interfaces.DcfInterfaceFilterSingle DestinationFilter
        {
            get { return destinationFilter; }
            set { destinationFilter = value; }
        }

        /// <summary>
        /// Gets or sets the SourceInterface property
        /// </summary>  
        public ConnectivityInterface SourceInterface
        {
            get { return sourceInterface; }
            set { sourceInterface = value; }
        }

        /// <summary>
        /// Gets or sets the DestinationInterface property
        /// </summary>  
        public ConnectivityInterface DestinationInterface
        {
            get { return destinationInterface; }
            set { destinationInterface = value; }
        }

        /// <summary>
        /// Gets or sets the PropertyFilter property
        /// </summary>  
        public Filters.DcfPropertyFilter PropertyFilter
        {
            get { return propertyFilter; }
            set { propertyFilter = value; }
        }

        /// <summary>
        /// Gets or sets the ElementKey property
        /// </summary>  
        public string ElementKey
        {
            get { return elementKey; }
            set { elementKey = value; }
        }
    }
}
