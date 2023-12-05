using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Options
{
    /// <summary>
    /// Indicates the DCFHelper is running in 'buffer' mode. It will buffer all changes until such a time the LastInBuffer is set to true or RemovalOptionsAuto is used.
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfRemovalOptionsBuffer : DcfRemovalOptions
    {
        /// <summary>
        /// The lastInBuffer field
        /// </summary>
        private bool lastInBuffer = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfRemovalOptionsBuffer" /> class.
        /// </summary>
        /// <param name="lastInBuffer">The lastInBuffer parameter</param>  
        public DcfRemovalOptionsBuffer(bool lastInBuffer)
        {
            LastInBuffer = lastInBuffer;
        }

        /// <summary>
        /// Gets or sets the LastInBuffer property
        /// </summary>  
        public bool LastInBuffer
        {
            get
            {
                return lastInBuffer;
            }

            set
            {
                if (value)
                {
                    HelperType = SyncOption.EndOfPolling;
                }
                else
                {
                    HelperType = SyncOption.PollingSync;
                }

                lastInBuffer = value;
            }
        }
    }
}
