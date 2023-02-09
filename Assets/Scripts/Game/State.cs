using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class State
{
    public string id = "1";
    public string stateName = "";
    public string owner = "000";
    public string region = "";
    public int religion = 0;
    public int income = 0;

    public bool canUse = true;
    public State(string identifier,string reg){
        id = identifier;
        region = reg;
    }
}
