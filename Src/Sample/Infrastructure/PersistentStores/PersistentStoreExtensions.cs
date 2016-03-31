namespace Sample.Infrastructure.PersistentStores
{
    public static class PersistentStoreExtensions
    {
        public static void Write<T>(this IPersistentStore store, T record)
        {
            store.Write(new[] { record });
        }

        public static void Write<T>(this IPersistentStore store, string groupName, T record)
        {
            store.Write(groupName, new[] { record });
        }
    }
}