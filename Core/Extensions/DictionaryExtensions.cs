using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddOrEnrichEntry(this IDictionary<string, object> dictionary, string key, string value, char? separator = ' ')
        {
            if (!dictionary.TryAdd(key, value))
            {
                if (dictionary[key] is string)
                {
                    dictionary[key] += $"{separator}{value}";
                }
            }
        }
    }
}
