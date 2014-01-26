﻿using XPress.StorageBank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.StorageProviders
{
    /// <summary>
    /// Implements a refrence bank storage unit. This special unit allows the refrence bank to be restored.
    /// </summary>
    public class JsonRefrenceBankStorageUnit<T> : BankStorageUnit
    {
        public JsonRefrenceBankStorageUnit(Reference.JsonRefrenceBank<T> bank)
        {
            Bank = bank;
        }

        /// <summary>
        /// Allows the bank associated with the unit to be accessable.
        /// </summary>
        public Reference.JsonRefrenceBank<T> Bank { get; protected set; }
    }
}
