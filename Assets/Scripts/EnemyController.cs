using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 1.0f;
    public int enemyHP = 100;
    Vector2 moveVec = new Vector2(-1.0f, 0);
    GameObject scannedObject;
    Vector2 dirVec = Vector2.left;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer rend;

    float attackDelay = 1.0f;
    bool canAttack = true;
    bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();

        rend.flipX = true;
    }

    // 이동 방식 이슈 있다 1. 플레이어가 부딪히면 밀림 <- 이건 플레이어의 콜리더와 에너미의 레이케스트가 부딪히지 않을때 무게로 미는 것. -> 플레이어의 콜리더 y축을 위로 쭉늘려서 해결가능

    void Update() 
    {
        attackDelay -= Time.deltaTime;

        if(attackDelay < 0)
        {
            canAttack = true;
        }

        if(!isDead)
        {
            if(scannedObject == null)
            {
                speed = 1.0f;
                transform.Translate(moveVec * speed * Time.deltaTime);
            }
            else if(scannedObject != null)
            {
                speed = 0f;
                transform.Translate(moveVec * speed * Time.deltaTime);
                AttackPlayer(scannedObject);
            }
        }

        if(enemyHP <= 0)
        {
            isDead = true;

            speed = 0f;
            transform.Translate(moveVec * speed * Time.deltaTime);

            anim.SetBool("isDead", true);
            Destroy(this.gameObject, 1.0f);
        }
    }

    private void FixedUpdate() {
        Debug.DrawRay(rigid.position, dirVec * 1.0f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 2.0f, LayerMask.GetMask("Player"));

        if(rayHit.collider != null )
        {
            scannedObject = rayHit.collider.gameObject;
            //Debug.Log("object  발견!! 이름은 " + scannedObject);
        }
        else
            scannedObject = null;
    }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     if(other.tag == "Arrow")
    //     {
    //         enemyHP -= other.GetComponent<ArrowController>().arrowDmg;
    //         Debug.Log("현재 적의 체력은 : " + enemyHP);
            
    //     }
    // }

    // 데미지 값 처리를 위해 head와 body 나누었다.
    public void HeadShot(Collider2D other)
    {
            enemyHP -= other.GetComponent<ArrowController>().arrowDmg * 2;
            Debug.Log("head shot!!! 현재 적의 체력은 : " + enemyHP);
    }

    public void BodyShot(Collider2D other)
    {
            enemyHP -= other.GetComponent<ArrowController>().arrowDmg;
            Debug.Log("Body shot!!! 현재 적의 체력은 : " + enemyHP);
    }

    void AttackPlayer(GameObject player)
    {
        if(canAttack)
            player.GetComponent<PlayerController>().playerHP -= 10;
        
        //첫 공격은 쿨타임 상관없이 하도록 미리 true 로 해두고 공격이 시작되는 대로 canAttack 과 Delay 값 초기화
        canAttack = false;
        attackDelay = 1.0f;
    }
}
