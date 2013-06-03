using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace KeyValueStorage.AzureTable
{
    public class KVEntity : TableEntity
    {
        public string Value { get; set; }

        public KVEntity()
        {
            
        }
    }
}
