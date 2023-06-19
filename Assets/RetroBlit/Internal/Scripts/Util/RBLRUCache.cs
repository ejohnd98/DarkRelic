namespace RetroBlitInternal
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Implementation of a simple LRU cache. Capacity is measured in bytes instead of value count.
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public class RBLRUCache<K, V>
    {
        private int mMaxBytes = 1024;
        private int mBytesUsed = 0;

        private LinkedList<LRUItem<K, V>> mCacheList = new LinkedList<LRUItem<K, V>>();
        private Dictionary<K, LinkedListNode<LRUItem<K, V>>> mCacheDict = new Dictionary<K, LinkedListNode<LRUItem<K, V>>>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxBytes">Maximum bytes allowed in the LRU. This is a loose limit,
        /// a single item of a larger size can fit in the cache, but subsequent additions would
        /// cause a cache removal.</param>
        public RBLRUCache(int maxBytes)
        {
            mMaxBytes = maxBytes;
        }

        /// <summary>
        /// Add a new key-value pair to the cache, with given estimated byte size. If total size in cache exceeds
        /// LRU limit then old items get tossed out.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="byteSize">Estimated byte size</param>
        public void Add(K key, V value, int byteSize)
        {
            if (mBytesUsed + byteSize > mMaxBytes)
            {
                // Full, throw something out
                if (mCacheList.First != null)
                {
                    var first = mCacheList.First;
                    mBytesUsed -= first.Value.size;
                    mCacheDict.Remove(first.Value.key);
                    mCacheList.RemoveFirst();
                }
            }

            var item = new LRUItem<K, V>();
            item.key = key;
            item.value = value;
            item.size = byteSize;

            var node = mCacheList.AddLast(item);
            mCacheDict[key] = node;

            mBytesUsed += byteSize;
        }

        /// <summary>
        /// Get a value from the cache by its key. If value exists its most to the front of the recency list.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Found value, or default value if key is not found</returns>
        public V Get(K key)
        {
            if (mCacheDict.ContainsKey(key))
            {
                // Update the position in the list (to most recent) and return item
                var node = mCacheDict[key];
                var item = node.Value;

                mCacheList.Remove(node);
                node = mCacheList.AddLast(item);
                mCacheDict[key] = node;

                return item.value;
            }
            else
            {
                // Value doesn't exit, return default value
                return default(V);
            }
        }

        /// <summary>
        /// Remove the given value with the given key from the cache
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(K key)
        {
            if (mCacheDict.ContainsKey(key))
            {
                var node = mCacheDict[key];
                int size = node.Value.size;
                mCacheList.Remove(node);
                mCacheDict.Remove(key);

                mBytesUsed -= size;
            }
        }

        /// <summary>
        /// Clear the entire cache
        /// </summary>
        public void Clear()
        {
            foreach (var item in mCacheDict)
            {
                mCacheList.Remove(item.Value);
            }

            if (mCacheList.Count > 0)
            {
                Debug.LogError("LRU cache list and dict were out of sync!");
            }

            mCacheDict.Clear();

            mBytesUsed = 0;
        }

        private class LRUItem<K2, V2>
        {
            /// <summary>
            /// Estimated byte size of the value
            /// </summary>
            public int size;

            /// <summary>
            /// Key
            /// </summary>
            public K2 key;

            /// <summary>
            /// Value
            /// </summary>
            public V2 value;
        }
    }
}
