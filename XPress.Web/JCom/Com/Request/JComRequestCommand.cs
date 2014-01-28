using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web.JCom.Com;
using XPress.Web.JavascriptClient;
using XPress.Serialization.Attributes;
using XPress.Web.JCom.Map;
using System.Reflection;
using XPress.Serialization;
using XPress.Serialization.Documents;

namespace XPress.Web.JCom.Com.Request
{
    /// <summary>
    /// Implements a service jcom command.
    /// </summary>
    public class JComRequestCommand :Core.XPressRequestCommand
    {
        internal JComRequestCommand()
            : base()
        {
        }

        #region members

        /// <summary>
        /// The JCom client associated with the command.
        /// </summary>
        [XPressIgnore]
        public JComClient JClient { get; internal set; }

        /// <summary>
        /// The object id associated with the specific object.
        /// </summary>
        public uint OId { get; set; }

        /// <summary>
        /// The associated member name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The method arguments.
        /// </summary>
        [XPress.Serialization.Attributes.XPressIgnore]
        public object[] Arguments { get; private set; }

        /// <summary>
        /// The arguments associated with the method. (can be null).
        /// </summary>
        [XPressMember]
        JsonArray<string> _args = null;

        #endregion

        #region methods

        /// <summary>
        /// Executes the command from jcom.
        /// </summary>
        public override void ExecuteCommand()
        {
            // get the object associated with the id.
            object o = JClient.ObjectSource.GetObject(OId);

            if (o == null)
            {
                // nothing to do. Object not found.
                return;
            }

            JComTypeInfo info = JComTypeInfo.Get(o.GetType());

            this.ResponseValue = null;
            if (info.Members.ContainsKey(Name))
            {
                JComMemberInfo jmi = info.Members[Name];

                // determine the command type for the current.
                JComCommandType cmndType = jmi.IsDataMember ? (_args.Count > 0 ? JComCommandType.Set : JComCommandType.Get) : JComCommandType.Invoke;
                switch (cmndType)
                {
                    case JComCommandType.Get:
                        // returns the value and updates the client side to the data.
                        if (jmi.IsDataMember && jmi.CanRead)
                        {
                            this.ResponseValue = jmi.IsProperty ? (jmi.MappedMember as PropertyInfo).GetValue(o) : (jmi.MappedMember as FieldInfo).GetValue(o);
                        }
                        break;
                    case JComCommandType.Invoke:
                        // invoke a method on the serverside.
                        if (!jmi.IsDataMember)
                        {
                            MethodInfo mi = jmi.MappedMember as MethodInfo;
                            object val = mi.Invoke(o, ParseMethodArguments(mi, _args));
                            this.ResponseValue = mi.ReturnType == typeof(void) ? null : val;
                        }
                        break;
                    case JComCommandType.Set:
                        // calls to set a value from the client side.
                        if (jmi.IsDataMember && jmi.CanWrite)
                        {
                            if (jmi.IsProperty)
                            {
                                PropertyInfo pi = jmi.MappedMember as PropertyInfo;
                                object val = _args[0].FromJSJson(pi.PropertyType);
                                pi.SetValue(o, val, new object[0]);
                            }
                            else
                            {
                                FieldInfo fi = jmi.MappedMember as FieldInfo;
                                object val = _args[0].FromJSJson(fi.FieldType);
                                fi.SetValue(0, val);
                            }
                        }
                        break;
                }
            }
        }

        #endregion

        #region parsing of helper values.

        /// <summary>
        /// The method agruments to parse.
        /// </summary>
        /// <param name="info"></param>
        object[] ParseMethodArguments(MethodInfo mi, JsonArray<string> args)
        {
            if (args == null)
                return new object[0];

            ParameterInfo[] prs = mi.GetParameters();
            int index = 0;
            List<object> rt = new List<object>();
            foreach (IJsonValue<string> val in _args)
            {
                if (prs.Length >= index)
                    break;
                rt.Add(val.FromJSJson(prs[index].ParameterType));
                index++;
            }

            return rt.ToArray();
        }

        #endregion
    }
}
