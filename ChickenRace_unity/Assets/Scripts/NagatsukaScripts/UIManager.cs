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
        UPSCORE,
        RANK,
    }

    enum CanvasChild
    {
        ImageObjects,
        RaceCount,
        LoaingImage
    }
    #endregion

    #region �萔�l
    const int IMAGE_OBSTACLE_NUM = 4;//�I�������Q���̉摜�̐�.
    #endregion

    private Vector2[] imagePosition =
    {
        new Vector2(-5,3),
        new Vector2(5, 3),
        new Vector2(-5,-5),
        new Vector2(5,-5)
    };

    private GameObject imageObjects;
    private GameObject resultPanel;
    private GameObject resultCharacters;//���U���g��ʂ̃L�����N�^�[(Name,Score,Rank�\��)
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
        imageObjects = transform.Find("ImageObjects").gameObject;
        resultPanel = transform.GetChild(0).transform.Find("ResultPanel").gameObject;
        resultCharacters = resultPanel.transform.GetChild(0).gameObject;
        raceCountText= resultPanel.transform.GetChild(1).GetComponent<Text>();
        loaingImage = transform.GetChild(0).transform.GetChild((int)CanvasChild.LoaingImage).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //PushID(testID);
            //StartCoroutine(Result(beScore, addscoreTest));
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
        //imageObjects.SetActive(true);

        ChangeObstacleImage();
    }

    /// <summary>
    /// 4�̉摜��ς���
    /// ������ID���t�^����
    /// </summary>
    public void ChangeObstacleImage()
    {
        //for (int i = 0; i < imageObjects.transform.childCount; i++)
        for (int i = 0; i < IMAGE_OBSTACLE_NUM; i++)
        {
            if (imageObjects.transform.childCount != IMAGE_OBSTACLE_NUM)//4�̉摜�I�u�W�F�N�g����������Ă��邩�`�F�b�N.
            {
                GameObject gameObject = Instantiate(imageObstacle, new Vector3(), Quaternion.identity, imageObjects.transform);
                gameObject.transform.position = imagePosition[i];
                //gameObject.GetComponent<RectTransform>().localPosition = imagePosition[i];

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

    public IEnumerator Result(List<int> beforeScore, List<int> addScore)
    {
        Debug.Log("GameManager��916�s�ڕt�߂̃o�O�΍��p�A�C��������֐����Ə���");
        return null;
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
        for (i = 0; i < beforeScore.Count; i++)
        {
            //foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
            {
                //resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text = player.NickName;
                resultCharacters.transform.GetChild(i).transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text
                    = names[i];
                resultCharacters.transform.GetChild(i).transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                    = "SCORE:" + beforeScore[i].ToString();
                resultCharacters.transform.GetChild(i).transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
                    = "+" + addScore[i].ToString();
            }
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

                    //�X�R�A�̃e�L�X�g��ύX����.
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                            = "SCORE:" + score[i].ToString();
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
                    = "+" + adscore[i].ToString();
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
                resultCharacters.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                resultCharacters.transform.GetChild(i).gameObject.SetActive(false);
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
            int rank = 1;
            for (int j = 0; j < score.Count; j++)
            {
                if (score[i] < score[j])//��������ׂ鎖�ɂȂ邪�A���Ȃ�,
                {
                    //Debug.Log(i + "�Ԗ�" + "Score1:" + score[i] + "��" + "Score2" + score[j + 1]);
                    rank++;
                }
            }
            resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.RANK).GetComponent<Text>().text
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
    }

    #endregion

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
            resultCharacters.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Text>().text = player.NickName;
        }
    }
}
