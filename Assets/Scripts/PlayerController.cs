using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public GameObject GameManager;
    public GameObject Arrow;
    public GameObject powerBar;
    public Slider hpBar;
    public AudioClip audioBowLoad;
    public AudioClip audioBowShot;
    AudioSource audioSource;
    Slider powerBarGauge;
    public float playerHP;
    public float speed;
    public float arrowSpeed;
    float h, v;
    float attackDelay = 0f;
    float shotDelay = 0.4f;
    float powerBarSpeed = 8.0f;
    bool canAttack;
    float cursorx;
    Rigidbody2D rigid;
    bool mouseDown;
    bool canMove = true;
    bool goDownSwitch = false; // 화살 속도 등락 조정 장치
    bool isDead;
    bool doOnce;
    Vector3 MousePosition;
    Vector3 realTimeMousePosition;
    Vector2 dir;
    Vector2 dirVec = Vector2.right;
    Camera myCamera;
    Animator anim;
    SpriteRenderer rend;
    GameObject newArrow;
    GameObject scannedObject;


    // item list
    bool basicShot;
    bool moreDamage;
    bool fasterSoldier;
    bool takeNoDamage;
    bool noPowerBar;
    bool doubleShot;
    bool quickShot;


    void Awake() {
        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        rigid = GetComponent<Rigidbody2D>();
        rigid.freezeRotation = true;
        audioSource = GetComponent<AudioSource>();

        playerHP = 100.0f; // 피 설정
        
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        powerBarGauge = powerBar.GetComponent<Slider>();
    }

    void Start()
    {
        canAttack = true; // 최초 1회의 공격은 쿨없이 나가도록 설정
    }

    void Update() {
        //OnSingleTouch(); 모바일 터치 구현때 사용할 것
        if(!isDead)
        {
            doOnce = false;

            if(Input.GetKeyDown(KeyCode.J)) // 더블샷 on/off 설정을 위함.
            {
                if(!doubleShot)
                    doubleShot = true;
                else
                    doubleShot = false;
                Debug.Log("double shot :" + doubleShot);
            }
            if(Input.GetKeyDown(KeyCode.K)) // quick샷 on/off 설정을 위함.
            {
                QuickShot();
                Debug.Log("quick shot :" + quickShot);
            }
            if(Input.GetKeyDown(KeyCode.L)) // nopower샷 on/off 설정을 위함.
            {
                NoPowerBar();
                Debug.Log("NoPowerBar :" + quickShot);
            }

            // 공격 쿨타임
            attackDelay += Time.deltaTime;
            if(attackDelay > shotDelay)
            {
                canAttack = true;
            }

            // HP 바 값 표시
            hpBar.value = playerHP / 100.0f;

            // 화살 파워바 위치 캐릭터 위로 고정하고 슬라이더 값에 arrowSpeed의 값을 full 값으로 나눈 값으로 넣어줬다.
            if(!noPowerBar)
            {
                powerBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1.0f, 0));
                powerBarGauge.value = arrowSpeed / 7.5f;
            }
            
            if(canAttack)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    if(EventSystem.current.IsPointerOverGameObject() == false)
                    {
                        audioSource.clip = audioBowLoad;
                        audioSource.Play();
                        anim.SetBool("doAttack1", true);
                        mouseDown = true;
                        canMove = false;
                    }

                }
                else if(Input.GetMouseButtonUp(0))
                {
                    if(EventSystem.current.IsPointerOverGameObject() == false)
                    {
                        audioSource.clip = audioBowShot;
                        audioSource.Play();
                        anim.SetBool("doAttack1", false);
                        canMove = true;
                        // 마우스를 떼는 시점의 좌표를 받는게 맞다고 생각하여 코드 위치 변경
                        MousePosition = Input.mousePosition;
                        MousePosition = myCamera.ScreenToWorldPoint(MousePosition);

                        dir = MousePosition - transform.position; // z축이 계산에 쓰이는 것을 막기 위해 Vector2 로 변환
                        //Debug.Log("나의 포지션: " + transform.position + " 화면의 위치: " +  MousePosition + " 벡터 크기 : " + dir/dir.magnitude );

                        newArrow = Instantiate(Arrow, transform.position, Quaternion.identity);

                        if(doubleShot)
                        {
                            GameObject newArrow2 = Instantiate(Arrow, transform.position + new Vector3(-0.5f, 0.2f, 0f), Quaternion.identity);
                            newArrow2.GetComponent<ArrowController>().ArrowShot(dir, arrowSpeed);
                        }
                        //Debug.Log("arrow speed :" + arrowSpeed);
                        //만들어진 화살 객체에 정보 전달
                        newArrow.GetComponent<ArrowController>().ArrowShot(dir, arrowSpeed);

                        canAttack = false;
                        attackDelay = 0f;
                        arrowSpeed = 1.0f;
                        mouseDown = false;
                    }
                }
            }

            if(mouseDown && !noPowerBar) // 마우스가 눌리는 동안 화살 속도를 증가
            {
                BasicShot();
            }
            else if(noPowerBar)
            {
                NoPowerBar();
            }
        }

        if(playerHP <= 0)
        {
            isDead = true;
            if(!doOnce)
            {
                doOnce = true;
                anim.SetTrigger("doDie");
                StartCoroutine(dieDelay());
            }
        }
        

    }

    void FixedUpdate() {
        
        if(!isDead)
        {
            realTimeMousePosition = Input.mousePosition;
            realTimeMousePosition = myCamera.ScreenToWorldPoint(realTimeMousePosition);
            cursorx = realTimeMousePosition.x - transform.position.x;

            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            if(canMove && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                anim.SetBool("doRun", true);
            else
                anim.SetBool("doRun", false);

            if(!mouseDown)
            {
                if(h > 0) // x축 이동시 이미지 좌우반전
                    transform.localScale = new Vector3(1, 1, 1);
                else if(h < 0)
                    transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                if(cursorx > 0) // 마우스 좌표 - 플레이어 좌표값
                    transform.localScale = new Vector3(1, 1, 1);
                else if(cursorx < 0)
                    transform.localScale = new Vector3(-1, 1, 1);
            }
            


            //Debug.Log("cursor : " + realTimeMousePosition + " player : " + transform.position + " cursor x value : " + cursorx);

            if(canMove)    
                transform.Translate(new Vector3(h, v, 0f) * Time.deltaTime * speed);
        }
        

    }

    void BasicShot()
    {

        if(arrowSpeed > 8.0f)
        {
            goDownSwitch = true;
        }
        else if(arrowSpeed < 1.0f)
        {
            goDownSwitch = false;
        }
        
        if(!goDownSwitch)
            arrowSpeed += Time.deltaTime * powerBarSpeed;
        else
            arrowSpeed -= Time.deltaTime * powerBarSpeed;
    }


    public void NoPowerBar()
    {
        noPowerBar = true;
        powerBar.SetActive(false);
        arrowSpeed = 9.0f;
    }

    public void DoubleShot()
    {
        doubleShot = true;
    }
    
    public void QuickShot()
    {
        quickShot = true;
        shotDelay = 0.2f;
        powerBarSpeed = 10.0f;
    }





    // 스마트폰 전용 - 터치 구현
    private void OnSingleTouch()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                Debug.Log("touch detect!! position is : "+ touch.position);
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                Debug.Log("touch ended!!");
            }
        }
    }

    public void aliveAgain()
    {
        isDead = false;
        playerHP = 100;
        canMove = true;
    }

    IEnumerator dieDelay()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.GetComponent<GameManager>().GameResult.SetActive(true);
        GameManager.GetComponent<GameManager>().ResultPage(false);
    }
}
