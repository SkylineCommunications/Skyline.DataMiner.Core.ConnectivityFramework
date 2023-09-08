using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Helpers;
using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Interfaces;
using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Skyline.Protocol.Library.ProtocolDCF;
using Skyline.DataMiner.Scripting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections
{
    /// <summary>
    /// A request object for performing SaveConnections
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfSaveConnectionRequest
    {
        /// <summary>
        /// The source field
        /// </summary>
        private ConnectivityInterface source;

        /// <summary>
        /// The destination field
        /// </summary>
        private ConnectivityInterface destination;

        /// <summary>
        /// The connectionType field
        /// </summary>
        private SaveConnectionType connectionType = SaveConnectionType.Unique_Name;

        /// <summary>
        /// The customName field
        /// </summary>
        private string customName;

        /// <summary>
        /// The customFilter field
        /// </summary>
        private string customFilter = string.Empty;

        /// <summary>
        /// The fixedConnection field
        /// </summary>
        private bool fixedConnection;

        /// <summary>
        /// The createExternalReturn field
        /// </summary>
        private bool createExternalReturn = true;

        /// <summary>
        /// The async field
        /// </summary>
        private bool async;

        /// <summary>
        /// The propertyRequests field
        /// </summary>
        private List<DcfSaveConnectionPropertyRequest> propertyRequests = new List<DcfSaveConnectionPropertyRequest>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>  
        public DcfSaveConnectionRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>        
        public DcfSaveConnectionRequest(ConnectivityInterface source, ConnectivityInterface destination, bool fixedConnection = false, bool async = false)
        {
            this.fixedConnection = fixedConnection;
            this.source = source;
            this.destination = destination;
            this.async = async;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>  
        public DcfSaveConnectionRequest(ConnectivityInterface source, ConnectivityInterface destination, SaveConnectionType connectionType, bool fixedConnection = false, bool async = false)
            : this(source, destination, fixedConnection, async)
        {
            this.connectionType = connectionType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="customName">The customName parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>        
        public DcfSaveConnectionRequest(ConnectivityInterface source, ConnectivityInterface destination, string customName, bool fixedConnection = false, bool async = false)
            : this(source, destination, fixedConnection, async)
        {
            this.customName = customName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="customName">The customName parameter</param>
        /// <param name="connectionFilter">The connectionFilter parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>  
        public DcfSaveConnectionRequest(ConnectivityInterface source, ConnectivityInterface destination, string customName, string connectionFilter, bool fixedConnection = false, bool async = false)
            : this(source, destination, fixedConnection, async)
        {
            this.customName = customName;
            customFilter = connectionFilter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="customName">The customName parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>  
        public DcfSaveConnectionRequest(ConnectivityInterface source, ConnectivityInterface destination, SaveConnectionType connectionType, string customName, bool fixedConnection = false, bool async = false)
            : this(source, destination, connectionType, fixedConnection, async)
        {
            this.customName = customName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="customName">The customName parameter</param>
        /// <param name="connectionFilter">The connectionFilter parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>  
        public DcfSaveConnectionRequest(ConnectivityInterface source, ConnectivityInterface destination, SaveConnectionType connectionType, string customName, string connectionFilter, bool fixedConnection = false, bool async = false)
            : this(source, destination, connectionType, fixedConnection, async)
        {
            this.customName = customName;
            customFilter = connectionFilter;
        }

        // Allow direct usage of DcfInterfaceFilter

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="dcf">The dcf parameter</param>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>  
        public DcfSaveConnectionRequest(Helpers.DcfHelper dcf, Interfaces.DcfInterfaceFilterSingle source, Interfaces.DcfInterfaceFilterSingle destination, bool fixedConnection = false, bool async = false)
        {
            var result = dcf.GetInterfaces(source, destination);
            this.source = null;
            this.destination = null;
            if (result[0] is Interfaces.DcfInterfaceResultSingle)
            {
                Interfaces.DcfInterfaceResultSingle singleResult = result[0] as Interfaces.DcfInterfaceResultSingle;
                if (singleResult != null)
                {
                    this.source = singleResult.DCFInterface;
                }
            }
            if (result[1] is Interfaces.DcfInterfaceResultSingle)
            {
                Interfaces.DcfInterfaceResultSingle singleResult = result[1] as Interfaces.DcfInterfaceResultSingle;
                if (singleResult != null)
                {
                    this.destination = singleResult.DCFInterface;
                }
            }
            this.fixedConnection = fixedConnection;
            this.async = async;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="dcf">The dcf parameter</param>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>  
        public DcfSaveConnectionRequest(Helpers.DcfHelper dcf, Interfaces.DcfInterfaceFilterSingle source, Interfaces.DcfInterfaceFilterSingle destination, SaveConnectionType connectionType, bool fixedConnection = false, bool async = false)
            : this(dcf, source, destination, fixedConnection, async)
        {
            this.connectionType = connectionType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="dcf">The dcf parameter</param>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="customName">The customName parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>  
        public DcfSaveConnectionRequest(Helpers.DcfHelper dcf, Interfaces.DcfInterfaceFilterSingle source, Interfaces.DcfInterfaceFilterSingle destination, SaveConnectionType connectionType, string customName, bool fixedConnection = false, bool async = false)
            : this(dcf, source, destination, connectionType, fixedConnection, async)
        {
            this.customName = customName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="dcf">The dcf parameter</param>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="customName">The customName parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>  
        public DcfSaveConnectionRequest(Helpers.DcfHelper dcf, Interfaces.DcfInterfaceFilterSingle source, Interfaces.DcfInterfaceFilterSingle destination, string customName, bool fixedConnection = false, bool async = false)
            : this(dcf, source, destination, fixedConnection, async)
        {
            this.customName = customName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="dcf">The dcf parameter</param>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="customName">The customName parameter</param>
        /// <param name="connectionFilter">The connectionFilter parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>  
        public DcfSaveConnectionRequest(Helpers.DcfHelper dcf, Interfaces.DcfInterfaceFilterSingle source, Interfaces.DcfInterfaceFilterSingle destination, string customName, string connectionFilter, bool fixedConnection = false, bool async = false)
            : this(dcf, source, destination, fixedConnection, async)
        {
            this.customName = customName;
            customFilter = connectionFilter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfSaveConnectionRequest" /> class.
        /// </summary>
        /// <param name="dcf">The dcf parameter</param>
        /// <param name="source">The source parameter</param>
        /// <param name="destination">The destination parameter</param>
        /// <param name="connectionType">The connectionType parameter</param>
        /// <param name="customName">The customName parameter</param>
        /// <param name="connectionFilter">The connectionFilter parameter</param>
        /// <param name="fixedConnection">The fixedConnection parameter</param>
        /// <param name="async">The async parameter</param>  
        public DcfSaveConnectionRequest(Helpers.DcfHelper dcf, Interfaces.DcfInterfaceFilterSingle source, Interfaces.DcfInterfaceFilterSingle destination, SaveConnectionType connectionType, string customName, string connectionFilter, bool fixedConnection = false, bool async = false)
            : this(dcf, source, destination, connectionType, fixedConnection, async)
        {
            this.customName = customName;
            customFilter = connectionFilter;
        }

        /// <summary>
        /// Gets or sets the Source property
        /// </summary>  
        public ConnectivityInterface Source
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// Gets or sets the Destination property
        /// </summary>  
        public ConnectivityInterface Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        /// <summary>
        /// Gets or sets the ConnectionType property
        /// </summary>  
        public SaveConnectionType ConnectionType
        {
            get { return connectionType; }
            set { connectionType = value; }
        }

        /// <summary>
        /// Gets or sets the CustomName property
        /// </summary>  
        public string CustomName
        {
            get { return customName; }
            set { customName = value; }
        }

        /// <summary>
        /// Gets or sets the ConnectionFilter property
        /// </summary>  
        public string ConnectionFilter
        {
            get { return customFilter; }
            set { customFilter = value; }
        }

        /// <summary>
        /// Gets or sets the CustomFilter property
        /// </summary>  
        public string CustomFilter
        {
            get { return customFilter; }
            set { customFilter = value; }
        }

        /// <summary>
        /// Gets or sets the FixedConnection property
        /// </summary>  
        public bool FixedConnection
        {
            get { return fixedConnection; }
            set { fixedConnection = value; }
        }

        /// <summary>
        /// Gets or sets the CreateExternalReturn property
        /// </summary>  
        public bool CreateExternalReturn
        {
            get { return createExternalReturn; }
            set { createExternalReturn = value; }
        }

        /// <summary>
        /// Gets or sets the Async property
        /// </summary>  
        public bool Async
        {
            get { return async; }
            set { async = value; }
        }

        /// <summary>
        /// Gets or sets the PropertyRequests property
        /// </summary>  
        public List<DcfSaveConnectionPropertyRequest> PropertyRequests
        {
            get { return propertyRequests; }
            set { propertyRequests = value; }
        }

        /// <summary>
        /// The AddPropertyRequest method
        /// </summary>
        /// <param name="requests">The requests parameter</param>
        /// <returns>The bool type object</returns>  
        public bool AddPropertyRequest(params DcfSaveConnectionPropertyRequest[] requests)
        {
            bool success = false;
            propertyRequests.AddRange(requests);
            return success;
        }
    }
}
