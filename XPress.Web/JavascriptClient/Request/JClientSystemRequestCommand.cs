using XPress.Web.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JavascriptClient.Request
{
    /// <summary>
    /// Implements a system service commands structure. Will be executed before normal commands.
    /// </summary>
    public class JClientSystemRequestCommand : JClientRequestCommand
    {
        /// <summary>
        /// Creates a jclient system command.
        /// </summary>
        /// <param name="call">The client call associated with the command.</param>
        /// <param name="command">The command to execute.</param>
        public JClientSystemRequestCommand(SystemCommands command)
        {
            Command = command;
        }

        #region members

        /// <summary>
        /// The command.
        /// </summary>
        public SystemCommands Command { get; private set; }

        #endregion

        #region execute command

        public override void ExecuteCommand()
        {
            switch (Command)
            {
                case SystemCommands.Register:
                    {
                        Call.Response.Trace.Write("Client registed on server.");
                        Call.Response.SyncedResponse = Call.Client.ClientRegisterMessage;
                    }
                    break;
                case SystemCommands.Disconnect:
                    {
                        // disconnecting the client and deleting all client info.
                        if (JClientCallContext.Current != null)
                        {
                            JClientState.DestroyClient(JClientCallContext.Current.State, XPressRazorHttpHandler.Bank);
                        }
                    }
                    break;
                case SystemCommands.Debug:
#if DEBUG
                    Call.Response.Trace.WriteLine("Running in debug mode. Server connection OK!");
#else
                    Call.Response.Trace.WriteLine("Running in relesee(or non debug) mode. Server connection OK!");
#endif
                    break;
                default:
                    {
                        throw new Exception("Unknown command to client system.");
                    }
                    break;
            }
        }

        #endregion
    }

    public enum SystemCommands
    {
        /// <summary>
        /// An unknown command.
        /// </summary>
        Unknown,
        /// <summary>
        /// Disconnect the client.
        /// </summary>
        Disconnect,
        /// <summary>
        /// Register the client.
        /// </summary>
        Register,
        /// <summary>
        /// For debug purpuse only.
        /// </summary>
        Debug
    };
}
