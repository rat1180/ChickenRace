using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResorceNames;
using Photon.Pun;
using Photon.Realtime;


public class UIManager : MonoBehaviourPunCallbacks
{
    #region �񋓌^
    enum ResultCharacterChild
    {
        NAME,
        SCORE,
        RANK,
        Spotlight,
    }

    enum CanvasChild
    {
        LoaingImage,
        ResultPanel,
        OsaraObject,
    }
    enum UIManagerChild { 
        ImageObjects,
        CanvasUI,
    }

    #endregion

    #region �萔�l
    const int IMAGE_OBSTACLE_NUM = 4;//�I�������Q���̉摜�̐�.
    const int MAX_PLAYERS = 3;
    #endregion

    private Vector2[] imagePosition =
    {
        new Vector2(-5,3),
        new Vector2(5, 3),
        new Vector2(-5,-5),
        new Vector2(5,-5)
    };

    private GameObject osaraPanel;//�I�׃e�L�X�g�Ƃ��M.
    private GameObject imageObjects;
    private GameObject resultPanel;
    //private GameObject resultCharacters;//���U���g��ʂ̃L�����N�^�[(Name,Score,Rank�\��)
    private GameObject[] resultCharacters;
    private Text raceCountText;//�扽���[�X����\������e�L�X�g.
    private List<int> id;

    public GameObject imageObstacle;

    //Loading�A�j���[�V�����֘A.
    private GameObject loaingImage;


    [Header("�f�o�b�O�p(���ۂ̃Q�[���ł͎g�p���Ȃ�)")]
    public string[] names = new string[3];
    public List<int> beScore;
    public List<int> addscoreTest;
    public List<int> testID;

    private void Awake()
    {
        imageObjects = transform.GetChild((int)UIManagerChild.ImageObjects).gameObject;
        GameObject canvasUI = transform.GetChild((int)UIManagerChild.CanvasUI).gameObject;
        
        
        loaingImage = canvasUI.transform.GetChild((int)CanvasChild.LoaingImage).gameObject;
        resultPanel = canvasUI.transform.GetChild((int)CanvasChild.ResultPanel).gameObject;
        osaraPanel = canvasUI.transform.GetChild((int)CanvasChild.OsaraObject).gameObject;

        resultCharacters = new GameObject[3];
        for(int i = 0; i < 3; i++)
        {
            resultCharacters[i] = resultPanel.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject;
        }
        
        raceCountText= resultPanel.transform.GetChild(1).GetComponent<Text>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //PushID(testID);
            //StartCoroutine(Result(beScore,5));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            //FinishSelect();
            //StartCoroutine(Result(beScore, addscoreTest));
        }
    }

    /// <summary>
    /// �S������Q���̑I�����I��������UI���\���ɂ���.
    /// </summary>
    public void FinishSelect()
    {
        //imageObjects.SetActive(false);
        for (int i = 0; i < IMAGE_OBSTACLE_NUM; i++)//�q�v�f�S�폜(childCount�ɂ���Ɛ���������indexout�G���[���N�������ߒ��ڎw��).
        {
            Destroy(imageObjects.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// GameManager����ID������֐�
    /// </summary>
    public void PushID(List<int> iD)
    {
        id = new List<int>();
        id = iD;

        ChangeObstacleImage();
    }

    /// <summary>
    /// 4�̉摜��ς���
    /// ������ID���t�^����
    /// </summary>
    public void ChangeObstacleImage()
    {
        for (int i = 0; i < IMAGE_OBSTACLE_NUM; i++)
        {
            if (imageObjects.transform.childCount != IMAGE_OBSTACLE_NUM)//4�̉摜�I�u�W�F�N�g����������Ă��邩�`�F�b�N.
            {
                GameObject gameObject = Instantiate(imageObstacle, new Vector3(), Quaternion.identity, imageObjects.transform);
                gameObject.transform.position = imagePosition[i];

                gameObject.GetComponent<SpriteRenderer>().sprite =
                ResourceManager.instance.GetObstacleImage(id[i]);//�摜��ύX.
                gameObject.AddComponent<ObstacleImage>().id = i;
            }
            else//4�������Ă�����ID���݂̂�ύX����.
            {
                imageObjects.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = 
                ResourceManager.instance.GetObstacleImage(id[i]);//�摜��ύX.

                //�R���|�[�l���g�����݂���΂��̂܂�ID��������
                imageObjects.transform.GetChild(i).gameObject.GetComponent<ObstacleImage>().id = i;
            }
        }
    }

    /// <summary>
    /// index���w�肵�Ċ������(0)�ɂ���
    /// </summary>
    public void SoldOut(int index)
    {
        imageObjects.transform.GetChild(index).GetComponent<Image>().sprite =
            ResourceManager.instance.GetObstacleImage(OBSTACLE_IMAGE_NAMES.Kanbaipop);//�����摜�ɕύX.
        imageObjects.transform.GetChild(index).GetComponent<ObstacleImage>().id = 0;
    }

    #region ���U���g�ύX�֘A

    /// <summary>
    /// ���ɑΉ�����
    /// �����ɉ��Z��X�R�A���w��A�X�R�A�������A�N�e�B�u�ɂ���
    /// </summary>
    /// <param name="beforeScore"></param>
    /// <param name="addScore"></param>
    /// <returns></returns>
    public IEnumerator Result(List<int> score,int raceCount)
    {
        ActiveResultPanel(true);
        //ActiveCharacters(score.Count);
        ActiveCharacters(PhotonNetwork.CurrentRoom.PlayerCount);
        raceCountText.text = "��" + raceCount + "���[�X�I������";
        SoundManager.instance.SimplePlaySE(SoundName.SECode.SE_Result);
        //Player�̐������[�v���ď�������.
        int i = 0;
        //for (i = 0; i < score.Count; i++)
        {
            foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
            {
                resultCharacters[i].transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text 
                    = player.NickName;
                //resultCharacters[i].transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text
                //    = names[i];
                i++;
            }
        }

        //����\������.
        for (i = 0; i < score.Count; i++)
        {
           for(int j = 0; j < score[i]; j++)//�X�R�A���q�v�f���A�N�e�B�u�ɂ���.
            {
                //�����擾.
                GameObject gameObject = resultCharacters[i].transform.GetChild((int)ResultCharacterChild.SCORE).transform.GetChild(j).gameObject;
                if (!gameObject.activeSelf)//�܂��A�N�e�B�u�ɂȂ��Ă��Ȃ���΃A�N�e�B�u��.
                {
                    gameObject.SetActive(true);
                }
                else//�A�N�e�B�u�ł���΃A�j���[�V�������Đ������Ȃ�.
                {
                    gameObject.GetComponent<Animator>().enabled = false;
                }
                
            }            
        }
        yield return new WaitForSeconds(2f);
        ChangeRank(score);
        yield return new WaitForSeconds(3f);
        ActiveResultPanel(false);
        //�I�����������Ȃ�R�R
        ActiveResultPanel(false);
        Debug.Log("�R���[�`�����イ��傤��");
        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// ���[�X�I�������U���g��ʂ�\�����邽�߂�GameManager����Ăяo���R���[�`��
    /// �����ɕύX�O�̃X�R�A�E���Z���邷��X�R�A�A���[�X�����w��.
    /// </summary>
    public IEnumerator Result(List<int> beforeScore, List<int> addScore,int raceCount)
    {
        ActiveResultPanel(true);
        ActiveCharacters(beforeScore.Count);
        raceCountText.text = "��" + raceCount + "���[�X�I������";
        //Player�̐������[�v���ď�������.
        int i = 0;
        foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
        { 
                resultCharacters[i].transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text 
                = player.NickName;
                resultCharacters[i].transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                    = "SCORE:" + beforeScore[i].ToString();
        }

        //�X�R�A�̃e�L�X�g��ύX����(��ChangeScoreText).
        List<int> score = beforeScore;
        List<int> adscore = addScore;
        int cnt = 0;
        while (true)//�S�v���C���[�̉��Z���I���܂Ń��[�v����(������ۂ�break).
        {
            for (i = 0; i < score.Count; i++)
            {
                if (adscore[i] >= 0)//���Z���镪�̃X�R�A��0�łȂ���΃e�L�X�g�ύX����.
                {
                    adscore[i]--;//���Z������-1����.
                    score[i]++;  //��������+1����.
                    if (adscore[i] <= 0)//���Z����X�R�A��0�ɂȂ�����J�E���g�𑝂₷.
                    {
                        cnt++;
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
            if (cnt == score.Count)//�ő�l�����S�ẴX�R�A���Z���I�������烋�[�v�𔲂��ď��ʂ�\������.
            {
                ChangeRank(score);
                break;
            }
        }


        yield return new WaitForSeconds(3f);
        ActiveResultPanel(false);
        //�I�����������Ȃ�R�R
        ActiveResultPanel(false);
        Debug.Log("�R���[�`�����イ��傤��");
        yield return new WaitForSeconds(0.1f);

    }



    /// <summary>
    /// Player�̐����L�����N�^�[��\��������֐�
    /// ������Player�̐����w��.
    /// </summary>
    private void ActiveCharacters(int cnt)
    {
        for (int i = 0; i < 3; i++)
        {

            if (i < cnt)
            {
                resultCharacters[i].SetActive(true);
            }
            else
            {
                resultCharacters[i].SetActive(false);
            }
        }
    }


    /// <summary>
    /// �X�R�A�ɉ����ď��ʂ�ύX����֐�
    /// �����ɕύX��̃X�R�A���w��.
    /// </summary>
    void ChangeRank(List<int> score)
    {
        for (int i = 0; i < score.Count; i++)
        {
            resultCharacters[i].transform.GetChild((int)ResultCharacterChild.RANK).
                gameObject.SetActive(true);
            int rank = 1;
            for (int j = 0; j < score.Count; j++)
            {
                if (score[i] < score[j])//��������ׂ鎖�ɂȂ邪�A���Ȃ�,
                {
                    //Debug.Log(i + "�Ԗ�" + "Score1:" + score[i] + "��" + "Score2" + score[j + 1]);
                    rank++;
                }
            }
            resultCharacters[i].transform.GetChild((int)ResultCharacterChild.RANK).GetComponent<Text>().text
                    = rank.ToString() + "��";
        }
    }

    /// <summary>
    /// ���U���g�p�l���̕\����\�����s���֐�
    /// �����ɐ؂芷��������(trune�Efalse)���w��
    /// </summary>
    /// <param name="flg"></param>
    private void ActiveResultPanel(bool flg)
    {
        resultPanel.SetActive(flg);
        for (int i = 0; i < 3; i++)
        {
            resultCharacters[i].transform.GetChild((int)ResultCharacterChild.RANK).
                gameObject.SetActive(false);
        }
    }

    #endregion

    /// <summary>
    /// �Q�[���I����(�X�R�A��3�ɂȂ����l���o����)���U���g���ė��p���Č��ʕ\������.
    /// �����ɍŏI�X�R�A���X�g���w��.
    /// </summary>
    public void EndGame(List<int> score)
    {
        int cnt = 0;//�����l���J�E���g�p.
        string name = "";//���������l�̖��O������.
        resultPanel.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < score[i]; j++)///�����ĕ\��
            {
                gameObject.SetActive(true);
                gameObject.GetComponent<Animator>().enabled = true;//�A�j���[�V�������Đ���.
            }
            if (score[i] >= 3)//�X�R�A��3�ȏ�̐l�̓X�|�b�g���C�g�𓖂Ă�.
            {
                resultCharacters[i].transform.GetChild((int)ResultCharacterChild.Spotlight).
                gameObject.SetActive(true);
                cnt++;
                name = resultCharacters[i].transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text;
            }
        }
        if (cnt == 1)//1�l���������������ꍇ�A���O��\��.
        {
            raceCountText.text = name + "�̏����I�I";
        }
        else
        {
            raceCountText.text = cnt + "�l�̏����I�I";
        }
    }

    /// <summary>
    /// �ʐM���A�j���\�V�����̕\���E��\�����s���֐�
    /// ������true�Efalse���w�肵�A�؂芷����
    /// </summary>
    /// <param name="flg"></param>
    public void ActiveLoaingImage(bool flg)
    {
        loaingImage.SetActive(flg);
    }



    void PushNameTest()
    {
        //Player�̐������[�v���ď�������.
        int i = 0;
        foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
        {
            resultCharacters[i].transform.GetChild(0).GetComponent<Text>().text = player.NickName;
        }
    }

    /// <summary>
    /// ���M�ƑI�׃e�L�X�g�̕\����؂芷����֐�
    /// ������bool�^���w�肵�A�A�N�e�B�u��؂芷����.
    /// </summary>
    public void SwitchActiveOsara(bool flg)
    {
        osaraPanel.SetActive(flg);
    }
}
