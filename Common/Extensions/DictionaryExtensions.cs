using System.Collections.Generic;

namespace Common.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Tries to add a new string item or append it to an existing one.
        /// </summary>
        /// <param name="dictionary">The dictionary to add to.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to add or append.</param>
        /// <param name="separator">The separator character used when appending.</param>
        /// <remarks>If the key doesn't exist yet in the dictionary, a new item is added. 
        /// If the key already exists, the corresponding item is appended with the value, using a separator character.</remarks>
        public static void AddOrAppendItem(this IDictionary<string, object> dictionary, string key, string value, char? separator = ' ')
        {
            bool added = default;

            try
            {
                dictionary.Add(key, value);
                added = true;
            }
            catch
            {
            }

            if (!added && dictionary[key] is string)
            {
                dictionary[key] += $"{separator}{value}";
            }
        }
    }
}
