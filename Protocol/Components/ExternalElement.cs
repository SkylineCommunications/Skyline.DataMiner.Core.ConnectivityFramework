using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Components
{
    public struct ExternalElement
    {
        /// <summary>
        /// The dmaId field
        /// </summary>
        private int dmaId;

        /// <summary>
        /// The timeOutTime field
        /// </summary>
        private int timeOutTime;

        /// <summary>
        /// The eleId field
        /// </summary>
        private int eleId;

        /// <summary>
        /// The elementKey field
        /// </summary>
        private string elementKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalElement" /> class.
        /// </summary>
        /// <param name="dmaID">The dmaID parameter</param>
        /// <param name="eleID">The eleID parameter</param>
        /// <param name="timeoutTime">The timeoutTime parameter</param>  
        public ExternalElement(int dmaID, int eleID, int timeoutTime)
        {
            dmaId = dmaID;
            eleId = eleID;
            elementKey = dmaID + "/" + eleID;
            timeOutTime = timeoutTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalElement" /> class.
        /// </summary>
        /// <param name="dmaID">The dmaID parameter</param>
        /// <param name="eleID">The eleID parameter</param>  
        public ExternalElement(int dmaID, int eleID)
            : this(dmaID, eleID, 20)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalElement" /> class.
        /// </summary>
        /// <param name="elementKey">The elementKey parameter</param>
        /// <param name="timeoutTime">The timeoutTime parameter</param>  
        public ExternalElement(string elementKey, int timeoutTime)
        {
            timeOutTime = timeoutTime;
            this.elementKey = elementKey;
            string[] elementKeyA = elementKey.Split('/');
            if (elementKeyA.Length > 1)
            {
                int.TryParse(elementKeyA[0], out dmaId);
                int.TryParse(elementKeyA[1], out eleId);
            }
            else
            {
                dmaId = -1;
                eleId = -1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalElement" /> class.
        /// </summary>
        /// <param name="elementKey">The elementKey parameter</param>  
        public ExternalElement(string elementKey)
            : this(elementKey, 20)
        {
        }

        /// <summary>
        /// Gets the DmaId property
        /// </summary>  
        public int DmaId
        {
            get { return dmaId; }
            private set { dmaId = value; }
        }

        /// <summary>
        /// Gets the EleId property
        /// </summary>  
        public int EleId
        {
            get { return eleId; }
            private set { eleId = value; }
        }

        /// <summary>
        /// Gets the TimeoutTime property
        /// </summary>  
        public int TimeoutTime
        {
            get { return timeOutTime; }
        }

        /// <summary>
        /// Gets the ElementKey property
        /// </summary>  
        public string ElementKey
        {
            get { return elementKey; }
        }
    }
}
