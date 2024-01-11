using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResorceNames;
using Photon.Pun;
using Photon.Realtime;


public class UIManager : MonoBehaviourPunCallbacks
{
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
        ResultPanel,
    }

    private GameObject imageObjects;
    private GameObject resultPanel;
    private GameObject resultCharacters;//���U���g��ʂ̃L�����N�^�[(Name,Score,Rank�\��)
    private List<int> id;


    [Header("�f�o�b�O�p(���ۂ̃Q�[���ł͎g�p���Ȃ�)")]
    public string[] names = new string[3];
    public List<int> beScore;
    public List<int> addscoreTest;
    public List<OBSTACLE_OBJECT> testID;

    private void Start()
    {
        imageObjects = transform.GetChild(0).transform.GetChild((int)CanvasChild.ImageObjects).gameObject;
        resultPanel =  transform.GetChild(0).transform.GetChild((int)CanvasChild.ResultPanel).gameObject;
        resultCharacters = resultPanel.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //List<int> test = testID;
            //PushID(testID);
            StartCoroutine(Result(beScore, addscoreTest));
        }
    }

    /// <summary>
    /// �S������Q���̑I�����I��������UI���\���ɂ���.
    /// </summary>
    public void FinishSelect()
    {
        imageObjects.SetActive(false);
    }

    /// <summary>
    /// GameManager����ID������֐�
    /// </summary>
    public void PushID(List<int> iD)
    {
        id = new List<int>();
        id = iD;
        imageObjects.SetActive(true);
        ChangeObstacleImage();
    }

    /// <summary>
    /// 4�̉摜��ς���
    /// ������ID���t�^����
    /// </summary>
    public void ChangeObstacleImage()
    {
        for (int i = 0; i < imageObjects.transform.childCount; i++)
        {
            imageObjects.transform.GetChild(i).GetComponent<Image>().sprite =
            //imageObjects.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite =
            ResourceManager.instance.GetObstacleImage(id[i]);//�摜��ύX.

            //�R���|�[�l���g�����݂��Ȃ���Βǉ�����ID�������A�t�ɑ��݂���΂��̂܂�ID��������
            if (imageObjects.transform.GetChild(i).gameObject.GetComponent<ObstacleImage>() == null)
            {
                imageObjects.transform.GetChild(i).gameObject.AddComponent<ObstacleImage>().id = i;
            }
            else
            {
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
        imageObjects.transform.GetChild(index).gameObject.GetComponent<ObstacleImage>().id = 0;
    }

    #region ���U���g�ύX�֘A

    /// <summary>
    /// ���[�X�I�������U���g��ʂ�\�����邽�߂�GameManager����Ăяo���R���[�`��
    /// �����ɕύX�O�̃X�R�A�E���Z���邷��X�R�A���w��.
    /// </summary>
    IEnumerator Result(List<int> beforeScore, List<int> addScore)
    {
        ActiveCharacters(names.Length);
        //Player�̐������[�v���ď�������.
        int i = 0;
        for (i = 0; i < beforeScore.Count; i++)
        {
            //foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
            {
                //resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text = player.NickName;
                resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text
                    = names[i];
                resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                    = "SCORE:" + beforeScore[i].ToString();
                resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
                    = "+" + addScore[i].ToString();
            }
        }
        //StartCoroutine(ChangeScoreText(beforeScore, addScore));

        //�X�R�A�̃e�L�X�g��ύX����(��ChangeScoreText).
        List<int> score = beforeScore;
        List<int> adscore = addScore;
        int cnt = 0;
        while (true)
        {
            for (i = 0; i < score.Count; i++)
            {
                if (adscore[i] != 0)//���Z���镪�̃X�R�A��0�łȂ���΃e�L�X�g�ύX����.
                {
                    adscore[i]--;//���Z������-1����.
                    score[i]++;  //��������+1����.

                    //�X�R�A�̃e�L�X�g��ύX����.
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                            = "SCORE:" + score[i].ToString();
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
                    = "+" + adscore[i].ToString();
                    if (adscore[i] == 0)//���Z����X�R�A��0�ɂȂ�����J�E���g�𑝂₷.
                    {
                        cnt++;
                    }
                }
            }
            //break;

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

        Debug.Log("�R���[�`�����イ��傤��");
        yield return new WaitForSeconds(0.1f);

    }



  //  public void Result(List<int> beforeScore, List<int> addScore)
//    {
        //ActiveCharacters(names.Length);
        ////Player�̐������[�v���ď�������.
        //int i = 0;
        //for (i = 0; i < beforeScore.Count; i++)
        //{
        //    //foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
        //    {
        //        //resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text = player.NickName;
        //        resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text
        //            = names[i];
        //        resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
        //            = "SCORE:" + beforeScore[i].ToString();
        //        resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
        //            = "+" + addScore[i].ToString();
        //    }       
        //}
        //StartCoroutine(ChangeScoreText(beforeScore, addScore));
    //}

    private void ActiveCharacters(int cnt)
    {
        //for(int i=0;i< ConectServer.RoomProperties.MaxPlayer; i++)
        for (int i = 0; i < 3 ; i++)
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
    /// ���U���g�p�l���̃X�R�A��ω�������֐�
    /// �����ɕύX�O�̃X�R�A�E���Z���邷��X�R�A���w��
    /// </summary>
    IEnumerator ChangeScoreText(List<int> beforeScore, List<int> addScore)
    {
        List<int> score = beforeScore;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
        List<int> adscore =  addScore;
        int cnt = 0;
        while (true)
        {
            for (int i = 0; i < score.Count; i++)
            {
                if (adscore[i] != 0)//���Z���镪�̃X�R�A��0�łȂ���΃e�L�X�g�ύX����.
                {
                    adscore[i]--;//���Z������-1����.
                    score[i]++;  //��������+1����.

                    //�X�R�A�̃e�L�X�g��ύX����.
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                            = "SCORE:" + score[i].ToString();
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
                    = "+" + adscore[i].ToString();
                    if (adscore[i] == 0)//���Z����X�R�A��0�ɂȂ�����J�E���g�𑝂₷.
                    {
                        cnt++;
                    }
                }
            }
            //break;

                yield return new WaitForSeconds(0.1f);
            if (cnt == score.Count)//�ő�l�����S�ẴX�R�A���Z���I�������烋�[�v�𔲂��ď��ʂ�\������.
            {
                ChangeRank(score);
                break;
            }
        }
        

        yield return new WaitForSeconds(3f);
        //Debug.Log("�R���[�`���I��");
        ActiveResultPanel(false);
        //�I�����������Ȃ�R�R

    }

    void ChangeRank(List<int> score)
    {

        for (int i = 0; i < 3; i++)
        {
            int rank = 1;
            for(int j = 0; j < 3; j++)
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

    void PushNameTest()
        {
            //for(int i = 0; i < 3; i++)
            //{
            //    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Text>().text = names[i];
            //}

            //Player�̐������[�v���ď�������.
            int i = 0;
            foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
            {
                resultCharacters.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Text>().text = player.NickName;
            }
        }
}
