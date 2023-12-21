using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PhotonMethods
{

    /// <summary>
    /// ネット接続時以外でもフォトンの生成を行ってくれるクラス
    /// 基本的に"プレファブ名".SafeInstantiate()を呼ぶこと
    /// </summary>
    public static class PhotonInstantiate
    {
        /// <summary>
        /// オフライン状態でもフォトンの生成を行う
        /// ただし、未接続の場合はオフライン接続を行うので
        /// その後に接続する場合は一度接続を切ること
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
     GameKeyの概要
    3つのKeyで進行の同期を行う
    Keyの進行時(true時)には1つ先のKeyをリセットし
    1つ前はそのままにすることで判定抜けを防ぐ

     */
    /// <summary>
    /// Photonのカスタムプロパティ拡張メソッド用クラス
    /// 使用する場合は Playerクラス.
    /// で使える
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
