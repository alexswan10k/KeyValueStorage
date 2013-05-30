using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueStorage.Interfaces
{
    public interface ITextSerializer
    {
        string Serialize<T>(T item);
        T Deserialize<T>(string itemSerialized);
    }
}
