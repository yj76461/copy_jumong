using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseSensor : MonoBehaviour
{

    public GameManager gameManager;
    public LayerMask enemyLayer;
    Rigidbody2D rigid;
    GameObject scannedObject;
    Vector2 dirVec = new Vector2(1.0f, 0f);
    bool touchOnce = true;
    
    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(scannedObject!= null && touchOnce)
        {
            Debug.Log("scanned objects name is "+ scannedObject.name);
            touchOnce = false;
            StartCoroutine("CoolTimer");
            gameManager.GameResult.SetActive(true);
            gameManager.ResultPage(false);
        }
    }
    void FixedUpdate()
    {
        Debug.DrawRay(rigid.position, dirVec, new Color(0, 0, 1));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 2.0f, enemyLayer);

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
        yield return new WaitForSeconds(0.5f);
        touchOnce = true;
    }
}
