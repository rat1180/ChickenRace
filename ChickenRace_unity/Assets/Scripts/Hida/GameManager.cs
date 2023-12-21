using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using PhotonMethods;
using ResorceNames;
using ConstList;
using UnityEngine.SceneManagement;

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

    const float DEAD = -1f;
    const int BONUS_SCORE = 1;
    const int BASE_SCORE = 3;
    const int GAME_END_SCORE = 10;

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
        public int userActorNumber;
    }

    [SerializeField, Tooltip("���݂̃Q�[�����")] GameStatus gameState;
    [SerializeField, Tooltip("�i�s���̃X�e�[�g�R���[�`��")] Coroutine stateCoroutine;
    [SerializeField, Tooltip("���̃N���X����n�����A���݂̃t�F�[�Y�I����m�点��ϐ�")] bool isFazeEnd; //Int�^�ɂ��ĕ����̏�ԂɑΉ��o����悤�ɂ��邩��
    [SerializeField, Tooltip("�Q�[���̐i�s�ɕK�v�ȃN���X�̂܂Ƃ�")] GameProgress gameProgress;
    [SerializeField, Tooltip("�f�o�b�O�p�̃��O��\�����邩�ǂ���")] bool isDebug;
    [SerializeField,Tooltip("�f�o�b�O�p�̃e�L�X�g�\���iSet�͔C�Ӂj")] 


    public static GameManager instance;

    #region Unity�C�x���g

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
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
        int index = gameProgress.user.GetIndex();
        if (index == -1) return false;
        //����Ɏ����Ă��邩�m�F
        if (gameProgress.dataSharingClass.ID[index] != 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// �f�[�^���L�N���X�̃^�C�����X�g���玩�g�̏��ʂ𔻒肷��
    /// �Ԃ�l�ŏ��ʂ�߂����A�S�����S�̏ꍇ�̂�0��Ԃ�
    /// ���g������ł���ꍇ��DEAD
    /// </summary>
    /// <returns></returns>
    int CheckRaceRank()
    {
        var times = gameProgress.dataSharingClass.rankTime;
        float mytime = times[gameProgress.userActorNumber];
        int rank = 1;
        int deadcnt = 0;
        for (int i = 0; i < times.Count; i++)
        {
            //���g�̏�Ԃ��`�F�b�N
            if (i == gameProgress.userActorNumber)
            {
                if (mytime == DEAD) return (int)DEAD;
            }

            //�Ώۂ̎��S���`�F�b�N
            if (times[i] == DEAD)
            {
                deadcnt++;
                continue;
            }

            //�^�C�����r
            if (mytime > times[i])
            {
                rank++;
            }
        }

        //�S�����S
        if (deadcnt == times.Count-1)
        {
            rank = 0;
        }

        return rank;
    }

    /// <summary>
    /// ���ʂ�n���Ƃ���ɉ������X�R�A���v�Z���A�Ԃ�
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    int SumScore(int rank)
    {
        //���S���̓|�C���g�Ȃ�
        if (rank == (int)DEAD) return 0;

        //���ʂɉ������{�[�i�X�����Z
        int addScore = (rank == 1) ? BONUS_SCORE : 0;
        return BASE_SCORE + addScore;
    }

    List<int> ScoreCalculation()
    {
        List<int> scores = new List<int>();
        foreach(var player in PhotonNetwork.PlayerList)
        {
            scores.Add(SumScore(player.GetRankStatus()));
        }

        return scores;
    }

    bool GameEnd()
    {
        foreach(var score in gameProgress.dataSharingClass.score)
        {
            if (score >= GAME_END_SCORE) return true;
        }
        return false;
    }

    /// <summary>
    /// ���ׂĂ�Game�nKey�����Z�b�g����
    /// ���������ɍs��
    /// </summary>
    void AllGameKeyReset()
    {
        PhotonNetwork.LocalPlayer.SetGameReadyStatus(false);
        PhotonNetwork.LocalPlayer.SetGameInGameStatus(false);
        PhotonNetwork.LocalPlayer.SetGameEndStatus(false);
    }

    /// <summary>
    /// GameReadyKey��true�ɂ��A�S����true�ł����true��Ԃ�
    /// ���̍ہAGameInGameKey��false�ɖ߂�
    /// </summary>
    /// <returns></returns>
    bool CheckReady()
    {
        PhotonNetwork.LocalPlayer.SetGameReadyStatus(true);
        PhotonNetwork.LocalPlayer.SetGameInGameStatus(false);

        foreach(var player in PhotonNetwork.PlayerList)
        {
            if (!player.GetGameReadyStatus()) return false;
        }
        return true;
    }

    /// <summary>
    /// GameInGameKey��true�ɂ��A�S����true�ł����true��Ԃ�
    /// ���̍ہAGameEndKey��false�ɖ߂�
    /// </summary>
    /// <returns></returns>
    bool CheckInGame()
    {
        PhotonNetwork.LocalPlayer.SetGameInGameStatus(true);
        PhotonNetwork.LocalPlayer.SetGameEndStatus(false);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.GetGameInGameStatus()) return false;
        }
        return true;
    }

    /// <summary>
    /// GameEndKey��true�ɂ��A�S����true�ł����true��Ԃ�
    /// ���̍ہAGameReadyKey��false�ɖ߂�
    /// </summary>
    /// <returns></returns>
    bool CheckEnd()
    {
        PhotonNetwork.LocalPlayer.SetGameEndStatus(true);
        PhotonNetwork.LocalPlayer.SetGameReadyStatus(false);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.GetGameEndStatus()) return false;
        }
        return true;
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

    /// <summary>
    /// ���S���ɌĂ΂��֐�
    /// ���g�̃^�C�������S�萔�ɂ���
    /// </summary>
    public void DeadPlayer()
    {
        gameProgress.dataSharingClass.PushGoalTime(gameProgress.userActorNumber, DEAD);
        EndFaze();
    }

    /// <summary>
    /// �S�[�����ɌĂ΂��֐�
    /// ���̎��_�ł̃^�C�������g�̃^�C���ɕۑ�����
    /// </summary>
    public void GoalPlayer()
    {
        gameProgress.dataSharingClass.PushGoalTime(gameProgress.userActorNumber, (float)PhotonNetwork.Time);
        EndFaze();
    }

    /// <summary>
    /// �z�X�g�ɂ���č쐬���ꂽ�f�[�^���L�N���X���Z�b�g����
    /// </summary>
    /// <param name="datasharingclass"></param>
    public void SetDataSheringClass(DataSharingClass datasharingclass)
    {
        gameProgress.dataSharingClass = datasharingclass;
    }

    /// <summary>
    /// �}�b�v�}�l�[�W���[��v������
    /// </summary>
    /// <returns></returns>
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

    void DebugInfomation()
    {

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
                coroutinename = "StateEND";
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

            //�i�s�ҋ@
            yield return new WaitUntil(() => CheckReady());
            DebugLog("�ڑ��m�F!");

        }

        //�}�X�^�[�̊J�n��ҋ@
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("�J�n�܂őҋ@");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.S));
            //��Ԃ𑗐M
            PhotonNetwork.LocalPlayer.SetGameInGameStatus(true);
        }
        else
        {
            Debug.Log("�J�n�܂őҋ@");
            yield return new WaitUntil(() => PhotonNetwork.MasterClient.GetGameInGameStatus());
        }

        //���̃v���C���[��ҋ@
        yield return new WaitUntil(() => CheckInGame());
        DebugLog("�l�̏������J�n");

        //2.�e�l��������<RESET
        {
            gameProgress = new GameProgress();
            gameState = GameStatus.READY;
            isFazeEnd = false;
            stateCoroutine = null;
            instance = this;
            gameProgress.userActorNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;

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

            DebugLog("�e�l�̏���������");
        }
        //3.�����������E���v���C���[��ҋ@<WAIT
        {
            DebugLog("�����܂őҋ@��");

            //�����ɃQ�[���J�n
            //���̃v���C���[��ҋ@
            yield return new WaitUntil(() => CheckEnd());
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
        //�i�s�ҋ@
        yield return new WaitUntil(() => CheckReady());

        //�����ς݂̏�Q�����Đ���
        //gameProgress.mapManager

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
            DebugLog("���I��ҋ@");
            yield return new WaitUntil(() => gameProgress.dataSharingClass.ID.Count != 0 ? true : false);
            yield return new WaitUntil(() => gameProgress.dataSharingClass.ID[gameProgress.dataSharingClass.ID.Count - 1] == 0);
        }

        DebugLog("���I�I��");

        //�i�s�ҋ@
        yield return new WaitUntil(() => CheckInGame());

       //�I���N���X�𐶐�
       gameProgress.user.GenerateMouse(0);

        //�I���N���X�ɂ���ďI���Ăяo��
        while (!isFazeEnd)
        {
            //��Q������\��
            List<int> list = new List<int>();

            foreach (var id in gameProgress.dataSharingClass.ID)
            {
                list.Add(id);
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

                    //�A�C�e����Index���Z�b�g
                    gameProgress.user.SetIndex(random);
                }
            }

            //�}�E�X�����Q����񂪑����Auser�ɓn��ΏI��
            if (CheckUserIsHave())
            {
                //User�ɏ�Q��ID��n��
                gameProgress.user.SetItemId(gameProgress.dataSharingClass.ID[gameProgress.user.GetIndex()]);
                //User����Index���󂯎��A����Index�ɉ������A�C�e�������X�g����폜
                gameProgress.dataSharingClass.ResetID(gameProgress.user.GetIndex());
                EndFaze();
            }

            yield return null;
        }

        //�}�E�X�폜
        gameProgress.user.DestroyMouse();

        //��Q�������Z�b�g


        DebugLog("�I���I���ҋ@");
        //�S���̏�Q���I���܂őҋ@
        while (!CheckEnd())
        {
            //��Q������\��
            List<int> list = new List<int>();

            foreach (var id in gameProgress.dataSharingClass.ID)
            {
                list.Add(id);
            }

            gameProgress.uiManager.PushID(list);

            yield return null;
        }

        DebugLog("��Q���I���I��");
        gameState++;

        //����̑I���t�F�[�Y�J�n����
        if (PhotonNetwork.IsMasterClient) gameProgress.dataSharingClass.ResetIDList();

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
        //�i�s�ҋ@
        yield return new WaitUntil(() => CheckReady());
        
        gameProgress.mapManager.CreativeModeStart();

        //�}�E�X����
        gameProgress.user.GenerateMouse(gameProgress.user.GetItemId());


        //�i�s�ҋ@
        yield return new WaitUntil(() => CheckInGame());
        while (!isFazeEnd)
        {
            //�ݒu��
            //�ݒu���ꂽ���ǂ�����mapManager������
            if (gameProgress.mapManager.IsInstallReference())
            {
                DebugLog("�ݒu����");
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

        DebugLog("�ݒu�I���ҋ@");
        //�S���̏�Q���I���܂őҋ@
        yield return new WaitUntil(() => CheckEnd());

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
        //�i�s�ҋ@
        yield return new WaitUntil(() => CheckReady());

        //�L�����̏o��
        gameProgress.user.GeneratePlayer();

        DebugLog("READY���o");

        DebugLog("���[�X�X�^�[�g");
        //�i�s�ҋ@
        yield return new WaitUntil(() => CheckInGame());
        //�L�����̑���̃��b�N������

        //�v���C���̉��o
        while (!isFazeEnd)
        {
            //���[�X���̕\���A���o

            DebugLog("���[�X��");

            if (Input.GetKeyDown(KeyCode.G)) GoalPlayer();
            if(Input.GetKeyDown(KeyCode.U)) DeadPlayer();

            yield return null;
        }

        //�i�s�ҋ@
        yield return new WaitUntil(() => CheckEnd());

        //�L�������폜
        gameProgress.user.DestroyPlayer();

        gameState++;

        //�X�e�[�g�R���[�`���̏I������
        ClearCoroutine();
    }

    IEnumerator StateRESULT()
    {
        DebugLog("���U���g�t�F�[�Y�J�n");
        //�i�s�ҋ@
        yield return new WaitUntil(() => CheckReady());

        //���ʂ̌v�Z
        int rank = CheckRaceRank();
        PhotonNetwork.LocalPlayer.SetRankStatus(rank);

        //�i�s�ҋ@
        yield return new WaitUntil(() => CheckInGame());


        //�X�R�A�̌v�Z
        var scorelist = ScoreCalculation();
        gameProgress.dataSharingClass.PushScore(gameProgress.userActorNumber, scorelist[gameProgress.userActorNumber]);

        DebugLog("���ʁA�X�R�A�̔��f���o");
        yield return new WaitForSeconds(2.0f);

        DebugLog("���o�I��");
        //�i�s�ҋ@
        yield return new WaitUntil(() => CheckEnd());


        if (GameEnd())
        {
            DebugLog("�Q�[���I��");
            gameState = GameStatus.END;
        }
        else
        {
            DebugLog("�I���t�F�[�Y�ɕԂ�");
            gameState = GameStatus.SELECT;
        }

        //�X�e�[�g�R���[�`���̏I������
        ClearCoroutine();

    }

    IEnumerator StateEND()
    {
        //�I�����o

        yield return new WaitForSeconds(2.0f);

        //�l�b�g���[�N����ؒf
        PhotonNetwork.Disconnect();

        //�^�C�g���Ɉړ�
        SceneManager.LoadScene(SceneNames.Lobby.ToString());
    }

    #endregion

    #endregion
}
