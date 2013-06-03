using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Interfaces;

namespace KeyValueStorage.FSText
{
    public class FSTextStoreProvider : IStoreProvider
    {
        private DirectoryInfo DI { get; set; }
        const string suffix = ".json";

        public FSTextStoreProvider(string path)
        {
            DI = new DirectoryInfo(path);

            if (!DI.Exists)
                DI.Create();
        }

        FileInfo GetFileInfo(string key)
        {
            return new FileInfo(Path.Combine(DI.FullName, key + suffix));
        }


        #region IStoreProvider
        public string Get(string key)
        {
            var fi = GetFileInfo(key);

            if (!fi.Exists)
                return null;

            using (var sr = fi.OpenText())
            {
                return sr.ReadToEnd();
            }
        }

        public void Set(string key, string value)
        {
            var fi = GetFileInfo(key);

            if(fi.Exists)
                fi.Delete();

            using (var fs = fi.CreateText())
            {
                fs.Write(value);
            }
        }

        public void Remove(string key)
        {
            var fi = GetFileInfo(key);

            if (fi.Exists)
                fi.Delete();
        }

        public string Get(string key, out ulong cas)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong cas)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, DateTime expires)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong CAS, DateTime expires)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value, ulong CAS, TimeSpan expiresIn)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key)
        {
            if (GetFileInfo(key).Exists)
                return true;
            return false;
        }

        public DateTime? ExpiresOn(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            foreach (var file in DI.GetFiles(key + "*"))
            {
                var keyItem = file.Name.Replace(suffix, "");
                yield return Get(keyItem);
            }
        }

        public IEnumerable<string> GetContaining(string key)
        {
            foreach (var file in DI.GetFiles("*" + key + "*"))
            {
                var keyItem = file.Name.Replace(suffix, "");
                yield return Get(keyItem);
            }
        }

        public IEnumerable<string> GetAllKeys()
        {
            foreach (var file in DI.GetFiles())
            {
                var keyItem = file.Name.Replace(suffix, "");
                yield return keyItem;
            }
        }

        public IEnumerable<string> GetKeysStartingWith(string key)
        {
            foreach (var file in DI.GetFiles(key + "*"))
            {
                var keyItem = file.Name.Replace(suffix, "");
                yield return keyItem;
            }
        }

        public IEnumerable<string> GetKeysContaining(string key)
        {
            foreach (var file in DI.GetFiles("*" + key + "*"))
            {
                var keyItem = file.Name.Replace(suffix, "");
                yield return keyItem;
            }
        }

        public int CountStartingWith(string key)
        {
            return DI.GetFiles(key + "*").Count();
        }

        public int CountContaining(string key)
        {
            return DI.GetFiles("*" + key + "*").Count();
        }

        public int CountAll()
        {
            return DI.GetFiles().Count();
        }

        public long GetNextSequenceValue(string key, int increment)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Dispose()
        {

        }
    }
}
