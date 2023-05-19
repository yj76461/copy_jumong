using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit2Controller2 : MonoBehaviour
{
    public LayerMask enemyLayer;
    public AudioClip audioAttack;
    AudioSource audioSource;
    //move to right side
    Vector3 moveVec = new Vector3(1.0f, 1.0f, 0);
    Vector3 targetVec;
    GameObject scannedObject;
    GameObject targetObject;
    //start from upside
    Vector3 dirVec = new Vector3(1.0f,0, 0);
    float rayDir = 1.0f;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer rend;
    bool canAttack = true;
    float attackTimer;
    float attackAnimationTime;
    bool isDead;

    int unitHP;
    float speed;

    void Awake(){
        

    }
    // Start is called before the first frame update
    void Start()
    {
        GetStatus();
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        if(attackTimer >= attackAnimationTime){
            canAttack = true;
        }else{
            canAttack = false;
        }

        if(unitHP <= 0){
            Die();
        }

    }

    void FixedUpdate(){
        if(unitHP > 0){
            Detect();
        }
    }

    void Move(Vector3 direction){
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void Die(){
        //isDead = true;
        // change to other layer
        gameObject.layer = 11;
        speed = 0f;
        transform.Translate(moveVec * speed * Time.deltaTime);
        anim.SetTrigger("doDie");
        Destroy(gameObject, 1.0f);
    }

    // 공격 알고리즘 지연시간 기능 추가 필요함 2023.05.19
    void Attack(GameObject enemy){
        anim.SetBool("doAttack", true);
        // float delay = attackAnimationTime / 4;
        // while(delay > 0){
        //     delay -= Time.deltaTime;
        // }
        audioSource.clip = audioAttack;
        audioSource.Play();

        // last check before shoot
        RaycastHit2D attackValid = Physics2D.Raycast(rigid.position, dirVec, 4.0f, enemyLayer);

        if(attackValid.collider != null){
            if(enemy == attackValid.collider.gameObject){
                enemy.GetComponent<EnemyController>().enemyHP -= 30;
                Debug.Log("unit 2 attacked enemy!!");
            }else{
                Debug.Log("when unit2 attacked, enemy had been dead already");
            }
        }

        // delay = attackAnimationTime * 3 / 4;
        // while(delay > 0){
        //     delay -= Time.deltaTime;
        // }
        anim.SetBool("doAttack", false);

    }

    void GetStatus(){
        speed = this.GetComponent<UnitController>().speed;
        unitHP = this.GetComponent<UnitController>().unitHP;
        audioSource = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();


        attackAnimationTime = GetAnimLength("unit2_attack");
    }

    float GetAnimLength(string name){
        float time = 0;
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;

        for(int i = 0; i < ac.animationClips.Length; i++){
            if(ac.animationClips[i].name == name){
                time = ac.animationClips[i].length;
                Debug.Log("animation : " + name + " length is " + time);
            }
            
        }
        return time;
    }

    void Detect(){
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 4.0f, enemyLayer);
        RaycastHit2D detectRay = Physics2D.Raycast(rigid.position, dirVec, 8.0f, enemyLayer);
        Debug.DrawRay(transform.position, dirVec * 8.0f, Color.red);

        if(rayHit.collider != null){
            targetObject = rayHit.collider.gameObject;
            // stop unit -> ready to attack
            transform.Translate(moveVec * Time.deltaTime * 0f);
            if(canAttack){
                attackTimer = 0f;
                Attack(targetObject);
            }
        }else{
            targetObject = null;
            MoveRay();
        }

        if(detectRay.collider != null){
            scannedObject = detectRay.collider.gameObject;
            FixRay(scannedObject);
        }else{
            scannedObject = null;
            MoveRay();
            Move(moveVec);
        }
        

    }

    void MoveRay(){
        // and go straight forward
        moveVec = new Vector3(1f, 0f, 0f);
        

        // increment direction change
        if(dirVec.y > 0.9f)
            rayDir = -0.5f;
        else if(dirVec.y < -0.9f)
            rayDir = 0.5f;
        dirVec.y += rayDir *Time.deltaTime;

    }

    void FixRay(GameObject target){
        targetVec = new Vector2(target.transform.position.x - this.transform.position.x,
         target.transform.position.y - this.transform.position.y);
        
        //normalize
        moveVec = targetVec / targetVec.magnitude;
        dirVec = targetVec / targetVec.magnitude;
    }

    public void UnderAttack(int dmg){
        unitHP -= dmg;
    }
}
