using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Nation
{
    public string id;
    public string nationName;
    public Color mapColor;
    public Sprite flag;
    public int money = 0;

    public string orignalRegion;

    public int religion;

    public List<string> statesUnderControl = new List<string>();


}
