using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Documents
{
    /// <summary>
    /// Represents a general json list value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JsonEnumrableObject<T, ListType> : List<ListType>, IJsonValue<T>, IJsonEnumrableObject<T>
        where ListType : IJsonValue<T>
    {
        public JsonEnumrableObject()
        {
        }

        /// <summary>
        /// Returns true of the collection has directives.
        /// </summary>
        public bool HasDirectives
        {
            get
            {
                return _directiveList != null && _directiveList.Count > 0 || m_directives != null && m_directives.Count > 0;
            }
        }

        List<JsonDirective<T>> m_directives;

        /// <summary>
        /// A collection of directives that apply to this object.
        /// </summary>
        public List<JsonDirective<T>> Directives
        {
            get
            {
                if (m_directives == null)
                {
                    m_directives = new List<JsonDirective<T>>();
                    if (_directiveList != null)
                    {
                        for (int i = 0; i < _directiveList.Count; i++)
                        {
                            JsonDirective<T> directive = ((JsonData<T>)_directiveList[i]).Value as JsonDirective<T>;
                            if (directive != null)
                                m_directives.Add(directive);
                        }
                        _directiveList = null;
                    }
                }
                return m_directives;
            }
        }

        List<IJsonValue<T>> _directiveList;

        /// <summary>
        /// Adds a directive to the directives collection. Directive values will be loaded from the raw directive collection 
        /// once needed.
        /// </summary>
        /// <param name="addedData">The added Json object to the directive. Applies only to objects.</param>
        /// <param name="directive">The directive command.</param>
        protected void AddDirective(JsonData<T> directive)
        {
            if (m_directives != null)
            {
                m_directives.Add(directive.Value as JsonDirective<T>);
            }
            else
            {
                if (_directiveList == null)
                    _directiveList = new List<IJsonValue<T>>();
                _directiveList.Add(directive);
            }
        }

        /// <summary>
        /// Adds a directive to the collection.
        /// </summary>
        /// <param name="directive">The json directive to add.</param>
        public virtual void AddDirective(JsonDirective<T> directive)
        {
            Directives.Add(directive);
        }

        /// <summary>
        /// Finds the directive that matches T.
        /// </summary>
        /// <param name="directive"></param>
        /// <returns></returns>
        public virtual JsonDirective<T> FindDirective(T marker)
        {
            return Directives.FirstOrDefault(d => d.Equals(marker));
        }
    }

    public interface IJsonEnumrableObject<T>
    {
        void AddDirective(JsonDirective<T> directive);
        bool HasDirectives { get; }

        JsonDirective<T> FindDirective(T marker);
    }
}
