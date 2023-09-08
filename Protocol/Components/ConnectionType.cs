using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Components { 
    /// <summary>
    /// Indicates if the connection is internal, external or both.
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public enum ConnectionType
    {
        Internal,
        External,
        Both
    }
}
