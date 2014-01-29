using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Coding.Storage
{
    public static class Extentions
    {
        /// <summary>
        /// Gets and validatets the existance of the partial storage path that is added to the storage.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string ToPartialStoragePath(this string partialPath)
        {
            partialPath = partialPath[0] == '\\' ? partialPath.Substring(1) : partialPath;
            string path = null;
            if (AppDomain.CurrentDomain.BaseDirectory.HasWriteAccessToFolder())
            {
                path = AppDomain.CurrentDomain.BaseDirectory + partialPath;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            else using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                throw new Exception("Partial storage not implemented yet.");
            }
            return path;
        }

        ///<summary>
        /// Validates we have acces to the folder to read and write.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static bool HasWriteAccessToFolder(this string path)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(path);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
