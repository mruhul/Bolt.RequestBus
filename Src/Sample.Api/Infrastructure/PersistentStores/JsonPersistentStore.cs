using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Bolt.Serializer;

namespace Sample.Api.Infrastructure.PersistentStores
{
    public class JsonPersistentStore : IPersistentStore
    {
        private readonly ISerializer serializer;
        private readonly string srcDir;
        private static readonly ReaderWriterLockSlim FileLock = new ReaderWriterLockSlim();
        

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
            var filePath = GetFilePath(groupName);

            FileLock.EnterWriteLock();

            try
            {
                if (!Directory.Exists(srcDir))
                {
                    Directory.CreateDirectory(srcDir);
                }

                var records = ReadWithoutLock<object>(filePath).ToList();

                records.AddRange(data.Select(x => (object)x));

                File.WriteAllText(filePath, serializer.Serialize(records));
            }
            finally
            {
                FileLock.ExitWriteLock();
            }
        }

        public IEnumerable<T> Read<T>()
        {
            return Read<T>(typeof (T).Name);
        }
        
        public IEnumerable<T> Read<T>(string groupName)
        {
            var path = GetFilePath(groupName);

            FileLock.EnterReadLock();

            try
            {
                return ReadWithoutLock<T>(path);
            }
            finally
            {
                FileLock.ExitReadLock();
            }
        }

        private IEnumerable<T> ReadWithoutLock<T>(string path)
        {
            if (!File.Exists(path)) return Enumerable.Empty<T>();
            var content = File.ReadAllText(path);

            return string.IsNullOrWhiteSpace(content)
                ? Enumerable.Empty<T>()
                : serializer.Deserialize<IEnumerable<T>>(content);
        }

        private string GetFilePath(string groupName)
        {
            return $"{srcDir}\\{groupName}.json";
        }
    }
}