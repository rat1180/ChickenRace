using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dictionary
{
    /// <summary>
    /// Dictoinary_Unity用のペアクラス
    /// これ単体では使われない
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class SerializableKeyPair<TKey, TValue>
    {
        public TKey key;
        public TValue value;
    }

    /// <summary>
    /// Dictionaryをインスペクターで表示出来るようにしたクラス
    /// 基本はDictionaryと同じ動きをするが、動作は関数を基本とする
    /// 注意点として、宣言時に初期化が行われるため、事前にインスペクターでセットはできない
    /// また、Value基準の探索もあるが、基本は最初に見つかったものを返す
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class Dictionary_Unity<TKey, TValue>
    {
        public List<SerializableKeyPair<TKey, TValue>> keyPairs = new List<SerializableKeyPair<TKey, TValue>>();
        
        /// <summary>
        /// Keyを基準にValueを返す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Dictionaryに値を追加する
        /// Keyは重複出来ないようになっているので注意
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
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

        /// <summary>
        /// Keyを基準に削除を行う
        /// Keyの判別は自分でKeyを用意するか、GetKey・GetKeyListを用いる
        /// </summary>
        /// <param name="key"></param>
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
        public SerializableKeyPair<TKey, TValue> SerchKey(TKey key)
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
        public List<TKey> SerchValue(TValue value)
        {
            List<TKey> keys = new List<TKey>();
            foreach (var pair in keyPairs)
            {
                if (EqualityComparer<TValue>.Default.Equals(pair.value, value)) keys.Add(pair.key);
            }

            return keys;
        }

        /// <summary>
        /// Dictionaryの要素数を返す
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return keyPairs.Count;
        }
    }
}