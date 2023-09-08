using Skyline.DataMiner.Scripting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Interfaces
{
    public class DcfInterfaceResultSingle : DcfInterfaceResult
    {
        /// <summary>
        /// The dcfInterface field
        /// </summary>
        private ConnectivityInterface dcfInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfInterfaceResultSingle" /> class.
        /// </summary>
        /// <param name="link">The link parameter</param>
        /// <param name="allInterfaces">The allInterfaces parameter</param>        
        public DcfInterfaceResultSingle(DcfInterfaceFilter link, ConnectivityInterface[] allInterfaces)
        {
            Link = link;
            if (allInterfaces != null && allInterfaces.Length > 0)
            {
                dcfInterface = allInterfaces[0];
            }
            else
            {
                dcfInterface = null;
            }
        }

        /// <summary>
        /// Gets the DCFInterface property
        /// </summary>  
        public ConnectivityInterface DCFInterface
        {
            get { return dcfInterface; }
        }
    }
}
