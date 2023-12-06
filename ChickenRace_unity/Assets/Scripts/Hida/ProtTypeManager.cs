using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtTypeManager : MonoBehaviour
{
    enum GameStatus
    {
        READY,
        SELECT,
        PLANT,
        RACE,
        RESULT
    }

    enum InitStatus
    {
        READY,
        NOW,
        WAIT
    }

    [SerializeField, Tooltip("���݂̃Q�[�����")] GameStatus gameState;


    public static ProtTypeManager Instance;

    #region Unity�C�x���g

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region �֐�

    /// <summary>
    /// �e�v���C���[�̏�������Ԃ��m�F����
    /// �S����Wait��Ԃ̎���true��Ԃ�
    /// ���̏�������Ԃ̓J�X�^���v���p�e�B�ŊǗ�����
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    bool CheckInitState()
    {
        //�S���̏�������Ԃ��m�F
        InitStatus playerinitlist = InitStatus.WAIT;
        if (playerinitlist == InitStatus.WAIT) return true;
        else return false;

    }

    #endregion

    #region �R���[�`��

    IEnumerator GameInit()
    {
        //��Ԃ𑗐M
        InitStatus initStatus = InitStatus.READY;

        //1.�ڑ����m�F
        //��Ԃ𑗐M
        initStatus = InitStatus.NOW;
        //�t�H�g���̋@�\�Őڑ����Ă��邩�m�F
        while (false)
        {
            //�ڑ��܂őҋ@
            Debug.Log("�ڑ��m�F��..");
            yield return null;
        }

        //��Ԃ𑗐M
        initStatus = InitStatus.WAIT;
        //���̃v���C���[��ҋ@
        yield return new WaitUntil(() => CheckInitState());

        //2.�e�l��������
        gameState = GameStatus.READY;

        //�e�}�l�[�W���[�𐶐�

        //�e�}�l�[�W���[�������m�F�E�ҋ@

        //Player�N���X����

        //�����������E���v���C���[��ҋ@
    }

    #endregion
}
