using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{

    [SerializeField]
    private int camspeed;

    private float currZoom = 25;

    public float mouseSensitivity = 0.05f;
    private Vector3 lastPosition;

    public GameObject border_up;
    public GameObject border_down;
    public GameObject border_left;
    public GameObject border_right;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   // Deplacement
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"),0);
        transform.position += move * camspeed * Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
        {
            lastPosition = Input.mousePosition;
        }
    
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Translate(delta.x * mouseSensitivity, delta.y * mouseSensitivity, 0);
            lastPosition = Input.mousePosition;
        }
        
        transform.position = new Vector3(
        Mathf.Clamp(transform.position.x, border_left.transform.position.x, border_right.transform.position.x),
        Mathf.Clamp(transform.position.y, border_down.transform.position.y, border_up.transform.position.y),
        transform.position.z);
        

        // Zoom

        if(Input.GetKey(KeyCode.P)){
            currZoom-=0.01f;
        }
        if(Input.GetKey(KeyCode.M)){
            currZoom+=0.01f;
        }
        
        currZoom = Mathf.Clamp(currZoom,10f,30f);
        if (GetComponent<Camera>().fieldOfView != currZoom){
            GetComponent<Camera>().fieldOfView = currZoom;
        }

    }

}
