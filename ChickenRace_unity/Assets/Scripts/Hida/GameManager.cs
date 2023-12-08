using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    enum GameStatus
    {
        READY,  //�J�n������
        START,  //�Q�[���J�n�O
        SELECT, //��Q���I��
        PLANT,  //��Q���ݒu��
        RACE,   //���[�X��
        RESULT, //�X�R�A�\���A���f��
        END     //�Q�[���I��
    }

    //�������̒i�K������
    //���̒i�K���I���Ǝ��̏�Ԃɐi��
    enum InitStatus
    {
        CONECT,  //�ڑ���
        RESET,   //��������
        WAIT,    //���v���C���[�ҋ@
        START,   //�Q�[���J�n�\
    }

    /// <summary>
    /// �Q�[���̐i�s�ɕK�v�ȃ}�l�[�W���[�����܂Ƃ߂��N���X
    /// </summary>
    [System.Serializable]
    public class GameProgress
    {
        public MapManager mapManager;
        public Image uiManager;
        public DataSharingClass dataSharingClass;
    }

    [SerializeField, Tooltip("���݂̃Q�[�����")] GameStatus gameState;
    [SerializeField, Tooltip("�i�s���̃X�e�[�g�R���[�`��")] Coroutine stateCoroutine;
    [SerializeField, Tooltip("���̃N���X����n�����A���݂̃t�F�[�Y�I����m�点��ϐ�")] bool isFazeEnd; //Int�^�ɂ��ĕ����̏�ԂɑΉ��o����悤�ɂ��邩��
    [SerializeField, Tooltip("�Q�[���̐i�s�ɕK�v�ȃN���X�̂܂Ƃ�")] GameProgress gameProgress;
    [SerializeField, Tooltip("�f�o�b�O�p�̃��O��\�����邩�ǂ���")] bool isDebug;


    public static GameManager Instance;

    #region Unity�C�x���g

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameInit());
    }

    // Update is called once per frame
    void Update()
    {
        GameLoop();

        if(Input.GetKeyDown(KeyCode.Space)){
            DebugNextState();
        }
    }

    #endregion

    #region �֐�

    #region �N���X���Ŏg�p����֐�

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

    /// <summary>
    /// �X�e�[�g�R���[�`�����N���A����
    /// �X�e�[�g�R���[�`���I�����ɕK���ĂԂ���
    /// </summary>
    void ClearCoroutine()
    {
        StopCoroutine(stateCoroutine);

        stateCoroutine = null;
        isFazeEnd = false;
    }

    #endregion

    #region �N���X�O�Ŏg�p����֐�

    /// <summary>
    /// �t�F�[�Y�Ɋւ��N���X�ŁA
    /// ���̃t�F�[�Y���ōs���K�v�̂��鏈�����I�����ꍇ�Ă�
    /// ��F�ݒu�����A�L�����q�b�g�Ŏ��S
    /// </summary>
    public void EndFaze()
    {
        isFazeEnd = true;
    }

    public void SetDataSheringClass(DataSharingClass datasharingclass)
    {
        gameProgress.dataSharingClass = datasharingclass;
    }

    #endregion

    #region �f�o�b�O�p

    void DebugLog(string message)
    {
        if (isDebug) Debug.Log(message);
    }

    void DebugLogWarning(string message)
    {
        if (isDebug) Debug.LogWarning(message);
    }

    void DebugNextState()
    {
        gameState++;
        ClearCoroutine();
    }

    #endregion

    /// <summary>
    /// ���݂�gameState�ɉ����ēK�؂ȃX�e�[�g�R���[�`�����Ăяo��
    /// �X�e�[�g�R���[�`���͏I�����ɃR���[�`�����Ŕj������A
    /// ���s���o�Ȃ��Ƃ��͏��NULL�ɂȂ�
    /// </summary>
    void GameLoop()
    {
        //�t�F�[�Y��
        if (stateCoroutine != null) return;

        string coroutinename = "null";
        switch (gameState)
        {
            case GameStatus.READY:
                coroutinename = "StateREADY";
                break;
            case GameStatus.START:
                coroutinename = "StateSTART";
                break;
            case GameStatus.SELECT:
                coroutinename = "StateSELECT";
                break;
            case GameStatus.PLANT:
                coroutinename = "StatePLANT";
                break;
            case GameStatus.RACE:
                break;
            case GameStatus.RESULT:
                break;
            case GameStatus.END:
                break;
        }
        if(coroutinename == "null")
        {
            DebugLogWarning("�R���[�`��������ɐU�蕪�����Ă��܂���");
            return;
        }
        stateCoroutine = StartCoroutine(coroutinename);
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
            while (!PhotonNetwork.InRoom)
            {
                //�ڑ��܂őҋ@
                DebugLog("�ڑ��m�F��..");
                yield return null;
            }

            //��Ԃ𑗐M
            initStatus = InitStatus.RESET;
            //���̃v���C���[��ҋ@
            yield return new WaitUntil(() => CheckInitState(InitStatus.RESET));
            DebugLog("�ڑ��m�F!");

        }
        //2.�e�l��������<RESET
        {
            gameState = GameStatus.READY;
            isFazeEnd = false;
            stateCoroutine = null;
            Instance = this;

            //�e�}�l�[�W���[�𐶐�
            //�f�[�^���L�N���X�𐶐�
            if (PhotonNetwork.LocalPlayer.IsMasterClient) //�z�X�g
            {
                //�f�[�^���L�N���X�𐶐�
                var obj = PhotonNetwork.Instantiate("NagatsukaObjects/DataSharingClass", Vector3.zero, Quaternion.identity);//�f�[�^���L�N���X�𐶐�����.
            }
            else                                          //�Q�X�g
            {
                //�f�[�^���L�N���X�����������܂őҋ@
                yield return new WaitUntil(() => gameProgress.dataSharingClass != null);
            }

            //User�N���X����

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
        DebugLog("����������");
        gameState = GameStatus.START;
    }

    #region �X�e�[�g�R���[�`��

    /// <summary>
    /// READY��Ԃ̎��ɌĂ΂��R���[�`��
    /// Init����state��START�ɕω�����܂őҋ@���A
    /// ���̊ԃ��[�U�[�Ƀ��[�h��ʂ�\������
    /// </summary>
    /// <returns></returns>
    IEnumerator StateREADY()
    {
        while(gameState != GameStatus.START)
        {
            //�������ł��邱�Ƃ�\��

            yield return null;
        }

        //�X�e�[�g�R���[�`���̏I������
        ClearCoroutine();
    }

    /// <summary>
    /// START��Ԃ̎��ɌĂ΂��R���[�`��
    /// �Q�[���J�n���O�ɉ��o��m�F���s��
    /// UIManager�ɉ��o��v������
    /// </summary>
    /// <returns></returns>
    IEnumerator StateSTART()
    {
        //�Q�[���J�n�O�ɍs�������E���o���s��
        DebugLog("�X�^�[�g�O�\��");
        //UIManager�̉��o�I���ɂ���ďI���Ăяo��
        while (!isFazeEnd)
        {
            DebugLog("���o��...");
            yield return new WaitForSeconds(3.0f);
            EndFaze();
            yield return null;
        }
        DebugLog("�Q�[���X�^�[�g");
        gameState++;
        //�X�e�[�g�R���[�`���̏I������
        ClearCoroutine();
    }

    /// <summary>
    /// SELECT��Ԃ̎��ɌĂ΂��R���[�`��
    /// ��Q���I���̂��߂�User�N���X�ɑI����Ԃ��w�����A
    /// ��Q���I����ʂ�\������
    /// </summary>
    /// <returns></returns>
    IEnumerator StateSELECT()
    {
        DebugLog("��Q���I���J�n");

        //�z�X�g�Ȃ��Q���𒊑I
        if (PhotonNetwork.IsMasterClient)
        {
            //�e�X�g
            for(int i = 0; i < 4; i++)
            {
                int id = Random.Range(1, 4);
                //��Q���ǉ�
                gameProgress.dataSharingClass.PushID(i == 3 ? 0 : id);
            }
        }
        //�Q�X�g�Ȃ璊�I�܂őҋ@
        else
        {
            yield return new WaitUntil(() => gameProgress.dataSharingClass.ID[gameProgress.dataSharingClass.ID.Count-1] == 0);
        }

        //�I���N���X�𐶐�

        //�I���N���X�ɂ���ďI���Ăяo��
        while (!isFazeEnd)
        {
            //��Q������\��
            var list = gameProgress.dataSharingClass.ID;

            //���Ԑ����ɂ�����ΏI��

            //��Q���I���܂őҋ@
            yield return null;
        }

        DebugLog("��Q���I���I��");

        //�X�e�[�g�R���[�`���̏I������
        ClearCoroutine();
    }

    /// <summary>
    /// PLANT��Ԃ̎��ɌĂ΂��R���[�`��
    /// ��Q���ݒu�̂��߂�User�N���X�ɐݒu��Ԃ��w�����A
    /// ��Q���ݒu��ʂ�\������
    /// </summary>
    /// <returns></returns>
    IEnumerator StatePLANT()
    {
        DebugLog("��Q���ݒu�J�n");
        gameProgress.mapManager.CreativeModeStart();

        while (!isFazeEnd)
        {
            //�ݒu��
            //�ݒu���ꂽ���ǂ�����mapManager������
            if (true)
            {

            }

            //�S�����ݒu�������邩���Ԑ؂�Őݒu����
            if (true)
            {

            }

            yield return null;
        }

        DebugLog("��Q���ݒu�I��");

        //�X�e�[�g�R���[�`���̏I������
        ClearCoroutine();
    }

    #endregion

    #endregion
}
