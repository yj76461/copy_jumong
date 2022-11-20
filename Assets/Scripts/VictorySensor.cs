using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictorySensor : MonoBehaviour
{

    public GameManager gameManager;
    public LayerMask unitsLayer;
    Rigidbody2D rigid;
    GameObject scannedObject;
    Vector2 dirVec = new Vector2(0f, 1.0f);
    bool touchOnce = true;
    float rot = 1.0f;
    int cnt = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        dirVec = Quaternion.Euler(0,0, rot) * dirVec;
        cnt++;
        if(cnt == 180)
        {
            cnt = 0;
            rot*= -1.0f;
        }
        if(scannedObject != null && touchOnce)
        {
            if(scannedObject.tag == "Units" || scannedObject.tag == "Player")
            {
                Debug.Log("scanned objects name is "+ scannedObject.name);
                touchOnce = false;
                StartCoroutine("CoolTimer");
                gameManager.GameResult.SetActive(true);
                gameManager.ResultPage(true);
            }
        }
    }
    void FixedUpdate()
    {
        Debug.DrawRay(rigid.position, dirVec, new Color(0, 0, 1));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 2.0f, unitsLayer);

        if(rayHit.collider != null)
        {
            scannedObject = rayHit.collider.gameObject;
        }
        else
        {
            scannedObject = null;
        }
    }

    IEnumerator CoolTimer()
    {
        yield return new WaitForSeconds(5.0f);
        touchOnce = true;
    }
}
