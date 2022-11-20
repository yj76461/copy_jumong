using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Attack : MonoBehaviour
{
    public LayerMask playerLayer; 
    public GameObject slime;
    AudioSource audioSource;
    public AudioClip audioAttack1;
    public AudioClip audioAttack2;
    public AudioClip audioAttack3;
    GameObject targetObject;
    GameObject scannedObject;
    Rigidbody2D rigid;
    Animator anim;
    GameObject GameManager = null;
    SpriteRenderer rend;
    Vector3 moveVec = new Vector3(-1.0f, 0f, 0f);
    Vector3 dirVec = new Vector3(0f, -1.0f, 0f);
    Vector3 targetVec;   
    float rot = 1.0f;
    float speed;
    float randomMoveTimer;
    float attack1Timer, attack2Timer, attack3Timer;
    float attack1Time = 2.0f, attack2Time = 5.0f, attack3Time= 10.0f;
    bool canAttack1, canAttack2, canAttack3;
    bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        gameObject.GetComponent<EnemyController>().enemyHP = 1000;
        gameObject.GetComponent<EnemyController>().speed = 0.5f;
        speed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localPosition = ClampPosition(transform.localPosition); // 이동범위 제한 걸기
        attack1Timer += Time.deltaTime;
        attack2Timer += Time.deltaTime;
        attack3Timer += Time.deltaTime;
        randomMoveTimer += Time.deltaTime;
        
        if(attack1Timer > attack1Time)
        {
            canAttack1 = true;
        }
        if(attack2Timer > attack2Time)
        {
            canAttack2 = true;
        }
        if(attack3Timer > attack3Time)
        {
            canAttack3 = true;
        }
        // if(randomMoveTimer > 2.0f)
        // {
        //     moveVec = new Vector3(Random.Range(-1, 2),Random.Range(-1, 2), 0f);
        //     randomMoveTimer = 0f;
        // }

        if(!isDead)
        {
            if(targetObject == null)
            {
                anim.SetBool("doAttack1", false);
                
                dirVec = Quaternion.Euler(0,0, rot) * dirVec;
                speed = 1.0f;
                transform.Translate(moveVec * speed * Time.deltaTime);
            }
            else if(targetObject != null)
            {
                if(scannedObject == null)
                {
                    speed = 1.0f;
                }
                else
                {
                    if(anim.GetCurrentAnimatorStateInfo(0).IsName("boss1_attack2"))
                        speed = 3.0f;
                    else
                        speed = 0;
                    
                    if(canAttack1) // 유닛이 두개 이상일때 범위기 작동
                    {
                        StartCoroutine(attack1Player());
                        attack1Timer = 0f;
                        canAttack1 = false;
                    }
                    else if(canAttack2)
                    {
                        StartCoroutine(attack2Player());
                        canAttack2 = false;
                        attack2Timer = 0f;
                    }
                    else if(canAttack3)
                    {
                        StartCoroutine(attack3Player());
                        canAttack3 = false;
                        attack3Timer = 0f;
                    }
                }
                targetVec = new Vector3(targetObject.transform.position.x - this.transform.position.x,
                targetObject.transform.position.y - this.transform.position.y, 0f);
                transform.Translate(targetVec / targetVec.magnitude * speed * Time.deltaTime);

                if(targetVec.x < 0)
                    transform.localScale = new Vector3 ( 1.5f, 1.5f, 1);
                else
                    transform.localScale = new Vector3 ( -1.5f, 1.5f, 1);
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
            speed = 0f;
            transform.Translate(moveVec * speed * Time.deltaTime);

            anim.SetBool("doDie", true);
            Destroy(this.gameObject, 1.0f);
        }
    }

    private void FixedUpdate() {
        Debug.DrawRay(rigid.position, dirVec * 5.0f, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 5.0f, playerLayer);
        RaycastHit2D detectRay = Physics2D.Raycast(rigid.position, dirVec, 10.0f, playerLayer);

        if(rayHit.collider != null )
        {
            scannedObject = rayHit.collider.gameObject;
            Debug.Log("boss 가 object  발견!! 이름은 " + scannedObject);
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

    IEnumerator attack1Player()
    {
        anim.SetTrigger("doAttack1");
        yield return new WaitForSeconds(0.6f);
        audioSource.clip = audioAttack1;
        audioSource.Play();
        RaycastHit2D attackHit = Physics2D.Raycast(rigid.position, dirVec, 5.0f, playerLayer); // 공격 모션이 나올 타이밍에 레이쏴서 아직 앞에 있는지 확인.
        
        if(attackHit.collider != null)
            {
                if(attackHit.collider.tag == "Player")
                    attackHit.collider.GetComponent<PlayerController>().playerHP -= 50;
                else
                {
                    attackHit.collider.GetComponent<UnitController>().unitHP -= 50;
                    Debug.Log("normal attack1 to unit! " + " unit hp is " + attackHit.collider.GetComponent<UnitController>().unitHP);
                }
            }
        else
            Debug.Log("아무도 감지되지않았습니다.");
    }

    IEnumerator attack2Player()
    {
        anim.SetTrigger("doAttack2");
        yield return new WaitForSeconds(0.6f);
        audioSource.clip = audioAttack2;
        audioSource.Play();
        RaycastHit2D[] skillHits = Physics2D.RaycastAll(rigid.position, dirVec, 3.0f, playerLayer); // 다중 공격이다.
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

    IEnumerator attack3Player()
    {
        anim.SetTrigger("doAttack3");
        yield return new WaitForSeconds(0.6f);
        audioSource.clip = audioAttack3;
        audioSource.Play();
        for(int i = 0; i < 3; i++)
        {
            GameObject newSlim = Instantiate(slime, new Vector3(Random.Range(-8.0f, 8.0f), 10.0f, 0f), Quaternion.identity);
            Vector3 targetPos = new Vector3(newSlim.transform.position.x, Random.Range(-4.0f, 2.0f), 0f);
            newSlim.GetComponent<SlimController>().getPos(targetPos);
        }
    }

    public Vector3 ClampPosition(Vector3 pos)
    {
        return new Vector3
        (
            Mathf.Clamp(pos.x, -7.0f, 8.0f), Mathf.Clamp(pos.y, -4.0f, 2.0f), 0
        );
    }
}
