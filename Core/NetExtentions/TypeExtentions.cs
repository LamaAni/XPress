using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.NetExtentions
{
    public static class TypeExtentions
    {
        /// <summary>
        /// Returns all the attributes assigned to a types interfaces.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        static IEnumerable<Tuple<Type, T>> GetCustomAttributesFromInterfaces<T>(this Type t, Func<Type, bool> typePerdict = null)
            where T : Attribute
        {
            return t.GetInterfaces().Where(i => typePerdict == null || typePerdict(i)).SelectMany(i => i.GetCustomAttributesWithBases<T>(typePerdict));
        }

        /// <summary>
        /// Retirns all the custom attributes for the type and the base types of the specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static IEnumerable<Tuple<Type, T>> GetCustomAttributesWithBases<T>(this Type t, Func<Type, bool> typePerdict = null) 
            where T : Attribute
        {
            IEnumerable<Tuple<Type, T>> attribs = typePerdict == null || typePerdict(t) ?
                t.GetCustomAttributes(typeof(T), false).Select(atr => new Tuple<Type, T>(t, atr as T))//Attribute.GetCustomAttributes(t, typeof(T)).Cast<T>() 
                : new Tuple<Type, T>[0];

            return t.BaseType == typeof(object) || t.BaseType == null ? attribs :
                t.BaseType.GetCustomAttributesWithBases<T>(typePerdict).Concat(attribs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="typePerdict"></param>
        /// <returns>A collection of the attributes and thire declaring types.</returns>
        public static IEnumerable<Tuple<Type, T>> GetAllCustomAttributesWithDeclaringTypes<T>(this Type t, Func<Type, bool> typePerdict = null) where T : Attribute
        {
            return t.GetCustomAttributesWithBases<T>(typePerdict)
                .Concat(t.GetCustomAttributesFromInterfaces<T>(typePerdict)).ToArray();
        }

        /// <summary>
        /// Returns all the custom attributes from interfaces or base types.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to serch for</typeparam>
        /// <param name="t">The type</param>
        /// <param name="typePerdict">Perdict which types are allowed.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllCustomAttributes<T>(this Type t, Func<Type, bool> typePerdict = null) where T : Attribute
        {
            return t.GetAllCustomAttributesWithDeclaringTypes<T>(typePerdict).Select(tpl => tpl.Item2).ToArray();
        }
    }
}
