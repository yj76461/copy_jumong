using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public int enemyHP;
    float coolTimer = 0f;
    bool canAttacked;

    private void Update() {
        coolTimer += Time.deltaTime; // 화살 중복 피격 방지 쿨타임선언

        if(coolTimer >= 0.01f)
        {
                canAttacked = true;
        }
    }
    

    // 데미지 값 처리를 위해 head와 body 나누었다.
    public void HeadShot(Collider2D other)
    {
        if(canAttacked)
        {
            enemyHP -= other.GetComponent<ArrowController>().arrowDmg * 2;
            Debug.Log("head shot!!! 현재 적의 체력은 : " + enemyHP);
            canAttacked = false;
            coolTimer = 0f;
        }
            
    }

    public void BodyShot(Collider2D other)
    {
        if(canAttacked)
        {
            enemyHP -= other.GetComponent<ArrowController>().arrowDmg;
            Debug.Log("Body shot!!! 현재 적의 체력은 : " + enemyHP);
            canAttacked = false;
            coolTimer = 0f;
        }
    }

}
