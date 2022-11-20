using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Attack : MonoBehaviour
{
    public LayerMask playerLayer; 
    public float speed;
    Vector3 moveVec = new Vector3(-1.0f, 0f, 0f);
    Vector3 targetVec;
    float rot = 1.0f;
    GameObject scannedObject;
    GameObject targetObject;
    GameObject GameManager = null;
    Vector3 dirVec = new Vector3(0f, -1.0f, 0f);
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer rend;
    bool canAttack;
    bool canSkill;
    bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        gameObject.GetComponent<EnemyController>().enemyHP = 70;
        gameObject.GetComponent<EnemyController>().speed = 1.0f;
        speed = gameObject.GetComponent<EnemyController>().speed;
        
        transform.localScale = new Vector3 ( -1, 1, 1);
    }

    // 이동 방식 이슈 있다 1. 플레이어가 부딪히면 밀림 <- 이건 플레이어의 콜리더와 에너미의 레이케스트가 부딪히지 않을때 무게로 미는 것. -> 플레이어의 콜리더 y축을 위로 쭉늘려서 해결가능

    void Update() 
    {

        if(!isDead)
        {
            if(targetObject == null)
            {
                anim.SetBool("doAttack", false);
                if(dirVec.y < -0.9f)
                    rot = -1.0f;
                else if(dirVec.y > 0.9f)
                    rot = 1.0f;
                dirVec = Quaternion.Euler(0,0, rot) * dirVec;
                speed = 1.0f;
                transform.Translate(moveVec * speed * Time.deltaTime);
            }
            else if(targetObject != null)
            {
                if(scannedObject == null)
                {
                    anim.SetBool("doAttack", false);
                    speed = 1.0f;
                }
                else
                {
                    
                    speed = 0f;
                    AttackPlayer(scannedObject);
                }
                targetVec = new Vector3(targetObject.transform.position.x - this.transform.position.x,
                targetObject.transform.position.y - this.transform.position.y, 0f);
                dirVec = targetVec;
                transform.Translate(targetVec / targetVec.magnitude * speed * Time.deltaTime);
            }

            
        }

        if(gameObject.GetComponent<EnemyController>().enemyHP <= 0)
        {
            if(!isDead)
            {
                 GameManager.GetComponent<GameManager>().dropEnergy();
                 GameManager.GetComponent<GameManager>().dropGold(3);
            }
            isDead = true;
            gameObject.layer = 11;
            transform.GetChild(0).gameObject.layer = 11;
            transform.GetChild(1).gameObject.layer = 11;
            speed = 0f;
            transform.Translate(moveVec * speed * Time.deltaTime);

            anim.SetBool("doDie", true);
            Destroy(this.gameObject, 1.0f);
        }
    }

    private void FixedUpdate() {
        //Debug.DrawRay(rigid.position, dirVec * 10.0f, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 1.5f, playerLayer);
        RaycastHit2D detectRay = Physics2D.Raycast(rigid.position, dirVec, 10.0f, playerLayer);

        if(rayHit.collider != null )
        {
            scannedObject = rayHit.collider.gameObject;
            //Debug.Log("object  발견!! 이름은 " + scannedObject);
        }
        else
            scannedObject = null;

        if(detectRay.collider != null)
        {
            targetObject = detectRay.collider.gameObject;
        }
        else
            targetObject = null;
    }

    

    void AttackPlayer(GameObject player)
    {
        anim.SetBool("doAttack", true);
        if(canAttack)
        {
            if(player.tag == "Player")
                player.GetComponent<PlayerController>().playerHP -= 20;
            else
            {
                player.GetComponent<UnitController>().unitHP -= 30;
                Debug.Log("normal attack to unit! " + " unit hp is " + player.GetComponent<UnitController>().unitHP);
            }
                
            canAttack = false;
        }
    }
    

    void CanAttack()
    {
        canAttack = true;
    }
}
