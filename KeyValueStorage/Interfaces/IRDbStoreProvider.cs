using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces
{
    public interface IRDbStoreProvider : IStoreProvider
    {
        IDbConnection Connection { get; }
        bool OwnsConnection { get;  }
        string KVSTableName { get;}

        bool SetupWorkingTable();
    }
}
