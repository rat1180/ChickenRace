using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PhotonMethods
{
    /// <summary>
    /// Photonのカスタムプロパティ拡張メソッド用クラス
    /// 使用する場合は Playerクラス.
    /// で使える
    /// </summary>
    public static class PhotonCustumPropertie
    {
        private const string InitStatusKey = "Is";

        private static readonly ExitGames.Client.Photon.Hashtable propsToSet = new ExitGames.Client.Photon.Hashtable();


        /// <summary>
        /// 引数でPhotonのプレイヤーを渡すことで
        /// 戻り値でそのプレイヤーの初期化情報が返る
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int GetInitStatus(this Photon.Realtime.Player player)
        {
            return (player.CustomProperties[InitStatusKey] is int status) ? status : 0;
        }

        /// <summary>
        /// 引数でPhotonのプレイヤーと初期化状態を渡すことで
        /// 他プレイヤーに送信する
        /// </summary>
        /// <param name="player"></param>
        public static void SetInitStatus(this Photon.Realtime.Player player, int status)
        {
            propsToSet[InitStatusKey] = status;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }
}
