using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit1Attack : MonoBehaviour
{
    public LayerMask enemyLayer;
    public AudioClip audioAttack;
    AudioSource audioSource;
    Vector3 moveVec = new Vector3(1.0f, 0, 0);
    Vector3 targetVec;
    GameObject scannedObject;
    GameObject targetObject;
    Vector3 dirVec = new Vector3(0, 1.0f, 0);
    float rot = -1.0f;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer rend;
    bool canAttack;
    float attackTimer;
    bool isDead;

    int unitHP;
    float speed;


    void Awake()
    {
        speed = this.GetComponent<UnitController>().speed;
        unitHP = this.GetComponent<UnitController>().unitHP;
        audioSource = GetComponent<AudioSource>();
    }

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
        attackTimer += Time.deltaTime;

        if(attackTimer >= anim.GetCurrentAnimatorStateInfo(0).length)
        {
            //Debug.Log("anim length is " + anim.GetCurrentAnimatorStateInfo(0).length);
            canAttack = true;
        }

        if(!isDead)
        {
            if(targetObject == null)
            {
                anim.SetBool("doAttack", false);
                if(dirVec.y < -0.9f)
                    rot = 1.0f;
                else if(dirVec.y > 0.9f)
                    rot = -1.0f;
                dirVec = Quaternion.Euler(0,0, rot) * dirVec;
                //Debug.Log("unit dirVec is " + dirVec);
                speed = this.GetComponent<UnitController>().speed;
                transform.Translate(moveVec * speed * Time.deltaTime);
            }
            else if(targetObject != null)
            {
                if(scannedObject == null)
                {
                    anim.SetBool("doAttack", false);
                    speed = this.GetComponent<UnitController>().speed;
                }
                else
                {
                    if(scannedObject.GetComponent<EnemyController>().enemyHP > 0)
                    {
                        speed = 0f;
                        AttackEnemy(scannedObject);
                    }
                    else
                        anim.SetBool("doAttack", false);
                }
                targetVec = new Vector2(targetObject.transform.position.x - this.transform.position.x,
                targetObject.transform.position.y - this.transform.position.y);
                dirVec = targetVec;
                transform.Translate(targetVec / targetVec.magnitude * speed * Time.deltaTime);
            }
        }

        if(this.GetComponent<UnitController>().unitHP <= 0)
        {
            isDead = true;
            gameObject.layer = 11;
            speed = 0f;
            transform.Translate(moveVec * speed * Time.deltaTime);
            anim.SetTrigger("doDie");

            Destroy(gameObject, 1.0f);
        }

    }

    private void FixedUpdate() {
        Debug.DrawRay(rigid.position, dirVec * 10.0f, new Color(0, 0, 1));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 1.0f, enemyLayer);
        RaycastHit2D detectRay = Physics2D.Raycast(rigid.position, dirVec, 10.0f, enemyLayer);

        if(rayHit.collider != null )
        {
            scannedObject = rayHit.collider.gameObject;
            //Debug.Log("object  발견!! tag 이름은 " + scannedObject.tag);
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

    
    void AttackEnemy(GameObject enemy)
    {
        //string scriptName = enemy.name.Substring(0, 6) + "Controller";
        if(canAttack)
        {
            anim.SetBool("doAttack", true);
            StartCoroutine(attack2Delay());
            canAttack = false;
            attackTimer = 0f;
        }
        
    }

    IEnumerator attack2Delay() // 애니메이션에서 공격 시점이 너무 늦어서 코루틴으로 뺐다.
    {
        yield return new WaitForSeconds(0.2f);
        audioSource.clip = audioAttack;
        audioSource.Play();
        RaycastHit2D skillHits = Physics2D.Raycast(rigid.position, dirVec, 2.0f, enemyLayer); // 공격 모션이 나올 타이밍에 레이쏴서 아직 앞에 있는지 확인.
        
        if(skillHits.collider != null)
        {
                skillHits.collider.GetComponent<EnemyController>().enemyHP -= 30;
                Debug.Log("normal attack to enemy! " + " enemy hp is " + skillHits.collider.GetComponent<EnemyController>().enemyHP);
        }
        else
            Debug.Log("아무도 감지되지않았습니다.");
        
    }
}
