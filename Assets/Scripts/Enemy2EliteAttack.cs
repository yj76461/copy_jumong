using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2EliteAttack : MonoBehaviour
{
    public LayerMask playerLayer; 
    public AudioClip audioSkill;
    public AudioClip audioAttack;
    AudioSource audioSource;
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
    
    float skillTimer = 0f;
    float skillTime = 5f;
    float attackTimer = 0f;
    float attackTime = 0.75f;
    bool canAttack;
    bool canSkill;
    bool isDead;
    bool manyUnit;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        gameObject.GetComponent<EnemyController>().enemyHP = 200;
        gameObject.GetComponent<EnemyController>().speed = 0.5f;
        speed = 0.5f;

        transform.localScale = new Vector3 ( -1.5f, 1.5f, 1);
    }

    // 이동 방식 이슈 있다 1. 플레이어가 부딪히면 밀림 <- 이건 플레이어의 콜리더와 에너미의 레이케스트가 부딪히지 않을때 무게로 미는 것. -> 플레이어의 콜리더 y축을 위로 쭉늘려서 해결가능

    void Update() 
    {
        skillTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if(skillTimer > skillTime)
        {
            canSkill = true;
        }

        if(attackTimer > attackTime)
        {
            canAttack = true;
        }

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
                    if(canSkill && manyUnit) // 유닛이 두개 이상일때 범위기 작동
                    {
                        anim.SetBool("doAttack", false);
                        anim.SetTrigger("doAttack2");
                        SkillPlayer();
                        canSkill = false;
                        skillTimer = 0f;
                    }
                    else if(canAttack)
                    {
                        anim.SetBool("doAttack", true);
                        AttackPlayer(scannedObject);
                        canAttack = false;
                        attackTimer = 0f;
                    }
                }
                targetVec = new Vector3(targetObject.transform.position.x - this.transform.position.x,
                targetObject.transform.position.y - this.transform.position.y, 0f);
                transform.Translate(targetVec / targetVec.magnitude * speed * Time.deltaTime);

                if(targetVec.x < 0)
                    transform.localScale = new Vector3 ( -1.5f, 1.5f, 1);
                else
                    transform.localScale = new Vector3 ( 1.5f, 1.5f, 1);
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
        Debug.DrawRay(rigid.position, dirVec * 2.0f, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 2.0f, playerLayer);
        RaycastHit2D[] rayHits = Physics2D.RaycastAll(rigid.position, dirVec, 2.0f, playerLayer);
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

        if(rayHits.Length > 1)
        {
            manyUnit = true;
            Debug.Log("unit count is " + rayHits.Length);
        }
        else
            manyUnit = false;
    }

    

    void AttackPlayer(GameObject player)
    {
        StartCoroutine(attackDelay());
    }
    
    void SkillPlayer()
    {
        StartCoroutine(skillDelay());
    }

    
    IEnumerator skillDelay() // 애니메이션에서 공격 시점이 너무 늦어서 코루틴으로 뺐다.
    {
        yield return new WaitForSeconds(1.0f);
        audioSource.clip = audioSkill;
        audioSource.Play();
        RaycastHit2D[] skillHits = Physics2D.RaycastAll(rigid.position, dirVec, 2.0f, playerLayer); // 다중 공격이다.
        for(int i = 0; i < skillHits.Length; i++)
        {
            RaycastHit2D hit = skillHits[i];
            if(hit.collider != null)
            {
                if(hit.collider.tag == "Player")
                    hit.collider.GetComponent<PlayerController>().playerHP -= 50;
                else
                {
                    hit.collider.GetComponent<UnitController>().unitHP -= 50;
                    Debug.Log("skill attack to unit! " + " unit hp is " + hit.collider.GetComponent<UnitController>().unitHP);
                }
            }
            else
                Debug.Log("아무도 감지되지않았습니다.");
        }
        
    }

    IEnumerator attackDelay()
    {
        yield return new WaitForSeconds(0.6f);
        audioSource.clip = audioAttack;
        audioSource.Play();
        RaycastHit2D attackHit = Physics2D.Raycast(rigid.position, dirVec, 2.0f, playerLayer); // 공격 모션이 나올 타이밍에 레이쏴서 아직 앞에 있는지 확인.
        
        if(attackHit.collider != null)
            {
                if(attackHit.collider.tag == "Player")
                    attackHit.collider.GetComponent<PlayerController>().playerHP -= 50;
                else
                {
                    attackHit.collider.GetComponent<UnitController>().unitHP -= 50;
                    Debug.Log("normal attack to unit! " + " unit hp is " + attackHit.collider.GetComponent<UnitController>().unitHP);
                }
            }
        else
            Debug.Log("아무도 감지되지않았습니다.");
    }
}
