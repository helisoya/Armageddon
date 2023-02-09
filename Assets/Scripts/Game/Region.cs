using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region
{
    public string stateName;


    public List<string> statesInRegion = new List<string>();

    public List<string> nationsInRegion = new List<string>();

    public Region(string nom){
        stateName = nom;
    }

    public void TryRemove(string id){
        if(!nationsInRegion.Contains(id)){
            return;
        }
        foreach(string state in statesInRegion){
            if (GameManager.instance.GetState(state).owner==id){
                return;
            }
        }
        nationsInRegion.Remove(id);
    }
    public bool Unified(){
        string potentialUnifier = nationsInRegion[0];
        foreach(string state in statesInRegion){
            if(GameManager.instance.GetState(state).owner!=potentialUnifier){
                return false;
            }
        }
        return true;
    }
}
