using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XPress.StorageBank
{
    /// <summary>
    /// Handles the loading and saving of services.
    /// </summary>
    public interface IStorageProvider<SUType>
        where SUType : BankStorageUnit
    {
        /// <summary>
        /// Returns a new object id with the prefix, and appendinx as is.
        /// </summary>
        /// <param name="prefix">The prefix</param>
        /// <param name="appendix">The appendix</param>
        /// <returns></returns>
        string GetNewUnitId(string prefix = null, string appendix = null);

        /// <summary>
        /// Returns the bank unit that mapps to the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SUType ReadUnit(string id);

        /// <summary>
        /// Sets the object data associated with the id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        void WriteUnit(string id, SUType unit);

        /// <summary>
        /// Pends the unit for writing, where the unit will be written as soon as possible. Threading api.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="unit"></param>
        void PendUnit(string id, SUType unit);

        /// <summary>
        /// Clears the object data for the specified id.
        /// </summary>
        /// <param name="id"></param>
        void DeleteUnit(string id);

        /// <summary>
        /// If true, the collection contains the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Contains(string id);

        /// <summary>
        /// Returns all unit ids in the provider.
        /// </summary>
        /// <returns></returns>
        string[] GetAllIds();

        /// <summary>
        /// Updates all the pending writes in the provider.
        /// </summary>
        void UpdatePending();
    }
}
