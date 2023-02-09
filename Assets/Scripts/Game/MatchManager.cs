using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> AIUnits = new List<Unit>();


    public Transform playerSpawnPoint;
    public Transform AISpawnPoint;

    public GameObject[] aiPrefab;
    private int[] aiCost = {15,20,30};

    void Start(){
        instance = this;
    }

    public void SpawnUnit(GameObject prefab,int cost,bool forPlayer){
        if(forPlayer){
            if(GameManager.instance.GetMoneyOfPlayer() < cost || playerSpawnPoint == null){
                return;
            }
            GameManager.instance.AddMoneyToMatchPlayer(-cost);
            Unit unit = Instantiate(prefab,transform).GetComponent<Unit>();
            unit.setOwnedByPlayer(true);
            unit.damage+=GameManager.instance.GetAttackBuffOfPlayer();
            unit.defense+=GameManager.instance.GetDefenseBuffOfPlayer();
            unit.GetComponent<NavMeshAgent>().Warp(playerSpawnPoint.transform.position + Utilities.RandomPointOnCircleEdge(10));
            playerUnits.Add(unit);
        }else{
            if(AIUnits.Count >= 8 || GameManager.instance.GetMoneyOfAI() < cost || AISpawnPoint == null){
                return;
            }
            GameManager.instance.AddMoneyToMatchAI(-cost);
            Unit unit = Instantiate(prefab,transform).GetComponent<Unit>();
            unit.damage+=GameManager.instance.GetAttackBuffOfAI();
            unit.defense+=GameManager.instance.GetDefenseBuffOfAI();
            unit.GetComponent<NavMeshAgent>().Warp(AISpawnPoint.transform.position + Utilities.RandomPointOnCircleEdge(10));
            AIUnits.Add(unit);
        }
    }


    public void RemoveUnitFromDataBase(Unit unit){
        if(unit.isOwnedByPlayer()){
            foreach(Unit u in AIUnits){
                u.TryRemoveFromKnownUnits(unit.transform);
            }
            playerUnits.Remove(unit);

            if(playerUnits.Count==0){
                GameManager.instance.EndMatch(false);
            }

        }else{
            foreach(Unit u in playerUnits){
                u.TryRemoveFromKnownUnits(unit.transform);
            }
            AIUnits.Remove(unit);

            if(AIUnits.Count==0){
                GameManager.instance.EndMatch(true);
            }
        }
        
    }

    public void EndMatch(){
        GameManager.instance.EndMatch(false);
    }


    void Update(){
        // IA

        if(Random.Range(0,100) >= 11){
            return;
        }

        AIUnits[Random.Range(0,AIUnits.Count)].GoTo(playerUnits[Random.Range(0,playerUnits.Count)].transform.position);



        if(Random.Range(0,100) <= 2){
            int prefab = Random.Range(0,aiPrefab.Length);
            SpawnUnit(aiPrefab[prefab],aiCost[prefab],false);
        }
    }
}
