namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> self, Dictionary<TKey, TValue> dict)
        {
            foreach (var item in dict)
            {
                self.Add(item.Key, item.Value);
            }
        }
    }
}
