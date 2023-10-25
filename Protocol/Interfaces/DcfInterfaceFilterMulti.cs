using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using DcfPropertyFilter = Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Filters.DcfPropertyFilter;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Interfaces
{
    /// <summary>
    /// Objects of this class represent multiple unique Interfaces, specified by it's ParameterGroupID. In case of a table, the Key of the table must also be specified. In case of external element, the elementKey (DmaID/EleID) must also be specified.
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfInterfaceFilterMulti : DcfInterfaceFilter
    {
        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterMulti class. It links to all interfaces on a specific element
        /// </summary>
        /// <param name="elementKey">The Element Key: dmaID/eleID</param>
        /// <param name="propertyFilter">Optional Filter for specific Properties on the interface</param>
        public DcfInterfaceFilterMulti(string elementKey, DcfPropertyFilter propertyFilter = null)
        {
            ParameterGroupID = -1;
            TableKey = null;
            ElementKey = elementKey;
            Custom = false;
            GetAll = true;
            PropertyFilter = propertyFilter;
            InterfaceName = null;
        }

        /// <summary>
        ///  Initializes a new instance of the DCFInterfaceFilterMulti class. It links to all interfaces on the local element.
        /// </summary>
        /// <param name="dmaID">The dmaID parameter</param>
        /// <param name="eleID">The eleID parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param> 
        public DcfInterfaceFilterMulti(int dmaID, int eleID, DcfPropertyFilter propertyFilter = null)
            : this(dmaID + "/" + eleID, propertyFilter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterMulti class. It links to all local interfaces
        /// </summary>
        /// <param name="propertyFilter">Optional filter for specific Properties on the interface</param> 
        public DcfInterfaceFilterMulti(DcfPropertyFilter propertyFilter = null)
            : this("local", propertyFilter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterMulti class. It links to all Interfaces for a specific parameterGroup on a specific Element
        /// </summary>
        /// <param name="parameterGroupID">The parameterGroupID parameter</param>
        /// <param name="elementKey">The elementKey parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>
        public DcfInterfaceFilterMulti(int parameterGroupID, string elementKey, DcfPropertyFilter propertyFilter = null)
        {
            ParameterGroupID = parameterGroupID;
            TableKey = "*";
            ElementKey = elementKey;
            Custom = false;
            GetAll = false;
            PropertyFilter = propertyFilter;
            InterfaceName = null;
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterMulti class. It links to all Interfaces for a specific parameterGroup on the local Element
        /// </summary>
        /// <param name="parameterGroupID">The parameterGroupID parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>  
        public DcfInterfaceFilterMulti(int parameterGroupID, DcfPropertyFilter propertyFilter = null)
            : this(parameterGroupID, "local", propertyFilter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DCFInterfaceFilterMulti class. It links to all Interfaces for a specific parameterGroup on a specific element
        /// </summary>
        /// <param name="parameterGroupID">The parameterGroupID parameter</param>
        /// <param name="dmaID">The dmaID parameter</param>
        /// <param name="eleID">The eleID parameter</param>
        /// <param name="propertyFilter">The propertyFilter parameter</param>  
        public DcfInterfaceFilterMulti(int parameterGroupID, int dmaID, int eleID, DcfPropertyFilter propertyFilter = null)
            : this(parameterGroupID, dmaID + "/" + eleID, propertyFilter)
        {
        }
    }
}
