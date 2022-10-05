using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public int arrowDmg = 33;
    Rigidbody2D rigid;
    bool arrowCrash = false;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 가는 방향으로 회전은 rotation으로 구하는 것이 아니라 rigid.velocity 와 eulerAngles로 구하는 것.
        if(!arrowCrash)
        {
            float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x) * Mathf.Rad2Deg;
            this.transform.eulerAngles = new Vector3(0f, 0f, angle);
        }

        
    }
    public void ArrowShot(Vector2 dir, float arrowSpeed)
    {
        // 구한 방향벡터를 바탕으로 화살을 z축 회전 시키는 코드 *중요하다.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
   	    GetComponent<Transform>().transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // 방향벡터의 방향으로 힘을 실어서 보냄
        GetComponent<Rigidbody2D>().AddForce(dir.normalized * arrowSpeed * 100f);

        Destroy(this.gameObject, 10.0f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "BodyShot" || other.tag == "HeadShot")
        {
            Debug.Log("enemy crash!");


            rigid.velocity = Vector2.zero;
            arrowCrash = true;
            rigid.isKinematic = true;
            transform.parent = other.transform;
        }
    }

}
