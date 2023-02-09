using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Dictionary<string,Nation> nations = new Dictionary<string, Nation>();
    public Nation[] nationsArray;


    public Dictionary<string,State> states = new Dictionary<string, State>();
    public Dictionary<string,Region> regions = new Dictionary<string, Region>();

    public string player = "DEV";

    public bool hasSelectedNation = false;

    public bool hasUnified = false;

    public Religion[] religions;

    int rebelCount = 0;

    List<string> rebelFrom = new List<string>();

    public Sprite rebelFlag;

    [SerializeField]
    int rebellionChance;

    [SerializeField]
    int convertChanceToMain;

    [SerializeField]
    int convertChanceToOther;

    [SerializeField]
    private Match currentMatch = new Match();


    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Utilities.Quit();
        }
    }

    void Awake()
    {
        if(instance != null){
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;

        foreach(Transform region in GameObject.Find("Map").transform){
            regions[region.name] = new Region(region.name);
            foreach(Transform province in region){
                states[province.name] = new State(province.name,region.name);
                regions[region.name].statesInRegion.Add(province.name);
            }
            
        }

        foreach(Nation nation in nationsArray){
            nations[nation.id] = nation;
            foreach(string ownedState in nation.statesUnderControl){
                states[ownedState].owner = nation.id;
            }
            regions[nation.orignalRegion].nationsInRegion.Add(nation.id);
        }
        player = nationsArray[0].id;
    }

    public Nation GetNation(string id){
        return nations[id];
    }   

    public Region GetNationRegion(string id){
        return regions[nations[id].orignalRegion];
    }  

    public State GetState(string id){
        return states[id];
    }  

    public Nation GetStateOwner(string id){
        if(!nations.ContainsKey(states[id].owner)){
            return null;
        }
        return nations[states[id].owner];
    }  

    public Region GetStateRegion(string id){
        return regions[states[id].region];
    }  


    public Region GetRegion(string id){
        return regions[id];
    }

    public Religion GetReligion(int id){
        return religions[id];
    }

    public Religion GetNationReligion(string id){
        return religions[nations[id].religion];
    }


    public int GetNationRevenue(string id){
        int rev = 0;
        foreach(string state in nations[id].statesUnderControl){
            if(states[state].religion==nations[id].religion){
                rev+=states[state].income;
            }
        }
        rev+=GetNationReligion(id).incomeBuff;

        return rev;
    }


    public void ValidNationChoice(){
        hasSelectedNation = true;
        MapUI.instance.DisableButtonChoiceNation();
        MapUI.instance.EnablePlayerUI(true);
        StateManager.instance.HideUnplayableLand();
    }


    public void NextTurn(){
        foreach(string prov in nations[player].statesUnderControl){
            states[prov].canUse = true;
        }
        UpdateAI();
        UpdateReligion();

        foreach(string country in rebelFrom){
            AddRebelNation(country);
        }
        rebelFrom.Clear();

        AddIncomeToNations();
        StateManager.instance.RefreshStatesColor();
        MapUI.instance.UpdateUI(GetNation(MapUI.instance.currentNation));
    }

    void AddIncomeToNations(){
        foreach(string nation in nations.Keys){
            foreach(string state in nations[nation].statesUnderControl){
                if(states[state].religion==nations[nation].religion){
                    nations[nation].money+=states[state].income;
                }
                
            }
            nations[nation].money+=GetNationReligion(nation).incomeBuff;
        }
    }


    int GetCountStateWithDifferentReligion(string nation){
        int count = 0;
        foreach(string state in nations[nation].statesUnderControl){
            if(states[state].religion!=nations[nation].religion){ 
                count++;
            }
        }
        return count;
    }

    int GetMajorityReligion(string country){
        int max = 0;
        int maxCount = 0;
        int currentCount = 0;
        for(int i = 0;i<religions.Length;i++){
            if(i==nations[country].religion){
                continue;
            }

            foreach(string state in nations[country].statesUnderControl){
                if(states[state].religion==i){
                    currentCount++;
                }
            }
            if(currentCount > maxCount){
                maxCount = currentCount;
                max = i;
            }
        }
        return max;
    }

    void AddRebelNation(string country){
        string rebelName = "RB"+rebelCount.ToString();
        nations[rebelName] = new Nation();
        nations[rebelName].flag = rebelFlag;
        nations[rebelName].id = rebelName;
        nations[rebelName].mapColor = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
        nations[rebelName].orignalRegion = nations[country].orignalRegion;
        regions[nations[rebelName].orignalRegion].nationsInRegion.Add(rebelName);
        nations[rebelName].nationName = "Rebelles de "+nations[country].nationName.Replace("Rebelles de ","");
        nations[rebelName].religion = GetMajorityReligion(country);

        List<string> toModify = new List<string>();

        foreach(string state in nations[country].statesUnderControl){
            if(states[state].religion==nations[rebelName].religion){
                toModify.Add(state);
            }
        }

        foreach(string state in toModify){
            nations[rebelName].statesUnderControl.Add(state);
            nations[country].statesUnderControl.Remove(state);
            states[state].owner = rebelName;
        }

        rebelCount++;
    }

    void UpdateReligion(){
        foreach(string nation in nations.Keys){

            if(Random.Range(0,100) <= rebellionChance &&
                GetCountStateWithDifferentReligion(nation) >= nations[nation].statesUnderControl.Count/2  
            ){ // Si il y a au moins 50% de provinces n'ayant pas la religion d'état, 5% de chances d'avoir une guerre civile
                rebelFrom.Add(nation);
                continue;
            }

            foreach(string state in nations[nation].statesUnderControl){
                if(states[state].religion!=nations[nation].religion){ 
                    if(Random.Range(0,100) <= convertChanceToMain){ // 15% de chance d'adopter la religion d'état si ce n'est pas le cas
                        states[state].religion=nations[nation].religion;
                        StateManager.instance.GetState(state).religion = nations[nation].religion;
                    }
                }else{ // Sinon, 2% de chance d'adopter celle d'a coté
                    if(Random.Range(0,100) <= convertChanceToOther){
                        foreach(string voisin in StateManager.instance.GetVoisinsState(state)){
                            if(states[voisin].religion != states[state].religion){
                                states[state].religion = states[voisin].religion;
                                StateManager.instance.GetState(state).religion = states[voisin].religion;
                                break;
                            }
                        }
                    }
                }
            }
            nations[nation].money+=GetNationReligion(nation).incomeBuff;
        }
    }


    void UnifyAllRegions(){
        foreach(string region in regions.Keys){
            if(nations[player].orignalRegion==region){
                continue;
            }
            string unifier = regions[region].nationsInRegion[Random.Range(0,regions[region].nationsInRegion.Count)];
            foreach(string province in regions[region].statesInRegion){
                if(states[province].owner!=unifier){
                    StealProvinceFrom(province,unifier,states[province].owner);
                }
            }
        }
    }



    public void Attack(string prov,string attacker,string defender,bool fast = false,bool aiStarted =false){
        currentMatch.attacker = attacker;
        currentMatch.defender = defender;
        currentMatch.province = prov;
        currentMatch.defensive = aiStarted;

        if(fast){
            if(nations[attacker].money-nations[defender].money > 0){
                EndMatch(true,true);
            }
        }else{
            SceneManager.LoadScene(
                "Field"+Random.Range(0,3).ToString()
            );
        }
    }

    void StealProvinceFrom(string province,string thief,string victim){
        states[province].owner = thief;
        nations[thief].statesUnderControl.Add(province);
        nations[victim].statesUnderControl.Remove(province);
        regions[states[province].region].TryRemove(victim);
        if(thief==player){
            states[province].canUse = false;
        }
    }

    public void EndMatch(bool attackerVictory,bool fast = false){
        if(attackerVictory){
            StealProvinceFrom(currentMatch.province,currentMatch.attacker,currentMatch.defender);
        }

        if(!hasUnified && regions[states[currentMatch.province].region].Unified()){
            hasUnified = true;
            UnifyAllRegions();
            if(fast){
                StateManager.instance.RefreshStatesColor();
            }
        }

        if(nations[player].statesUnderControl.Count==states.Count){
                SceneManager.LoadScene("Victory");
                return;
        }else if(nations[player].statesUnderControl.Count==0){
                SceneManager.LoadScene("Defeat");
                return;
        }


        if(fast){
            StateManager.instance.RefreshProvince(currentMatch.province);
            StateManager.instance.HideUnplayableLand();
        }else if(currentMatch.attacker==player || currentMatch.defender==player){
            SceneManager.LoadScene("Map");
        }
    }

    public int GetMoneyOfPlayer(){
        return nations[player].money;
    }

    public void AddMoneyToMatchPlayer(int val){
        nations[player].money+=val;
    }

    public int GetMoneyOfAI(){
        if(currentMatch.defensive){
            return nations[currentMatch.attacker].money;
        }else{
            return nations[currentMatch.defender].money;
        }
        
    }

    public void AddMoneyToMatchAI(int val){
        if(currentMatch.defensive){
            nations[currentMatch.attacker].money+=val;
        }else{
            nations[currentMatch.defender].money+=val;
        }
    }


    public int GetDefenseBuffOfPlayer(){
        return GetNationReligion(player).defenseBuff;
    }

    public int GetDefenseBuffOfAI(){
        if(currentMatch.defensive){
            return GetNationReligion(currentMatch.attacker).defenseBuff;
        }else{
            return GetNationReligion(currentMatch.defender).defenseBuff;
        }
    }
    public int GetAttackBuffOfPlayer(){
        return GetNationReligion(player).damageBuff;
    }

    public int GetAttackBuffOfAI(){
        if(currentMatch.defensive){
            return GetNationReligion(currentMatch.attacker).damageBuff;
        }else{
            return GetNationReligion(currentMatch.defender).damageBuff;
        }
    }


    public void UpdateAI(){
        string[] list = new string[nations.Count];
        nations.Keys.CopyTo(list,0);
        foreach(string id in list){
            if(id != player &&
                nations[id].statesUnderControl.Count!=0 
            && (hasUnified || (nations[id].orignalRegion==nations[player].orignalRegion))){
                AI(id);
            }
        }

    }


    void AI(string id){
        if(Random.Range(0,100)<=20){
            return;
        }
        List<string> possibilities = new List<string>();
        foreach(string province in nations[id].statesUnderControl){
            foreach(string voisin in StateManager.instance.GetVoisinsState(province)){
                if(states[voisin].owner!=id){
                    possibilities.Add(voisin);
                }
            }
        }
        if(possibilities.Count!=0){
            AI_Attack(id,possibilities[Random.Range(0,possibilities.Count)]); 
        }
           
    }


    void AI_Attack(string nation,string prov){
        string victim = states[prov].owner;
        if(victim == player){
            MapUI.instance.Invasion(nation,prov);
        }else{
            if(Random.Range(0,100) <= 30){
                nations[nation].money= Mathf.Clamp(nations[nation].money-30,0,int.MaxValue);
                StealProvinceFrom(prov,nation,victim);
            }
        }
    }

}
