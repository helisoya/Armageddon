using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.EventSystems;

public class ClickOnState : MonoBehaviour
{
    private new SpriteRenderer renderer;


    void Start(){
        renderer = GetComponent<SpriteRenderer>();
    }

    public void OnMouseDown(){
        if (EventSystem.current.IsPointerOverGameObject()){
            return;
        }

        if(!GameManager.instance.hasSelectedNation){
            GameManager.instance.player = GameManager.instance.GetState(gameObject.name).owner;
            if(StateManager.instance.currentMapMode == StateManager.MAPMODE.PLAYER){
                StateManager.instance.RefreshStatesColor();
            }
        }
        StateManager.instance.SelectState(gameObject.name);
        MapUI.instance.UpdateUI(GameManager.instance.GetStateOwner(gameObject.name));

    }


    public void OnMouseEnter(){
        renderer.color = Color.Lerp(renderer.color,Color.white,0.5f);
    }

    public void OnMouseExit(){
        StateManager.instance.RefreshProvince(gameObject.name);
    }
}
