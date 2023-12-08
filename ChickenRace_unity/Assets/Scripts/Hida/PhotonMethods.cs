using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PhotonMethods
{
    /// <summary>
    /// Photon�̃J�X�^���v���p�e�B�g�����\�b�h�p�N���X
    /// �g�p����ꍇ�� Player�N���X.
    /// �Ŏg����
    /// </summary>
    public static class PhotonCustumPropertie
    {
        private const string InitStatusKey = "Is";

        private static readonly ExitGames.Client.Photon.Hashtable propsToSet = new ExitGames.Client.Photon.Hashtable();


        /// <summary>
        /// ������Photon�̃v���C���[��n�����Ƃ�
        /// �߂�l�ł��̃v���C���[�̏�������񂪕Ԃ�
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int GetInitStatus(this Photon.Realtime.Player player)
        {
            return (player.CustomProperties[InitStatusKey] is int status) ? status : 0;
        }

        /// <summary>
        /// ������Photon�̃v���C���[�Ə�������Ԃ�n�����Ƃ�
        /// ���v���C���[�ɑ��M����
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
