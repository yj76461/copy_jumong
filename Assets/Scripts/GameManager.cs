using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public List<Dictionary<string, object>> roundDB = new List<Dictionary<string, object>>();
    public Button btnUnit1, btnUnit2;
    public GameObject intermission;
    public GameObject settingWindow;
    public GameObject startScreen;
    public GameObject goldObject;
    public GameObject inStage;
    public GameObject player;
    public GameObject fadePanel;
    public GameObject GameResult;
    public Button gotoIntermissionBtn;
    public Image fadePanelAlpha;
    public GameObject[] Enemy;
    public GameObject[] Boss;
    public GameObject[] Units;
    public List<GameObject> spawnedUnits = new List<GameObject>();
    public Text resultText;
    public Text debugText;
    public Text energyText;
    public Text goldText;
    public Text introText;
    
    // my cost
    public int energy = 0;
    public int gold = 0;
    bool gameStart = false;
    bool isStartScreen = true;
    int roundId;
    int round;
    float spawningTime;
    int smallEnemy;
    bool round5boss;

    float realTime = 0.0f;

    //audio
    public AudioMixer audioMixer;
    public Slider audioSlider;
    AudioSource audioSource;
    public AudioClip audioStage;
    public AudioClip audioLobby;
    
    

    // Start is called before the first frame update
    void Start()
    {

        roundDB = CSVReader.Read("roundDB");
        
        btnUnit1.onClick.AddListener(spawnUnit1);
        btnUnit2.onClick.AddListener(spawnUnit2);

        startScreen.SetActive(true);
        fadePanel.SetActive(false);
        GameResult.SetActive(false);
        introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, 1);
        StartCoroutine(myIntro());
        intermission.SetActive(false);
        inStage.SetActive(false);
        player.SetActive(false);
        goldObject.SetActive(false);
        settingWindow.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioLobby;
        audioSource.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isStartScreen)
            StopCoroutine(myIntro());

        realTime += Time.deltaTime;

        if(realTime > spawningTime && gameStart == true)
        {
            spawnEnemy();
            if(spawnedUnits.Count == 10 - round)
            {
                spawnBoss(0);
                Debug.Log("boss spawned!");
            }
            
            realTime = 0.0f;
        }
        if(round5boss && spawnedUnits.Count == 10)
        {
            round5boss = false;
            spawnBoss(1);// 5round boss spawn
            Debug.Log("real boss spawned!!");
        }


        energyText.text = energy.ToString(); // 몇갠지 지속적으로 업데이트
        goldText.text = gold.ToString();
    }

    void setRound(int id)
    {
        for(int i = 0; i < roundDB.Count; i ++)
        {
            if(id == (int)roundDB[i]["id"]){
                roundId = id;
                round = (int)roundDB[i]["round"];
                spawningTime = float.Parse(roundDB[i]["spawningTime"].ToString()); // object to float 는 int 때와 방식이 다르다.
                smallEnemy = (int)roundDB[i]["smallEnemy"];
                break;
            }
        }
        Debug.Log("round setting complete!");
        debugText.text = "round Id" + roundId + '\n' +
                        "round" + round + '\n' +
                        "spawnTime" + spawningTime + '\n';

        if(round == 5)
        {
            round5boss = true;
            Debug.Log("round 5 is true");
        }
    }

    void spawnEnemy()
    {
        float ranNum = Random.Range(-3.5f , 0.5f);
        GameObject newEnemy = Instantiate(Enemy[0], new Vector3(11.0f, ranNum, 0f), Quaternion.identity);
        spawnedUnits.Add(newEnemy);
        Debug.Log("enemy spawn func executation!");
    }

    void spawnBoss(int Bossidx)
    {
        float ranNum = Random.Range(-3.5f , 0.5f);
        GameObject newEnemy = Instantiate(Boss[Bossidx], new Vector3(11.0f, ranNum, 0f), Quaternion.identity);
        spawnedUnits.Add(newEnemy);
        Debug.Log("enemy spawn func executation!");
    }

    public void spawnUnit1()
    {
        if(energy > 0)
        {
            energy--;
            float ranNum = Random.Range(-3.5f , 0.5f);
            GameObject newUnit = Instantiate(Units[0], new Vector3(-10.0f, ranNum, 0f), Quaternion.identity);
            spawnedUnits.Add(newUnit);
            Debug.Log("unit1 pawn func executation!");
        }
        else
            Debug.Log("not enough energy!!");
    }

    public void spawnUnit2()
    {
        if(energy > 3)
        {
            energy -= 4;
            float ranNum = Random.Range(-3.5f , 0.5f);
            GameObject newUnit = Instantiate(Units[1], new Vector3(-10.0f, ranNum, 0f), Quaternion.identity);
            spawnedUnits.Add(newUnit);
            Debug.Log("unit1 pawn func executation!");
        }
        else
            Debug.Log("not enough energy!!");
    }

    public void GameVictory()
    {
        StartCoroutine(FadeInOut_GameVictory());
    }

    public void GameLose()
    {
       StartCoroutine(FadeInOut_GameLose());
    }

    // roundId / round 변수를 잘 확인하며 사용하자. roundId 는 실제 라운드를 조작하기 위함. round는 화면에 보여주기 위한 라운드 숫자이다.
    public void GoToNextRound()
    {
        StartCoroutine(FadeInOut_GoToNextRound());
        audioSource.clip = audioStage;
        audioSource.Play();
    }

    public void startRound()
    {
        StartCoroutine(FadeInOut_startRound());
        Debug.Log("gettin started");
        audioSource.clip = audioStage;
        audioSource.Play();
        // startScreen.SetActive(false);

        // goldObject.SetActive(true); // 게임 시작할때 한번만 띄우면 다시 끌일은 없다.

        // player.SetActive(true);
        // inStage.SetActive(true);
        // setRound(0);
        // gameStart = true;
    }

    

    public void Buy_PowerShot()
    {
        
    }

    public void dropEnergy()
    {
        energy ++;
    }

    public void dropGold(int qtt)
    {
        gold += qtt;
    }

    IEnumerator myIntro()
    {
        float m = 0.1f;

        while (true)
        {
            if(introText.color.a > 0.95f)
                m = -0.1f;
            else if(introText.color.a < 0.05f)
                m = 0.1f;
            introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, introText.color.a + m);
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    IEnumerator FadeInOut_startRound()
    {
        float m = 0.2f;
        fadePanel.SetActive(true);
        for(int i = 0; i < 5; i++)
        {
            fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, fadePanelAlpha.color.a + m);
            yield return new WaitForSeconds(0.1f);
        }

        startScreen.SetActive(false);
        goldObject.SetActive(true); // 게임 시작할때 한번만 띄우면 다시 끌일은 없다.
        player.SetActive(true);
        player.GetComponent<PlayerController>().aliveAgain();
        inStage.SetActive(true);
        setRound(0);
        gameStart = true;

        for(int i = 0; i < 5; i++)
        {
            fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, fadePanelAlpha.color.a - m);
            yield return new WaitForSeconds(0.1f);
        }
        fadePanel.SetActive(false);

    }

    IEnumerator FadeInOut_GoToNextRound()
    {
        float m = 0.2f;
        fadePanel.SetActive(true);
        for(int i = 0; i < 5; i++)
        {
            fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, fadePanelAlpha.color.a +  m);
            yield return new WaitForSeconds(0.1f);
        }

        
        gotoIntermissionBtn.onClick.RemoveAllListeners();
        intermission.SetActive(false);
        inStage.SetActive(true);
        player.SetActive(true);
        player.GetComponent<PlayerController>().aliveAgain();
        gameStart = true;
        setRound(roundId); 

        for(int i = 0; i < 5; i++)
        {
            fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, fadePanelAlpha.color.a -  m);
            yield return new WaitForSeconds(0.1f);
        }
        fadePanel.SetActive(false);
        

    }

    IEnumerator FadeInOut_GameVictory()
    {
        float m = 0.2f;
        fadePanel.SetActive(true);
        GameResult.SetActive(false);
        player.transform.position = new Vector3(-7.0f, -2.5f, 0);

        
        fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, 0f);
        for(int i = 0; i < 5; i++)
        {
            fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, fadePanelAlpha.color.a +  m);
            yield return new WaitForSeconds(0.1f);
        }

        
        fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, 1f);
        Debug.Log("you win! ");

        player.SetActive(false);
        inStage.SetActive(false);
        intermission.SetActive(true);
        energy = 0;

        roundId++;
        int nextRound = round + 1;
        intermission.transform.GetChild(1).GetComponent<Text>().text = "Round " + nextRound; // roundInfo


        for(int i = 0; i < 5; i++)
        {
            fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, fadePanelAlpha.color.a -  m);
            Debug.Log("b panel alpha value:  " + fadePanelAlpha.color.a + " i is " + i);
            yield return new WaitForSeconds(0.1f);
        }
        fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, 0f);
        fadePanel.SetActive(false);

    }

    IEnumerator FadeInOut_GameLose()
    {
        float m = 0.2f;
        fadePanel.SetActive(true);
        GameResult.SetActive(false);
        player.transform.position = new Vector3(-7.0f, -2.5f, 0);

        
        fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, 0f);
        while (fadePanelAlpha.color.a < 0.9f)
        {
            fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, fadePanelAlpha.color.a +  m);
            
            Debug.Log("a panel alpha value:  " + fadePanelAlpha.color.a);
            yield return new WaitForSeconds(0.1f);
        }

        
        fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, 1f);

        Debug.Log("you Lose! ");

        player.SetActive(false);
        inStage.SetActive(false);
        intermission.SetActive(true);
        energy = 0;
        gameStart = false;

        int nextRound = round; // 다시 라운드 시작
        intermission.transform.GetChild(1).GetComponent<Text>().text = "Round " + nextRound; // roundInfo

        while (fadePanelAlpha.color.a > 0.2f)
        {
            fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, fadePanelAlpha.color.a -  m);
            Debug.Log("b panel alpha value:  " + fadePanelAlpha.color.a);
            yield return new WaitForSeconds(0.1f);
        }
        fadePanelAlpha.color = new Color(fadePanelAlpha.color.r, fadePanelAlpha.color.g, fadePanelAlpha.color.b, 0f);
        fadePanel.SetActive(false);

    }

    public void ResultPage(bool result)
    {
        gameStart = false;
        // 게임내 스폰된 유닛들 싹다 삭제하고 리스트 초기화
        for(int i = 0; i < spawnedUnits.Count; i++)
            Destroy(spawnedUnits[i]);
        spawnedUnits.Clear();
        
        if(result)
        {
            resultText.text = "you WIN!";
            gotoIntermissionBtn.onClick.AddListener(GameVictory);
            Debug.Log("win button listen");
        }
        else
        {
            Debug.Log("resultPage you lose");
            resultText.text = "you Lose!";
            gotoIntermissionBtn.onClick.AddListener(GameLose);
        }
    }

    public void audioControl()
    {
        float sound = audioSlider.value;

        if(sound == -40f)
            audioMixer.SetFloat("BGM", -80);
        else
            audioMixer.SetFloat("BGM", sound);
    }

    public void settingOn()
    {
        settingWindow.SetActive(true);
    }
    public void settingOff()
    {
        settingWindow.SetActive(false);
    }
}
