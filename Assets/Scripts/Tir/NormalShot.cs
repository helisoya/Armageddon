using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalShot : Shot
{

    void Update()
    {
        if(timeBeforeExplosion>0){
            timeBeforeExplosion-=Time.deltaTime;
        }else{
            Destroy(gameObject);
            return;
        }
        transform.position += transform.forward * speed * Time.deltaTime;

    }

}
