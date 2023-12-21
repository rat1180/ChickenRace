using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PhotonMethods
{

    /// <summary>
    /// �l�b�g�ڑ����ȊO�ł��t�H�g���̐������s���Ă����N���X
    /// ��{�I��"�v���t�@�u��".SafeInstantiate()���ĂԂ���
    /// </summary>
    public static class PhotonInstantiate
    {
        /// <summary>
        /// �I�t���C����Ԃł��t�H�g���̐������s��
        /// �������A���ڑ��̏ꍇ�̓I�t���C���ڑ����s���̂�
        /// ���̌�ɐڑ�����ꍇ�͈�x�ڑ���؂邱��
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="position"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static GameObject SafeInstantiate(this string prefabName,Vector3 position,Quaternion quaternion)
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.OfflineMode = true;
            }
            return PhotonNetwork.Instantiate(prefabName, position, quaternion);
        }
    }

    /*
     GameKey�̊T�v
    3��Key�Ői�s�̓������s��
    Key�̐i�s��(true��)�ɂ�1���Key�����Z�b�g��
    1�O�͂��̂܂܂ɂ��邱�ƂŔ��蔲����h��

     */
    /// <summary>
    /// Photon�̃J�X�^���v���p�e�B�g�����\�b�h�p�N���X
    /// �g�p����ꍇ�� Player�N���X.
    /// �Ŏg����
    /// </summary>
    public static class PhotonCustumPropertie
    {
        private const string InitStatusKey = "Is";
        private const string RankStatusKey = "Rs";
        private const string GameReadyStatusKey = "GRs";
        private const string GameInGameStatusKey = "GIGs";
        private const string GameEndStatusKey = "GEs";

        private static readonly ExitGames.Client.Photon.Hashtable propsToSet = new ExitGames.Client.Photon.Hashtable();

        public static int GetInitStatus(this Photon.Realtime.Player player)
        {
            return (player.CustomProperties[InitStatusKey] is int status) ? status : 0;
        }

        public static void SetInitStatus(this Photon.Realtime.Player player, int status)
        {
            propsToSet[InitStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        public static bool GetGameInGameStatus(this Photon.Realtime.Player player)
        {
            return (player.CustomProperties[GameInGameStatusKey] is bool status) ? status : false ;
        }

        public static void SetGameInGameStatus(this Photon.Realtime.Player player, bool status)
        {
            propsToSet[GameInGameStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        public static bool GetGameReadyStatus(this Photon.Realtime.Player player)
        {
            return (player.CustomProperties[GameReadyStatusKey] is bool status) ? status : false;
        }

        public static void SetGameReadyStatus(this Photon.Realtime.Player player, bool status)
        {
            propsToSet[GameReadyStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        public static bool GetGameEndStatus(this Photon.Realtime.Player player)
        {
            return (player.CustomProperties[GameEndStatusKey] is bool status) ? status : false;
        }

        public static void SetGameEndStatus(this Photon.Realtime.Player player, bool status)
        {
            propsToSet[GameEndStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        public static int GetRankStatus(this Photon.Realtime.Player player)
        {
            return (player.CustomProperties[RankStatusKey] is int status) ? status : 0;
        }

        /// <param name="player"></param>
        public static void SetRankStatus(this Photon.Realtime.Player player, int status)
        {
            propsToSet[RankStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }
}
