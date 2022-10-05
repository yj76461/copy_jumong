using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject GameManager;
    public GameObject Arrow;
    public GameObject powerBar;
    public Slider hpBar;
    Slider powerBarGauge;
    public float playerHP;
    public float speed;
    public float arrowSpeed;
    float attackDelay;
    float shotDelay = 0.4f;
    bool canAttack;
    Rigidbody2D rigid;
    bool touchOn;
    bool mouseDown;
    bool goDownSwitch = false; // 화살 속도 등락 조정 장치
    Vector3 MousePosition;
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

    int layerMask = (1 << 8);

    void Awake() {
        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        rigid = GetComponent<Rigidbody2D>();
        rigid.freezeRotation = true;

        playerHP = 100.0f; // 피 설정
        
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        powerBarGauge = powerBar.GetComponent<Slider>();
        attackDelay = shotDelay;
    }

    void Start()
    {
        canAttack = true; // 최초 1회의 공격은 쿨없이 나가도록 설정
    }

    void Update() {
        //OnSingleTouch(); 모바일 터치 구현때 사용할 것

        if(Input.GetKeyDown(KeyCode.K)) // 더블샷 on/off 설정을 위함.
        {
            if(!doubleShot)
                doubleShot = true;
            else
                doubleShot = false;
            Debug.Log("double shot :" + doubleShot);
        }
        if(Input.GetKeyDown(KeyCode.F)) // quick샷 on/off 설정을 위함.
        {
            QuickShot();
            Debug.Log("quick shot :" + quickShot);
        }

        // 공격 쿨타임
        if(!canAttack)
            attackDelay -= Time.deltaTime;
        
        if(attackDelay <= 0f)
        {
            canAttack = true;
            attackDelay = shotDelay;
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
                
                mouseDown = true;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                // 마우스를 떼는 시점의 좌표를 받는게 맞다고 생각하여 코드 위치 변경
                MousePosition = Input.mousePosition;
                MousePosition = myCamera.ScreenToWorldPoint(MousePosition);

                dir = MousePosition - transform.position; // z축이 계산에 쓰이는 것을 막기 위해 Vector2 로 변환
                Debug.Log("나의 포지션: " + transform.position + " 화면의 위치: " +  MousePosition + " 벡터 크기 : " + dir/dir.magnitude );

                newArrow = Instantiate(Arrow, transform.position, Quaternion.identity);

                if(doubleShot)
                {
                    GameObject newArrow2 = Instantiate(Arrow, transform.position + new Vector3(0f, 0.2f, 0f), Quaternion.identity);
                    newArrow2.GetComponent<ArrowController>().ArrowShot(dir, arrowSpeed);
                }
                Debug.Log("arrow speed :" + arrowSpeed);
                //만들어진 화살 객체에 정보 전달
                newArrow.GetComponent<ArrowController>().ArrowShot(dir, arrowSpeed);

                canAttack = false;
                
                mouseDown = false;
            }
        }

        if(mouseDown && !noPowerBar) // 마우스가 눌리는 동안 화살 속도를 증가
        {
            arrowSpeedControll();
        }
        else if(!noPowerBar)
        {
            arrowSpeed = 1.0f;
        }

        if(scannedObject != null)
        {
            if(scannedObject.tag == "Finish")
            {
                GameManager.GetComponent<GameManager>().GameVictory();
                gameObject.transform.position = new Vector3(-7.0f, -2.5f, 0f);
            }
        }
    }

    void FixedUpdate() {
        float h = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            anim.SetBool("doRun", true);
        else
            anim.SetBool("doRun", false);

        if(h >= 0)
            rend.flipX = false;
        else
            rend.flipX = true;
            
        transform.position = new Vector2(transform.position.x + h, transform.position.y + v);



        Debug.DrawRay(rigid.position, dirVec * 1.0f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 2.0f, layerMask);

        if(rayHit.collider != null )
        {
            scannedObject = rayHit.collider.gameObject;
            Debug.Log("player 가 object  발견!! 이름은 " + scannedObject);
        }
        else
            scannedObject = null;
    }

    void arrowSpeedControll()
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
            arrowSpeed += 0.01f;
        else
            arrowSpeed -= 0.01f;
    }

    public void BasicShot()
    {
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
    }





    //터치 구현
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
}
