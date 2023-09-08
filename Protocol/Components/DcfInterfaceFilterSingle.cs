using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Skyline.Protocol.Library.ProtocolDCF;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Components
{
    public class DcfInterfaceFilterSingle : DcfInterfaceFilter
    {
        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterSingle class. It links to a specific interface on a specific element
        /// </summary>
        /// <param name="interfaceName">Name of the Interface</param>
        /// <param name="elementKey">The Element Key: dmaID/eleID</param>
        /// <param name="customName">Optional indication to use the Custom Name or not (default:false)</param>
        /// <param name="propertyFilter">Optional filter for specific Properties on the interface</param>
        public DcfInterfaceFilterSingle(string interfaceName, string elementKey, bool customName = false, DcfPropertyFilter propertyFilter = null)
        {
            this.ParameterGroupID = -1;
            this.TableKey = null;
            this.ElementKey = elementKey;
            this.Custom = customName;
            this.GetAll = false;
            this.PropertyFilter = propertyFilter;
            this.InterfaceName = interfaceName;
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterSingle class. It links to a specific interface on the local element
        /// </summary>
        /// <param name="interfaceName">Name of the Interface</param>
        /// <param name="customName">Optional indication to use the Custom Name or Note (default:false)</param>
        /// <param name="propertyFilter">Optional filter for specific Properties on the interface</param>
        public DcfInterfaceFilterSingle(string interfaceName, bool customName = false, DcfPropertyFilter propertyFilter = null)
            : this(interfaceName, "local", customName, propertyFilter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterSingle class. It links to a specific interface on a specific element
        /// </summary>
        /// <param name="interfaceName">Name of the Interface</param>
        /// <param name="dmaID">DataMiner ID</param>
        /// <param name="eleID">Element ID</param>
        /// <param name="customName">Optional indication if the Custom Name should be used or not (default:false)</param>
        /// <param name="propertyFilter">Optional filter for specific Properties on the interface</param>
        public DcfInterfaceFilterSingle(string interfaceName, int dmaID, int eleID, bool customName = false, DcfPropertyFilter propertyFilter = null)
            : this(interfaceName, dmaID + "/" + eleID, customName, propertyFilter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterSingle class. It links to one Interface on a specific table
        /// </summary>
        /// <param name="parameterGroupID">ParameterGroup that creates the Interface</param>
        /// <param name="tableKey">Key of the row that creates an Interface</param>
        /// <param name="elementKey">DmaID/EleID of the element where the Interface is found</param>
        /// <param name="propertyFilter">An optional PropertyFilter</param>
        public DcfInterfaceFilterSingle(int parameterGroupID, string tableKey, string elementKey, DcfPropertyFilter propertyFilter = null)
        {
            this.ParameterGroupID = parameterGroupID;
            this.TableKey = tableKey;
            this.ElementKey = elementKey;
            this.Custom = false;
            this.GetAll = false;
            this.PropertyFilter = propertyFilter;
            this.InterfaceName = null;
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterSingle class. It links to a single interface selected by a propertyFilter. Note if multiple interfaces match, the first one will be selected.
        /// </summary>
        /// <param name="elementKey">The Element Key: dmaID/eleID</param>
        /// <param name="propertyFilter">Optional Filter for specific Properties on the interface</param>
        public DcfInterfaceFilterSingle(string elementKey, DcfPropertyFilter propertyFilter)
        {
            this.ParameterGroupID = -1;
            this.TableKey = null;
            this.ElementKey = elementKey;
            this.Custom = false;
            this.GetAll = true;
            this.PropertyFilter = propertyFilter;
            this.InterfaceName = null;
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterSingle class. It links to a single interface on a specific element. Filtered on a specific Property. Note if multiple interfaces match, the first one will be selected.
        /// </summary>
        /// <param name="propertyFilter">Optional filter for specific Properties on the interface</param>
        public DcfInterfaceFilterSingle(DcfPropertyFilter propertyFilter)
            : this("local", propertyFilter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterSingle class. It links to a single Interface on the DataMiner System
        /// </summary>
        /// <param name="parameterGroupID">ParameterGroup that creates the Interface</param>
        public DcfInterfaceFilterSingle(int parameterGroupID, DcfPropertyFilter propertyFilter = null)
            : this(parameterGroupID, null, "local", propertyFilter)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the DCFInterfaceFilterSingle class. It links to a single Interface on a specific element.
        /// </summary>
        /// <param name="parameterGroupID">The parameterGroupID parameter</param>
        /// <param name="dmaID">The dmaID parameter</param>
        /// <param name="eleID">The eleID parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param> >
        public DcfInterfaceFilterSingle(int parameterGroupID, int dmaID, int eleID, DcfPropertyFilter propertyFilter = null)
            : this(parameterGroupID, null, dmaID + "/" + eleID, propertyFilter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterSingle class. It links to a one interface on the local element.
        /// </summary>
        /// <param name="parameterGroupID">The parameterGroupID parameter</param>
        /// <param name="tableKey">The tableKey parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param> 
        public DcfInterfaceFilterSingle(int parameterGroupID, string tableKey, DcfPropertyFilter propertyFilter = null)
            : this(parameterGroupID, tableKey, "local", propertyFilter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterSingle class. It links to one interface on a specific element.
        /// </summary>
        /// <param name="parameterGroupID">The parameterGroupID parameter</param>
        /// <param name="tableKey">The tableKey parameter</param>
        /// <param name="dmaID">The dmaID parameter</param>
        /// <param name="eleID">The eleID parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>  
        public DcfInterfaceFilterSingle(int parameterGroupID, string tableKey, int dmaID, int eleID, DcfPropertyFilter propertyFilter = null)
            : this(parameterGroupID, tableKey, dmaID + "/" + eleID, propertyFilter)
        {
        }
    }
}
