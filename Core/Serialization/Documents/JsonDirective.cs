using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Documents
{
    /// <summary>
    /// Represents a json directive value that may have a specific directive and then value in the same line.
    /// The value is always string.
    /// Directive is combined of the following. #[directiveId]#[string value]
    /// </summary>
    public class JsonDirective<T> : IJsonValue<T>, IEquatable<T>
    {
        public JsonDirective(T directive, object data)
        {
            Directive = directive;
            Data = data;
        }

        /// <summary>
        /// The directive
        /// </summary>
        public T Directive { get; private set; }

        /// <summary>
        /// The data assoicated with the directive.
        /// </summary>
        public object Data { get; private set; }

        /// <summary>
        /// Retruns the object data.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Data == null ? Directive.ToString() : Directive + "#" + Data;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;
            JsonDirective<T> other = (JsonDirective<T>)obj;
            return other.Directive.Equals(this.Directive);
        }

        #region IEquatable<T> Members

        public bool Equals(T other)
        {
            return Directive.Equals(other);
        }

        #endregion
    }
}
