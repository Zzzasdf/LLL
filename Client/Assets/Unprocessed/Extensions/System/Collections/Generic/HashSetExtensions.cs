namespace System.Collections.Generic
{
    public static class HashSetExtensions
    {
        public static void AddRange<T>(this HashSet<T> self, HashSet<T> hashSet)
        {
            foreach (var item in hashSet)
            {
                self.Add(item);
            }
        }
    }
}