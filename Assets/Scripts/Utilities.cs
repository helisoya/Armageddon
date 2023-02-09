using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public static Transform FindParent(Transform start, string tag){
        while(start.parent != null && start.parent.tag==tag){
            start = start.parent;
        }
        return start;
    }

    public static Vector3 RandomPointOnCircleEdge(float radius)
    {
    var vector2 = Random.insideUnitCircle.normalized * radius;
    return new Vector3(vector2.x, 0, vector2.y);
    }


    public static void Quit(){
        Application.Quit();
    }

    public static void Reset(){
        GameObject obj = GameManager.instance.gameObject;
        GameManager.instance = null;
        GameObject.Destroy(obj);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map");
    }
}
