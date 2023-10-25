using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Columns
{
    /// <summary>
    /// Represents a DVE column.
    /// </summary>
    public struct DveColumn
    {
        /// <summary>
        /// The tablePID field
        /// </summary>
        private int tablePID;

        /// <summary>
        /// The columnIDX field
        /// </summary>
        private int columnIDX;

        /// <summary>
        /// The timeoutTime field
        /// </summary>
        private int timeoutTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="DveColumn" /> class.
        /// </summary>
        /// <param name="tablePID">The tablePID parameter</param>
        /// <param name="columnIDX">The columnIDX parameter</param>  
        public DveColumn(int tablePID, int columnIDX)
            : this(tablePID, columnIDX, 20)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DveColumn" /> class.
        /// </summary>
        /// <param name="tablePID">The tablePID parameter</param>
        /// <param name="columnIDX">The columnIDX parameter</param>
        /// <param name="timeoutTime">The timeoutTime parameter</param>  
        public DveColumn(int tablePID, int columnIDX, int timeoutTime)
        {
            this.tablePID = tablePID;
            this.columnIDX = columnIDX;
            this.timeoutTime = timeoutTime;
        }

        /// <summary>
        /// Gets the TablePID property
        /// </summary>  
        public int TablePID
        {
            get { return tablePID; }
        }

        /// <summary>
        /// Gets the ColumnIDX property
        /// </summary>  
        public int ColumnIDX
        {
            get { return columnIDX; }
        }

        /// <summary>
        /// Gets the TimeoutTime property
        /// </summary>  
        public int TimeoutTime
        {
            get { return timeoutTime; }
        }
    }
}
