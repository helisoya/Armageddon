using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;

    public MapState[] mapStatesArray;

    Dictionary<string,MapState> mapStates = new Dictionary<string, MapState>();

    public enum MAPMODE{
        NORMAL,
        PLAYER,
        RELIGION,
        INCOME
    }

    public MAPMODE currentMapMode = MAPMODE.NORMAL;

    public string currentState = "";

    public void SelectState(string state){
        currentState = state;
    }


    void Start()
    {

        foreach(MapState state in mapStatesArray){
            mapStates.Add(state.id,state);
        }

        foreach(Transform region in transform){
            foreach(Transform province in region){
                mapStates[province.name].renderer = province.GetComponent<SpriteRenderer>();
                if(GameManager.instance.hasSelectedNation){ // Equivalent à "si c'est pas la première fois"
                    mapStates[province.name].religion = GameManager.instance.GetState(province.name).religion;
                    mapStates[province.name].income = GameManager.instance.GetState(province.name).income;
                }else{
                    GameManager.instance.GetState(province.name).religion = mapStates[province.name].religion;
                    GameManager.instance.GetState(province.name).income = mapStates[province.name].income;
                }
            }
        }

        instance = this;
        RefreshStatesColor();

        HideUnplayableLand();
    }


    public SpriteRenderer GetStateRenderer(string id){
        return mapStates[id].renderer;
    }

    public string[] GetVoisinsState(string id){
        return mapStates[id].voisins;
    }

    public int GetReligion(string id){
        return mapStates[id].religion;
    }

    public int GetIncome(string id){
        return mapStates[id].income;
    }

    public MapState GetState(string id){
        return mapStates[id];
    }


    public void RefreshStatesColor(){
        foreach(string province in GameManager.instance.states.Keys){
            RefreshProvince(province);
        }
    }


    public void HideUnplayableLand(){
        foreach(Transform region in transform){
            if(GameManager.instance.hasSelectedNation && !GameManager.instance.hasUnified 
                && GameManager.instance.GetNation(GameManager.instance.player).orignalRegion != region.name ){
                    region.gameObject.SetActive(false);
            }else{
                region.gameObject.SetActive(true);
            }
            
        }
    }

    public bool NationCanAttackProvince(string nation,string prov){
        if(GameManager.instance.nations[nation].statesUnderControl.Contains(prov)){
            return false;
        }
        foreach(string owned in GameManager.instance.nations[nation].statesUnderControl){
            if(!GameManager.instance.states[owned].canUse){
                continue;
            }
            foreach(string voisin in mapStates[owned].voisins){
                if(voisin==prov){
                    return true;
                }
            }
        }
        return false;
    }

    public void RefreshProvince(string province){
        switch(currentMapMode){
            case MAPMODE.NORMAL:
                GetStateRenderer(province).color = 
                    (GameManager.instance.GetStateOwner(province) != null ) ? 
                    GameManager.instance.GetStateOwner(province).mapColor : Color.gray;
                break;
            case MAPMODE.PLAYER:
                GetStateRenderer(province).color = 
                    (GameManager.instance.player==GameManager.instance.GetStateOwner(province).id) ?
                    GameManager.instance.GetNation(GameManager.instance.player).mapColor : Color.black;
                break;
            case MAPMODE.RELIGION:
                GetStateRenderer(province).color = 
                    GameManager.instance.GetReligion(GetReligion(province)).mapColor;
                break;
            case MAPMODE.INCOME:
                GetStateRenderer(province).color = 
                    Color.Lerp(Color.red,Color.yellow,GetIncome(province)*0.2f);
                break;
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            currentMapMode = MAPMODE.NORMAL;
            RefreshStatesColor();
        }else if(Input.GetKeyDown(KeyCode.Alpha2)){
            currentMapMode = MAPMODE.PLAYER;
            RefreshStatesColor();
        }else if(Input.GetKeyDown(KeyCode.Alpha3)){
            currentMapMode = MAPMODE.RELIGION;
            RefreshStatesColor();
        }else if(Input.GetKeyDown(KeyCode.Alpha4)){
            currentMapMode = MAPMODE.INCOME;
            RefreshStatesColor();
        }
    }

}
