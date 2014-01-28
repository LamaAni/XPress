using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XPress.Web.Controls.Services
{
    /// <summary>
    /// Handles the loading and saving of services.
    /// </summary>
    public interface IStorageProvider
    {
        string GetNewObjectId();
        string GetObjectData(string id);
        void SetObjectData(string id, string data);
        void ClearObjectData(string id);
        bool Contains(string id);
    }
}
