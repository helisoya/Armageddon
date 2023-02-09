using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private int health;

    private int maxHealth;

    public int damage;
    public int defense;
    [SerializeField]
    private int speed;

    private bool canMove = true;

    private NavMeshAgent agent;

    [SerializeField]
    private GameObject marker;

    [SerializeField]
    private bool ownedByPlayer; 

    private List<Transform> ennemyUnitsInRange = new List<Transform>();

    private Transform currentTarget = null;

    [SerializeField]
    private Transform turretPart;

    private UnitHealthBar healthBar;

    [SerializeField]
    private float cooldown;
    private float maxCooldown;
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private Transform canon;

    [SerializeField]
    private GameObject prefabTir;

    void Start(){
        maxHealth = health;
        maxCooldown = cooldown;
        healthBar = GetComponent<UnitHealthBar>();
        healthBar.SetColorOfHealthBar(ownedByPlayer);
        agent = GetComponent<NavMeshAgent>();


        if(agent==null){
            return;
        }

        agent.speed = speed;
        if(speed==0){
            agent.isStopped=true;
        }
    }

    public void setOwnedByPlayer(bool val){
        ownedByPlayer = val;
    }

    public void GoTo(Vector3 target){
        if(agent != null && agent.remainingDistance>= 1 && !ownedByPlayer){
            return;
        }
        if(agent!=null && canMove){
            agent.SetDestination(target);
        }
    }


    public void setCanMove(bool val){
        canMove = val;
        agent.isStopped = !val;
    }

    public void SetMarkerActive(bool val){
        marker.SetActive(val);
    }

    public bool isOwnedByPlayer(){
        return ownedByPlayer;
    }

    public void TryRemoveFromKnownUnits(Transform unit){
        if(ennemyUnitsInRange.Contains(transform)){
            ennemyUnitsInRange.Remove(transform);  
        }
    }

    public void TakeDamage(int val){
        int bonus = Random.Range(0,100) <= 20 ? 1 : 0;
        if (val-defense+bonus<=0){
            return;
        }
        
        health -= (val-defense+bonus);
        if(health<=0){
            MatchManager.instance.RemoveUnitFromDataBase(this);
            GameObject obj = MatchManager.Instantiate(explosionPrefab,MatchManager.instance.transform);
            obj.transform.position = transform.position;
            Destroy(obj,2);
            Destroy(gameObject);
        }else{
            healthBar.SetHealthBarLength(health,maxHealth);
        }
    }

    void OnTriggerEnter(Collider col){
        if(col==null){
            return;
        }
        if(col.GetType() == typeof(BoxCollider)){
            return;
        }
        if(col.tag=="Unit" && Utilities.FindParent(col.transform,"Unit").GetComponent<Unit>().ownedByPlayer!=ownedByPlayer){
            ennemyUnitsInRange.Add(Utilities.FindParent(col.transform,"Unit"));
        }
    }

    void OnTriggerExit(Collider col){
        if(col.GetType() == typeof(BoxCollider)){
            return;
        }
        if(col.tag=="Unit" && ennemyUnitsInRange.Contains(Utilities.FindParent(col.transform,"Unit"))){
            ennemyUnitsInRange.Remove(Utilities.FindParent(col.transform,"Unit"));
            if(Utilities.FindParent(col.transform,"Unit") == currentTarget){
                currentTarget = null;
            }
        }
    }


    public bool UnitInRange(Transform unit){
        return ennemyUnitsInRange.Contains(unit);
    }

    public void SetCurrentTarget(Transform target){
        if(UnitInRange(target)){
            currentTarget = target;
        }
    }


    void Update(){

        if(currentTarget == null && ennemyUnitsInRange.Count != 0){
            currentTarget = ennemyUnitsInRange[Random.Range(0,ennemyUnitsInRange.Count)];
        }

        if(currentTarget != null){
            turretPart.LookAt(currentTarget);

            if(cooldown > 0){
                cooldown-=Time.deltaTime;
            }else{
                cooldown = maxCooldown;
                Fire();
            }
        }
    }

    void Fire(){
        GameObject obj = Instantiate(explosionPrefab,canon);
        Destroy(obj,2);
        obj = Instantiate(prefabTir,MatchManager.instance.transform);
        obj.transform.position = canon.transform.position;
        obj.transform.rotation = canon.transform.rotation;
        obj.GetComponent<Shot>().dmg = damage;
        obj.GetComponent<Shot>().playerShot = ownedByPlayer;
        obj.GetComponent<Shot>().shooter = transform;
    }
}
