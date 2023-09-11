using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace Skyline.DataMiner.Core.ConnectivityFramework.Protocol.Collections
{
    /// <summary>
    /// A table like collection where multiple indexed values can be selected
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[DISCodeLibrary(Version = 1)]
    public class FastCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// The _items field
        /// </summary>        
        private IList<T> _items;

        /// <summary>
        /// The _lookups field
        /// </summary>        
        private IList<Expression<Func<T, object>>> _lookups;

        /// <summary>
        /// The _indexes field
        /// </summary>        
        private Dictionary<string, ILookup<object, T>> _indexes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastCollection" /> class.
        /// </summary>
        /// <param name="data">The data parameter</param>        
        public FastCollection(IList<T> data)
        {
            _items = data;
            _lookups = new List<Expression<Func<T, object>>>();
            _indexes = new Dictionary<string, ILookup<object, T>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastCollection" /> class.
        /// </summary>        
        public FastCollection()
        {
            _lookups = new List<Expression<Func<T, object>>>();
            _indexes = new Dictionary<string, ILookup<object, T>>();
        }

        /// <summary>
        /// The AddIndex method
        /// </summary>
        /// <param name="property">The property parameter</param>        
        public void AddIndex(Expression<Func<T, object>> property)
        {
            if (!_indexes.ContainsKey(property.ToString()))
            {
                _lookups.Add(property);
                _indexes.Add(property.ToString(), _items.ToLookup(property.Compile()));
            }
        }

        /// <summary>
        /// The Add method
        /// </summary>
        /// <param name="item">The item parameter</param>        
        public void Add(T item)
        {
            if (_items == null)
            {
                _items = new List<T>();
                _items.Add(item);
            }
            else
            {
                _items.Add(item);
            }

            RebuildIndexes();
        }

        /// <summary>
        /// The Add method
        /// </summary>
        /// <param name="data">The data parameter</param>
        /// <param name="comparer">The comparer parameter</param>        
        public void Add(IList<T> data, IEqualityComparer<T> comparer)
        {
            if (_items == null)
            {
                _items = data;
            }
            else
            {
                _items = data.Union(_items, comparer).ToList();
            }

            RebuildIndexes();
        }

        /// <summary>
        /// The Remove method
        /// </summary>
        /// <param name="item">The item parameter</param>        
        public void Remove(T item)
        {
            _items.Remove(item);
            RebuildIndexes();
        }

        /// <summary>
        /// The RebuildIndexes method
        /// </summary>        
        public void RebuildIndexes()
        {
            if (_lookups.Count > 0)
            {
                _indexes = new Dictionary<string, ILookup<object, T>>();
                foreach (var lookup in _lookups)
                {
                    _indexes.Add(lookup.ToString(), _items.ToLookup(lookup.Compile()));
                }
            }
        }

        /// <summary>
        /// The FindValue method
        /// </summary>
        /// <param name="property">The property parameter</param>
        /// <param name="value">The value parameter</param>
        /// <returns>The System.Collections.Generic.IEnumerable T type object</returns>        
        public IEnumerable<T> FindValue<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            var key = property.ToString();
            if (_indexes.ContainsKey(key))
            {
                return _indexes[key][value];
            }
            else
            {
                var c = property.Compile();
                return _items.Where(x => c(x).Equals(value));
            }
        }

        /// <summary>
        /// The GetEnumerator method
        /// </summary>
        /// <returns>The System.Collections.Generic.IEnumerator T type object</returns>        
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// The System.Collections.IEnumerable.GetEnumerator method
        /// </summary>
        /// <returns>The System.Collections.IEnumerator type object</returns>        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
