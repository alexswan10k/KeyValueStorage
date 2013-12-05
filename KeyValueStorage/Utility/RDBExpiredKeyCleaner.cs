using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Interfaces.Utility;
using KeyValueStorage.Utility.Sql;

namespace KeyValueStorage.Utility
{
    public class RdbExpiredKeyCleaner : IExpiredKeyCleaner
    {
        public IStoreProvider Provider { get { return _provider; } }
        protected IRDbStoreProvider _provider;
        private SqlDialectProviderCommon _sqlProvider = new SqlDialectProviderCommon();

        public RdbExpiredKeyCleaner(IRDbStoreProvider provider)
        {
            _provider = provider;
        }

        public void CleanupKeys()
        {
            _sqlProvider.ExecuteDeleteParams(_provider.Connection, _provider.KVSTableName,
                                            new WhereClause("Expires", Operator.LessThan, DateTime.UtcNow));
        }

        public void SetKeyExpiry(string key, DateTime expires)
        {
            _sqlProvider.ExecuteUpdateParams(_provider.Connection, _provider.KVSTableName,
                                             new[] {new WhereClause("[Key]", Operator.Equals, key)},
                                             new ColumnValue("Expires", expires));
        }

	    public DateTime? GetKeyExpiry(string key)
	    {
	        var dt = _sqlProvider.ExecuteSelectParams(_provider.Connection, _provider.KVSTableName, new[] {"Expires"},
	                                         new WhereClause("[Key]", Operator.Equals, key));

            if(dt.Rows.Count < 1)
                return null;

	        return (DateTime)dt.Rows[0]["Expires"];
	    }
    }
}
