using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabLight;
    [SerializeField]
    private GameObject prefabMedium;
    [SerializeField]
    private GameObject prefabHeavy;

    [SerializeField]
    private Text moneyText;


    void Start(){
        UpdateMoneyUI();
    }
    void UpdateMoneyUI(){
        moneyText.text = "Or : "+GameManager.instance.GetMoneyOfPlayer().ToString();
    }

    public void SpawnLight(){
        MatchManager.instance.SpawnUnit(prefabLight,15,true);
        UpdateMoneyUI();
    }

    public void SpawnMedium(){
        MatchManager.instance.SpawnUnit(prefabMedium,20,true);
        UpdateMoneyUI();
    }

    public void SpawnHeavy(){
        MatchManager.instance.SpawnUnit(prefabHeavy,30,true);
        UpdateMoneyUI();
    }

}
