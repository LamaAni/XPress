using XPress.Serialization;
using XPress.StorageBank;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using XPress.Coding.Storage;

namespace XPress.Serialization.StorageProviders
{
    /// <summary>
    /// Implements a direct serialization storage provider that reads and writes the serialized member, and member value 
    /// using a json serializer.
    /// </summary>
    /// <typeparam name="T">Type T must be a stream writer capable type. The writer will write the data to stream.</typeparam>
    public abstract class JsonFileStorageProvider<SUType, T> : XPress.StorageBank.IStorageProvider<SUType>
        where SUType : BankStorageUnit
    {
        /// <summary>
        /// Creates the file storage serializer. 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="extention"></param>
        /// <param name="path">If null, a directory named "Cache\\Serialzier" will be created if possible. Not then throw error.</param>
        public JsonFileStorageProvider(string extention = "cache.dat", string path = null)
        {
            Extention = extention;

            if (path == null)
                path = "Cache\\Serialized";

            if(path[1]!=':') // is partial path.
            {
                path = path.ToPartialStoragePath();
            }

            Path = path;

            if (!path.HasWriteAccessToFolder())
                throw new Exception("Cannot write to path \"" + Path + "\", or path dose not exist.");

            UsePrettyJson =
#if DEBUG
 true;
#else
 false;
#endif
        }

        #region members

        /// <summary>
        /// If true the current will use pretty json.
        /// </summary>
        public bool UsePrettyJson { get; private set; }

        /// <summary>
        /// The path to where the providers stores the data.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The file extention the storage provider uses.
        /// </summary>
        public string Extention { get; private set; }

        System.Collections.Concurrent.ConcurrentDictionary<string, SUType> m_PendingUnits = new System.Collections.Concurrent.ConcurrentDictionary<string, SUType>();

        #endregion

        #region abstract methods

        /// <summary>
        /// Serializes the BankStorageUnit to byte array.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        protected abstract byte[] ToByteArray(SUType unit);

        /// <summary>
        /// Deserializes the bank storage unit from the byte array.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        protected abstract SUType FromByteArray(byte[] bytes);

        #endregion

        #region helper methods

        string IdFromFilename(string filename)
        {
            return filename.Substring(0, filename.Length - (Extention.Length + 1));
        }

        string IdToFileName(string id)
        {
            return id + "." + Extention;
        }

        string IdToFullPath(string id)
        {
            return System.IO.Path.Combine(Path, IdToFileName(id));
        }

        #endregion

        #region IStorageProvider Members

        public string GetNewUnitId(string prefix = null, string appendix = null)
        {
            if (prefix == null)
                prefix = "";
            if (appendix == null)
                appendix = "";
            string id;
            do
            {
                id = ((int)20).GenerateCode();
            }
            while (File.Exists(IdToFullPath(prefix + id + appendix)));

            // creates an empty file and closes it.
            File.Create(IdToFullPath(prefix + id + appendix)).Close();

            return prefix + id + appendix;
        }

        public SUType ReadUnit(string id)
        {
            string fn = IdToFullPath(id);
            if (!File.Exists(fn))
                return null;
            return FromByteArray(File.ReadAllBytes(fn)); // Serializer.Deserialize(Serializer.ParseByteArray(File.ReadAllBytes(fn)), typeof(BankStorageUnit)) as BankStorageUnit;
        }

        public void WriteUnit(string id, SUType unit)
        {
            File.WriteAllBytes(IdToFullPath(id), ToByteArray(unit));
        }

        public void DeleteUnit(string id)
        {
            string fn = IdToFileName(id);
            if (File.Exists(fn))
                File.Delete(fn);
        }

        public bool Contains(string id)
        {
            string fn = IdToFileName(id);
            return File.Exists(fn);
        }

        public string[] GetAllIds()
        {
            return Directory.GetFiles(Path, "*." + Extention).Select(fn => fn.Remove(0, Path.Length + 1)).Select(fn => IdFromFilename(fn)).ToArray();
        }


        bool _isUpdateingPending = false;

        public void UpdatePending()
        {
            if (_isUpdateingPending)
                return;
            _isUpdateingPending = true;

            Task.Run(() =>
            {
                try
                {
                    while (m_PendingUnits.Count > 0)
                    {
                        System.Collections.Concurrent.ConcurrentDictionary<string, SUType> pending = m_PendingUnits;
                        m_PendingUnits = new System.Collections.Concurrent.ConcurrentDictionary<string, SUType>();
                        pending.ForEach(kvp =>
                        {
                            WriteUnit(kvp.Key, kvp.Value);
                        });
                    }
                }
                catch(Exception ex)
                {
                    // update so there will be others that may be written.
                    _isUpdateingPending = false;
                    throw ex;
                }
            });
        }

        public void PendUnit(string id, SUType unit)
        {
            m_PendingUnits[id] = unit;
        }

        #endregion
    }
}
