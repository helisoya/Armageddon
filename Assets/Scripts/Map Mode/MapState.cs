using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapState
{
    public string id;
    public int income = 0;
    public int religion = 0;
    public string[] voisins;
    public SpriteRenderer renderer;
}
