using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyValueStorage.Exceptions;
using KeyValueStorage.Interfaces;
using KeyValueStorage.Utility;

namespace KeyValueStorage.FSText
{
    public class FSTextStoreProvider : IStoreProvider
    {
        static FSTextStoreProvider()
        {
            KeyCharSubstitutorDefault = new CharSubstitutor();
            KeyCharSubstitutorDefault.SubstitutionTable.Add(':', '`');
        }

        private DirectoryInfo DI { get; set; }
        const string suffix = ".json";
        const string dataSuffix = ".data";
        const string casSuffix = "-CAS";
        const string expSuffix = "-EXP";
        const string lockSuffix = "-L";
        public static CharSubstitutor KeyCharSubstitutorDefault = new CharSubstitutor();
        CharSubstitutor KeyCharSubstitutor {get;set;}

        public FSTextStoreProvider(string path)
        {
            DI = new DirectoryInfo(path);

            if (!DI.Exists)
                DI.Create();

            KeyCharSubstitutor = KeyCharSubstitutorDefault;
        }

        public FSTextStoreProvider(string path, CharSubstitutor keyCharsubstitutor)
            :this(path)
        {
            KeyCharSubstitutor = keyCharsubstitutor;
        }

        FileInfo GetFileInfo(string key, bool isData = false)
        {
            return new FileInfo(Path.Combine(DI.FullName, !isData? key + suffix : key + dataSuffix));
        }


        #region IStoreProvider
        public string Get(string key)
        {
            key = KeyCharSubstitutor.Convert(key);

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
            key = KeyCharSubstitutor.Convert(key);

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
            key = KeyCharSubstitutor.Convert(key);

            var fi = GetFileInfo(key);

            if (fi.Exists)
                fi.Delete();
        }

        public string Get(string key, out ulong cas)
        {
            key = KeyCharSubstitutor.Convert(key);

            var fi = GetFileInfo(key+casSuffix, true);

            if (fi.Exists)
            {
                using (var sr = fi.OpenRead())
                {
                    byte[] bytes = new byte[sr.Length];
                    sr.Read(bytes, 0, (int)sr.Length);

                    if (bytes.Length > 0)
                    {
                        try
                        {
                            cas = BitConverter.ToUInt64(bytes, 0);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Data for key "+key+casSuffix+" is not CAS data", ex);
                        }
                    }
                    else
                        cas = 0;
                }
            }
            else
                cas = 0;

            return this.Get(key);
        }

        public void Set(string key, string value, ulong cas)
        {
            key = KeyCharSubstitutor.Convert(key);

            var fi = GetFileInfo(key + casSuffix, true);
            var fiLock = GetFileInfo(key + lockSuffix, true);

            if (!fi.Exists)
                fi.Create().Dispose();

            using(var swLock = fiLock.Create())
            {
                ulong readCASVal = 0;

                //get current cas
                byte[] bytes = null;
                using (var srCAS = fi.OpenRead())
                {
                    bytes = new byte[srCAS.Length];
                    srCAS.Read(bytes, 0, (int)srCAS.Length);
                }
                using (var swCAS = fi.OpenWrite())
                {

                    swCAS.Seek(0, SeekOrigin.Begin);

                    if (bytes.Count() > 0)
                        readCASVal = BitConverter.ToUInt64(bytes, 0);

                    if (readCASVal != cas)
                        throw new CASException("CAS expired");

                    //do our actual set operation
                    Set(key, value);

                    var bytesToWrite = BitConverter.GetBytes(cas + 1);
                    swCAS.Write(bytesToWrite, 0, bytesToWrite.Count());
                    swCAS.Flush();
                }
            }
            fiLock.Delete();
        }

        public void Set(string key, string value, DateTime expires)
        {
            key = KeyCharSubstitutor.Convert(key);
            throw new NotImplementedException();//awaiting better cleanup implementation
        }

        public void Set(string key, string value, TimeSpan expiresIn)
        {
            key = KeyCharSubstitutor.Convert(key);
            throw new NotImplementedException();//awaiting better cleanup implementation
        }

        public void Set(string key, string value, ulong CAS, DateTime expires)
        {
            key = KeyCharSubstitutor.Convert(key);
            throw new NotImplementedException();//awaiting better cleanup implementation
        }

        public void Set(string key, string value, ulong CAS, TimeSpan expiresIn)
        {
            key = KeyCharSubstitutor.Convert(key);

            throw new NotImplementedException();//awaiting better cleanup implementation
        }

        public bool Exists(string key)
        {
            key = KeyCharSubstitutor.Convert(key);

            if (GetFileInfo(key).Exists)
                return true;
            return false;
        }

        public DateTime? ExpiresOn(string key)
        {
            key = KeyCharSubstitutor.Convert(key);

            throw new NotImplementedException();
        }

        public IEnumerable<string> GetStartingWith(string key)
        {
            key = KeyCharSubstitutor.Convert(key);

            foreach (var file in DI.GetFiles(key + "*"))
            {
                var keyItem = file.Name.Replace(suffix, "");
                yield return Get(keyItem);
            }
        }

        public IEnumerable<string> GetContaining(string key)
        {
            key = KeyCharSubstitutor.Convert(key);

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
            key = KeyCharSubstitutor.Convert(key);

            foreach (var file in DI.GetFiles(key + "*"))
            {
                var keyItem = file.Name.Replace(suffix, "");
                keyItem = KeyCharSubstitutor.ConvertBack(keyItem);
                yield return keyItem;
            }
        }

        public IEnumerable<string> GetKeysContaining(string key)
        {
            key = KeyCharSubstitutor.Convert(key);

            foreach (var file in DI.GetFiles("*" + key + "*"))
            {
                var keyItem = file.Name.Replace(suffix, "");
                keyItem = KeyCharSubstitutor.ConvertBack(keyItem);
                yield return keyItem;
            }
        }

        public int CountStartingWith(string key)
        {
            key = KeyCharSubstitutor.Convert(key);

            return DI.GetFiles(key + "*").Count();
        }

        public int CountContaining(string key)
        {
            key = KeyCharSubstitutor.Convert(key);

            return DI.GetFiles("*" + key + "*").Count();
        }

        public int CountAll()
        {
            return DI.GetFiles().Count();
        }

        public ulong GetNextSequenceValue(string key, int increment)
        {
            key = KeyCharSubstitutor.Convert(key);
            return getNextSequenceValue(key, increment, 0);
        }

        protected ulong getNextSequenceValue(string key, int increment, int tryCount)
        {
            if(tryCount == 0)
                key = KeyCharSubstitutor.Convert(key);

            try
            {
                ulong cas;
                var obj = Get(key, out cas);
                ulong seqVal;

                if (!ulong.TryParse(obj, out seqVal))
                {
                    seqVal = 0;
                }
                seqVal = seqVal + (ulong)increment;
                Set(key, seqVal.ToString(), cas);
                return seqVal;
            }
            catch (CASException casEx)
            {
                if (tryCount >= 10)
                    throw new Exception("Could not get sequence value", casEx);

                System.Threading.Thread.Sleep(20);
                //retry
                return getNextSequenceValue(key, increment, tryCount++);
            }
            return 0;
        }

        public void Append(string key, string value)
        {
            key = KeyCharSubstitutor.Convert(key);

            var fi = GetFileInfo(key);

            if (!fi.Exists)
                fi.Create();

            using (var fs = fi.OpenWrite())
            {
                fs.Seek(fs.Length,SeekOrigin.Begin);
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        #endregion

        public void Dispose()
        {

        }
    }
}
