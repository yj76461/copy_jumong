using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Dictionary<string, object>> roundDB = new List<Dictionary<string, object>>();
    public GameObject intermission;
    public GameObject[] Enemy;
    public Text debugText;
    bool gameEnd;
    bool isVictory;
    bool canVictory = true;
    int roundId;
    int round;
    float spawningTime;
    float victoryCoolTime;
    int smallEnemy;

    float realTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        roundDB = CSVReader.Read("roundDB");

        intermission.SetActive(false);

        setRound(0);
        spawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        realTime += Time.deltaTime;

        if(realTime > spawningTime && isVictory == false)
        {
            //spawnEnemy();
            realTime = 0.0f;
        }
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
    }

    void spawnEnemy()
    {
        float ranNum = Random.Range(-2.5f , -1.2f);
        Instantiate(Enemy[0], new Vector3(11.0f, ranNum, 0f), Quaternion.identity);
        Debug.Log("spawn func executation!");
    }

    public void GameVictory()
    {
        StartCoroutine("CoolTimer");
        if(canVictory)
        {
            canVictory = false;
            Debug.Log("you win! ");
            intermission.SetActive(true);
            //Time.timeScale = 0;
            isVictory = true;
            int nextRound = round + 1;
            intermission.transform.GetChild(0).GetComponent<Text>().text = "Round " + nextRound; // roundInfo
        }
    }
    // roundId / round 변수를 잘 확인하며 사용하자. roundId 는 실제 라운드를 조작하기 위함. round는 화면에 보여주기 위한 라운드 숫자이다.
    public void GoToNextRound()
    {
        intermission.SetActive(false);
        //Time.timeScale = 1;
        isVictory = false;

        // 기존 라운드 아이디를 1 증가 시킨뒤 setRound 함수에 인수로 집어 넣는다.
        roundId++;
        setRound(roundId); 
    }

    IEnumerator CoolTimer() // GameVictory 함수가 2번 실행하는 것을 방지한다.
    {
        yield return new WaitForSeconds(0.2f);
        canVictory = true;
    }

    public void Buy_PowerShot()
    {
        
    }

}
