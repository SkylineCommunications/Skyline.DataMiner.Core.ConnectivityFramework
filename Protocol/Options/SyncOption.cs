using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
    public enum SyncOption
    {
        /// <summary>
        /// This will update the currentMapping every time a remove or add is performed.
        /// </summary>
        Custom,

        /// <summary>
        /// Can be used when you received part of the whole DCF structure from a device and wish to keep track of it until the end of a buffer.
        /// </summary>
        PollingSync,

        /// <summary>
        ///  Can be used when you have received and parsed all the data from a device and wish to automatically remove data you didn't  receiving during the refresh.
        /// </summary>
        EndOfPolling
    }
}
