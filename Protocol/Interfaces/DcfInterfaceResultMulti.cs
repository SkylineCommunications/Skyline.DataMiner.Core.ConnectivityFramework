using Skyline.DataMiner.Scripting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Interfaces
{
    /// <summary>
    /// Contains The Result from a GetInterfaces Query to Multiple Interfaces. If a specific DcfInterfaceFilter was not found then this object will be null.
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfInterfaceResultMulti : DcfInterfaceResult
    {
        /// <summary>
        /// The dcfInterfaces field
        /// </summary>
        private ConnectivityInterface[] dcfInterfaces;

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfInterfaceResultMulti" /> class.
        /// </summary>
        /// <param name="link">The link parameter</param>
        /// <param name="allInterfaces">The allInterfaces parameter</param>  
        public DcfInterfaceResultMulti(DcfInterfaceFilter link, ConnectivityInterface[] allInterfaces)
        {
            Link = link;
            if (allInterfaces != null && allInterfaces.Length > 0)
            {
                dcfInterfaces = allInterfaces;
            }
            else
            {
                dcfInterfaces = null;
            }
        }

        /// <summary>
        /// Gets the DcfInterfaces property
        /// </summary>  
        public ConnectivityInterface[] DcfInterfaces
        {
            get { return dcfInterfaces; }
        }
    }
}
