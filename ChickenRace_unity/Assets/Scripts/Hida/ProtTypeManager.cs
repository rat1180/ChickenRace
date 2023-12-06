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

    //�������̒i�K������
    //���̒i�K���I���Ǝ��̏�Ԃɐi��
    enum InitStatus
    {
        CONECT,
        RESET,
        WAIT,
        START
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
    /// �S���������̏�Ԃ̎���true��Ԃ�
    /// ���̏�������Ԃ̓J�X�^���v���p�e�B�ŊǗ�����
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    bool CheckInitState(InitStatus status)
    {
        //�S���̏�������Ԃ��m�F
        InitStatus playerinitlist = status;
        if (playerinitlist == status) return true;
        else return false;

    }

    #endregion

    #region �R���[�`��

    IEnumerator GameInit()
    {
        //��Ԃ𑗐M
        InitStatus initStatus = InitStatus.CONECT;

        //1.�ڑ����m�F<CONECT
        {
            //�t�H�g���̋@�\�Őڑ����Ă��邩�m�F
            while (false)
            {
                //�ڑ��܂őҋ@
                Debug.Log("�ڑ��m�F��..");
                yield return null;
            }

            //��Ԃ𑗐M
            initStatus = InitStatus.RESET;
            //���̃v���C���[��ҋ@
            yield return new WaitUntil(() => CheckInitState(InitStatus.RESET));
            Debug.Log("�ڑ��m�F!");

        }
        //2.�e�l��������<RESET
        {
            gameState = GameStatus.READY;

            //�e�}�l�[�W���[�𐶐�

            //�e�}�l�[�W���[�������m�F�E�ҋ@

            //Player�N���X����

            //��Ԃ𑗐M
            initStatus = InitStatus.WAIT;
            //���̃v���C���[��ҋ@
            yield return new WaitUntil(() => CheckInitState(InitStatus.WAIT));
        }
        //3.�����������E���v���C���[��ҋ@<WAIT
        {
            //�����ɃQ�[���J�n
            //��Ԃ𑗐M
            initStatus = InitStatus.START;
            //���̃v���C���[��ҋ@
            yield return new WaitUntil(() => CheckInitState(InitStatus.START));
        }
        Debug.Log("����������");
    }

    #endregion
}
