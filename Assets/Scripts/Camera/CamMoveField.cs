using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMoveField : MonoBehaviour
{

    [SerializeField]
    private int camspeed;


    public GameObject border_up;
    public GameObject border_down;
    public GameObject border_left;
    public GameObject border_right;


    // Update is called once per frame
    void Update()
    {   // Deplacement
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        transform.position += move * camspeed * Time.deltaTime;


        
        transform.position = new Vector3(
        Mathf.Clamp(transform.position.x, border_left.transform.position.x, border_right.transform.position.x),
        transform.position.y,        
        Mathf.Clamp(transform.position.z, border_down.transform.position.z, border_up.transform.position.z));
        



    }

}