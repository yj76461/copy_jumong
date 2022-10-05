using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 충돌 판정만 받고 물리 효과는 무시하고 싶을때 ontrigger
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Arrow")
        {
            transform.parent.GetComponent<EnemyController>().HeadShot(other);
        }
    }
}
