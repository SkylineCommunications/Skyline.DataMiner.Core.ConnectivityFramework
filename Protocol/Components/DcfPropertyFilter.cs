using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Components
{
    /// <summary>
    /// A PropertyFilter used when performing a GetConnections to further filter down the retrieved connections based on their properties
    /// </summary>
    //[DISCodeLibrary(Version = 1)]
    public class DcfPropertyFilter
    {
        /// <summary>
        /// The name field
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The type field
        /// </summary>
        private string type = string.Empty;

        /// <summary>
        /// The value field
        /// </summary>
        private string value = string.Empty;

        /// <summary>
        /// The id field
        /// </summary>
        private int id = -1;

        /// <summary>
        /// The and field
        /// </summary>
        private List<DcfPropertyFilter> and = new List<DcfPropertyFilter>();

        /// <summary>
        /// The or field
        /// </summary>
        private List<DcfPropertyFilter> or = new List<DcfPropertyFilter>();
        // private bool not = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfPropertyFilter" /> class.
        /// </summary>
        /// <param name="id">The id parameter</param>  
        public DcfPropertyFilter(int id)
        {
            this.ID = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfPropertyFilter" /> class.
        /// </summary>
        /// <param name="name">The name parameter</param>
        /// <param name="type">The type parameter</param>
        /// <param name="value">The value parameter</param>  
        public DcfPropertyFilter(string name = "", string value = "", string type = "")
        {
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfPropertyFilter" /> class.
        /// </summary>
        /// <param name="name">The name parameter</param>
        /// <param name="value">The value parameter</param>  
        public DcfPropertyFilter(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DcfPropertyFilter" /> class.
        /// </summary>
        /// <param name="name">The name parameter</param>  
        public DcfPropertyFilter(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the Name property
        /// </summary>  
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the Type property
        /// </summary>  
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the Value property
        /// </summary>  
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets or sets the ID property
        /// </summary>  
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        // public bool NOT
        // {
        //    get { return not; }
        //    set { not = value; }
        // }
        // public List<PropertyFilter> AND
        // {
        //    get { return and; }
        //    set { and = value; }
        // }
        // public List<PropertyFilter> OR
        // {
        //    get { return or; }
        //    set { or = value; }
        // }
    }
}
