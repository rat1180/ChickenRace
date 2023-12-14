using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using PhotonMethods;
using ResorceNames;

public class GameManager : MonoBehaviour
{
    enum GameStatus
    {
        SLEEP,
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

    enum InGameStatus
    {
        NONE,
        READY,
        INGAME,
        END,
    }

    /// <summary>
    /// �Q�[���̐i�s�ɕK�v�ȃ}�l�[�W���[�����܂Ƃ߂��N���X
    /// </summary>
    [System.Serializable]
    public class GameProgress
    {
        public MapManager mapManager;
        public UIManager uiManager;
        public DataSharingClass dataSharingClass;
        public User user;
    }

    [SerializeField, Tooltip("���݂̃Q�[�����")] GameStatus gameState;
    [SerializeField, Tooltip("�i�s���̃X�e�[�g�R���[�`��")] Coroutine stateCoroutine;
    [SerializeField, Tooltip("���̃N���X����n�����A���݂̃t�F�[�Y�I����m�点��ϐ�")] bool isFazeEnd; //Int�^�ɂ��ĕ����̏�ԂɑΉ��o����悤�ɂ��邩��
    [SerializeField, Tooltip("�Q�[���̐i�s�ɕK�v�ȃN���X�̂܂Ƃ�")] GameProgress gameProgress;
    [SerializeField, Tooltip("�f�o�b�O�p�̃��O��\�����邩�ǂ���")] bool isDebug;


    public static GameManager instance;

    #region Unity�C�x���g

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameStatus.SLEEP;
        StartCoroutine(GameInit());
    }

    // Update is called once per frame
    void Update()
    {
        GameLoop();

        if(Input.GetKeyDown(KeyCode.RightArrow)){
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
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value.GetInitStatus() != (int)status) return false;
        }
        return true;
    }

    bool CheckInGameState(InGameStatus status)
    {
        //�S���̏�������Ԃ��m�F
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value.GetInGameStatus() != (int)status) return false;
        }
        return true;
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


    /// <summary>
    /// �Ώۂ̃��X�g����Index�̒l���擾����
    /// ���X�g���Z�b�g����Ă��Ȃ��Ƃ��̓G���[�Ƃ���-1��Ԃ�
    /// </summary>
    /// <param name="id_list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    int CheckObstacleIndex(List<int> id_list, int index)
    {
        if (id_list.Count == 0) return -1;

        return id_list[index];
    }

    /// <summary>
    /// User���I�����I���A����ɃA�C�e���������Ă��邩���m�F����
    /// </summary>
    /// <returns></returns>
    bool CheckUserIsHave()
    {
        //����Ɏ����Ă��邩�m�F
        if (/*gameProgress.user. != -1*/false)
        {
            PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.END);
            return true;
        }
        return false;
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

    /// <summary>
    /// ��Q���I���t�F�[�Y�Ń}�E�X���I�������I�u�W�F�N�g���擾����
    /// ���X�g�ɑΉ�����Index�����炢�A���X�g����0(�I���ς�)�łȂ��ꍇ�ɑI���m��Ƃ���
    /// �Ή�����ID��0�̎���false,0�o�Ȃ��Ƃ���true��Ԃ�
    /// </summary>
    /// <param name="id"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool MouseSelected(int index)
    {
        var id = CheckObstacleIndex(gameProgress.dataSharingClass.ID, index);
        if (id == -1 || id == 0) return false;

        gameProgress.dataSharingClass.ResetID(index);
        //gameProgress.user = id;
        return true;
    }

    public void DeadPlayer()
    {
        EndFaze();
    }

    public void SetDataSheringClass(DataSharingClass datasharingclass)
    {
        gameProgress.dataSharingClass = datasharingclass;
    }

    public GameObject GetMapManager()
    {
        return gameProgress.mapManager.gameObject;
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
            case GameStatus.SLEEP:
                return;
                break;
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
                coroutinename = "StateRACE";
                break;
            case GameStatus.RESULT:
                coroutinename = "StateRESULT";
                break;
            case GameStatus.END:
                break;
        }
        if(coroutinename == "null")
        {
            DebugLogWarning("�R���[�`��������ɐU�蕪�����Ă��܂���");
            return;
        }
        DebugLog(coroutinename);
        stateCoroutine = StartCoroutine(coroutinename);
    }

    #endregion

    #region �R���[�`��

    IEnumerator GameInit()
    {
        //�������g���擾
        var localplayer = PhotonNetwork.LocalPlayer;
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
            initStatus = InitStatus.CONECT;
            localplayer.SetInitStatus((int)initStatus);

            //���̃v���C���[��ҋ@
            yield return new WaitUntil(() => CheckInitState(InitStatus.CONECT));
            DebugLog("�ڑ��m�F!");

        }

        //�}�X�^�[�̊J�n��ҋ@
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("�J�n�܂őҋ@");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.S));
            //��Ԃ𑗐M
            initStatus = InitStatus.RESET;
            localplayer.SetInitStatus((int)initStatus);
        }
        else
        {
            Debug.Log("�J�n�܂őҋ@");
            yield return new WaitUntil(() => PhotonNetwork.MasterClient.GetInitStatus() == (int)InitStatus.RESET);
            //��Ԃ𑗐M
            initStatus = InitStatus.RESET;
            localplayer.SetInitStatus((int)initStatus);
        }

        //���̃v���C���[��ҋ@
        yield return new WaitUntil(() => CheckInitState(InitStatus.RESET));

        //2.�e�l��������<RESET
        {
            gameProgress = new GameProgress();
            gameState = GameStatus.READY;
            isFazeEnd = false;
            stateCoroutine = null;
            instance = this;

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
            var user_class = Instantiate((GameObject)Resources.Load("User"), Vector3.zero, Quaternion.identity);
            gameProgress.user = user_class.GetComponent<User>();

            //MapManager�𐶐�
            var map_class = Instantiate((GameObject)Resources.Load("MapManager"), Vector3.zero, Quaternion.identity);
            gameProgress.mapManager = map_class.GetComponent<MapManager>();

            //UIManager������
            gameProgress.uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

            //��Ԃ𑗐M
            initStatus = InitStatus.WAIT;
            localplayer.SetInitStatus((int)initStatus);
            //���̃v���C���[��ҋ@
            yield return new WaitUntil(() => CheckInitState(InitStatus.WAIT));
        }
        //3.�����������E���v���C���[��ҋ@<WAIT
        {
            //�����ɃQ�[���J�n
            //��Ԃ𑗐M
            initStatus = InitStatus.START;
            localplayer.SetInitStatus((int)initStatus);
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
            for (int i = 0; i < 4; i++)
            {
                int id = Random.Range(1, 4);
                //��Q���ǉ�
                gameProgress.dataSharingClass.PushID(i == 3 ? 0 : id);
            }
        }
        //�Q�X�g�Ȃ璊�I�܂őҋ@
        else
        {
            yield return new WaitUntil(() => gameProgress.dataSharingClass.ID[gameProgress.dataSharingClass.ID.Count - 1] == 0);
        }

        //�I���N���X�𐶐�
        gameProgress.user.GenerateMouse(0);
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.READY);

        //�I���N���X�ɂ���ďI���Ăяo��
        while (!isFazeEnd)
        {
            //��Q������\��
            List<OBSTACLE_IMAGE_NAMES> list = new List<OBSTACLE_IMAGE_NAMES>();

            foreach(var id in gameProgress.dataSharingClass.ID)
            {
                list.Add((OBSTACLE_IMAGE_NAMES)id);
            }

            gameProgress.uiManager.PushID(list);

            //���Ԑ����ɂ�����ΏI��
            if (false)
            {
                //�}�E�X�폜
                gameProgress.user.DestroyMouse();

                //���̃A�C�e���n��
                {
                    int random;
                    while (true)
                    {
                        random = Random.Range(0, gameProgress.dataSharingClass.ID.Count - 1);
                        if (gameProgress.dataSharingClass.ID[random] != 0) break;
                    }

                    //gameProgress.user. = gameProgress.dataSharingClass.ID[random];
                }
            }

            //�}�E�X�����Q����񂪑����Auser�ɓn��ΏI��
            if (CheckUserIsHave())
            {
                EndFaze();
            }

            //�e�X�g�p
            if (Input.GetKeyDown(KeyCode.S)) EndFaze();

            yield return null;
        }

        //�}�E�X�폜
        gameProgress.user.DestroyMouse();

        //��ԑ��M
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.END);

        //�S���̏�Q���I���܂őҋ@
        yield return new WaitUntil(() => CheckInGameState(InGameStatus.END));

        DebugLog("��Q���I���I��");
        gameState++;

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
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.READY);
        gameProgress.mapManager.CreativeModeStart();

        //�}�E�X����
        gameProgress.user.GenerateMouse(1);

        while (!isFazeEnd)
        {
            //�ݒu��
            //�ݒu���ꂽ���ǂ�����mapManager������
            if (gameProgress.mapManager.IsInstallReference())
            { 
                EndFaze();
            }

            //���Ԑ؂�Őݒu�I��
            if (false)
            {
                EndFaze();
            }


            yield return null;
        }

        //�ݒu�I���w��
        gameProgress.mapManager.CreativeModeEnd();
        //�}�E�X�폜
        gameProgress.user.DestroyMouse();

        //��ԑ��M
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.END);

        //�S���̏�Q���I���܂őҋ@
        yield return new WaitUntil(() => CheckInGameState(InGameStatus.END));

        DebugLog("��Q���ݒu�I��");
        gameState++;

        //�X�e�[�g�R���[�`���̏I������
        ClearCoroutine();
    }

    /// <summary>
    /// RACE��Ԃ̎��ɌĂ΂��R���[�`��
    /// �L�������S�ďI������܂őҋ@���A
    /// �I�����Ɍ��ʂ��܂Ƃ߂�
    /// </summary>
    /// <returns></returns>
    IEnumerator StateRACE()
    {
        DebugLog("���[�X�t�F�[�Y�J�n");
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.READY);

        //�S�����ҋ@��ԂɂȂ�܂őҋ@
        yield return new WaitUntil(() => CheckInGameState(InGameStatus.READY));

        //�L�����̏o��
        gameProgress.user.GeneratePlayer();

        DebugLog("READY���o");

        DebugLog("���[�X�X�^�[�g");
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.INGAME);
        //�L�����̑���̃��b�N������

        //�v���C���̉��o
        while (!isFazeEnd)
        {
            //���[�X���̕\���A���o

            DebugLog("���[�X��");

            yield return null;
        }

        //���g�̏�Ԃ𑗐M
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.END);

        while (!CheckInGameState(InGameStatus.END))
        {
            //���S��A�S�[����̊ϐ�

            yield return null;
        }

        //�X�R�A�̑��M

        //�X�e�[�g�R���[�`���̏I������
        ClearCoroutine();
    }

    IEnumerator StateRESULT()
    {
        while (true)
        {
            yield return null;
        }
        
    }

    #endregion

    #endregion
}
