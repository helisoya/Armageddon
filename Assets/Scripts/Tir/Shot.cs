using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shot : MonoBehaviour
{
    public Transform shooter;
    public float timeBeforeExplosion;
    public float speed;

    public bool playerShot = true;

    [HideInInspector]
    public float timeBeforeExplosionMax;

    public int dmg = 2;

    void Start(){
        timeBeforeExplosionMax = timeBeforeExplosion;
    }

    void OnTriggerEnter(Collider col){
        if(col.GetType() == typeof(BoxCollider)){
            return;
        }
        if(col.tag == "Unit" && FindParent(col.transform,"Unit") == shooter){
            return;
        }
        
        if(col.tag=="Unit"){
            Unit unit = FindParent(col.transform,"Unit").GetComponent<Unit>();
            if(unit.isOwnedByPlayer()!=playerShot){
                unit.TakeDamage(dmg);
            }
        }
        Destroy(gameObject);
    }

    Transform FindParent(Transform start, string tag){
        while(start.parent != null && start.parent.tag==tag){
            start = start.parent;
        }
        return start;
    }
}
