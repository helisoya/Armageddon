using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealthBar : MonoBehaviour
{
    [SerializeField]
    private Material playerColor;
    [SerializeField]
    private Material aiColor;

    [SerializeField]
    private Transform healthBar;

    [SerializeField]
    private int maxLengthOfHealthBar;

    public void SetColorOfHealthBar(bool usePlayerColor){
        healthBar.GetComponent<Renderer>().material = usePlayerColor ? playerColor : aiColor;
    }


    public void SetHealthBarLength(int currHealth,int maxHealth){
        healthBar.localScale = new Vector3((currHealth*maxLengthOfHealthBar)/maxHealth,0.2f,0.2f);
    }

    void Update(){
        healthBar.LookAt(Camera.main.transform);
    }
}
