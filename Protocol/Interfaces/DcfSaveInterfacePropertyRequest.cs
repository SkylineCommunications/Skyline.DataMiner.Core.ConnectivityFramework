using Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Connections;
using Skyline.DataMiner.Scripting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Interfaces
{
    /// <summary>
    /// A request object for performing a SaveConnectionProperties call
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfSaveInterfacePropertyRequest
    {
        /// <summary>
        /// The name field
        /// </summary>
        private string name;

        /// <summary>
        /// The type field
        /// </summary>
        private string type;

        /// <summary>
        /// The value field
        /// </summary>
        private string value;

        /// <summary>
        /// The fixedProperty field
        /// </summary>
        private bool fixedProperty;

        /// <summary>
        /// The async field
        /// </summary>
        private bool async;

		/// <summary>
		/// Initializes a new instance of the <see cref="DcfSaveConnectionPropertyRequest" /> class.
		/// </summary>
		/// <param name="name">The name parameter</param>
		/// <param name="type">The type parameter</param>
		/// <param name="value">The value parameter</param>
		/// <param name="fixedProperty">The fixedProperty parameter</param>
		/// <param name="asynchronous">The asynchronous parameter</param>  
		public DcfSaveInterfacePropertyRequest(string name, string type, string value, bool fixedProperty = false, bool asynchronous = true)
        {
            this.name = name;
            this.type = type;
            this.value = value;
            this.fixedProperty = fixedProperty;
            this.async = asynchronous;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="DcfSaveConnectionPropertyRequest" /> class.
		/// </summary>
		/// <param name="property">The property parameter</param>
		/// <param name="fixedProperty">The fixedProperty parameter</param>
		/// <param name="asynchronous">The asynchronous parameter</param>  
		public DcfSaveInterfacePropertyRequest(ConnectivityConnectionProperty property, bool fixedProperty = false, bool asynchronous = true)
            : this(property.ConnectionPropertyName, property.ConnectionPropertyType, property.ConnectionPropertyValue, fixedProperty, asynchronous)
        {
        }

        /// <summary>
        /// Gets the Name property
        /// </summary>  
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the Type property
        /// </summary>  
        public string Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the Value property
        /// </summary>  
        public string Value
        {
            get { return value; }
        }

        // private bool full;
        // public bool Full
        // {
        //    get { return full; }
        // }

        /// <summary>
        /// Gets the FixedProperty property
        /// </summary>  
        public bool FixedProperty
        {
            get { return fixedProperty; }
        }

        // private bool onBothConnections;
        // public bool OnBothConnections
        // {
        //    get { return onBothConnections; }
        // }

        /// <summary>
        /// Gets the Async property
        /// </summary>  
        public bool Async
        {
            get { return async; }
        }
    }
}
