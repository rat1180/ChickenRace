using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dictionary
{
    /// <summary>
    /// Dictoinary_Unity�p�̃y�A�N���X
    /// ����P�̂ł͎g���Ȃ�
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
    /// Dictionary���C���X�y�N�^�[�ŕ\���o����悤�ɂ����N���X
    /// ��{��Dictionary�Ɠ������������邪�A����͊֐�����{�Ƃ���
    /// ���ӓ_�Ƃ��āA�錾���ɏ��������s���邽�߁A���O�ɃC���X�y�N�^�[�ŃZ�b�g�͂ł��Ȃ�
    /// �܂��AValue��̒T�������邪�A��{�͍ŏ��Ɍ����������̂�Ԃ�
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class Dictionary_Unity<TKey, TValue>
    {
        public List<SerializableKeyPair<TKey, TValue>> keyPairs = new List<SerializableKeyPair<TKey, TValue>>();
        
        /// <summary>
        /// Key�����Value��Ԃ�
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
        /// ���X�g�̐擪�ɍł��߂��A��v����Value������Key��Ԃ�
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
        /// ��v����Value�����AKey�̃��X�g��Ԃ�
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
        /// Dictionary�ɒl��ǉ�����
        /// Key�͏d���o���Ȃ��悤�ɂȂ��Ă���̂Œ���
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key,TValue value)
        {
            if (ContainKey(key))
            {
                Debug.LogWarning("Key���d�����Ă��܂�");
            }

            SerializableKeyPair<TKey, TValue> pair = new SerializableKeyPair<TKey, TValue>();
            pair.key = key;
            pair.value = value;

            keyPairs.Add(pair);
        }

        /// <summary>
        /// Key����ɍ폜���s��
        /// Key�̔��ʂ͎�����Key��p�ӂ��邩�AGetKey�EGetKeyList��p����
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
        /// Key��ň�v����Key��{�����A�y�A�^�ŕԂ�
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
        /// Value��ő{�����A�܂܂�Ă���Key�̃��X�g��Ԃ�
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
        /// Dictionary�̗v�f����Ԃ�
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return keyPairs.Count;
        }
    }
}