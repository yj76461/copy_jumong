using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimController : MonoBehaviour
{
    public LayerMask playerLayer;
    Vector3 targetPos;
    Animator anim;
    Rigidbody2D rigid;
    BoxCollider2D boxCol;
    bool doOnce;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.2f);
        if(!doOnce)
        {
            StartCoroutine(boomPlayer());
            doOnce = true;
        }
    }
    void FixedUpdate()
    {

    }

    public void getPos(Vector3 pos)
    {
        targetPos = pos;
    }

    IEnumerator boomPlayer()
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("trigger operation");
        anim.SetTrigger("doBoom");

        RaycastHit2D[] skillHits = Physics2D.BoxCastAll(rigid.position, boxCol.bounds.size, 0f, Vector2.up, 1f, playerLayer); // 다중 공격이다.

        for(int i = 0; i < skillHits.Length; i++)
        {
            RaycastHit2D hit = skillHits[i];
            if(hit.collider != null)
            {
                Debug.Log("슬라임 맞은 사람 " + hit.collider.name);
                if(hit.collider.tag == "Player")
                    hit.collider.GetComponent<PlayerController>().playerHP -= 50;
                else if(hit.collider.tag == "Units")
                {
                    hit.collider.GetComponent<UnitController>().unitHP -= 50;
                    Debug.Log(" attack3 to unit! " + " unit hp is " + hit.collider.GetComponent<UnitController>().unitHP);
                }
            }
            else
                Debug.Log("아무도 감지되지않았습니다.");
        }

        
        Destroy(this.gameObject, 2.0f);
    }
}
