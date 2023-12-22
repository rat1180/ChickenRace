using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dictionary
{
    [System.Serializable]
    public class SerializableKeyPair<TKey, TValue>
    {
        public TKey key;
        public TValue value;
    }
    [System.Serializable]
    public class Dictionary_Unity<TKey, TValue>
    {
        public List<SerializableKeyPair<TKey, TValue>> keyPairs = new List<SerializableKeyPair<TKey, TValue>>();
        
        public TValue GetValue(TKey key)
        {
            var result = SerchKey(key);
            if (result != default(SerializableKeyPair<TKey, TValue>)) return result.value;
            return default(TValue);
        }

        /// <summary>
        /// リストの先頭に最も近い、一致したValueを持つKeyを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TKey GetKey(TValue value)
        {
            var result = SerchValue(value);
            if (result.Count > 0) return result[0];
            return default(TKey);
        }

        /// <summary>
        /// 一致したValueを持つ、Keyのリストを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<TKey> GetKeyList(TValue value)
        {
            var result = SerchValue(value);
            if (result.Count > 0) return result;
            return default(List<TKey>);
        }

        public void Add(TKey key,TValue value)
        {
            if (ContainKey(key))
            {
                Debug.LogWarning("Keyが重複しています");
            }

            SerializableKeyPair<TKey, TValue> pair = new SerializableKeyPair<TKey, TValue>();
            pair.key = key;
            pair.value = value;

            keyPairs.Add(pair);
        }

        public void Remove(TKey key)
        {
            var result = SerchKey(key);
            if (result != default(SerializableKeyPair<TKey, TValue>)) keyPairs.Remove(result);
        }

        public bool ContainKey(TKey key)
        {
            var result = SerchKey(key);
            if (result != default(SerializableKeyPair<TKey, TValue>)) return true;
            return false;
        }

        public bool ContainValue(TValue value)
        {
            if (SerchValue(value).Count > 0) return true;
            return false;
        }

        /// <summary>
        /// Key基準で一致するKeyを捜索し、ペア型で返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        SerializableKeyPair<TKey, TValue> SerchKey(TKey key)
        {
            foreach (var pair in keyPairs)
            {
                if (EqualityComparer<TKey>.Default.Equals(pair.key, key)) return pair;
            }

            return default(SerializableKeyPair<TKey, TValue>);
        }

        /// <summary>
        /// Value基準で捜索し、含まれていたKeyのリストを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        List<TKey> SerchValue(TValue value)
        {
            List<TKey> keys = new List<TKey>();
            foreach (var pair in keyPairs)
            {
                if (EqualityComparer<TValue>.Default.Equals(pair.value, value)) keys.Add(pair.key);
            }

            return keys;
        }
    }
}