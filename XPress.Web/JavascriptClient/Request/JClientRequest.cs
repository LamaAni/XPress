using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Web;
using XPress.Serialization;
using XPress.Serialization.Attributes;
using System.ComponentModel;
using System.Collections.Concurrent;
using XPress.Serialization.Documents;

namespace XPress.Web.JavascriptClient.Request
{
    public class JClientRequest
    {
        #region static members

        static JClientRequest()
        {
            Translators = new ConcurrentDictionary<string, Func<JsonObject<string>, JClient, Core.XPressRequestCommand>>();
        }

        static ConcurrentDictionary<string, Func<JsonObject<string>, JClient, Core.XPressRequestCommand>> Translators { get; set; }

        /// <summary>
        ///  Adds a new translator.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="f"></param>
        public static void AddTranslator(string type, Func<JsonObject<string>, JClient, Core.XPressRequestCommand> f)
        {
            Translators[type.ToLower()] = f;
        }

        public static void RemoveTranslator(string type)
        {
            if (Translators.ContainsKey(type))
                Translators.TryRemove(type);
        }

        public static bool ContainsTranslator(string type)
        {
            return Translators.ContainsKey(type);
        }

        #endregion

        #region members

        /// <summary>
        /// The sequential id of the request.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// If true the current request is a buffered request.
        /// </summary>
        [XPressMember("isBuffer", IgnoreMode = XPressIgnoreMode.IfDefualt)]
        [DefaultValue(false)]
        public bool IsBufferedRequest { get; private set; }

        /// <summary>
        /// The random id of the request.
        /// </summary>
        public int RandId { get; private set; }

        /// <summary>
        /// The date and time on the client the command was sent.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Holds information about commands implementation.
        /// </summary>
        [XPress.Serialization.Attributes.XPressMember(Name = "Commands")]
        protected JsonArray<string> CommandsSourceArray { get; private set; }

        /// <summary>
        /// Window unload commands.
        /// </summary>
        List<Core.XPressRequestCommand> m_Commands = null;

        [XPress.Serialization.Attributes.XPressIgnore]
        public List<Core.XPressRequestCommand> Commands
        {
            get
            {
                if (m_Commands == null)
                {
                    ReadCommandsFromJsonValue(JClientCallContext.Current.Client);
                }
                return m_Commands;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Reads the commands collection from the bason value of the specific command.
        /// </summary>
        public void ReadCommandsFromJsonValue(JClient client)
        {
            m_Commands = new List<Core.XPressRequestCommand>();
            if (CommandsSourceArray != null)
            {
                m_Commands.AddRange(CommandsSourceArray.Select<IJsonValue<string>, Core.XPressRequestCommand>(dat =>
                {
                    JsonObject<string> co = dat as JsonObject<string>;
                    if (co == null)
                        return null;

                    JsonPair<string> tpair = co.FindPair("Type");
                    if (tpair == null)
                        return null;

                    string type = ((tpair.Value as JsonData<string>).Value as string).ToLower();
                    if (!Translators.ContainsKey(type))
                        return null;

                    return Translators[type](co, client);
                }).Where(c => c != null).OrderBy(c => c.Priority));
            }
        }

        #endregion
    }

}
