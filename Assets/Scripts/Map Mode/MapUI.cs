using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    public static MapUI instance;

    public Image uiFlag;

    public GameObject choiceButton;

    public GameObject playerUI;

    public Text moneyText;

    public Text nationNameText;
    public string currentNation;

    public GameObject[] attackButtons;

    public string aiInvader;
    public string provinceInvaded;

    public GameObject invasionHolder;

    void Start()
    {
        instance = this;

        if(GameManager.instance.hasSelectedNation){
            DisableButtonChoiceNation();
        }else{
            EnablePlayerUI(false);
        }

        UpdateUI(GameManager.instance.GetNation(GameManager.instance.player));
    }

    public void UpdateUI(Nation nation)
    {
        currentNation = nation.id;
        uiFlag.sprite = nation.flag;
        nationNameText.text = nation.nationName;
        moneyText.text = "Or : "+nation.money+" ("+GameManager.instance.GetNationRevenue(nation.id)+")";

        bool val = StateManager.instance.NationCanAttackProvince(GameManager.instance.player,StateManager.instance.currentState);
        foreach(GameObject button in attackButtons){
            button.SetActive(val);
        }
    }


    public void DisableButtonChoiceNation(){
        choiceButton.SetActive(false);
    }

    public void EnablePlayerUI(bool val){
        playerUI.SetActive(val);
    }


    public void EndTurn(){
        GameManager.instance.NextTurn();
    }

    public void LaunchAttack(bool fast){
        GameManager.instance.Attack(StateManager.instance.currentState,GameManager.instance.player,currentNation,fast);
    }

    public void LaunchAIAttack(bool fast){
        SetInvasionHolderActive(false);
        GameManager.instance.Attack(provinceInvaded,aiInvader,GameManager.instance.player,fast,true);
    }

    public void SetInvasionHolderActive(bool val){
        invasionHolder.SetActive(val);
    }

    public void Invasion(string ai,string province){
        provinceInvaded = province;
        aiInvader = ai;
        SetInvasionHolderActive(true);
    }

}
