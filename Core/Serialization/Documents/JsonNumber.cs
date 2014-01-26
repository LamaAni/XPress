using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Documents
{
    /// <summary>
    /// The number as a mutable string object.
    /// </summary>
    public class JsonNumber<T>
    {
        /// <summary>
        /// A general number notation that can be converted into any number.
        /// </summary>
        /// <param name="convertor">A function that converts according to the type. Must allow for double.</param>
        /// <param name="data"></param>
        public JsonNumber(Func<Type, T, object> convertor, T data)
        {
            Convertor = convertor;
            RawData = data;
        }

        public T RawData { get; private set; }

        protected Func<Type, T, object> Convertor { get; private set; }

        /// <summary>
        /// Returns the value as a type.
        /// </summary>
        /// <typeparam name="NType">The type to return as.</typeparam>
        /// <returns></returns>
        public NType As<NType>()
        {
            try
            {
                return (NType) As(typeof(NType));
            }
            catch (Exception ex)
            {
                throw new Exception("Return value from converter when asked for type " + typeof(NType), ex);
            }
        }
        
        /// <summary>
        /// Converts the value to the object of type T.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public object As(Type t)
        {
            return Convertor(t, RawData);
        }

        /// <summary>
        /// The default as double.
        /// </summary>
        public double Value
        {
            get { return As<double>(); }
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
