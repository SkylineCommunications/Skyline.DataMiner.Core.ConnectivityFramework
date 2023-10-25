using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Interfaces
{
    /// <summary>
    /// Represents a DCF interface result.
    /// </summary>
    public class DcfInterfaceResult
    {
        /// <summary>
        /// The link field
        /// </summary>
        private DcfInterfaceFilter link;

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfInterfaceResult" /> class.
        /// </summary>  
        public DcfInterfaceResult()
        {
            link = null;
        }

        /// <summary>
        /// Gets the Link property
        /// </summary>  
        public DcfInterfaceFilter Link
        {
            get { return link; }
            protected set { link = value; }
        }
    }
}
