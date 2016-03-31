using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bolt.Serializer;

namespace Sample.Infrastructure.PersistentStores
{
    public class JsonPersistentStore : IPersistentStore
    {
        private readonly ISerializer serializer;
        private readonly string srcDir;
        private static readonly object FileWriteLock = new object();

        public JsonPersistentStore(ISerializer serializer)
        {
            this.serializer = serializer;
            srcDir = $"{AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\')}\\Data";
        }

        public void Write<T>(IEnumerable<T> data)
        {
            Write(typeof(T).Name, data);
        }

        public void Write<T>(string groupName, IEnumerable<T> data)
        {
            lock (FileWriteLock)
            {
                var records = Read<T>(groupName).ToList();

                records.AddRange(data);
                
                File.WriteAllText(GetFilePath(groupName), serializer.Serialize(records));
            }
        }

        public IEnumerable<T> Read<T>()
        {
            return Read<T>(typeof (T).Name);
        }
        
        public IEnumerable<T> Read<T>(string groupName)
        {
            var path = GetFilePath(groupName);

            return File.Exists(path) 
                ? serializer.Deserialize<IEnumerable<T>>(File.ReadAllText(path)) ?? Enumerable.Empty<T>() 
                : Enumerable.Empty<T>();
        }

        private string GetFilePath<T>()
        {
            return GetFilePath(typeof (T).Name);
        }

        private string GetFilePath(string groupName)
        {
            return $"{srcDir}\\{groupName}.json";
        }
    }
}